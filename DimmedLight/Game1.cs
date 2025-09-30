using DimmedLight.Animated;
using DimmedLight.Background;
using DimmedLight.Enemies;
using DimmedLight.ETC;
using DimmedLight.Isplayer;
using DimmedLight.Managers;
using DimmedLight.UI;
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
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;

        private Gameplay _gamePlay;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _gamePlay = new Gameplay(this, _graphics);
            base.Initialize();
        }
        //haha lol
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

            base.Draw(gameTime);
        }
    }
}
