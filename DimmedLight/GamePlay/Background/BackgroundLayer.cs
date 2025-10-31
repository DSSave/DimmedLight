using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.GamePlay.Background
{
    public class BackgroundLayer
    {
        public Texture2D Texture { get; private set; }
        public Vector2[] Positions { get; private set; }
        public float SpeedFactor { get; private set; }

        public BackgroundLayer(Texture2D texture, int copies, float speedFactor)
        {
            Texture = texture;
            Positions = new Vector2[copies];
            SpeedFactor = speedFactor;
            for (int i = 0; i < copies; i++) Positions[i] = new Vector2(i * texture.Width, 0);
        }

        public void Update(float platformSpeed)
        {
            float speed = platformSpeed * SpeedFactor;
            for (int i = 0; i < Positions.Length; i++) Positions[i].X -= speed;
            int width = Texture.Width;
            for (int i = 0; i < Positions.Length; i++)
            {
                if (Positions[i].X <= -width)
                {
                    int prev = (i - 1 + Positions.Length) % Positions.Length;
                    if (prev < 0) prev = Positions.Length - 1;
                    Positions[i].X = Positions[prev].X + width;
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (var p in Positions) sb.Draw(Texture, p, Color.White); // วาดแต่ละชิ้นที่ตำแหน่งที่เก็บไว้
        }

        public void Reset()
        {
            for (int i = 0; i < Positions.Length; i++) Positions[i] = new Vector2(i * Texture.Width, 0); // วางตำแหน่งแต่ละชิ้นชิดกันแนวนอน
        }
    }
}
