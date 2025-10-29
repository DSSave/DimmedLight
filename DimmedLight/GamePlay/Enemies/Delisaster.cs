using DimmedLight.GamePlay.Animated;
using DimmedLight.GamePlay.Isplayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.GamePlay.Enemies
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

        private Vector2 projectileOffSet = new Vector2(305, 750);

        private bool IsDashing = false;
        private bool IsDashReturning = false;
        private Vector2 DashStart;
        private Vector2 DashTarget;
        private float DashForwardSpeed = 650f;
        public float DashReturnSpeed = 25f;
        public bool IsFlipped = false;
        public bool IsInEvent { get; set; } = false;

        public Delisaster()
        {
            Idle = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            Position = new Vector2(-650, -50);
            OriginalPosition = Position;
        }

        public void Load(ContentManager content)
        {
            Idle.Load(content, "delisaster", 1, 1, 15);
        }

        public void Update(float delta, Player player)
        {
            if (player.canWalk)
            {
                if (!IsReturning)
                {
                    IsReturning = true;
                    ReturnTimer = ReturnPos;
                }
                IsFlipped = false;
            }
            if (IsDashing)
            {
                float step = DashForwardSpeed * delta;

                if (!IsDashReturning)
                {
                    Position.X = MathHelper.Min(Position.X + DashForwardSpeed * delta, DashTarget.X);
                    if (Position.X >= DashTarget.X)
                        IsDashReturning = true;
                }
                else
                {
                    Position.X = MathHelper.Max(Position.X - DashReturnSpeed * delta, DashStart.X);
                    if (Position.X <= DashStart.X)
                    {
                        IsDashing = false;
                        IsInEvent = false;
                        Position = DashStart;
                    }
                }

                Idle.UpdateFrame(delta);
                return;
            }
            if (IsInEvent)
            {
                Idle.UpdateFrame(delta);
                IsFlipped = true;
                return;
            }

            if (IsReturning)
            {
                ReturnTimer -= delta;
                float d = 1f - ReturnTimer / ReturnPos;
                d = MathHelper.Clamp(d, 0f, 1f);
                Position.X = MathHelper.Lerp(Position.X, OriginalPosition.X, d);
                if (ReturnTimer <= 0f)
                {
                    IsReturning = false;
                    Position = OriginalPosition;
                }
                IsFlipped = false;
            }
            else
            {
                if (currentOffset < MaxForwardOffset)
                {
                    currentOffset += AppearSpeed * delta;
                    currentOffset = MathHelper.Clamp(currentOffset, 0f, MaxForwardOffset);
                }
                Position.X = OriginalPosition.X + currentOffset;
                IsFlipped = false;
            }
            IsFlipped = false;
            Idle.UpdateFrame(delta);
        }
        public void DashForward(Vector2 target)
        {
            IsDashing = true;
            IsDashReturning = false;
            DashStart = Position;
            DashTarget = target;
            IsInEvent = true;
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
            IsDashing = false;
            IsDashReturning = false;
        }
        public Vector2 ProjectileSpawn()
        {
            return Position + projectileOffSet;
        }

        public void Draw(SpriteBatch sb)
        {
            Idle.DrawFrame(sb, Position, IsFlipped);
        }
    }
}
