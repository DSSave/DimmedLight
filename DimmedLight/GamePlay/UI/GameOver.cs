using DimmedLight.MainMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.GamePlay.UI
{
    public class GameOver
    {
        private Game1 game;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D gameOverTex;
        private Texture2D reStart;
        private Texture2D restartHolder;
        private Texture2D mainMenu;
        private Texture2D mainMenuHolder;

        private Rectangle restartRect;
        private Rectangle mainMenuRect;

        private MouseState previousMouse;
        private int _selectedIndex = -1;
        private int _previousSelectedIndex = -1;

        private Vector2 pos = Vector2.Zero;

        public bool RestartRequested { get; private set; }
        public GameOver(Game1 game, GraphicsDeviceManager graphics)
        {
            this.game = game;
            _graphics = graphics;
        }
        public void LoadContent()
        {
            gameOverTex = game.Content.Load<Texture2D>("gameOver_แก้");
            reStart = game.Content.Load<Texture2D>("cursorRestart2");
            restartHolder = game.Content.Load<Texture2D>("cursorRestart1");
            mainMenu = game.Content.Load<Texture2D>("cursorMainmenu1");
            mainMenuHolder = game.Content.Load<Texture2D>("cursorMainmenu2");

            int screenW = _graphics.PreferredBackBufferWidth;
            int screenH = _graphics.PreferredBackBufferHeight;
            int centerX = screenW / 2;
            int centerY = screenH / 2;
            restartRect = new Rectangle(centerX - reStart.Width / 2 - 200, centerY + 150, reStart.Width, reStart.Height);
            mainMenuRect = new Rectangle(centerX - mainMenu.Width / 2 + 360, centerY + 150, mainMenu.Width, mainMenu.Height);
        }
        public void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            _previousSelectedIndex = _selectedIndex;
            _selectedIndex = -1;

            if(restartRect.Contains(mouse.Position))
            {
                _selectedIndex = 0;
            }
            else if(mainMenuRect.Contains(mouse.Position))
            {
                _selectedIndex = 1;
            }
            if(_previousSelectedIndex != _selectedIndex && _selectedIndex != -1)
            {
                SoundManager.PlayUIHover();
            }

            if (mouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released)
            {
                if (restartRect.Contains(mouse.Position))
                {
                    SoundManager.PlayUIClick();
                    RestartRequested = true;
                }
                else if (mainMenuRect.Contains(mouse.Position))
                {
                    SoundManager.PlayUIClick();
                    game.ChangeScreen(new MenuScreen(game, _graphics, game.GraphicsDevice, game.Content));
                    RestartRequested = false;
                }
            }

            previousMouse = mouse;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            int screenW = _graphics.PreferredBackBufferWidth;
            int screenH = _graphics.PreferredBackBufferHeight;

            int imgW = gameOverTex.Width;
            int imgH = gameOverTex.Height;

            spriteBatch.Draw(gameOverTex, pos, Color.White);

            MouseState mouse = Mouse.GetState();
            if (restartRect.Contains(mouse.Position))
                spriteBatch.Draw(restartHolder, restartRect, Color.White);
            else
                spriteBatch.Draw(reStart, restartRect, Color.White);

            // วาดปุ่ม Main Menu
            if (mainMenuRect.Contains(mouse.Position))
                spriteBatch.Draw(mainMenuHolder, mainMenuRect, Color.White);
            else
                spriteBatch.Draw(mainMenu, mainMenuRect, Color.White);
        }

        public void Reset()
        {
            RestartRequested = false;
        }
    }
}
