using DimmedLight.GamePlay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.MainMenu
{
    public class GameplayScreen : Screen
    {
        private Gameplay _gameplay;

        public GameplayScreen(Game1 game, GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game, graphics, graphicsDevice, content)
        {
            _gameplay = new Gameplay(game, graphics);
        }

        public override void LoadContent()
        {
            _gameplay.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            _gameplay.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _gameplay.Draw(gameTime);
        }
    }
}
