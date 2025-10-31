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
        public readonly GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;

        private Screen _currentScreen;
        public bool SettingScreenWasOpen { get; set; }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1920,
                PreferredBackBufferHeight = 1080,
                IsFullScreen = false
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        protected override void Initialize()
        {
            base.Initialize();
        }
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            SoundManager.LoadUISound(Content);
            ChangeScreen(new MenuScreen(this, _graphics, GraphicsDevice, Content));
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
            _currentScreen = newScreen;
            _currentScreen?.LoadContent();
        }
        public void SetFullScreen(bool fullScreen)
        {
            _graphics.IsFullScreen = fullScreen;
            _graphics.ApplyChanges();
        }
    }
}