using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.MainMenu
{
    public class MenuScreen : Screen
    {
        private SpriteFont _menuFont;
        private SpriteFont Stepalange;
        // private SpriteFont _titleFont; // ลบออก ไม่ได้ใช้แล้ว
        private MouseState _previousMouseState;
        private KeyboardState _previousKeyboardState;
        private GamePadState _previousGamePadState;

        // --- Texture ---
        private Texture2D _backgroundTexture;
        private Texture2D _selectedButtonTexture;
        private Texture2D _titleTexture; // << เพิ่ม: ตัวแปรสำหรับรูปภาพ Title

        private Texture2D _lockChainTexture;
        private Vector2 _lockChainPosition;
        private float _lockChainRotation = 0f;
        private float _lockChainShakeTimer = 0f;
        private const float LockChainShakeDuration = 0.5f;
        // --- ตำแหน่งข้อความ ---
        private Vector2 _titlePosition;

        // --- ระบบปุ่มเมนู ---
        private List<Rectangle> _buttons = new List<Rectangle>();
        private List<string> _buttonLabels = new List<string>();
        private int _selectedButtonIndex = 0;
        private int _previousSelectedButtonIndex = 0;
        private Song _mainMenuMusic;
        public MenuScreen(Game1 game, GraphicsDeviceManager graphicsDeviceManager, GraphicsDevice graphicsDevice, ContentManager content)
                   : base(game, graphicsDeviceManager, graphicsDevice, content)
        {
        }
        public override void LoadContent()
        {
            _menuFont = Content.Load<SpriteFont>("gameFont");
            Stepalange = Content.Load<SpriteFont>("Fonts/StepalangeFont");
            _backgroundTexture = Content.Load<Texture2D>("UX_UIAsset/mainmenu_page/Background");
            _selectedButtonTexture = Content.Load<Texture2D>("UX_UIAsset/cursor/cursor_frame");
            _titleTexture = Content.Load<Texture2D>("UX_UIAsset/mainmenu_page/Title");

            _lockChainTexture = Content.Load<Texture2D>("lockChain");
            // --- ปุ่ม ---
            int buttonWidth = 450;
            int buttonHeight = 90;
            int startX = (int)(GraphicsDevice.Viewport.Width / 2f - buttonWidth / 2f + 590);
            int startY = (int)(GraphicsDevice.Viewport.Height * 0.40f);
            int spacingY = 110;

            AddButton(new Rectangle(startX, startY, buttonWidth, buttonHeight), "PLAY");
            AddButton(new Rectangle(startX, startY + spacingY, buttonWidth, buttonHeight), "UPGRADE");
            AddButton(new Rectangle(startX, startY + spacingY * 2, buttonWidth, buttonHeight), "SETTING");
            AddButton(new Rectangle(startX, startY + spacingY * 3, buttonWidth, buttonHeight), "CREDIT");
            AddButton(new Rectangle(startX, startY + spacingY * 4, buttonWidth, buttonHeight), "EXIT");

            // --- รูปภาพ Title ---
            // << แก้ไข: กำหนดตำแหน่งรูปภาพ
            _titlePosition = new Vector2(
                GraphicsDevice.Viewport.Width / 2f + 580, // ปรับตำแหน่งแกน X ตามต้องการ
                GraphicsDevice.Viewport.Height /2f - _titleTexture.Height + 30    // ปรับตำแหน่งแกน Y ตามต้องการ
            );

            Rectangle upgradeButtonRect = _buttons[1];
            _lockChainPosition = new Vector2(upgradeButtonRect.Center.X, upgradeButtonRect.Center.Y - 6);

            _mainMenuMusic = Content.Load<Song>("Audio/MainMenu");
            if (MediaPlayer.State != MediaState.Playing)
            {
                MediaPlayer.Play(_mainMenuMusic);
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Volume = 0.07f * SoundManager.BgmVolume;
            }

            _previousMouseState = Mouse.GetState();
            _previousKeyboardState = Keyboard.GetState();
            _previousGamePadState = GamePad.GetState(PlayerIndex.One);
        }

        private void AddButton(Rectangle bounds, string label)
        {
            _buttons.Add(bounds);
            _buttonLabels.Add(label);
        }

        public override void Update(GameTime gameTime)
        {
            var mouse = Mouse.GetState();
            var keyboard = Keyboard.GetState();
            var gamePad = GamePad.GetState(PlayerIndex.One);

            _previousSelectedButtonIndex = _selectedButtonIndex;

            // --- เลื่อนลง/ขึ้น ---
            bool movedDown = keyboard.IsKeyDown(Keys.Down) && _previousKeyboardState.IsKeyUp(Keys.Down) ||
                                gamePad.IsButtonDown(Buttons.DPadDown) && _previousGamePadState.IsButtonUp(Buttons.DPadDown);
            bool movedUp = keyboard.IsKeyDown(Keys.Up) && _previousKeyboardState.IsKeyUp(Keys.Up) ||
                                gamePad.IsButtonDown(Buttons.DPadUp) && _previousGamePadState.IsButtonUp(Buttons.DPadUp);

            //--- curent state ---

            //setteingSource = SettingSource.MainMenu;

            if (movedDown)
            {
                _selectedButtonIndex = (_selectedButtonIndex + 1) % _buttons.Count;
            }
            else if (movedUp)
            {
                _selectedButtonIndex--;
                if (_selectedButtonIndex < 0) _selectedButtonIndex = _buttons.Count - 1;
            }

            // --- เมาส์ชี้ ---
            var mousePos = new Point(mouse.X, mouse.Y);
            if (mouse.X != _previousMouseState.X || mouse.Y != _previousMouseState.Y)
            {
                for (int i = 0; i < _buttons.Count; i++)
                {
                    if (_buttons[i].Contains(mousePos))
                    {
                        _selectedButtonIndex = i;
                        break;
                    }
                }
            }
            if(_previousSelectedButtonIndex != _selectedButtonIndex)
            {
                SoundManager.PlayUIHover();
            }

            // --- ยืนยัน ---
            bool isConfirmPressed = mouse.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released ||
                                      keyboard.IsKeyDown(Keys.Enter) && _previousKeyboardState.IsKeyUp(Keys.Enter) ||
                                      gamePad.IsButtonDown(Buttons.A) && _previousGamePadState.IsButtonUp(Buttons.A);

            if (isConfirmPressed)
            {
                bool wasClicked = mouse.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released;
                if (!wasClicked || wasClicked && _buttons[_selectedButtonIndex].Contains(mousePos))
                {
                    SoundManager.PlayUIClick();
                    ExecuteButtonAction(_selectedButtonIndex);
                }
            }

            if (_lockChainShakeTimer > 0f)
            {
                _lockChainShakeTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                _lockChainRotation = (float)Math.Sin(_lockChainShakeTimer * 30f) * 0.2f;

                if (_lockChainShakeTimer <= 0f)
                {
                    _lockChainRotation = 0f;
                }
            }

            _previousMouseState = mouse;
            _previousKeyboardState = keyboard;
            _previousGamePadState = gamePad;
        }

        private void ExecuteButtonAction(int buttonIndex)
        {
            switch (buttonIndex)
            {
                case 0: // Play
                    MediaPlayer.Stop();
                    if (SettingScreen.ShowTutorial)
                        Game.ChangeScreen(new TutorialScreen(Game, Game._graphics, GraphicsDevice, Content));
                    else
                    {
                        Game.ChangeScreen(new GameplayScreen(Game, Game._graphics, GraphicsDevice, Content));
                    }
                    break;
                case 1: // Upgrade
                    //Game.ChangeScreen(new UpgradeScreen(Game, Game._graphics, GraphicsDevice, Content));
                    _lockChainShakeTimer = LockChainShakeDuration; // <-- เพิ่ม: เริ่มการสั่น
                    SoundManager.PlayUIHover();
                    break;
                case 2: // Setting
                    //Game.ChangeScreen(new SettingScreen(Game, Game._graphics, GraphicsDevice, Content, SettingSource.MainMenu));
                    Game.ChangeScreen(new SettingScreen(Game, Game._graphics, GraphicsDevice, Content, SettingScreen.SettingSource.MainMenu));
                    break;
                case 3: // Credit
                    Game.ChangeScreen(new CreditScreen(Game, Game._graphics, GraphicsDevice, Content));
                    break;
                case 4: // Exit
                    Game.ChangeScreen(new ExitScreen(Game, Game._graphics, GraphicsDevice, Content));
                    break;
            }
        }

        private void DrawStringWithShadow(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, float scale, Color color, Color shadowColor)
        {
            float shadowOffset = 2.0f * scale;

            // --- วาดเงา ---
            spriteBatch.DrawString(font, text, position + new Vector2(shadowOffset, shadowOffset), shadowColor, 0f, font.MeasureString(text) / 2f, scale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(font, text, position + new Vector2(-shadowOffset, shadowOffset), shadowColor, 0f, font.MeasureString(text) / 2f, scale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(font, text, position + new Vector2(shadowOffset, -shadowOffset), shadowColor, 0f, font.MeasureString(text) / 2f, scale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(font, text, position + new Vector2(-shadowOffset, -shadowOffset), shadowColor, 0f, font.MeasureString(text) / 2f, scale, SpriteEffects.None, 0f);

            // --- วาดข้อความหลัก ---
            spriteBatch.DrawString(font, text, position, color, 0f, font.MeasureString(text) / 2f, scale, SpriteEffects.None, 0f);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            // --- พื้นหลัง ---
            spriteBatch.Draw(_backgroundTexture, GraphicsDevice.Viewport.Bounds, Color.White);

            Texture2D blackPixel = new Texture2D(GraphicsDevice, 1, 1);
            blackPixel.SetData(new[] { Color.White });
            spriteBatch.Draw(blackPixel, new Rectangle(1250, 0, GraphicsDevice.Viewport.Width / 2 + 730, GraphicsDevice.Viewport.Height), Color.Black * 0.5f);

            // --- Title Image ---
            // << แก้ไข: วาดรูปภาพแทนข้อความ LOOP และ DIMMEDLIGHT
            spriteBatch.Draw(
                _titleTexture,
                _titlePosition,
                null,
                Color.White,
                0f,
                new Vector2(_titleTexture.Width / 2f, _titleTexture.Height / 2f), // ตั้งจุดหมุนไว้กึ่งกลางรูป
                1.0f, // ขนาดของรูปภาพ (ปรับได้)
                SpriteEffects.None,
                0f
            );

            // --- ปุ่ม ---
            for (int i = 0; i < _buttons.Count; i++)
            {
                Rectangle buttonRect = _buttons[i];
                string buttonLabel = _buttonLabels[i];

                if (i == _selectedButtonIndex)
                {
                    spriteBatch.Draw(_selectedButtonTexture, buttonRect, Color.White);
                }

                Vector2 textSize = Stepalange.MeasureString(buttonLabel);
                spriteBatch.DrawString(
                    Stepalange,
                    buttonLabel,
                    new Vector2(buttonRect.Center.X, buttonRect.Center.Y),
                    Color.White,
                    0f,
                    textSize / 2f,
                    0.9f,
                    SpriteEffects.None,
                    0f
                );
            }
            Vector2 lockOrigin = new Vector2(_lockChainTexture.Width / 2f, _lockChainTexture.Height / 2f);
            spriteBatch.Draw(
                _lockChainTexture,
                _lockChainPosition,
                null,
                Color.White,
                _lockChainRotation,
                lockOrigin,
                0.7f, // ขนาด
                SpriteEffects.None,
                0f
            );

            spriteBatch.End();
        }
    }
}
