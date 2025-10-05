using DimmedLight.GamePlay;
using DimmedLight.MainMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Drawing;
using System.Reflection.Metadata;
using DimmedLight.Gameplay.MainMenu;

namespace DimmedLight
{
    public class Game1 : Game //local
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;

        private Screen _currentScreen;

        // --- FIX #2: สร้าง Public Property ให้คลาสอื่นเรียกใช้ _graphics ได้ ---
        public GraphicsDeviceManager Graphics => _graphics;

        private Screen _currentScreen;
        private Screen _previousScreen;

        private int _screenWidth;
        private int _screenHeight;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        public void ChangeScreen(Screen newScreen)
        {
            _currentScreen = newScreen;
            if (Content != null && GraphicsDevice != null)
            {
                _currentScreen.LoadContent();
            }
        }

        // --- FIX #3: สร้างเมธอด SetFullScreen ---
        public void SetFullScreen(bool isFullScreen)
        {
            _graphics.IsFullScreen = isFullScreen;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _gamePlay.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            _gamePlay.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _gamePlay.Draw(gameTime);
            //
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
