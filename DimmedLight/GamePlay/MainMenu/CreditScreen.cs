using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MainMenu_02
{
    public class CreditScreen : Screen
    {
        private Texture2D _mainBackground;
        private Texture2D _creditsOverlay;

        private KeyboardState _previousKeyboardState;
        private GamePadState _previousGamePadState;

        public CreditScreen(Game1 game, GraphicsDeviceManager graphicsDeviceManager, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game, graphicsDeviceManager, graphicsDevice, content)
        {
        }

        public override void LoadContent()
        {
            _mainBackground = Content.Load<Texture2D>("Background");
            _creditsOverlay = Content.Load<Texture2D>("credits");

            _previousKeyboardState = Keyboard.GetState();
            _previousGamePadState = GamePad.GetState(PlayerIndex.One);
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            GamePadState currentGamePadState = GamePad.GetState(PlayerIndex.One);

            // Check for Escape key or B button press to go back
            bool isEscapePressed = currentKeyboardState.IsKeyDown(Keys.Escape) && _previousKeyboardState.IsKeyUp(Keys.Escape);
            bool isBButtonPressed = currentGamePadState.IsButtonDown(Buttons.B) && _previousGamePadState.IsButtonUp(Buttons.B);

            if (isEscapePressed || isBButtonPressed)
            {
                Game.ChangeScreen(new MenuScreen(Game, Game.Graphics, GraphicsDevice, Content));
            }

            _previousKeyboardState = currentKeyboardState;
            _previousGamePadState = currentGamePadState;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            // 1. วาดพื้นหลัง (Background.jpg) ให้เต็มหน้าจอก่อน
            spriteBatch.Draw(_mainBackground, GraphicsDevice.Viewport.Bounds, Color.White);

            // --- CHANGE IS HERE ---
            // 2. กำหนดขนาดและตำแหน่งของกรอบ 16:9 ที่จะวาดภาพเครดิตลงไป
            int destWidth = 1600;
            int destHeight = 900;
            int destX = (GraphicsDevice.Viewport.Width - destWidth) / 2;  // จัดกลางแนวนอน
            int destY = (GraphicsDevice.Viewport.Height - destHeight) / 2; // จัดกลางแนวตั้ง

            Rectangle destinationRectangle = new Rectangle(destX, destY, destWidth, destHeight);

            // 3. สั่งวาดภาพ credits.png ให้พอดีกับกรอบสี่เหลี่ยมที่กำหนดไว้
            spriteBatch.Draw(_creditsOverlay, destinationRectangle, Color.White);

            spriteBatch.End();
        }
    }
}