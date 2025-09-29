using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.GamePlay.Background
{
    public class PlatformManager
    {
        public Texture2D PlatformTexture { get; private set; }
        public Texture2D PlatformAsset { get; private set; }
        private List<Vector2> assetPositions = new List<Vector2>();
        private float assetSpawnTimer = 0f;
        private float assetSpawnCooldown = 5f;
        private Random rng = new Random();
        public Vector2[] Positions { get; private set; }

        public PlatformManager(Texture2D texture, Texture2D asset, int copies) // copies = จำนวนชิ้นที่ต้องการ
        {
            PlatformTexture = texture;
            PlatformAsset = asset;
            Positions = new Vector2[copies];
            int w = texture.Width;
            int screenHeight = 1080;
            int y = screenHeight - texture.Height + 115; // Y ต่ำลง 130 
            for (int i = 0; i < Positions.Length; i++) Positions[i] = new Vector2(i * w, y); // วางตำแหน่งแต่ละชิ้นชิดกันแนวนอน
        }

        public void Update(float platformSpeed, float deltaTime)
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

            assetSpawnTimer += deltaTime;
            if(assetSpawnTimer >= assetSpawnCooldown)
            {
                assetSpawnTimer = 0f;
                assetSpawnCooldown = rng.Next(3, 5);
                float x = 1920 + rng.Next(500, 900);
                float y = Positions[0].Y - 150;
                assetPositions.Add(new Vector2(x, y));
            }
            for(int i = assetPositions.Count - 1; i >= 0; i--)
            {
                assetPositions[i] = new Vector2(assetPositions[i].X - platformSpeed, assetPositions[i].Y);
                if(assetPositions[i].X < -PlatformAsset.Width) assetPositions.RemoveAt(i);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (var p in Positions) sb.Draw(PlatformTexture, p, Color.White); // วาดแต่ละชิ้นที่ตำแหน่งที่เก็บไว้
            foreach (var a in assetPositions) sb.Draw(PlatformAsset, a, Color.White);
        }

        public void Reset()
        {
            int w = PlatformTexture.Width;
            int y = (int)Positions[0].Y;
            for (int i = 0; i < Positions.Length; i++) Positions[i] = new Vector2(i * w, y);

            assetPositions.Clear();
            assetSpawnTimer = 0f;
        }
    }
}
