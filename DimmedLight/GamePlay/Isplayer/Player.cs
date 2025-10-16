using DimmedLight.GamePlay.Animated;
using DimmedLight.GamePlay.Enemies;
using DimmedLight.GamePlay.ETC;
using DimmedLight.GamePlay.Managers;
using DimmedLight.GamePlay.UI;
using DimmedLight.MainMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.GamePlay.Isplayer
{
    public class Player
    {
        #region animation
        public AnimatedTexture Idle { get; }
        public AnimatedTexture Walk { get; }
        public AnimatedTexture Jump { get; }
        public AnimatedTexture Attack { get; }
        public AnimatedTexture Parry { get; }
        public AnimatedTexture Death { get; }
        #endregion

        #region state
        public Vector2 Position;
        public Rectangle HurtBox { get; private set; }
        public Rectangle HitBoxAttack { get; private set; }
        public Rectangle HitBoxParry { get; private set; }
        public bool IsJumping { get; private set; }
        public bool IsAttacking { get; private set; }
        public bool IsParrying { get; private set; }
        public bool IsDead { get; set; }
        #endregion

        #region jumping
        private float _velocityY;
        private const float Gravity = 0.5f;
        private const float GroundLevel = 655f;
        private float _jumpPower = -15f;
        #endregion

        #region action timings
        private float _attackDuration = 0.4f;
        private float _attackTimer;
        private const float AttackDelay = 0.50f;
        private float _attackDelayTimer;

        private float _parryDuration = 0.22f;
        private float _parryTimer;
        private const float ParryDelay = 0.23f;
        private float _parryDelayTimer;
        #endregion

        #region death
        public bool DeathAnimationStarted { get; private set; }
        private const float DeathDuration = 0.8f;
        public float PostDeathDelay { get; } = 3f;
        public bool DeathDelayStarted { get; private set; }
        public float DeathDelayTimer { get; private set; }
        public bool IsVisible { get; private set; } = true;
        #endregion

        #region health and invincibility
        public byte Health { get; set; } = 2;
        public bool IsInvincible { get; set; }
        public float InvincibilityTime { get; } = 1.5f;
        public float InvincibilityTimer { get; set; }
        private const float HealingCooldown = 4f;
        public float HealingTimer { get; set; }
        #endregion

        #region return mechanics
        public Vector2 OriginalPosition;
        public Vector2 LastSafePosition;
        public float ReturnX { get; } = 400f;
        public float ReturnTimer { get; set; }
        public bool IsReturning { get; set; }
        public Vector2 KnockBack { get; } = new Vector2(-150, 0);
        #endregion

        #region delay before can walk
        public float idleDelay = 2f;
        public float idleTimer = 0f;
        public bool canWalk = false;
        private bool _inEvent;
        #endregion

        #region Sound&Music
        private SoundEffect _attackEffect, _jumpEffect;
        private PhaseManager _phaseManager;
        private readonly ScoreManager _scoreManager;
        #endregion
        public Player(PhaseManager manager, ScoreManager score)
        {
            Idle = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            Walk = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            Jump = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            Attack = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            Parry = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            Death = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            Position = new Vector2(395, GroundLevel);
            _phaseManager = manager;
            _scoreManager = score;
        }

        public void Load(ContentManager content)
        {
            Idle.Load(content, "player_idle", 1, 1, 15);
            Walk.Load(content, "player_running_Spritesheet", 14, 1, 24);
            Jump.Load(content, "player_jumping_Spritesheet", 9, 1, 8);
            Attack.Load(content, "player_attack_spritesheet", 10, 1, 24);
            Parry.Load(content, "player_attack_spritesheet", 10, 1, 20);
            Death.Load(content, "game_over_spritesheet", 1, 22, 10);

            _attackEffect = content.Load<SoundEffect>("Audio/LOOP_SFX_PlayerAttack");
            _jumpEffect = content.Load<SoundEffect>("Audio/LOOP_SFX_Jump");
        }
        public void Update(GameTime gameTime, KeyboardState keyState, GamePadState gpState, KeyboardState prevKey, GamePadState prevGp, float delta)
        {
            HurtBox = new Rectangle((int)Position.X + 30, (int)Position.Y, 156, 174);

            HandleInitialDelay(delta);
            HandleJumping(keyState, gpState, delta);
            HandleActions(keyState, gpState, prevKey, prevGp, delta);
            HandleInvincibilityAndHealing(delta);
            HandleReturnToSafePosition(delta);
            UpdateAnimations(delta);
        }
        private void HandleInitialDelay(float delta)
        {
            if (!canWalk)
            {
                idleTimer += delta;
                if (idleTimer >= idleDelay)
                {
                    canWalk = true;
                }
            }
        }
        private void HandleJumping(KeyboardState keyState, GamePadState gpState, float delta)
        {
            bool jumpPressed = keyState.IsKeyDown(Keys.Space) || gpState.IsButtonDown(Buttons.A);
            if (jumpPressed && !IsJumping && !IsDead && canWalk)
            {
                IsJumping = true;
                _velocityY = _jumpPower;
                if (!_inEvent) _jumpEffect.Play(0.3f * SoundManager.SfxVolume, 0f, 0f);
                Jump.Reset();
            }

            if (IsJumping)
            {
                _velocityY += Gravity;
                Position.Y += _velocityY;
                if (Position.Y >= GroundLevel)
                {
                    Position.Y = GroundLevel;
                    IsJumping = false;
                }
            }
        }
        private void HandleActions(KeyboardState keyState, GamePadState gpState, KeyboardState prevKey, GamePadState prevGp, float delta)
        {
            if (_attackDelayTimer > 0f) _attackDelayTimer -= delta;
            if (_parryDelayTimer > 0f) _parryDelayTimer -= delta;

            HandleAttack(keyState, gpState, prevKey, prevGp, delta);
            HandleParry(keyState, gpState, prevKey, prevGp, delta);
            HandleUltimate(keyState, gpState);
        }
        private void HandleAttack(KeyboardState keyState, GamePadState gpState, KeyboardState prevKey, GamePadState prevGp, float delta)
        {
            bool attackPressed = (keyState.IsKeyDown(Keys.D) && !prevKey.IsKeyDown(Keys.D)) ||
                                 (keyState.IsKeyDown(Keys.F) && !prevKey.IsKeyDown(Keys.F)) ||
                                 (gpState.IsButtonDown(Buttons.LeftShoulder) && !prevGp.IsButtonDown(Buttons.LeftShoulder));

            if (attackPressed && !IsAttacking && !IsParrying && _attackDelayTimer <= 0f && !IsDead)
            {
                IsAttacking = true;
                _attackTimer = _attackDuration;
                _attackDelayTimer = AttackDelay;
                Attack.Reset();
                if (canWalk) _attackEffect.Play(0.3f * SoundManager.SfxVolume, 0f, 0f);
            }

            if (IsAttacking)
            {
                _attackTimer -= delta;
                HitBoxAttack = new Rectangle((int)Position.X + 186, (int)Position.Y, 76, 174);
                if (_attackTimer <= 0f) IsAttacking = false;
            }
            else
            {
                HitBoxAttack = Rectangle.Empty;
            }
        }
        private void HandleParry(KeyboardState keyState, GamePadState gpState, KeyboardState prevKey, GamePadState prevGp, float delta)
        {
            bool parryPressed = (keyState.IsKeyDown(Keys.J) && !prevKey.IsKeyDown(Keys.J)) ||
                                (keyState.IsKeyDown(Keys.K) && !prevKey.IsKeyDown(Keys.K)) ||
                                (gpState.IsButtonDown(Buttons.RightShoulder) && !prevGp.IsButtonDown(Buttons.RightShoulder));

            if (parryPressed && !IsParrying && !IsAttacking && _parryDelayTimer <= 0f && !IsDead)
            {
                IsParrying = true;
                _parryTimer = _parryDuration;
                _parryDelayTimer = _inEvent ? 0.4f : ParryDelay;
                Parry.Reset();
            }

            if (IsParrying)
            {
                _parryTimer -= delta;
                HitBoxParry = _inEvent
                    ? new Rectangle((int)Position.X + 180, (int)Position.Y + 45, 112, 135)
                    : new Rectangle((int)Position.X + 186, (int)Position.Y - 6, 54, 188);
                if (_parryTimer <= 0f) IsParrying = false;
            }
            else
            {
                HitBoxParry = Rectangle.Empty;
            }
        }
        private void HandleUltimate(KeyboardState keyState, GamePadState gpState)
        {
            bool ultiPressed = (keyState.IsKeyDown(Keys.F) && keyState.IsKeyDown(Keys.J)) ||
                               (gpState.IsButtonDown(Buttons.RightShoulder) && gpState.IsButtonDown(Buttons.LeftShoulder));

            if (_scoreManager.SoulGauge >= 600 && ultiPressed && !IsDead && !_phaseManager.IsInEvent)
            {
                UltimateReset();
                _scoreManager.removeSoul(600);
            }
        }
        private void HandleInvincibilityAndHealing(float delta)
        {
            if (IsInvincible)
            {
                InvincibilityTimer -= delta;
                if (InvincibilityTimer <= 0f) IsInvincible = false;
            }
            else
            {
                HealingTimer += delta;
                if (HealingTimer >= HealingCooldown && Health < 2)
                {
                    Health++;
                    HealingTimer = 0f;
                }
            }
        }
        private void HandleReturnToSafePosition(float delta)
        {
            if (IsReturning)
            {
                ReturnTimer -= delta;
                float t = 1f - MathHelper.Clamp(ReturnTimer / ReturnX, 0f, 1f);
                Position.X = MathHelper.Lerp(Position.X, OriginalPosition.X, t);

                if (ReturnTimer <= 0f)
                {
                    IsReturning = false;
                    Position.X = OriginalPosition.X;
                }
            }
        }
        private void UpdateAnimations(float delta)
        {
            if (!canWalk) Idle.UpdateFrame(delta);
            else if (IsAttacking) Attack.UpdateFrame(delta);
            else if (IsParrying) Parry.UpdateFrame(delta);
            else if (IsJumping) Jump.UpdateFrame(delta);
            else if (!IsDead) Walk.UpdateFrame(delta);
        }
        public void TakeDamage(Delisaster delisaster)
        {
            if (IsInvincible) return;

            Health--;
            IsInvincible = true;
            InvincibilityTimer = InvincibilityTime;
            HealingTimer = 0f;

            if (!IsReturning)
            {
                OriginalPosition = LastSafePosition;
                ReturnTimer = ReturnX;
                IsReturning = true;
            }
            Position += KnockBack;

            if (!delisaster.IsReturning)
            {
                delisaster.OriginalPosition = delisaster.Position;
                delisaster.ReturnTimer = delisaster.ReturnPos;
                delisaster.IsReturning = true;
            }
            else
            {
                delisaster.ReturnTimer += 4f;
            }
            delisaster.Position += delisaster.Move;
        }
        public void SetEvent(bool active)
        {
            _inEvent = active;
            _jumpPower = active ? 0 : -15f;
            _attackDuration = active ? 0.22f : 0.4f;
        }

        public void UpdateDeathAnimation(float delta)
        {
            if (!DeathAnimationStarted)
            {
                DeathAnimationStarted = true;
                IsAttacking = IsParrying = IsJumping = IsInvincible = IsReturning = false;
                Position = new Vector2(195, GroundLevel);
                Death.Reset();
                Death.Loop = false;
            }

            Death.UpdateFrame(delta);

            if (Death.IsEnded && !DeathDelayStarted)
            {
                DeathDelayStarted = true;
                DeathDelayTimer = 0f;
            }

            if (DeathDelayStarted)
            {
                DeathDelayTimer += delta;
            }
        }
        public void Draw(SpriteBatch sb, Texture2D hurtBoxTex, Texture2D hitBoxTex)
        {
            if (!IsVisible && !DeathAnimationStarted) return;

            if (DeathAnimationStarted) Death.DrawFrame(sb, Position);
            else if (!canWalk) Idle.DrawFrame(sb, Position);
            else if (IsAttacking) Attack.DrawFrame(sb, Position);
            else if (IsParrying) Parry.DrawFrame(sb, Position, false, Color.LightSkyBlue);
            else if (IsJumping) Jump.DrawFrame(sb, Position);
            else if (IsVisible) Walk.DrawFrame(sb, Position);
        }
        public void SetPhaseManager(PhaseManager manager) => _phaseManager = manager;

        public void Reset()
        {
            IsDead = false;
            IsJumping = false;
            IsAttacking = false;
            IsParrying = false;
            Health = 2;
            DeathAnimationStarted = false;
            DeathDelayStarted = false;
            DeathDelayTimer = 0f;
            IsInvincible = false;
            InvincibilityTimer = 0f;
            HealingTimer = 0f;
            IsReturning = false;
            ReturnTimer = ReturnX;
            Position = new Vector2(395, GroundLevel);
            OriginalPosition = Position;
            idleTimer = 0f;
            canWalk = false;
            IsVisible = true;

            Walk.Reset();
            Jump.Reset();
            Attack.Reset();
            Parry.Reset();
            Death.Reset();
            Death.Loop = true;
            SetEvent(false);
        }
        public void UltimateReset()
        {
            _phaseManager?.ResetCurrentPhase(_phaseManager.CurrentPhaseIndex);
            if (IsJumping)
            {
                IsJumping = false;
                _velocityY = 0f;
                Position.Y = GroundLevel;
            }
            Reset();
            _scoreManager.clear();
        }
    }
}
