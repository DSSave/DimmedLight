using DimmedLight.GamePlay.Isplayer;
using DimmedLight.GamePlay.UI;
using DimmedLight.MainMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.GamePlay.Enemies
{
    public class Trauma : EnemyBase
    {
        public Texture2D ProjectileTex;
        public Projectile ProjectileObj;
        public bool IsAttacking = false;
        public float AttackTimer = 0f;
        public float AttackDuration = 0.5f;
        public bool HitTriggered = false;
        public float ProjectileSpeed = 600f;
        public float AttackRange = 1000f;
        private bool _hasFired = false;
        public Trauma(Texture2D proj)
        {
            ProjectileTex = proj;
            ProjectileObj = new Projectile(ProjectileTex);
            EnemyType = "Trauma";
            _hasFired = false;
        }

        public override void Load(ContentManager content)
        {
        }

        public override void Update(GameTime gameTime, float delta, Player player, ref bool globalFlip, Delisaster delisaster, ScoreManager scoreManager)
        {
            if (player.IsDead) return;

            Position.X -= speed * delta * 60;
            HurtBox = new Rectangle((int)Position.X, (int)Position.Y + 50, 109, 170);

            if (!IsDead)
            {
                IsFlipped = player.Position.X > Position.X;
                HandleShooting(player, delta);
                HandleProjectile(player, delisaster, scoreManager, delta);
                HandlePlayerCollision(player, scoreManager);

                if (Position.X < -HurtBox.Width) StartDeathAnimation(playSound: false);
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

        private void HandleShooting(Player player, float delta)
        {
            float distance = Vector2.Distance(Position, player.Position);
            if (!HitTriggered && !ProjectileObj.Active && distance <= AttackRange && !IsAttacking && !_hasFired)
            {
                IsAttacking = true;
                AttackTimer = 0f;
                AttackAnim.Reset();
                ProjectileObj.Fire(new Vector2(Position.X, Position.Y + 125), new Vector2(player.Position.X + 250, 650), ProjectileSpeed);
                _hasFired = true;
                AmmoShoot?.Play(0.3f * SoundManager.SfxVolume, 0f, 0f);
            }

            if (IsAttacking)
            {
                AttackTimer += delta;
                AttackAnim.UpdateFrame(delta);
                if (AttackTimer >= AttackDuration)
                {
                    IsAttacking = false;
                    HitTriggered = false;
                }
            }
            else
            {
                Idle.UpdateFrame(delta);
            }
        }

        private void HandleProjectile(Player player, Delisaster delisaster, ScoreManager scoreManager, float delta)
        {
            if (!ProjectileObj.Active) return;

            ProjectileObj.Update(delta);
            if (!HitTriggered && ProjectileObj.HitBox.Intersects(player.HurtBox))
            {
                if (player.IsParrying)
                {
                    if (!IsDead) OnKilled(scoreManager, "Parry");
                    PlayParryHit();
                }
                else
                {
                    player.TakeDamage(delisaster);
                    PlayerHit?.Play(0.3f * SoundManager.SfxVolume, 0f, 0f);
                    IsAttacking = false;
                }
                ProjectileObj.Deactivate();
                HitTriggered = true;
            }

            if (ProjectileObj.Position.X < -ProjectileTex.Width)
            {
                ProjectileObj.Deactivate();
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
            deathTimer = 0f;
            ProjectileObj.Deactivate();
            if (playSound) EnemiesDead?.Play(0.3f * SoundManager.SfxVolume, 0f, 0f);
        }
        public override void Draw(SpriteBatch sb, Texture2D hurtBoxTex, Texture2D hitBoxTex, bool flip)
        {
            if (!IsDead || DeathAnimationStarted)
            {
                if (DeathAnimationStarted && IsDead)
                {
                    Death.DrawFrame(sb, new Vector2(Position.X, Position.Y), IsFlipped);
                }
                else if (IsAttacking)
                {
                    AttackAnim.DrawFrame(sb, new Vector2(Position.X, Position.Y), IsFlipped);
                }
                else
                {
                    Idle.DrawFrame(sb, new Vector2(Position.X, Position.Y), IsFlipped);
                }
                //sb.Draw(hurtBoxTex, HurtBox, Color.Red * 0.4f);
                if (ProjectileObj.Active)
                {
                    ProjectileObj.Draw(sb);
                    //sb.Draw(hitBoxTex, ProjectileObj.HitBox, Color.Blue * 0.4f);
                }
            }
        }
    }
}
