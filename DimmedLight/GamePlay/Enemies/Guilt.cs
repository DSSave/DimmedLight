using DimmedLight.GamePlay.Animated;
using DimmedLight.GamePlay.Isplayer;
using DimmedLight.GamePlay.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.GamePlay.Enemies
{
    public class Guilt : EnemyBase
    {
        public AnimatedTexture Attack { get; internal set; }
        public Rectangle HitBox { get; private set; }
        public bool IsAttacking { get; private set; }
        public float AttackTimer { get; private set; }

        private const float AttackDuration = 0.5f;
        private const float AttackHitTime = 0.25f;
        private bool _hitTriggered;
        private readonly Color _attackColor = new Color(255, 144, 100);
        public Guilt()
        {
            EnemyType = "Guilt";
            Attack = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
        }

        public override void Load(ContentManager content)
        {
        }
        public override void Update(GameTime gameTime, float delta, Player player, ref bool globalFlip, Delisaster delisaster, ScoreManager scoreManager)
        {
            if (player.IsDead) return;

            Position.X -= speed * delta * 60;
            HurtBox = new Rectangle((int)Position.X, (int)Position.Y, 144, 178);
            HitBox = new Rectangle((int)Position.X - 45, (int)Position.Y, 144, 178);

            if (!IsDead)
            {
                IsFlipped = player.Position.X > Position.X;

                HandleAttacking(delta, player, delisaster, scoreManager);
                HandlePlayerCollision(player, scoreManager);

                if (Position.X < -HurtBox.Width)
                {
                    StartDeathAnimation(false);
                }
            }

            if (IsDead && !DeathAnimationStarted)
            {
                StartDeathAnimation();
            }

            if (DeathAnimationStarted)
            {
                HandleDeathAnimation(delta);
            }
        }
        private void HandleAttacking(float delta, Player player, Delisaster delisaster, ScoreManager scoreManager)
        {
            if (!IsAttacking && !_hitTriggered && HitBox.Intersects(player.HurtBox))
            {
                IsAttacking = true;
                AttackTimer = 0f;
                _hitTriggered = false;
                Attack.Reset();
            }

            if (IsAttacking)
            {
                AttackTimer += delta;
                Attack.UpdateFrame(delta);

                bool isHitTime = AttackTimer >= AttackHitTime && AttackTimer <= AttackHitTime + 0.1f;
                if (!_hitTriggered && isHitTime && player.HurtBox.Intersects(HitBox))
                {
                    if (player.IsParrying)
                    {
                        if (!IsDead) OnKilled(scoreManager, "Parry");
                        PlayParryHit();
                        StartDeathAnimation();
                    }
                    else
                    {
                        player.TakeDamage(delisaster);
                        PlayerHit?.Play(0.3f, 0f, 0f);
                    }
                    _hitTriggered = true;
                }

                if (AttackTimer >= AttackDuration)
                {
                    IsAttacking = false;
                }
            }
            else
            {
                Idle.UpdateFrame(delta);
            }
        }
        private void HandlePlayerCollision(Player player, ScoreManager scoreManager)
        {
            if (player.IsAttacking && player.HitBoxAttack.Intersects(HurtBox))
            {
                if (!IsDead) OnKilled(scoreManager, "Attack");
                StartDeathAnimation();
            }
        }
        private void StartDeathAnimation(bool playSound = true)
        {
            if (IsDead) return;
            IsDead = true;
            DeathAnimationStarted = true;
            deathTimer = 0;
            if (playSound) EnemiesDead?.Play(0.3f, 0f, 0f);
        }
        public override void Draw(SpriteBatch sb, Texture2D hurtBoxTex, Texture2D hitBoxTex, bool flip)
        {
            if (IsDead && !DeathAnimationStarted) return;

            if (DeathAnimationStarted)
            {
                Death.DrawFrame(sb, new Vector2(Position.X - 6, Position.Y - 68), IsFlipped);
            }
            else if (IsAttacking)
            {
                Attack.DrawFrame(sb, new Vector2(Position.X - 45, Position.Y), IsFlipped, _attackColor);
            }
            else
            {
                Idle.DrawFrame(sb, Position, IsFlipped);
            }
        }
    }
}
