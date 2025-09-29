using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.GamePlay.Enemies
{
    public class Projectile
    {
        public Texture2D Texture;
        public Vector2 Position;
        public Vector2 Direction;
        public float Speed;
        public Rectangle HitBox => new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
        public bool Active = false;

        public bool canParry = false;
        public bool canAttack = true;

        public Projectile(Texture2D texture) { Texture = texture; }

        public void Fire(Vector2 start, Vector2 target, float speed) // start ตำแหน่งที่ยิง target ตำแหน่งที่เล็ง
        {
            Position = start;
            Direction = Vector2.Normalize(target - start); // หาค่าเวกเตอร์ทิศทางจากตำแหน่งเริ่มต้นไปยังเป้าหมาย
            Speed = speed;
            Active = true;
        }

        public void Update(float delta)
        {
            if (!Active) return; // ถ้าไม่ทำงานก็ไม่ต้องอัพเดต
            Position += Direction * Speed * delta;
        }

        public void Draw(SpriteBatch sb)
        {
            if (!Active) return; // ถ้าไม่ทำงานก็ไม่ต้องวาด
            sb.Draw(Texture, Position, Color.White);
        }

        public void Deactivate() // ปิดการทำงานของกระสุน
        {
            Active = false;
        }
    }
}
