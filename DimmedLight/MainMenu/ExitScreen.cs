using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.MainMenu
{
    public class ExitScreen : Screen
    {
        private SpriteFont _menuFont; // แก้ไข: เปลี่ยนชื่อตัวแปร
        private Texture2D _pixelTexture;

        private MouseState _previousMouseState;
        private KeyboardState _previousKeyboardState;
        private GamePadState _previousGamePadState;

        private Rectangle _dialogBox;
        private Rectangle _yesButton;
        private Rectangle _noButton;

        private int _selectedButtonIndex = 1; // 0 for Yes, 1 for No (เริ่มที่ No)

        // --- เพิ่มตัวแปรสำหรับ Alpha-fade highlight ---
        private float[] _buttonAlphas = new float[2] { 0f, 0f };
        private const float FADE_SPEED = 7f;

        public ExitScreen(Game1 game, GraphicsDeviceManager graphicsDeviceManager, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game, graphicsDeviceManager, graphicsDevice, content)
        {
        }

        public override void LoadContent()
        {
            _menuFont = Content.Load<SpriteFont>("gameFont"); // แก้ไข: เปลี่ยนชื่อฟอนต์
            _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });

            int dialogWidth = 400;
            int dialogHeight = 200;
            int screenCenterX = GraphicsDevice.Viewport.Width / 2;
            int screenCenterY = GraphicsDevice.Viewport.Height / 2;

            _dialogBox = new Rectangle(screenCenterX - dialogWidth / 2, screenCenterY - dialogHeight / 2, dialogWidth, dialogHeight);

            int buttonWidth = 100;
            int buttonHeight = 50;
            int buttonY = _dialogBox.Bottom - buttonHeight - 30;
            int buttonSpacing = 40; // ระยะห่างระหว่างปุ่ม
            _yesButton = new Rectangle(_dialogBox.Center.X - buttonWidth - buttonSpacing / 2, buttonY, buttonWidth, buttonHeight);
            _noButton = new Rectangle(_dialogBox.Center.X + buttonSpacing / 2, buttonY, buttonWidth, buttonHeight);

            _previousMouseState = Mouse.GetState();
            _previousKeyboardState = Keyboard.GetState();
            _previousGamePadState = GamePad.GetState(PlayerIndex.One);
        }

        public override void Update(GameTime gameTime)
        {
            var mouse = Mouse.GetState();
            var keyboard = Keyboard.GetState();
            var gamePad = GamePad.GetState(PlayerIndex.One);

            var mousePos = new Point(mouse.X, mouse.Y);
            bool isMouseClicked = mouse.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released;

            bool isConfirmPressed = (keyboard.IsKeyDown(Keys.Enter) && _previousKeyboardState.IsKeyUp(Keys.Enter)) ||
                                    (gamePad.IsButtonDown(Buttons.A) && _previousGamePadState.IsButtonUp(Buttons.A));
            bool isLeftPressed = (keyboard.IsKeyDown(Keys.Left) && _previousKeyboardState.IsKeyUp(Keys.Left)) ||
                                 (gamePad.IsButtonDown(Buttons.DPadLeft) && _previousGamePadState.IsButtonUp(Buttons.DPadLeft)) ||
                                 (gamePad.ThumbSticks.Left.X < -0.5f && _previousGamePadState.ThumbSticks.Left.X >= -0.5f);
            bool isRightPressed = (keyboard.IsKeyDown(Keys.Right) && _previousKeyboardState.IsKeyUp(Keys.Right)) ||
                                  (gamePad.IsButtonDown(Buttons.DPadRight) && _previousGamePadState.IsButtonUp(Buttons.DPadRight)) ||
                                  (gamePad.ThumbSticks.Left.X > 0.5f && _previousGamePadState.ThumbSticks.Left.X <= 0.5f);
            bool isBackPressed = (keyboard.IsKeyDown(Keys.Escape) && _previousKeyboardState.IsKeyUp(Keys.Escape)) ||
                                 (gamePad.IsButtonDown(Buttons.B) && _previousGamePadState.IsButtonUp(Buttons.B));

            // Mouse hover selection
            if (mouse.Position != _previousMouseState.Position)
            {
                if (_yesButton.Contains(mousePos)) _selectedButtonIndex = 0;
                else if (_noButton.Contains(mousePos)) _selectedButtonIndex = 1;
            }

            // Keyboard/Gamepad navigation
            if (isRightPressed || isLeftPressed)
            {
                _selectedButtonIndex = (_selectedButtonIndex + 1) % 2; // สลับระหว่าง 0 และ 1
            }

            // Action on confirmation (Click or Enter/A)
            if (isConfirmPressed || isMouseClicked)
            {
                bool confirmed = false;
                if (isMouseClicked)
                {
                    if (_yesButton.Contains(mousePos)) { _selectedButtonIndex = 0; confirmed = true; }
                    else if (_noButton.Contains(mousePos)) { _selectedButtonIndex = 1; confirmed = true; }
                }
                else
                {
                    confirmed = true;
                }

                if (confirmed)
                {
                    if (_selectedButtonIndex == 0) // Yes
                    {
                        Game.Exit();
                    }
                    else // No
                    {
                        Game.ChangeScreen(new MenuScreen(Game, Game._graphics, GraphicsDevice, Content));
                    }
                }
            }

            // Back button returns to menu
            if (isBackPressed)
            {
                Game.ChangeScreen(new MenuScreen(Game, Game._graphics, GraphicsDevice, Content));
            }

            // Update alphas for fade effect
            for (int i = 0; i < _buttonAlphas.Length; i++)
            {
                float targetAlpha = (i == _selectedButtonIndex) ? 0.5f : 0f;
                _buttonAlphas[i] += (targetAlpha - _buttonAlphas[i]) * FADE_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // Update states for next frame
            _previousMouseState = mouse;
            _previousKeyboardState = keyboard;
            _previousGamePadState = gamePad;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // ไม่ต้อง clear หน้าจอ เพื่อให้เห็นฉากก่อนหน้าเป็นพื้นหลัง
            spriteBatch.Begin();

            // Draw a semi-transparent overlay to dim the background
            spriteBatch.Draw(_pixelTexture, GraphicsDevice.Viewport.Bounds, Color.Black * 0.7f);

            // Draw the dialog box
            spriteBatch.Draw(_pixelTexture, _dialogBox, Color.DarkSlateGray);
            DrawBorder(spriteBatch, _dialogBox, Color.Black, 2);

            // Draw the question text
            string text = "Are you sure?";
            Vector2 textSize = _menuFont.MeasureString(text);
            Vector2 textPosition = new Vector2(_dialogBox.Center.X - textSize.X / 2, _dialogBox.Y + 30);
            spriteBatch.DrawString(_menuFont, text, textPosition, Color.White);

            // --- Draw buttons with alpha-fade highlighting ---
            // Yes Button
            spriteBatch.Draw(_pixelTexture, _yesButton, Color.DarkGreen);
            spriteBatch.Draw(_pixelTexture, _yesButton, Color.White * _buttonAlphas[0]); // Highlight
            DrawTextInBox(spriteBatch, _menuFont, "Yes", _yesButton);

            // No Button
            spriteBatch.Draw(_pixelTexture, _noButton, Color.Maroon);
            spriteBatch.Draw(_pixelTexture, _noButton, Color.White * _buttonAlphas[1]); // Highlight
            DrawTextInBox(spriteBatch, _menuFont, "No", _noButton);

            spriteBatch.End();
        }

        private void DrawTextInBox(SpriteBatch spriteBatch, SpriteFont font, string text, Rectangle box)
        {
            Vector2 size = font.MeasureString(text);
            Vector2 pos = new Vector2(box.Center.X - size.X / 2, box.Center.Y - size.Y / 2);
            spriteBatch.DrawString(font, text, pos, Color.White);
        }

        private void DrawBorder(SpriteBatch spriteBatch, Rectangle rect, Color color, int thickness)
        {
            spriteBatch.Draw(_pixelTexture, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
            spriteBatch.Draw(_pixelTexture, new Rectangle(rect.X, rect.Y + rect.Height - thickness, rect.Width, thickness), color);
            spriteBatch.Draw(_pixelTexture, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
            spriteBatch.Draw(_pixelTexture, new Rectangle(rect.X + rect.Width - thickness, rect.Y, thickness, rect.Height), color);
        }
    }
}
