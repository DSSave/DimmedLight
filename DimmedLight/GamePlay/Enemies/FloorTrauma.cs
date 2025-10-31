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
    public class FloorTrauma : EnemyBase
    {
        public Texture2D ProjectileTex;
        public Projectile ProjectileObj;
        public bool IsAttacking = false;
        public float AttackTimer = 0f;
        public float AttackDuration = 0.5f;
        public bool HitTriggered = false;
        public float ProjectileSpeed;
        public float AttackRange = 1600f;
        private bool _hasFired = false;
        public FloorTrauma(Texture2D proj)
        {
            ProjectileTex = proj;
            ProjectileObj = new Projectile(ProjectileTex);
            EnemyType = "FloorTrauma";
            _hasFired = false;
        }

        public override void Load(ContentManager content)
        {

        }
        public override void SetSpeed(float speed)
        {
            base.SetSpeed(speed); // เรียกใช้เมธอด SetSpeed ของคลาสแม่

            ProjectileSpeed = 600f + speed * 50f; // ปรับความเร็วของโปรเจกไทล์ตามความเร็วศัตรู
        }

        public override void Update(GameTime gameTime, float delta, Player player, ref bool globalFlip, Delisaster delisaster, ScoreManager scoreManager)
        {
            if (!player.IsDead)
            {
<<<<<<< Updated upstream
                Position.X -= speed * delta * 60;
                HurtBox = new Rectangle((int)Position.X, (int)Position.Y, 109, 175);
                float distance = Vector2.Distance(Position, player.Position); // คำนวณระยะห่างระหว่างศัตรูกับผู้เล่น
                if (!IsDead)
=======
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
                ProjectileObj.Fire(new Vector2(Position.X, Position.Y + 145), new Vector2(player.Position.X, 745), ProjectileSpeed);
                _hasFired = true;
                AmmoShoot?.Play(0.3f * SoundManager.SfxVolume, 0f, 0f);
            }

            if (IsAttacking)
            {
                AttackTimer += delta;
                AttackAnim.UpdateFrame(delta);
                if (AttackTimer >= AttackDuration)
>>>>>>> Stashed changes
                {
                    IsFlipped = player.Position.X > Position.X; // หันหน้าไปทางผู้เล่น
                    if (!HitTriggered && !ProjectileObj.Active && distance <= AttackRange && !IsAttacking) // ถ้าศัตรูยังไม่โจมตีและโปรเจกไทล์ไม่ทำงาน และผู้เล่นอยู่ในระยะโจมตี
                    {
                        IsAttacking = true;
                        AttackTimer = 0f;
                        AttackAnim.Reset();
                        ProjectileObj.Fire(new Vector2(Position.X, Position.Y + 125), new Vector2(player.Position.X, 745), ProjectileSpeed); // ยิงโปรเจกไทล์ไปยังตำแหน่งผู้เล่น
                        HitTriggered = false;
                        AmmoShoot?.Play(0.3f, 0f, 0f);
                    }
                    if (IsAttacking)
                    {
                        AttackTimer += delta;
                        AttackAnim.UpdateFrame(delta);
                        if (AttackTimer >= AttackDuration)
                            IsAttacking = false;
                        if (ProjectileObj.Position.X < -ProjectileTex.Width) // ถ้าโปรเจกไทล์ออกนอกหน้าจอ
                        {
                            IsAttacking = false;
                            HitTriggered = false;
                        }
                    }
                    else
                    {
                        Idle.UpdateFrame(delta);
                    }

                    if (ProjectileObj.Active) // ถ้าโปรเจกไทล์กำลังทำงาน
                    {
                        ProjectileObj.Update(delta);
                        if (!HitTriggered && ProjectileObj.HitBox.Intersects(player.HurtBox)) // ถ้าโปรเจกไทล์ชนกับผู้เล่น
                        {
                            if (player.IsParrying) // ถ้าผู้เล่นกำลังแพร์รี่
                            {
                                if (!IsDead)
                                {
                                    OnKilled(scoreManager, "Parry"); // เรียกใช้เมธอด OnKilled เพื่อเพิ่มคะแนน
                                }
                                PlayParryHit();
                                ProjectileObj.Active = false;
                                HitTriggered = true;
                            }
                            else if (!player.IsInvincible) // ถ้าผู้เล่นไม่อยู่ในสถานะอมตะ
                            {
                                player.Health--; // ลดพลังชีวิตผู้เล่น
                                player.IsInvincible = true;
                                player.InvincibilityTimer = player.InvincibilityTime; // ตั้งเวลาสถานะอมตะ
                                player.HealingTimer = 0f;
                                if (!player.IsReturning) // ถ้าผู้เล่นไม่ได้ถูกดึงกลับ
                                {
                                    player.OriginalPosition = player.LastSafePosition;
                                    player.ReturnTimer = player.ReturnX;
                                    player.IsReturning = true;
                                }
                                else
                                {
                                    player.ReturnTimer += 4f; // เพิ่มเวลาการดึงกลับ
                                }
                                player.Position += player.KnockBack; // ดันผู้เล่นออก
                                PlayerHit?.Play(0.3f, 0f, 0f);
                                if (!delisaster.IsReturning) // ถ้า Delisaster ไม่ได้ถูกดึงกลับ
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
                                ProjectileObj.Active = false;
                                HitTriggered = true;
                                IsAttacking = false;
                            }
                        }
                        if (ProjectileObj.Position.X < -ProjectileTex.Width) // ถ้าโปรเจกไทล์ออกนอกหน้าจอ
                        {
                            ProjectileObj.Active = false;
                            HitTriggered = true;
                        }
                    }

                    if (player.IsAttacking && player.HitBoxAttack.Intersects(HurtBox)) // ถ้าผู้เล่นกำลังโจมตีและโดนศัตรู
                    {
                        if (!IsDead)
                        {
                            OnKilled(scoreManager, "Attack");
                        }
                        IsDead = true;
                        DeathAnimationStarted = true;
                        DeathTimer = 0f;
                        ProjectileObj.Active = false;
                        EnemiesDead?.Play(0.3f, 0f, 0f);
                    }
                    if (Position.X < 0) // ถ้าศัตรูหลุดซ้ายหน้าจอ
                    {
                        IsDead = true;
                        DeathAnimationStarted = true;
                        DeathTimer = 0f;
                        ProjectileObj.Active = false;
                    }
                    if (IsDead && !DeathAnimationStarted) // ถ้าศัตรูตายและยังไม่เริ่มแอนิเมชันตาย
                    {
                        DeathAnimationStarted = true;
                        DeathTimer = 0f;
                    }
                }
            }
            if (DeathAnimationStarted) // ถ้าเริ่มแอนิเมชันตายแล้ว
            {
                HandleDeathAnimation(delta); // เรียกใช้เมธอดจัดการแอนิเมชันตาย
            }
        }

        public override void Draw(SpriteBatch sb, Texture2D hurtBoxTex, Texture2D hitBoxTex, bool flip)
        {
            if (!IsDead || DeathAnimationStarted)
            {
                if (DeathAnimationStarted && IsDead) // ถ้าแอนิเมชันตายกำลังเล่นอยู่
                {
                    Death.DrawFrame(sb, new Vector2(HurtBox.X, HurtBox.Y), IsFlipped);
                }
                else if (IsAttacking)
                {
                    AttackAnim.DrawFrame(sb, new Vector2(HurtBox.X, HurtBox.Y), IsFlipped);
                }
                else
                {
                    Idle.DrawFrame(sb, new Vector2(HurtBox.X, HurtBox.Y), IsFlipped);
                }
                //sb.Draw(hurtBoxTex, HurtBox, Color.Red * 0.4f);
                if (ProjectileObj.Active) // ถ้าโปรเจกไทล์กำลังทำงาน
                {
                    ProjectileObj.Draw(sb); // วาดโปรเจกไทล์
                    //sb.Draw(hitBoxTex, ProjectileObj.HitBox, Color.Blue * 0.4f);
                }
            }
        }
    }
}
