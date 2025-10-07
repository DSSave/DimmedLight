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
        private SpriteFont _menuFont;
        private Texture2D _pixelTexture;
        private Texture2D _exitFrameTexture;
        private Texture2D _buttonFrameTexture;
        private Texture2D _mainBackground;

        private MouseState _previousMouseState;
        private KeyboardState _previousKeyboardState;
        private GamePadState _previousGamePadState;

        private Rectangle _dialogBox;
        private Rectangle _yesButton;
        private Rectangle _noButton;

        private int _selectedButtonIndex = 1; // 0 for Yes, 1 for No (เริ่มที่ No)

        // REMOVED: ไม่ต้องใช้ Alpha fade แล้ว
        // private float[] _buttonAlphas = new float[2] { 0f, 0f };
        // private const float FADE_SPEED = 7f;

        public ExitScreen(Game1 game, GraphicsDeviceManager graphicsDeviceManager, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game, graphicsDeviceManager, graphicsDevice, content)
        {
        }

        public override void LoadContent()
        {
            //_menuFont = Content.Load<SpriteFont>("UX_UI/TextFont"); gameFont
            _mainBackground = Content.Load<Texture2D>("UX_UIAsset/mainmenu_page/Background");
            _menuFont = Content.Load<SpriteFont>("gameFont");
            _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
            _exitFrameTexture = Content.Load<Texture2D>("UX_UIAsset/exit_page/ExitScreen");
            _buttonFrameTexture = Content.Load<Texture2D>("UX_UI/Memu_Frame02");

            // --- CHANGE: ปรับขนาดและตำแหน่งให้คล้ายในรูป ---
            int dialogWidth = 600;
            int dialogHeight = 300;
            int screenCenterX = GraphicsDevice.Viewport.Width / 2;
            int screenCenterY = GraphicsDevice.Viewport.Height / 2;

            // ปรับขนาด
            //_dialogBox = new Rectangle(screenCenterX - dialogWidth / 2, screenCenterY - dialogHeight / 2, dialogWidth, dialogHeight);
            _dialogBox = new Rectangle(screenCenterX - _exitFrameTexture.Width / 2, screenCenterY - _exitFrameTexture.Height / 2, _exitFrameTexture.Width, _exitFrameTexture.Height);

            /*int buttonWidth = 280;
            int buttonHeight = 80;
            int buttonSpacing = 10;*/

            int buttonWidth = 420;
            int buttonHeight = 120;
            int buttonSpacing = 30;
            int buttonX = _dialogBox.Center.X - buttonWidth / 2;

            // ปรับตำแหน่ง Y ของปุ่มให้คล้ายในรูป
            int yesButtonY = _dialogBox.Y + 320; // Y ของ Yes
            int noButtonY = yesButtonY + buttonHeight - buttonSpacing; // Y ของ No

            _yesButton = new Rectangle(buttonX, yesButtonY, buttonWidth, buttonHeight);
            _noButton = new Rectangle(buttonX, noButtonY, buttonWidth, buttonHeight);

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

            bool isUpPressed = (keyboard.IsKeyDown(Keys.Up) && _previousKeyboardState.IsKeyUp(Keys.Up)) ||
                             (gamePad.IsButtonDown(Buttons.DPadUp) && _previousGamePadState.IsButtonUp(Buttons.DPadUp)) ||
                             (gamePad.ThumbSticks.Left.Y > 0.5f && _previousGamePadState.ThumbSticks.Left.Y <= 0.5f);
            bool isDownPressed = (keyboard.IsKeyDown(Keys.Down) && _previousKeyboardState.IsKeyUp(Keys.Down)) ||
                               (gamePad.IsButtonDown(Buttons.DPadDown) && _previousGamePadState.IsButtonUp(Buttons.DPadDown)) ||
                               (gamePad.ThumbSticks.Left.Y < -0.5f && _previousGamePadState.ThumbSticks.Left.Y >= -0.5f);

            bool isBackPressed = (keyboard.IsKeyDown(Keys.Escape) && _previousKeyboardState.IsKeyUp(Keys.Escape)) ||
                                 (gamePad.IsButtonDown(Buttons.B) && _previousGamePadState.IsButtonUp(Buttons.B));

            // Mouse hover selection
            if (mouse.Position != _previousMouseState.Position)
            {
                if (_yesButton.Contains(mousePos)) _selectedButtonIndex = 0;
                else if (_noButton.Contains(mousePos)) _selectedButtonIndex = 1;
            }

            // Keyboard/Gamepad navigation (Up/Down)
            if (isUpPressed || isDownPressed)
            {
                _selectedButtonIndex = (_selectedButtonIndex + 1) % 2;
            }

            // Action on confirmation
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
                    if (_selectedButtonIndex == 0) Game.Exit();
                    else Game.ChangeScreen(new MenuScreen(Game, Game._graphics, GraphicsDevice, Content));
                }
            }

            if (isBackPressed)
            {
                Game.ChangeScreen(new MenuScreen(Game, Game._graphics, GraphicsDevice, Content));
            }

            // REMOVED: ไม่ต้องอัปเดต Alpha แล้ว

            _previousMouseState = mouse;
            _previousKeyboardState = keyboard;
            _previousGamePadState = gamePad;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            //--- Draw: Background ---
            spriteBatch.Draw(_mainBackground, GraphicsDevice.Viewport.Bounds, Color.White);

            // วาด Overlay สีดำโปร่งแสง
            spriteBatch.Draw(_pixelTexture, GraphicsDevice.Viewport.Bounds, Color.Black * 0.7f);

            // --- CHANGE: วาดกรอบ Exit หลักให้โปร่งแสง ---
            spriteBatch.Draw(_exitFrameTexture, _dialogBox, Color.White * 0.85f);

            // --- CHANGE: ตรรกะการวาดปุ่มใหม่ทั้งหมด ---
            if (_selectedButtonIndex == 0) // ถ้าเลือก "Yes"
            {
                // วาด "Yes" พร้อมกรอบ
                spriteBatch.Draw(_buttonFrameTexture, _yesButton, Color.White);
                DrawTextInBox(spriteBatch, _menuFont, "Yes", _yesButton, true);

                // วาด "No" แบบไม่มีกรอบ
                DrawTextInBox(spriteBatch, _menuFont, "No", _noButton, false);
            }
            else // ถ้าเลือก "No"
            {
                // วาด "Yes" แบบไม่มีกรอบ
                DrawTextInBox(spriteBatch, _menuFont, "Yes", _yesButton, false);

                // วาด "No" พร้อมกรอบ
                spriteBatch.Draw(_buttonFrameTexture, _noButton, Color.White);
                DrawTextInBox(spriteBatch, _menuFont, "No", _noButton, true);
            }

            spriteBatch.End();
        }

        // --- CHANGE: ปรับฟังก์ชัน DrawTextInBox เล็กน้อย ---
        private void DrawTextInBox(SpriteBatch spriteBatch, SpriteFont font, string text, Rectangle box, bool isSelected)
        {
            Vector2 size = font.MeasureString(text);
            Vector2 pos = new Vector2(box.Center.X - size.X / 2, box.Center.Y - size.Y / 2);

            // ข้อความที่ไม่ได้เลือกจะจางลงเล็กน้อย
            Color textColor = isSelected ? Color.White : Color.Gray;

            spriteBatch.DrawString(font, text, pos + new Vector2(2, 2), Color.Black * 0.8f); // เงา
            spriteBatch.DrawString(font, text, pos, textColor);
        }
    }
}
