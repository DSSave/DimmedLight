using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimmedLight.GamePlay.Isplayer;

namespace DimmedLight.GamePlay.UI
{
    public class HUD
    {
        public void DrawHealth(SpriteBatch sb, Texture2D healthPlayer, byte health, Player player)
        {
            int startX = (int)player.Position.X + 20;
            int startY = 900;
            int spacing = healthPlayer.Width + 40;
            float scale = 1.5f;
            for (int i = 0; i < health; i++)
            {
                Vector2 iconPosition = new Vector2(startX + i * spacing, startY);

                sb.Draw(healthPlayer, iconPosition, null, Color.White, 0f,                     
                    Vector2.Zero, scale, SpriteEffects.None, 0f                      
                );
            }
        }
    }
}
