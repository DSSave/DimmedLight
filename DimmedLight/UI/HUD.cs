using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.UI
{
    public class HUD
    {
        public void DrawHealth(SpriteBatch sb, Texture2D boxTex, byte health)
        {
            for (int i = 0; i < health; i++)
            {
                sb.Draw(boxTex, new Rectangle(50 + i * 40, 50, 30, 30), Color.Red);
            }
        }
    }
}
