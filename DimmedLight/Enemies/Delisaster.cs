using DimmedLight.Animated;
using DimmedLight.Isplayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.Enemies
{
    public class Delisaster
    {
        public AnimatedTexture Idle;
        public Vector2 Position;
        public Vector2 OriginalPosition;
        public Vector2 Move = new Vector2(150, 0);
        public float ReturnPos = 400f;
        public float ReturnTimer = 0f;
        public bool IsReturning = false;

        public float AppearSpeed = 150f; // ความเร็วในการขยับไปทางขวาเมื่อเริ่มเกม
        public float RetreatSpeed = 120f; // ความเร็วในการดึงกลับไปตำแหน่งเดิม
        public float MaxForwardOffset = 300f; // ระยะทางสูงสุดที่ Delisaster จะขยับไปทางขวา
        public float currentOffset = 0f;

        private Vector2 projectileOffSet = new Vector2(-50, 398);

        public bool IsInEvent { get; set; } = false;
        public Delisaster()
        {
            Idle = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            Position = new Vector2(-500, 298);
            OriginalPosition = Position;
        }

        public void Load(ContentManager content)
        {
            Idle.Load(content, "BossFollow", 6, 1, 15);
        }

        public void Update(float delta, Player player)
        {
            if (player.canWalk)
            {
                // Player กำลังเดิน → เริ่มดึง Delisaster กลับไปตำแหน่งเดิม
                if (!IsReturning)
                {
                    IsReturning = true;
                    ReturnTimer = ReturnPos; // เริ่มจับเวลาคืนตำแหน่ง
                }
            }

            if (IsInEvent)
            {
                Idle.UpdateFrame(delta);
                return;
            }

            if (IsReturning) // กำลังดึงกลับไปตำแหน่งเดิม
            {
                ReturnTimer -= delta;
                float d = 1f - ReturnTimer / ReturnPos; // คำนวณเปอร์เซ็นต์การดึงกลับ
                d = MathHelper.Clamp(d, 0f, 1f); // จำกัดค่า d ให้อยู่ระหว่าง 0 ถึง 1
                Position.X = MathHelper.Lerp(Position.X, OriginalPosition.X, d); // ใช้ Lerp เพื่อขยับไปยังตำแหน่งเดิมอย่างนุ่มนวล
                if (ReturnTimer <= 0f)
                {
                    IsReturning = false;
                    Position = OriginalPosition;
                }
            }
            else
            {
                // Player กำลัง idle → ขยับไปทางขวาตามระยะ MaxForwardOffset
                if (currentOffset < MaxForwardOffset) // ขยับไปทางขวา
                {
                    currentOffset += AppearSpeed * delta; // เพิ่มค่า currentOffset ตามความเร็วและเวลา
                    currentOffset = MathHelper.Clamp(currentOffset, 0f, MaxForwardOffset); // จำกัดไม่ให้เกิน MaxForwardOffset
                }
                Position.X = OriginalPosition.X + currentOffset; // อัพเดตตำแหน่ง X ของ Delisaster
            }

            Idle.UpdateFrame(delta);
        }

        public void movetToRight(Vector2 target)
        {
            Position = target;
            currentOffset = 0f;
            IsReturning = false;
            IsInEvent = true;
        }
        public void ResetPosition()
        {
            Position = OriginalPosition;
            currentOffset = 0f;
            IsReturning = false;
            IsInEvent = false;
        }
        public Vector2 ProjectileSpawn()
        {
            return Position + projectileOffSet;
        }

        public void Draw(SpriteBatch sb)
        {
            Idle.DrawFrame(sb, Position, false);
        }
    }
}
