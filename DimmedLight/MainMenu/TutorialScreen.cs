using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.MainMenu
{
    public class TutorialScreen : Screen
    {
        private Texture2D _tutorialPage1;
        private Texture2D _tutorialPage2;
        private Texture2D _tutorialPage3;
        private int _currentPage = 1;
        private KeyboardState _previousKeyboard;
        private GamePadState _previousGamePad;

        public TutorialScreen(Game1 game, GraphicsDeviceManager graphicsDeviceManager, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game, graphicsDeviceManager, graphicsDevice, content)
        {
        }

        public override void LoadContent()
        {
            _tutorialPage1 = Content.Load<Texture2D>("MenuAsset/keyboardTutorial");
            _tutorialPage2 = Content.Load<Texture2D>("MenuAsset/controllerTutorial");
            _tutorialPage3 = Content.Load<Texture2D>("MenuAsset/enemyTutorial");

            _previousKeyboard = Keyboard.GetState();
            _previousGamePad = GamePad.GetState(PlayerIndex.One);
        }

        public override void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();
            var gamePad = GamePad.GetState(PlayerIndex.One);

            // Handle navigation
            bool advancePage = keyboard.IsKeyDown(Keys.Right) && _previousKeyboard.IsKeyUp(Keys.Right) ||
                               keyboard.IsKeyDown(Keys.Enter) && _previousKeyboard.IsKeyUp(Keys.Enter) ||
                               gamePad.IsButtonDown(Buttons.A) && _previousGamePad.IsButtonUp(Buttons.A) ||
                               gamePad.IsButtonDown(Buttons.DPadRight) && _previousGamePad.IsButtonUp(Buttons.DPadRight) ||
                               gamePad.ThumbSticks.Left.X > 0.5f && _previousGamePad.ThumbSticks.Left.X <= 0.5f;

            bool backPage = keyboard.IsKeyDown(Keys.Left) && _previousKeyboard.IsKeyUp(Keys.Left) ||
                            gamePad.IsButtonDown(Buttons.B) && _previousGamePad.IsButtonUp(Buttons.B) ||
                            gamePad.IsButtonDown(Buttons.DPadLeft) && _previousGamePad.IsButtonUp(Buttons.DPadLeft) ||
                            gamePad.ThumbSticks.Left.X < -0.5f && _previousGamePad.ThumbSticks.Left.X >= -0.5f;

            // Added logic for the Escape key
            bool escapePressed = keyboard.IsKeyDown(Keys.Escape) && _previousKeyboard.IsKeyUp(Keys.Escape);

            if (advancePage)
            {
                if (_currentPage < 3)
                {
                    _currentPage++;
                }
                else
                {
                    Game.ChangeScreen(new GameplayScreen(Game, Game._graphics, GraphicsDevice, Content));
                }
            }
            else if (backPage)
            {
                if (_currentPage > 1)
                {
                    _currentPage--;
                }
                else
                {
                    Game.ChangeScreen(new GameplayScreen(Game, Game._graphics, GraphicsDevice, Content));
                }
            }
            // Check if the escape key was pressed
            else if (escapePressed)
            {
                Game.ChangeScreen(new MenuScreen(Game, Game._graphics, GraphicsDevice, Content));
            }

            _previousKeyboard = keyboard;
            _previousGamePad = gamePad;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            if (_currentPage == 1)
            {
                spriteBatch.Draw(_tutorialPage1, GraphicsDevice.Viewport.Bounds, Color.White);
            }
            else if (_currentPage == 2)
            {
                spriteBatch.Draw(_tutorialPage2, GraphicsDevice.Viewport.Bounds, Color.White);
            }
            else if (_currentPage == 3)
            {
                spriteBatch.Draw(_tutorialPage3, GraphicsDevice.Viewport.Bounds, Color.White);
            }

            spriteBatch.End();
        }
    }
}
