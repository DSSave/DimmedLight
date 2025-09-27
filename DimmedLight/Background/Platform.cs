using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.Background
{
    public class PlatformManager
    {
        public Texture2D PlatformTexture { get; private set; }
        public Vector2[] Positions { get; private set; }

        public PlatformManager(Texture2D texture, int copies) // copies = จำนวนชิ้นที่ต้องการ
        {
            PlatformTexture = texture;
            Positions = new Vector2[copies];
            int w = texture.Width;
            int screenHeight = 1080;
            int y = screenHeight - texture.Height + 115; // Y ต่ำลง 130 
            for (int i = 0; i < Positions.Length; i++) Positions[i] = new Vector2(i * w, y); // วางตำแหน่งแต่ละชิ้นชิดกันแนวนอน
        }

        public void Update(float platformSpeed)
        {
            int width = PlatformTexture.Width;
            for (int i = 0; i < Positions.Length; i++) Positions[i].X -= platformSpeed; // เคลื่อนตำแหน่ง ไปทางซ้าย ตามความเร็วที่ปรับแล้ว
            for (int i = 0; i < Positions.Length; i++) // วนเช็คแต่ละตำแหน่ง
            {
                if (Positions[i].X <= -width) // ถ้าตำแหน่งนี้เลยขอบซ้ายของหน้าจอไปแล้ว
                {
                    int prev = i - 1;
                    if (prev < 0) prev = Positions.Length - 1;
                    Positions[i].X = Positions[prev].X + width; // ย้ายตำแหน่งนี้ไปต่อท้ายตำแหน่งก่อนหน้า
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (var p in Positions) sb.Draw(PlatformTexture, p, Color.White); // วาดแต่ละชิ้นที่ตำแหน่งที่เก็บไว้
        }

        public void Reset()
        {
            int w = PlatformTexture.Width;
            int y = (int)Positions[0].Y;
            for (int i = 0; i < Positions.Length; i++) Positions[i] = new Vector2(i * w, y);
        }
    }
}
