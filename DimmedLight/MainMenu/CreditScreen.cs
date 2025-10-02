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
    public class CreditScreen : Screen
    {
        private Texture2D _background;

        // --- Input States ---
        private KeyboardState _previousKeyboardState;
        private GamePadState _previousGamePadState;

        public CreditScreen(Game1 game, GraphicsDeviceManager graphicsDeviceManager, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game, graphicsDeviceManager, graphicsDevice, content)
        {
        }

        public override void LoadContent()
        {
            _background = Content.Load<Texture2D>("MenuAsset/credits");

            // --- FIX: Reset input states on screen load to prevent "leak" ---
            _previousKeyboardState = Keyboard.GetState();
            _previousGamePadState = GamePad.GetState(PlayerIndex.One);
        }

        public override void Update(GameTime gameTime)
        {
            // Get the current keyboard and gamepad states
            KeyboardState currentKeyboardState = Keyboard.GetState();
            GamePadState currentGamePadState = GamePad.GetState(PlayerIndex.One);

            // Check for Escape key or B button press
            bool isEscapePressed = currentKeyboardState.IsKeyDown(Keys.Escape) && _previousKeyboardState.IsKeyUp(Keys.Escape);
            bool isBButtonPressed = currentGamePadState.IsButtonDown(Buttons.B) && _previousGamePadState.IsButtonUp(Buttons.B);

            // If either is pressed, change to the main menu screen
            if (isEscapePressed || isBButtonPressed)
            {
                Game.ChangeScreen(new MenuScreen(Game, Game._graphics, GraphicsDevice, Content));
            }

            // Update previous states for the next frame
            _previousKeyboardState = currentKeyboardState;
            _previousGamePadState = currentGamePadState;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.Draw(_background, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.End();
        }
    }
}
