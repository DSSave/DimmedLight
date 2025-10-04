using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DimmedLight.Gameplay.MainMenu
{
    public class GameplayScreen : Screen
    {
        private DimmedLight.GamePlay.Gameplay _gameplay;

        public GameplayScreen(Game1 game, GraphicsDeviceManager graphicsDeviceManager, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game, graphicsDeviceManager, graphicsDevice, content)
        {
            _gameplay = new DimmedLight.GamePlay.Gameplay(game, graphicsDeviceManager);
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