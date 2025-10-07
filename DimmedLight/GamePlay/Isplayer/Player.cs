using DimmedLight.GamePlay.Enemies;
using DimmedLight.GamePlay.ETC;
using DimmedLight.GamePlay.Animated;
using DimmedLight.GamePlay.Managers;
using DimmedLight.GamePlay.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace DimmedLight.GamePlay.Isplayer
{
    public class Player
    {
        #region animation objects
        public AnimatedTexture Idle { get; private set; }
        public AnimatedTexture Walk { get; private set; }
        public AnimatedTexture Jump { get; private set; }
        public AnimatedTexture Attack { get; private set; }
        public AnimatedTexture Parry { get; private set; }
        public AnimatedTexture Death { get; private set; }
        #endregion

        #region state
        public Vector2 Position;
        public Rectangle HurtBox;
        public Rectangle HitBoxAttack { get; private set; }
        public Rectangle HitBoxParry { get; private set; }

        public bool IsJumping;
        public bool IsAttacking;
        public bool IsParrying;
        public bool IsDead;
        #endregion

        #region jumping
        private float velocityY = 0f;
        private float gravity = 0.5f;
        private float jumpPower = -15f; //ค่าเดิม = -15
        #endregion

        #region action timings
        private float attackDuration = 0.4f;
        private float attackTimer = 0f;
        private float attackDelay = 0.50f;
        private float attackDelayTimer = 0f;

        private float parryDuration = 0.22f;
        private float parryTimer = 0f;
        private float parryDelay = 0.23f;
        private float parryDelayTimer = 0f;

        //private float actionDelay = 1f;
        //private float actionDelayTimer = 0f;
        #endregion

        #region death
        public bool DeathAnimationStarted;
        public float DeathTimer;
        public float DeathDuration = 0.8f;
        public float PostDeathDelay = 3f;
        public bool DeathDelayStarted;
        public float DeathDelayTimer;
        public bool IsVisible = true;
        #endregion

        #region health and invincibility
        public byte Health = 2;
        public bool IsInvincible;
        public float InvincibilityTime = 1.5f; //อมตะ 1.5 วิ
        public float InvincibilityTimer = 0f;
        public float HealingCooldown = 4f;
        public float HealingTimer = 0f;
        #endregion

        #region return mechanics
        public Vector2 OriginalPosition;
        public Vector2 LastSafePosition;
        public float ReturnX = 400f;
        public float ReturnTimer = 0f;
        public bool IsReturning = false;
        public Vector2 KnockBack = new Vector2(-150, 0);
        #endregion

        #region helper
        //private Vector2 objectOffset = new Vector2(-50, -45);
        #endregion

        #region delay before can walk
        public float idleDelay = 2f;
        public float idleTimer = 0f;
        public bool canWalk = false;
        private bool inEvent = false;
        #endregion

        #region Sound&Music
        private SoundEffect attackEffect, jumpEffect;
        #endregion

        private PhaseManager phaseManager;
        private ScoreManager scoreManager;
        public Player(PhaseManager manager, ScoreManager score)
        {
            Idle = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f); //ใส่อนิเมชั่น Rotate, Scale, Depth
            Walk = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            Jump = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            Attack = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            Parry = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            Death = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            Position = new Vector2(395, 655);
            phaseManager = manager;
            scoreManager = score;
        }

        public void Load(ContentManager content)
        {
            Idle.Load(content, "player_idle", 1, 1, 15); //โหลดไฟล์ใส่อนิเมชั่น กำหนด frame count, row, fps 
            Walk.Load(content, "player_running_Spritesheet", 14, 1, 24); //frame count = จำนวนช่องใน 1 แถว, row = จำนวนแถว, fps = ความเร็ว
            Jump.Load(content, "player_jumping_Spritesheet", 9, 1, 8);
            Attack.Load(content, "player_attack_spritesheet", 10, 1, 24);
            Parry.Load(content, "player_attack_spritesheet", 10, 1, 20);
            Death.Load(content, "game_over_spritesheet", 1, 22, 10);

            #region Sound&Effect
            attackEffect = content.Load<SoundEffect>("Audio/LOOP_SFX_PlayerAttack");
            jumpEffect = content.Load<SoundEffect>("Audio/LOOP_SFX_Jump");
            #endregion
        }

        public void Update(GameTime gameTime, KeyboardState keyState, GamePadState gpState, KeyboardState prevKey, GamePadState prevGp, float delta)
        {
            // update hitboxes
            HurtBox = new Rectangle((int)Position.X, (int)Position.Y, 156, 174); //สร้าง hurtbox

            if (!canWalk) // delay before can walk
            {
                idleTimer += delta;
                Idle.UpdateFrame(delta);
                if (idleTimer >= idleDelay)
                {
                    canWalk = true;
                }
            }

            // jumping
            if ((keyState.IsKeyDown(Keys.Space) || gpState.IsButtonDown(Buttons.A)) && !IsJumping && !IsDead && canWalk) //กดกระโดด
            {
                IsJumping = true;
                velocityY = jumpPower;
                if (!inEvent)
                {
                    jumpEffect.Play();
                }
                Jump.Reset();
            }
            if (IsJumping) //กระโดดอยู่
            {
                velocityY += gravity; //ความเร่งโน้มถ่วง
                Position.Y += velocityY; //อัพเดทตำแหน่ง
                Jump.UpdateFrame(delta); //อัพเดทอนิเมชั่นกระโดด
                if (Position.Y >= 650) //ถึงพื้น
                {
                    Position.Y = 650;
                    IsJumping = false;
                    velocityY = 0f;
                }
            }

            // timers
            //if (actionDelayTimer > 0f) actionDelayTimer -= delta;
            if (attackDelayTimer > 0f) attackDelayTimer -= delta; //นับถอยหลังดีเลย์ก่อนโจมตีครั้งต่อไป
            if (parryDelayTimer > 0f) parryDelayTimer -= delta;

            #region player action
            bool attackPressed = keyState.IsKeyDown(Keys.D) && !prevKey.IsKeyDown(Keys.D) || keyState.IsKeyDown(Keys.F) && !prevKey.IsKeyDown(Keys.F) || gpState.IsButtonDown(Buttons.LeftShoulder) && !prevGp.IsButtonDown(Buttons.LeftShoulder); //ตั้งปุ่มโจมตี
            if (attackPressed && !IsAttacking && !IsParrying && attackDelayTimer <= 0f && /*actionDelayTimer <= 0f &&*/ !IsDead)
            {
                IsAttacking = true;
                attackTimer = attackDuration;
                attackDelayTimer = attackDelay;
                //actionDelayTimer = actionDelay;
                Attack.Reset(); //เริ่มอนิเมชั่นโจมตีใหม่
                if (canWalk)
                {
                    attackEffect.Play();
                }
            }
            if (IsAttacking)
            {
                HitBoxAttack = new Rectangle((int)Position.X + 156, (int)Position.Y, 76, 174);//ต่ำแหน่ง x y width height
                attackTimer -= delta;
                Attack.UpdateFrame(delta);
                if (attackTimer <= 0f) IsAttacking = false;
            }
            else
            {
                HitBoxAttack = Rectangle.Empty;
            }
            bool parryPressed = keyState.IsKeyDown(Keys.J) && !prevKey.IsKeyDown(Keys.J) || keyState.IsKeyDown(Keys.K) && !prevKey.IsKeyDown(Keys.K) || gpState.IsButtonDown(Buttons.RightShoulder) && !prevGp.IsButtonDown(Buttons.RightShoulder); //ตั้งปุ่มแพร์รี่
            if (!inEvent)
            {
                if (parryPressed && !IsParrying && !IsAttacking && parryDelayTimer <= 0f && /*actionDelayTimer <= 0f &&*/ !IsDead)
                {
                    IsParrying = true;
                    parryTimer = parryDuration;
                    parryDelayTimer = parryDelay;
                    //actionDelayTimer = actionDelay;
                    Parry.Reset(); //เริ่มอนิเมชั่นแพร์รี่ใหม่
                }
                if (IsParrying)
                {
                    HitBoxParry = new Rectangle((int)Position.X + 156, (int)Position.Y - 6, 54, 188);
                    parryTimer -= delta;
                    Parry.UpdateFrame(delta);
                    if (parryTimer <= 0f) IsParrying = false;
                }
                else
                {
                    HitBoxParry = Rectangle.Empty;
                }
            }
            else
            {
                if (parryPressed && !IsParrying && !IsAttacking && parryDelayTimer <= 0f && /*actionDelayTimer <= 0f &&*/ !IsDead)
                {
                    IsParrying = true;
                    parryTimer = parryDuration;
                    parryDelayTimer = parryDelay;
                    //actionDelayTimer = actionDelay;
                    Parry.Reset(); //เริ่มอนิเมชั่นแพร์รี่ใหม่
                }
                if (IsParrying)
                {
                    HitBoxParry = new Rectangle((int)Position.X + 120, (int)Position.Y + 45, 112, 135);
                    parryTimer -= delta;
                    Parry.UpdateFrame(delta);
                    if (parryTimer <= 0f) IsParrying = false;
                }
                else
                {
                    HitBoxParry = Rectangle.Empty;
                }
            }

                bool ultiPressed = keyState.IsKeyDown(Keys.F) && keyState.IsKeyDown(Keys.J) || gpState.IsButtonDown(Buttons.RightShoulder) && gpState.IsButtonDown(Buttons.LeftShoulder);
            if (scoreManager.SoulGauge >= 500 && ultiPressed && !IsDead)
            {
                if (!phaseManager.IsInEvent)
                {
                    UltimateReset();
                    scoreManager.removeSoul(500);
                }
                
            }
            #endregion

            // invincibility and healing
            if (IsInvincible) //อมตะ
            {
                InvincibilityTimer -= delta;
                if (InvincibilityTimer <= 0f) IsInvincible = false;
            }
            else //Healing
            {
                HealingTimer += delta;
                if (HealingTimer >= HealingCooldown && Health < 2)
                {
                    Health++;
                    HealingTimer = 0f;
                }
            }
            if (canWalk && !IsJumping && !IsAttacking && !IsParrying && !IsDead) //ถ้าไม่ได้กระโดด โจมตี แพร์รี่ หรือตาย
            {
                Walk.UpdateFrame(delta);
            }
            // return to safe
            if (IsReturning)
            {
                ReturnTimer -= delta;
                float t = 1f - ReturnTimer / ReturnX;
                t = MathHelper.Clamp(t, 0f, 1f); //จำกัดค่า t ให้อยู่ระหว่าง 0-1
                Position.X = MathHelper.Lerp(Position.X, OriginalPosition.X, t); //lerp = linear interpolation = การคำนวณค่าระหว่างสองจุด
                if (ReturnTimer <= 0f)
                {
                    IsReturning = false;
                    Position = OriginalPosition;
                }
            }

        }
        public void SetEvent(bool active)
        {
            inEvent = active;

            if (active)
            {
                parryDelay = 0.4f;
                attackDelay = 0.4f;
                jumpPower = 0;
                attackDuration = 0.22f;
            }
            else
            {
                parryDelay = 0.23f;
                attackDelay = 0.5f;
                jumpPower = -15f;
                attackDuration = 0.32f;
            }
        }

        public void UpdateDeathAnimation(float delta) //อัพเดทอนิเมชั่นตอนตาย
        {
            if (!DeathAnimationStarted)
            {
                DeathAnimationStarted = true;
                IsAttacking = false;
                IsParrying = false;
                IsJumping = false;
                IsInvincible = false;
                IsReturning = false;

                Position = new Vector2(195, 655);

                Death.Reset();
                Death.Loop = false;
            }

            Death.UpdateFrame(delta);

            if (Death.IsEnd && !DeathDelayStarted)
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
            if (!IsVisible && !DeathAnimationStarted)
                return;

            if (!canWalk)
            {
                Idle.DrawFrame(sb, new Vector2(Position.X, Position.Y), false);
            }
            else if (IsAttacking)
            {
                Attack.DrawFrame(sb, new Vector2(Position.X, Position.Y), false);
                //sb.Draw(hitBoxTex, HitBoxAttack, Color.Blue * 0.4f);
            }
            else if (IsParrying)
            {
                Parry.DrawFrame(sb, Position, Color.LightSkyBlue * 1f, false);
                //sb.Draw(hitBoxTex, HitBoxParry, Color.Blue * 0.4f);
            }
            else if (IsJumping)
            {
                Jump.DrawFrame(sb, new Vector2(HurtBox.X, HurtBox.Y), false);
            }
            else if (DeathAnimationStarted)
            {
                Death.DrawFrame(sb, new Vector2(HurtBox.X, HurtBox.Y), false);
            }
            else
            {
                if (IsVisible) 
                    Walk.DrawFrame(sb, new Vector2(HurtBox.X, HurtBox.Y), false);
            }
            //sb.Draw(hurtBoxTex, HurtBox, Color.Red * 0.4f);
        }
        public void SetPhaseManager(PhaseManager manager) 
        { 
            phaseManager = manager; 
        }
        public void Reset() //รีเซ็ตสถานะทั้งหมด
        {
            IsDead = false;
            IsJumping = false;
            IsAttacking = false;
            IsParrying = false;
            Health = 2;
            DeathAnimationStarted = false;
            DeathTimer = 0f;
            DeathDelayStarted = false;
            DeathDelayTimer = 0f;
            Walk.Reset();
            Jump.Reset();
            Attack.Reset();
            Parry.Reset();
            Death.Reset();
            Death.Loop = true;
            IsInvincible = false;
            InvincibilityTimer = 0f;
            HealingTimer = 0f;
            IsReturning = false;
            ReturnTimer = ReturnX;
            OriginalPosition = Position;
            idleTimer = 0f;
            canWalk = false;
            SetEvent(false);
            IsVisible = true;
        }
        public void UltimateReset()
        {
            if (phaseManager != null)
            {
                int currentPhaseIndex = phaseManager.CurrentPhaseIndex;
                phaseManager.ResetCurrentPhase(currentPhaseIndex);
            }
            if(IsJumping)
            {
                IsJumping = false;
                velocityY = 0f;
                Position.Y = 650;
            }
            Reset();
            scoreManager.clear();
        }
    }
}
