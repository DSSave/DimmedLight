using DimmedLight.GamePlay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
    public class SettingScreen : Screen
    {
        public enum SettingSource
        {
            MainMenu,
            PauseMenu
        }
        public static bool ShowTutorial = true;

        #region Assets
        private Texture2D _background;
        private SpriteFont _menuFont;
        private SpriteFont _headingFont;
        private Texture2D _pixelTexture;
        private SoundEffect _testSfx;
        #endregion

        #region Input State
        private MouseState _previousMouseState;
        private KeyboardState _previousKeyboardState;
        private GamePadState _previousGamePadState;
        #endregion

        #region Settings State
        private float _bgVolume = 0.7f;
        private float _sfxVolume = 0.9f;
        private bool _isDraggingBgKnob = false;
        private bool _isDraggingSfxKnob = false;
        #endregion

        // --- Tabs ---
        private enum SettingTab { Mode, Sound, Controller }
        private SettingTab _currentTab = SettingTab.Mode;
        private List<Rectangle> _tabButtons = new List<Rectangle>();
        private List<string> _tabLabels = new List<string>();
        private List<float> _tabAlphas = new List<float>();
        private const float FADE_SPEED = 7f;

        // --- UI Rectangles ---
        private Rectangle _fullscreenCheckbox;
        private Rectangle _windowedCheckbox;
        private Rectangle _tutorialCheckbox;
        private Rectangle _bgSliderBar;
        private Rectangle _sfxSliderBar;
        private Rectangle _bgSliderKnob;
        private Rectangle _sfxSliderKnob;

        // --- UI Positions ---
        private Vector2 _screenHeadingTextPos, _generalHeadingTextPos;
        private Vector2 _fullscreenTextPosition, _windowedTextPosition, _tutorialTextPosition;
        private Vector2 _bgTextPosition, _sfxTextPosition;
        private Vector2 _keyboardTextPosition, _xboxControllerTextPos;
        private Vector2 _jumpActionTextPos, _attackActionTextPos, _parryActionTextPos, _ultimateActionTextPos;
        private Vector2 _jumpKeyTextPos, _attackKeyTextPos, _parryKeyTextPos, _ultimateKeyTextPos;

        // --- Controller Navigation State ---
        private int _selectedModeIndex = 0;
        private int _selectedSoundIndex = 0;
        private int _selectedTabIndex = 0;
        private bool _isNavigatingTabs = true;
        private int _selectedControllerSubIndex = 0;
        private int _selectedControllerIndex = 0;

        // --- Option Alphas for Highlighting ---
        private float[] _modeOptionAlphas = new float[3];
        private float[] _soundOptionAlphas = new float[2];

        // --- Key Mappings ---
        private Keys[] _keyMappings = new Keys[4] { Keys.Space, Keys.D, Keys.J, Keys.F };
        private Buttons[] _gamepadMappings = new Buttons[4] { Buttons.A, Buttons.X, Buttons.LeftTrigger, Buttons.LeftShoulder };

        // --- state ---
        private SettingSource _source;
        private Gameplay _previousGameplay;

        public SettingScreen(Game1 game, GraphicsDeviceManager graphicsDeviceManager, GraphicsDevice graphicsDevice, ContentManager content, SettingSource source, Gameplay previousGameplay = null)
    : base(game, graphicsDeviceManager, graphicsDevice, content)
        {
            _source = source;
            _previousGameplay = previousGameplay;
        }

        public override void LoadContent()
        {
            _background = Content.Load<Texture2D>("UX_UIAsset/mainmenu_page/Background");
            //_menuFont = Content.Load<SpriteFont>("UX_UIAsset/Font/TextFont");
            //_headingFont = Content.Load<SpriteFont>("UX_UIAsset/Font/TextFont"); // คุณสามารถเปลี่ยนไปใช้ Font อื่นสำหรับหัวข้อได้ที่นี่
            _menuFont = Content.Load<SpriteFont>("gameFont");
            _headingFont = Content.Load<SpriteFont>("gameFont");
            _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });

            _bgVolume = SoundManager.BgmVolume;
            _sfxVolume = SoundManager.SfxVolume;

            CalculateLayout();
            UpdateSliderKnobs();

            _previousMouseState = Mouse.GetState();
            _previousKeyboardState = Keyboard.GetState();
            _previousGamePadState = GamePad.GetState(PlayerIndex.One);

            _selectedModeIndex = GraphicsDeviceManager.IsFullScreen ? 0 : 1;
        }

        private void AddTab(Rectangle bounds, string label)
        {
            _tabButtons.Add(bounds);
            _tabLabels.Add(label);
            _tabAlphas.Add(0f);
        }

        // --- HELPER FUNCTION for vertical text alignment ---
        private Vector2 AlignTextToRectangle(Rectangle rect, string text, SpriteFont font, int horizontalOffset)
        {
            var textSize = font.MeasureString(text);
            return new Vector2(
                rect.Right + horizontalOffset,
                rect.Y + (rect.Height / 2) - (textSize.Y / 2)
            );
        }

        private void CalculateLayout()
        {
            var menuStartPos = new Vector2(100, 200);
            int menuSpacing = 60;

            AddTab(new Rectangle((int)menuStartPos.X, (int)menuStartPos.Y, 250, 50), "MODE");
            AddTab(new Rectangle((int)menuStartPos.X, (int)menuStartPos.Y + menuSpacing, 250, 50), "SOUND");
            AddTab(new Rectangle((int)menuStartPos.X, (int)menuStartPos.Y + (menuSpacing * 2), 250, 50), "CONTROLLER");

            int rightColumnX = 450;
            int rightColumnYStart = 200;

            // --- Spacing variables for better control ---
            int headingToOptionSpacing = 50;
            int optionToOptionSpacing = 60;
            int groupSpacing = 80;
            int optionIndent = 30;
            int checkboxSize = 24;
            int textOffset = 20;

            int y = rightColumnYStart;

            // --- Mode Tab Layout ---
            _screenHeadingTextPos = new Vector2(rightColumnX, y);
            y += headingToOptionSpacing;
            _fullscreenCheckbox = new Rectangle(rightColumnX + optionIndent, y, checkboxSize, checkboxSize);
            _fullscreenTextPosition = AlignTextToRectangle(_fullscreenCheckbox, "Fullscreen", _menuFont, textOffset);

            y += optionToOptionSpacing;
            _windowedCheckbox = new Rectangle(rightColumnX + optionIndent, y, checkboxSize, checkboxSize);
            _windowedTextPosition = AlignTextToRectangle(_windowedCheckbox, "Windowed", _menuFont, textOffset);

            y += groupSpacing;
            _generalHeadingTextPos = new Vector2(rightColumnX, y);

            y += headingToOptionSpacing;
            _tutorialCheckbox = new Rectangle(rightColumnX + optionIndent, y, checkboxSize, checkboxSize);
            _tutorialTextPosition = AlignTextToRectangle(_tutorialCheckbox, "Show Tutorial on Start", _menuFont, textOffset);

            // --- Sound Tab Layout ---
            y = rightColumnYStart;
            _bgTextPosition = new Vector2(rightColumnX, y);
            _bgSliderBar = new Rectangle(rightColumnX + 250, y + 15, 300, 5); // Increased X offset for better spacing
            y += optionToOptionSpacing;
            _sfxTextPosition = new Vector2(rightColumnX, y);
            _sfxSliderBar = new Rectangle(rightColumnX + 250, y + 15, 300, 5);

            // --- Controller Tab Layout ---
            y = rightColumnYStart;
            int rowHeight = 70; // Specific to this tab
            _keyboardTextPosition = new Vector2(rightColumnX, y);
            _xboxControllerTextPos = new Vector2(rightColumnX + 280, y);
            y += rowHeight;
            int actionColX = rightColumnX;
            int keyColX = rightColumnX + 280;
            _jumpActionTextPos = new Vector2(actionColX, y);
            _jumpKeyTextPos = new Vector2(keyColX, y);
            y += rowHeight;
            _attackActionTextPos = new Vector2(actionColX, y);
            _attackKeyTextPos = new Vector2(keyColX, y);
            y += rowHeight;
            _parryActionTextPos = new Vector2(actionColX, y);
            _parryKeyTextPos = new Vector2(keyColX, y);
            y += rowHeight;
            _ultimateActionTextPos = new Vector2(actionColX, y);
            _ultimateKeyTextPos = new Vector2(keyColX, y);
        }

        public override void Update(GameTime gameTime)
        {
            var mouse = Mouse.GetState();
            var keyboard = Keyboard.GetState();
            var gamePad = GamePad.GetState(PlayerIndex.One);
            var mousePos = new Point(mouse.X, mouse.Y);

            bool movedRight = (keyboard.IsKeyDown(Keys.Right) && _previousKeyboardState.IsKeyUp(Keys.Right)) ||
                             (gamePad.ThumbSticks.Left.X > 0.5f && _previousGamePadState.ThumbSticks.Left.X <= 0.5f) ||
                             (gamePad.IsButtonDown(Buttons.DPadRight) && _previousGamePadState.IsButtonUp(Buttons.DPadRight));
            bool movedLeft = (keyboard.IsKeyDown(Keys.Left) && _previousKeyboardState.IsKeyUp(Keys.Left)) ||
                             (gamePad.ThumbSticks.Left.X < -0.5f && _previousGamePadState.ThumbSticks.Left.X >= -0.5f) ||
                             (gamePad.IsButtonDown(Buttons.DPadLeft) && _previousGamePadState.IsButtonUp(Buttons.DPadLeft));
            bool movedDown = (keyboard.IsKeyDown(Keys.Down) && _previousKeyboardState.IsKeyUp(Keys.Down)) ||
                             (gamePad.ThumbSticks.Left.Y < -0.5f && _previousGamePadState.ThumbSticks.Left.Y >= -0.5f) ||
                             (gamePad.IsButtonDown(Buttons.DPadDown) && _previousGamePadState.IsButtonUp(Buttons.DPadDown));
            bool movedUp = (keyboard.IsKeyDown(Keys.Up) && _previousKeyboardState.IsKeyUp(Keys.Up)) ||
                           (gamePad.ThumbSticks.Left.Y > 0.5f && _previousGamePadState.ThumbSticks.Left.Y <= 0.5f) ||
                           (gamePad.IsButtonDown(Buttons.DPadUp) && _previousGamePadState.IsButtonUp(Buttons.DPadUp));
            bool isConfirmPressed = (keyboard.IsKeyDown(Keys.Enter) && _previousKeyboardState.IsKeyUp(Keys.Enter)) ||
                                     (gamePad.IsButtonDown(Buttons.A) && _previousGamePadState.IsButtonUp(Buttons.A)) ||
                                     (mouse.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released);
            bool goBack = (keyboard.IsKeyDown(Keys.Escape) && _previousKeyboardState.IsKeyUp(Keys.Escape)) ||
                          (gamePad.IsButtonDown(Buttons.B) && _previousGamePadState.IsButtonUp(Buttons.B));

            if (goBack)
            {
                if (_source == SettingSource.MainMenu)
                    Game.ChangeScreen(new MenuScreen(Game, Game._graphics, GraphicsDevice, Content));
                else if (_source == SettingSource.PauseMenu)
                {
                    ((Game1)Game).SettingScreenWasOpen = true;
                    Game.ChangeScreen(new GameplayScreen(Game, Game._graphics, GraphicsDevice, Content, _previousGameplay));
                }
            }

            if (movedDown) _selectedTabIndex = (_selectedTabIndex + 1) % _tabButtons.Count;
            if (movedUp) _selectedTabIndex = (_selectedTabIndex - 1 + _tabButtons.Count) % _tabButtons.Count;
            _currentTab = (SettingTab)_selectedTabIndex;
            for (int i = 0; i < _tabButtons.Count; i++)
            {
                if (_tabButtons[i].Contains(mousePos) && isConfirmPressed)
                {
                    _selectedTabIndex = i;
                    _currentTab = (SettingTab)i;
                }
            }
            switch (_currentTab)
            {
                case SettingTab.Mode:
                    HandleModeTabInput(isConfirmPressed, movedUp, movedDown, mousePos, mouse);
                    break;
                case SettingTab.Sound:
                    HandleSoundTabInput(movedUp, movedDown, movedLeft, movedRight, mouse);
                    break;
                case SettingTab.Controller:
                    HandleControllerTabInput(movedUp, movedDown, movedLeft, movedRight, mousePos);
                    break;
            }

            UpdateAlphas(gameTime);

            _previousMouseState = mouse;
            _previousKeyboardState = keyboard;
            _previousGamePadState = gamePad;
        }

        private void HandleModeTabInput(bool confirm, bool up, bool down, Point mousePos, MouseState mouse)
        {
            if (mouse.X != _previousMouseState.X || mouse.Y != _previousMouseState.Y || confirm)
            {
                if (_fullscreenCheckbox.Contains(mousePos)) _selectedModeIndex = 0;
                else if (_windowedCheckbox.Contains(mousePos)) _selectedModeIndex = 1;
                else if (_tutorialCheckbox.Contains(mousePos)) _selectedModeIndex = 2;
            }

            if (up) _selectedModeIndex = (_selectedModeIndex - 1 + 3) % 3;
            if (down) _selectedModeIndex = (_selectedModeIndex + 1) % 3;

            if (confirm || Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                ApplySelectedModeOption();
            }
        }

        private void HandleSoundTabInput(bool up, bool down, bool left, bool right, MouseState mouse)
        {
            if (mouse.LeftButton != ButtonState.Pressed && (mouse.X != _previousMouseState.X || mouse.Y != _previousMouseState.Y))
            {
                Rectangle bgArea = new Rectangle(_bgTextPosition.ToPoint(), new Point(600, 50));
                Rectangle sfxArea = new Rectangle(_sfxTextPosition.ToPoint(), new Point(600, 50));

                if (bgArea.Contains(mouse.Position)) _selectedSoundIndex = 0;
                else if (sfxArea.Contains(mouse.Position)) _selectedSoundIndex = 1;
            }

            if (up) _selectedSoundIndex = 0;
            if (down) _selectedSoundIndex = 1;

            float oldBgVolume = _bgVolume;
            float oldSfxVolume = _sfxVolume;

            if (left || right)
            {
                float change = left ? -0.05f : 0.05f;
                if (_selectedSoundIndex == 0) _bgVolume = Math.Clamp(_bgVolume + change, 0, 1);
                else _sfxVolume = Math.Clamp(_sfxVolume + change, 0, 1);
            }

            if (mouse.LeftButton == ButtonState.Pressed)
            {
                if (_bgSliderKnob.Contains(mouse.Position) || _bgSliderBar.Contains(mouse.Position)) _isDraggingBgKnob = true;
                if (_sfxSliderKnob.Contains(mouse.Position) || _sfxSliderBar.Contains(mouse.Position)) _isDraggingSfxKnob = true;
            }
            else
            {
                _isDraggingBgKnob = false;
                _isDraggingSfxKnob = false;
            }

            if (_isDraggingBgKnob) _bgVolume = Math.Clamp((float)(mouse.X - _bgSliderBar.X) / _bgSliderBar.Width, 0f, 1f);
            if (_isDraggingSfxKnob) _sfxVolume = Math.Clamp((float)(mouse.X - _sfxSliderBar.X) / _sfxSliderBar.Width, 0f, 1f);

            UpdateSliderKnobs();

            bool bgChanged = Math.Abs(oldBgVolume - _bgVolume) > float.Epsilon;
            bool sfxChanged = Math.Abs(oldSfxVolume - _sfxVolume) > float.Epsilon;
            if (bgChanged)
            {
                SoundManager.BgmVolume = _bgVolume;
                // Since menu screens don't have BGM, this change will be applied when GameplayScreen is loaded/resumed.
                // If there was menu music, we would set MediaPlayer.Volume here.
            }

            if (sfxChanged)
            {
                SoundManager.SfxVolume = _sfxVolume;
                bool discreteChange = left || right || up || down;
                bool dragReleased = (_isDraggingSfxKnob && mouse.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed);

                if ((discreteChange || dragReleased) && _testSfx != null)
                {
                    _testSfx.Play(SoundManager.SfxVolume, 0f, 0f);
                }
            }
        }

        private void HandleControllerTabInput(bool up, bool down, bool left, bool right, Point mousePos)
        {
            if (left) _selectedControllerIndex = 0;
            if (right) _selectedControllerIndex = 1;

            Rectangle keyboardRect = new Rectangle((int)_keyboardTextPosition.X, (int)_keyboardTextPosition.Y, 150, 40);
            Rectangle xboxRect = new Rectangle((int)_xboxControllerTextPos.X, (int)_xboxControllerTextPos.Y, 200, 40);
            if (keyboardRect.Contains(mousePos)) _selectedControllerIndex = 0;
            else if (xboxRect.Contains(mousePos)) _selectedControllerIndex = 1;

            if (up) _selectedControllerSubIndex = (_selectedControllerSubIndex - 1 + 4) % 4;
            if (down) _selectedControllerSubIndex = (_selectedControllerSubIndex + 1) % 4;
        }

        private void UpdateAlphas(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < _tabAlphas.Count; i++)
            {
                bool isSelectedTab = (i == _selectedTabIndex && _isNavigatingTabs) ||
                                     (i == (int)_currentTab && !_isNavigatingTabs);
                float targetAlpha = isSelectedTab ? 0.5f : 0f;
                _tabAlphas[i] = MathHelper.Lerp(_tabAlphas[i], targetAlpha, FADE_SPEED * dt);
            }

            if (!_isNavigatingTabs)
            {
                for (int i = 0; i < _modeOptionAlphas.Length; i++)
                {
                    float target = (_currentTab == SettingTab.Mode && i == _selectedModeIndex) ? 0.5f : 0f;
                    _modeOptionAlphas[i] = MathHelper.Lerp(_modeOptionAlphas[i], target, FADE_SPEED * dt);
                }
                for (int i = 0; i < _soundOptionAlphas.Length; i++)
                {
                    float target = (_currentTab == SettingTab.Sound && i == _selectedSoundIndex) ? 0.5f : 0f;
                    _soundOptionAlphas[i] = MathHelper.Lerp(_soundOptionAlphas[i], target, FADE_SPEED * dt);
                }
            }
            else
            {
                for (int i = 0; i < _modeOptionAlphas.Length; i++)
                    _modeOptionAlphas[i] = MathHelper.Lerp(_modeOptionAlphas[i], 0f, FADE_SPEED * dt);
                for (int i = 0; i < _soundOptionAlphas.Length; i++)
                    _soundOptionAlphas[i] = MathHelper.Lerp(_soundOptionAlphas[i], 0f, FADE_SPEED * dt);
            }
        }

        private void ApplySelectedModeOption()
        {
            switch (_selectedModeIndex)
            {
                case 0:
                case 1:
                    ((Game1)Game).SetFullScreen(_selectedModeIndex == 0);
                    break;
                case 2:
                    ShowTutorial = !ShowTutorial;
                    break;
            }
        }

        private void UpdateSliderKnobs()
        {
            _bgSliderKnob = new Rectangle(
                _bgSliderBar.X + (int)(_bgSliderBar.Width * _bgVolume) - 8,
                _bgSliderBar.Y - 8,
                16, 21);
            _sfxSliderKnob = new Rectangle(
                _sfxSliderBar.X + (int)(_sfxSliderBar.Width * _sfxVolume) - 8,
                _sfxSliderBar.Y - 8,
                16, 21);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp); // Use PointClamp for sharp pixels

            spriteBatch.Draw(_background, GraphicsDevice.Viewport.Bounds, Color.White);

            Texture2D blackPixel = new Texture2D(GraphicsDevice, 1, 1);
            blackPixel.SetData(new[] { Color.White });
            spriteBatch.Draw(blackPixel, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.Black * 0.5f);

            for (int i = 0; i < _tabButtons.Count; i++)
            {
                Color baseColor = _currentTab == (SettingTab)i && !_isNavigatingTabs ? Color.Black * 0.5f : Color.Black * 0.3f;
                spriteBatch.Draw(_pixelTexture, _tabButtons[i], baseColor);
                spriteBatch.Draw(_pixelTexture, _tabButtons[i], Color.White * _tabAlphas[i]);

                var textSize = _menuFont.MeasureString(_tabLabels[i]);
                var textPos = new Vector2(
                    _tabButtons[i].X + (_tabButtons[i].Width - textSize.X) / 2,
                    _tabButtons[i].Y + (_tabButtons[i].Height - textSize.Y) / 2
                );
                spriteBatch.DrawString(_menuFont, _tabLabels[i], textPos, Color.White);
            }

            switch (_currentTab)
            {
                case SettingTab.Mode: DrawModeSection(spriteBatch); break;
                case SettingTab.Sound: DrawSoundSection(spriteBatch); break;
                case SettingTab.Controller: DrawControllerSection(spriteBatch); break;
            }

            spriteBatch.End();
        }

        private void DrawModeSection(SpriteBatch spriteBatch)
        {
            // Define colors for text based on selection
            Color fullscreenColor = _selectedModeIndex == 0 && !_isNavigatingTabs ? Color.Yellow : Color.White;
            Color windowedColor = _selectedModeIndex == 1 && !_isNavigatingTabs ? Color.Yellow : Color.White;
            Color tutorialColor = _selectedModeIndex == 2 && !_isNavigatingTabs ? Color.Yellow : Color.White;

            // --- Draw Screen Section ---
            spriteBatch.DrawString(_headingFont, "SCREEN", _screenHeadingTextPos, Color.White * 0.8f);

            spriteBatch.DrawString(_menuFont, "Fullscreen", _fullscreenTextPosition, fullscreenColor);
            DrawBorder(spriteBatch, _fullscreenCheckbox, Color.White, 2);
            spriteBatch.Draw(_pixelTexture, _fullscreenCheckbox, Color.White * _modeOptionAlphas[0]);
            if (GraphicsDeviceManager.IsFullScreen)
            {
                spriteBatch.Draw(_pixelTexture, new Rectangle(_fullscreenCheckbox.X + 5, _fullscreenCheckbox.Y + 5, _fullscreenCheckbox.Width - 10, _fullscreenCheckbox.Height - 10), Color.LimeGreen);
            }

            spriteBatch.DrawString(_menuFont, "Windowed", _windowedTextPosition, windowedColor);
            DrawBorder(spriteBatch, _windowedCheckbox, Color.White, 2);
            spriteBatch.Draw(_pixelTexture, _windowedCheckbox, Color.White * _modeOptionAlphas[1]);
            if (!GraphicsDeviceManager.IsFullScreen)
            {
                spriteBatch.Draw(_pixelTexture, new Rectangle(_windowedCheckbox.X + 5, _windowedCheckbox.Y + 5, _windowedCheckbox.Width - 10, _windowedCheckbox.Height - 10), Color.LimeGreen);
            }

            // --- Draw General Section ---
            spriteBatch.DrawString(_headingFont, "GENERAL", _generalHeadingTextPos, Color.White * 0.8f);

            spriteBatch.DrawString(_menuFont, "Show Tutorial on Start", _tutorialTextPosition, tutorialColor);
            DrawBorder(spriteBatch, _tutorialCheckbox, Color.White, 2);
            spriteBatch.Draw(_pixelTexture, _tutorialCheckbox, Color.White * _modeOptionAlphas[2]);
            if (ShowTutorial)
            {
                spriteBatch.Draw(_pixelTexture, new Rectangle(_tutorialCheckbox.X + 5, _tutorialCheckbox.Y + 5, _tutorialCheckbox.Width - 10, _tutorialCheckbox.Height - 10), Color.LimeGreen);
            }
        }

        private void DrawSoundSection(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_menuFont, "BG Volume", _bgTextPosition, _selectedSoundIndex == 0 && !_isNavigatingTabs ? Color.Yellow : Color.White);
            spriteBatch.Draw(_pixelTexture, _bgSliderBar, Color.LightGray);
            spriteBatch.Draw(_pixelTexture, new Rectangle(_bgSliderBar.X, _bgSliderBar.Y, (int)(_bgSliderBar.Width * _bgVolume), _bgSliderBar.Height), Color.White);
            spriteBatch.Draw(_pixelTexture, _bgSliderKnob, Color.White);

            spriteBatch.DrawString(_menuFont, "SFX Volume", _sfxTextPosition, _selectedSoundIndex == 1 && !_isNavigatingTabs ? Color.Yellow : Color.White);
            spriteBatch.Draw(_pixelTexture, _sfxSliderBar, Color.LightGray);
            spriteBatch.Draw(_pixelTexture, new Rectangle(_sfxSliderBar.X, _sfxSliderBar.Y, (int)(_sfxSliderBar.Width * _sfxVolume), _sfxSliderBar.Height), Color.White);
            spriteBatch.Draw(_pixelTexture, _sfxSliderKnob, Color.White);
        }

        private void DrawControllerSection(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_menuFont, "Keyboard", _keyboardTextPosition, _selectedControllerIndex == 0 ? Color.Yellow : Color.White);
            spriteBatch.DrawString(_menuFont, "Xbox Controller", _xboxControllerTextPos, _selectedControllerIndex == 1 ? Color.Yellow : Color.White);

            var actionLabels = new[] { "Jump:", "Attack:", "Parry:", "Ultimate:" };
            string[] keyMappings = _selectedControllerIndex == 0
                ? Array.ConvertAll(_keyMappings, k => k.ToString())
                : Array.ConvertAll(_gamepadMappings, b => b.ToString());

            for (int i = 0; i < actionLabels.Length; i++)
            {
                var actionPos = new Vector2(_jumpActionTextPos.X, _jumpActionTextPos.Y + i * 70);
                var keyPos = new Vector2(_jumpKeyTextPos.X, _jumpKeyTextPos.Y + i * 70);

                string keyText = keyMappings[i];

                Color textColor = _selectedControllerSubIndex == i ? Color.Yellow : Color.White;
                spriteBatch.DrawString(_menuFont, actionLabels[i], actionPos, textColor);
                spriteBatch.DrawString(_menuFont, keyText, keyPos, textColor);
            }
        }

        private void DrawBorder(SpriteBatch spriteBatch, Rectangle rect, Color color, int thickness)
        {
            spriteBatch.Draw(_pixelTexture, new Rectangle(rect.Left, rect.Top, rect.Width, thickness), color);
            spriteBatch.Draw(_pixelTexture, new Rectangle(rect.Left, rect.Bottom - thickness, rect.Width, thickness), color);
            spriteBatch.Draw(_pixelTexture, new Rectangle(rect.Left, rect.Top, thickness, rect.Height), color);
            spriteBatch.Draw(_pixelTexture, new Rectangle(rect.Right - thickness, rect.Top, thickness, rect.Height), color);
        }
    }
}
