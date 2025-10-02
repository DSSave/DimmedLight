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
    public abstract class Screen
    {
        protected GraphicsDeviceManager GraphicsDeviceManager { get; }
        protected ContentManager Content { get; }
        protected GraphicsDevice GraphicsDevice { get; }
        protected Game1 Game { get; }


        public Screen(Game1 game, GraphicsDeviceManager graphicsDeviceManager, GraphicsDevice graphicsDevice, ContentManager content)
        {
            Game = game;
            GraphicsDeviceManager = graphicsDeviceManager;
            GraphicsDevice = graphicsDevice;
            Content = content;
        }
        public abstract void LoadContent();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
