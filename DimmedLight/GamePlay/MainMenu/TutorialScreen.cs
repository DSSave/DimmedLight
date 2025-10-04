using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DimmedLight.Gameplay.MainMenu
{
    public class TutorialScreen : Screen
    {
        private Texture2D _background;
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
            _background = Content.Load<Texture2D>("UX_UI/Totorial_Background");
            _tutorialPage1 = Content.Load<Texture2D>("UX_UI/Tutorial01_04");
            _tutorialPage2 = Content.Load<Texture2D>("UX_UI/Tutorial02_04");
            _tutorialPage3 = Content.Load<Texture2D>("UX_UI/Tutorial03_04");

            _previousKeyboard = Keyboard.GetState();
            _previousGamePad = GamePad.GetState(PlayerIndex.One);
        }

        public override void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();
            var gamePad = GamePad.GetState(PlayerIndex.One);

            // Handle navigation
            bool advancePage = (keyboard.IsKeyDown(Keys.Right) && _previousKeyboard.IsKeyUp(Keys.Right)) ||
                               (keyboard.IsKeyDown(Keys.Enter) && _previousKeyboard.IsKeyUp(Keys.Enter)) ||
                               (gamePad.IsButtonDown(Buttons.A) && _previousGamePad.IsButtonUp(Buttons.A)) ||
                               (gamePad.IsButtonDown(Buttons.DPadRight) && _previousGamePad.IsButtonUp(Buttons.DPadRight)) ||
                               (gamePad.ThumbSticks.Left.X > 0.5f && _previousGamePad.ThumbSticks.Left.X <= 0.5f);

            bool backPage = (keyboard.IsKeyDown(Keys.Left) && _previousKeyboard.IsKeyUp(Keys.Left)) ||
                            (gamePad.IsButtonDown(Buttons.B) && _previousGamePad.IsButtonUp(Buttons.B)) ||
                            (gamePad.IsButtonDown(Buttons.DPadLeft) && _previousGamePad.IsButtonUp(Buttons.DPadLeft)) ||
                            (gamePad.ThumbSticks.Left.X < -0.5f && _previousGamePad.ThumbSticks.Left.X >= -0.5f);

            bool escapePressed = keyboard.IsKeyDown(Keys.Escape) && _previousKeyboard.IsKeyUp(Keys.Escape);

            if (advancePage)
            {
                if (_currentPage < 3)
                {
                    _currentPage++;
                }
                else
                {
                    // ไปหน้า Gameplay เมื่อกด Enter ที่หน้า 3
                    Game.ChangeScreen(new GameplayScreen(Game, Game.Graphics, GraphicsDevice, Content));
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
                    Game.ChangeScreen(new MenuScreen(Game, Game.Graphics, GraphicsDevice, Content));
                }
            }
            else if (escapePressed)
            {
                Game.ChangeScreen(new MenuScreen(Game, Game.Graphics, GraphicsDevice, Content));
            }

            _previousKeyboard = keyboard;
            _previousGamePad = gamePad;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            // วาดพื้นหลังให้เต็มหน้าจอ
            spriteBatch.Draw(_background, GraphicsDevice.Viewport.Bounds, Color.White);

            // เลือก Texture ของหน้าปัจจุบัน
            Texture2D currentPageTexture = null;
            if (_currentPage == 1)
            {
                currentPageTexture = _tutorialPage1;
            }
            else if (_currentPage == 2)
            {
                currentPageTexture = _tutorialPage2;
            }
            else if (_currentPage == 3)
            {
                currentPageTexture = _tutorialPage3;
            }

            // วาดภาพในกรอบขนาดคงที่
            if (currentPageTexture != null)
            {
                int destWidth = 1600;
                int destHeight = 900;
                int destX = (GraphicsDevice.Viewport.Width - destWidth) / 2;
                int destY = (GraphicsDevice.Viewport.Height - destHeight) / 2;

                Rectangle destinationRectangle = new Rectangle(destX, destY, destWidth, destHeight);
                spriteBatch.Draw(currentPageTexture, destinationRectangle, Color.White);
            }

            spriteBatch.End();
        }
    }
}