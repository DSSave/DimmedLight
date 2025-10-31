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

            _background = Content.Load<Texture2D>("UX_UIAsset/mainmenu_page/Background");
            _tutorialPage1 = Content.Load<Texture2D>("UX_UIAsset/tutorial_page/Tutorial01_keybord");
            _tutorialPage2 = Content.Load<Texture2D>("UX_UIAsset/tutorial_page/Tutorial02_controller");
            _tutorialPage3 = Content.Load<Texture2D>("UX_UIAsset/tutorial_page/Tutorial03_enemy");

            //_background = Content.Load<Texture2D>("Totorial_Background");
            _tutorialPage1 = Content.Load<Texture2D>("MenuAsset/tutorialKeyboard_New");
            _tutorialPage2 = Content.Load<Texture2D>("MenuAsset/tutorialController_New");
            _tutorialPage3 = Content.Load<Texture2D>("MenuAsset/tutorialEnemy_New");


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
                    MediaPlayer.Stop();
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
                    Game.ChangeScreen(new MenuScreen(Game, Game._graphics, GraphicsDevice, Content));
                }
            }
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

            // 1. วาดพื้นหลังให้เต็มหน้าจอก่อนเสมอ
            spriteBatch.Draw(_background, GraphicsDevice.Viewport.Bounds, Color.White);

            // 2. เลือก Texture ของหน้าปัจจุบันที่จะวาด
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

            // --- CHANGE IS HERE ---
            // 3. บังคับให้วาดภาพลงในพื้นที่สี่เหลี่ยมขนาดเดียวกันเสมอ
            if (currentPageTexture != null)
            {
                // กำหนดขนาดและคำนวณตำแหน่งของกรอบที่จะวาดภาพลงไป
                /*int destWidth = 1600;
                int destHeight = 900;*/

                /*int destWidth = 1578;
                int destHeight = 1116;*/

                /*int destWidth = 1920;
                int destHeight = 1080;*/

                /*int destX = (GraphicsDevice.Viewport.Width - destWidth) / 2;  // จัดกลางแนวนอน
                int destY = (GraphicsDevice.Viewport.Height - destHeight) / 2; // จัดกลางแนวตั้ง*/

                int destX = (GraphicsDevice.Viewport.Width - currentPageTexture.Width) / 2;  // จัดกลางแนวนอน
                int destY = (GraphicsDevice.Viewport.Height - currentPageTexture.Height ) / 2; // จัดกลางแนวตั้ง

                //Rectangle destinationRectangle = new Rectangle(destX, destY, destWidth, destHeight);

                // สั่งวาดภาพให้พอดีกับกรอบสี่เหลี่ยมที่กำหนดไว้
                //spriteBatch.Draw(currentPageTexture, destinationRectangle, Color.White);

                Vector2 position = new Vector2(
                (GraphicsDevice.Viewport.Width - currentPageTexture.Width) / 2,
                (GraphicsDevice.Viewport.Height - currentPageTexture.Height) / 2);

                //spriteBatch.Draw(currentPageTexture, position, Color.White);

                spriteBatch.Draw(currentPageTexture, new Vector2(destX,destY), Color.White);

            }

            spriteBatch.End();
        }
    }
}
