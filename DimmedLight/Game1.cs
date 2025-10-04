using DimmedLight.GamePlay;
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

        private Screen _currentScreen;

        // --- FIX #2: สร้าง Public Property ให้คลาสอื่นเรียกใช้ _graphics ได้ ---
        public GraphicsDeviceManager Graphics => _graphics;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
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

            // เปลี่ยนหน้าจอตอนเริ่มต้น
            ChangeScreen(new MenuScreen(this, _graphics, GraphicsDevice, Content));
        }

        protected override void Update(GameTime gameTime)
        {
            if (_currentScreen != null)
            {
                _currentScreen.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (_currentScreen != null)
            {
                _currentScreen.Draw(gameTime, _spriteBatch);
            }

            base.Draw(gameTime);
        }
    }
}
