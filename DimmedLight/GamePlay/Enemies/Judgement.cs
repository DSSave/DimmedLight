using DimmedLight.GamePlay.Isplayer;
using DimmedLight.GamePlay.UI;
using DimmedLight.MainMenu;
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
    public class Judgement : EnemyBase
    {
        public Rectangle HitN { get; private set; }
        public Judgement()
        {
            EnemyType = "Judgement";
        }
        public override void Load(ContentManager content) { }

        public override void Update(GameTime gameTime, float delta, Player player, ref bool globalFlip, Delisaster delisaster, ScoreManager scoreManager)
        {
            if (player.IsDead) return;

            Position.X -= speed * delta * 60;
            HitN = new Rectangle((int)Position.X + 39, (int)Position.Y + 38, 101, 103);

            if (!IsDead)
            {
                IsFlipped = player.Position.X > Position.X;

                // Player collision
                if (player.HurtBox.Intersects(HitN) && !player.IsInvincible)
                {
                    player.TakeDamage(delisaster);
                    PlayerHit?.Play(0.3f * SoundManager.SfxVolume, 0f, 0f);
                }

                // Player attack collision
                if (player.IsAttacking && player.HitBoxAttack.Intersects(HitN))
                {
                    if (!IsDead) OnKilled(scoreManager, "Attack");
                    StartDeathAnimation();
                }

                // Off-screen check
                if (Position.X < -HitN.Width && !DeathAnimationStarted)
                {
                    StartDeathAnimation(false);
                }

                Idle.UpdateFrame(delta);
            }

            if (DeathAnimationStarted)
            {
                HandleDeathAnimation(delta);
            }
        }

        private void StartDeathAnimation(bool playSound = true)
        {
            if (IsDead) return;
            IsDead = true;
            DeathAnimationStarted = true;
            deathTimer = 0f;
            if (playSound) EnemiesDead?.Play(0.3f * SoundManager.SfxVolume, 0f, 0f);
        }

        // FIX: Re-implemented the missing Draw method
        public override void Draw(SpriteBatch sb, Texture2D hurtBoxTex, Texture2D hitBoxTex, bool flip)
        {
            if (IsDead && !DeathAnimationStarted) return;

            if (DeathAnimationStarted)
            {
                Death.DrawFrame(sb, new Vector2(Position.X + 5, Position.Y - 31), IsFlipped);
            }
            else
            {
                Idle.DrawFrame(sb, new Vector2(Position.X, Position.Y), IsFlipped);
            }
            sb.Draw(hitBoxTex, HitN, Color.Red * 0.6f);
        }
    }
}
