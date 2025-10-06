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
        public AnimatedTexture Attack;
        public Rectangle HitBox;
        public bool IsAttacking = false;
        public float AttackTimer = 0f;
        public float AttackDuration = 0.5f;
        public float AttackHitTime = 0.25f;
        public bool HitTriggered = false;
        private Color attack = new Color(255, 144, 100);
        public Guilt()
        {
            EnemyType = "Guilt";
        }

        public override void Load(ContentManager content)
        {
        }

        public override void Update(GameTime gameTime, float delta, Player player, ref bool globalFlip, Delisaster delisaster, ScoreManager scoreManager)
        {
            if (!player.IsDead)
            {
                Position.X -= speed * delta * 60;

                HurtBox = new Rectangle((int)Position.X, (int)Position.Y, 144, 178);
                HitBox = new Rectangle((int)Position.X - 45, (int)Position.Y, 144, 178);
                if (!IsDead)
                {
                    IsFlipped = player.Position.X > Position.X; // หันหน้าไปทางผู้เล่น
                    if (!IsAttacking && !HitTriggered && HitBox.Intersects(player.HurtBox)) // ถ้าศัตรูยังไม่โจมตีและผู้เล่นอยู่ในระยะโจมตี
                    {
                        IsAttacking = true;
                        AttackTimer = 0f;
                        HitTriggered = false;
                    }
                    if (IsAttacking)
                    {
                        AttackTimer += delta;
                        Attack.UpdateFrame(delta);
                        if (!HitTriggered && AttackTimer >= AttackHitTime && AttackTimer <= AttackHitTime + 0.1f) // ช่วงเวลาที่เกิดการโจมตี
                        {
                            if (player.HurtBox.Intersects(HitBox)) // ถ้าผู้เล่นอยู่ในระยะโจมตี
                            {
                                if (player.IsParrying) // ถ้าผู้เล่นกำลังแพร์รี่
                                {
                                    if (!IsDead)
                                    {
                                        OnKilled(scoreManager, "Parry"); // เรียกใช้เมธอด OnKilled เพื่อเพิ่มคะแนน
                                    }
                                    PlayParryHit();
                                    IsDead = true;
                                    DeathAnimationStarted = true;
                                    DeathTimer = 0f;
                                }
                                else if (!player.IsInvincible) // ถ้าผู้เล่นไม่อยู่ในสถานะอมตะ
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
                            }
                            HitTriggered = true;
                        }

                        if (AttackTimer >= AttackDuration)
                        {
                            IsAttacking = false;
                            HitTriggered = true;
                        }
                    }
                    else
                    {
                        Idle.UpdateFrame(delta);
                    }

                    if (player.IsAttacking && player.HitBoxAttack.Intersects(HurtBox)) // ถ้าผู้เล่นกำลังโจมตีและโดนศัตรู
                    {
                        if (!IsDead)
                        {
                            OnKilled(scoreManager, player.IsParrying ? "Parry" : "Attack"); // เรียกใช้เมธอด OnKilled เพื่อเพิ่มคะแนน
                        }
                        IsDead = true;
                        DeathAnimationStarted = true;
                        DeathTimer = 0;
                    }
                    if (Position.X < 0) // ถ้าศัตรูออกนอกหน้าจอด้านซ้าย
                    {
                        IsDead = true;
                        DeathAnimationStarted = true;
                        DeathTimer = 0f;
                    }
                    if (IsDead && !DeathAnimationStarted) // เริ่มเล่นอนิเมชันตายถ้ายังไม่เริ่ม
                    {
                        DeathAnimationStarted = true;
                        DeathTimer = 0f;
                    }
                }
            }
            if (DeathAnimationStarted) // ถ้าเริ่มเล่นอนิเมชันตายแล้ว
            {
                HandleDeathAnimation(delta); // เรียกใช้เมธอดจัดการอนิเมชันตาย
            }
        }

        public override void Draw(SpriteBatch sb, Texture2D hurtBoxTex, Texture2D hitBoxTex, bool flip)
        {
            if (!IsDead || DeathAnimationStarted)
            {
                if (DeathAnimationStarted && IsDead)
                {
                    Death.DrawFrame(sb, new Vector2(HurtBox.X, HurtBox.Y - 68), IsFlipped);
                }
                else if (IsAttacking)
                {
                    Attack.DrawFrame(sb, new Vector2(HurtBox.X, HurtBox.Y), attack * 1f, IsFlipped);
                    //sb.Draw(hitBoxTex, HitBox, Color.Blue * 0.4f);
                }
                else
                {
                    Idle.DrawFrame(sb, new Vector2(HurtBox.X, HurtBox.Y), IsFlipped);
                }
                //sb.Draw(hurtBoxTex, HurtBox, Color.Red * 0.4f);
            }
        }
    }
}
