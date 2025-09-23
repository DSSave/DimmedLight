using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.UI
{
    public class FloatingText
    {
        public string Text;
        public Vector2 Position;
        public Color Color;

        private float lifetime;
        private float elapsed;
        private float speedY;

        public bool IsDead => elapsed >= lifetime;

        public FloatingText(string text, Vector2 position, Color color, float lifetime = 1.0f, float speedY = 30f)
        {
            Text = text;
            Position = position;
            Color = color;
            this.lifetime = lifetime;
            this.speedY = speedY;
            elapsed = 0f;
        }

        public void Update(GameTime gameTime)
        {
            elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            // ขยับขึ้นด้านบน
            Position.Y -= speedY * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch sb, SpriteFont font)
        {
            float alpha = MathHelper.Clamp(1f - (elapsed / lifetime), 0f, 1f);
            sb.DrawString(font, Text, Position, Color * alpha);
        }
    }
}
