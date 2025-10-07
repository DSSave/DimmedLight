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
        Player player;
        public void DrawHealth(SpriteBatch sb, Texture2D boxTex, byte health, Player player)
        {
            for (int i = 0; i < health; i++)
            {
                sb.Draw(boxTex, new Rectangle(((int)player.Position.X + 90) + i * 40, 920, 30, 30), Color.Red);
            }
        }
    }
}
