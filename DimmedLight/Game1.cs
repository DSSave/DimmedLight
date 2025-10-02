using DimmedLight.GamePlay;
using DimmedLight.MainMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Reflection.Metadata;

namespace DimmedLight
{
    public class Game1 : Game
    {
        public GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        private SpriteFont font;

        private Gameplay _gamePlay;

        private Screen _currentScreen;
        private Screen _previousScreen;

        private int _screenWidth;
        private int _screenHeight;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _gamePlay = new Gameplay(this, _graphics);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            var menuScreen = new MenuScreen(this, _graphics, GraphicsDevice, Content);
            ChangeScreen(menuScreen);
        }
        protected override void Update(GameTime gameTime)
        {
            _currentScreen?.Update(gameTime);
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _currentScreen?.Draw(gameTime, _spriteBatch);
            base.Draw(gameTime);
        }
        public void ChangeScreen(Screen newScreen)
        {
            _previousScreen = _currentScreen;
            _currentScreen = newScreen;

            // เรียก LoadContent() ของหน้าจอใหม่
            _currentScreen?.LoadContent();
        }
        public void SetFullScreen(bool fullScreen)
        {
            _graphics.IsFullScreen = fullScreen;
            _graphics.ApplyChanges();
        }
    }
}
