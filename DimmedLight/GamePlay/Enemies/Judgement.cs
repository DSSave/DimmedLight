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
    public class Judgement : EnemyBase
    {
        public Rectangle HitN;

        public Judgement()
        {
            EnemyType = "Judgement";
        }
        public override void Load(ContentManager content)
        {
        }

        public override void Update(GameTime gameTime, float delta, Player player, ref bool globalFlip, Delisaster delisaster, ScoreManager scoreManager)
        {
            if (!player.IsDead)
            {
                Position.X -= speed * delta * 60;
                HitN = new Rectangle((int)Position.X + 20, (int)Position.Y + 35, 138, 90);
                if (!IsDead)
                {
                    IsFlipped = player.Position.X > Position.X;
                    if (player.HurtBox.Intersects(HitN) && !player.IsInvincible) // ถ้าผู้เล่นโดนชนและไม่อยู่ในสถานะอมตะ
                    {
                        player.Health--;
                        player.IsInvincible = true;
                        player.InvincibilityTimer = player.InvincibilityTime;
                        player.HealingTimer = 0f;
                        if (!player.IsReturning)
                        {
                            player.OriginalPosition = player.LastSafePosition;
                            player.ReturnTimer = player.ReturnX;
                            player.IsReturning = true;
                        }
                        player.Position += player.KnockBack;

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
                    if (player.IsAttacking && player.HitBoxAttack.Intersects(HitN)) // ถ้าผู้เล่นกำลังโจมตีและโดนศัตรู
                    {
                        if (!IsDead)
                        {
                            OnKilled(scoreManager, "Attack"); // เรียกใช้เมธอด OnKilled เพื่อเพิ่มคะแนน
                        }
                        IsDead = true;
                        DeathAnimationStarted = true;
                        DeathTimer = 0f;
                    }
                    if (Position.X < 0 && !DeathAnimationStarted) // ถ้าศัตรูออกนอกหน้าจอ
                    {
                        IsDead = true;
                        DeathAnimationStarted = true;
                        DeathTimer = 0f;
                    }
                    Idle.UpdateFrame(delta);
                }
            }
            if (DeathAnimationStarted)
            {
                HandleDeathAnimation(delta);
            }
        }
        public override void Draw(SpriteBatch sb, Texture2D hurtBoxTex, Texture2D hitBoxTex, bool flip)
        {
            if (!IsDead || DeathAnimationStarted)
            {
                if (DeathAnimationStarted && IsDead)
                {
                    Death.DrawFrame(sb, new Vector2(Position.X, Position.Y), IsFlipped);
                }
                else
                {
                    Idle.DrawFrame(sb, new Vector2(Position.X, Position.Y), IsFlipped);
                }
                //sb.Draw(hurtBoxTex, HitN, Color.Red * 0.4f);
            }
        }
    }
}
