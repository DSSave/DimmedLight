using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MainMenu_02
{
    public class Game1 : Game
    {
        public GraphicsDeviceManager Graphics { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }

        private Screen _currentScreen;
        private Screen _previousScreen;

        // ตัวแปรสำหรับเก็บขนาดหน้าจอที่ต้องการ
        private int _screenWidth;
        private int _screenHeight;

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // กำหนดความละเอียดหน้าจอที่ต้องการ
            _screenWidth = 1920;
            _screenHeight = 1080;

            Graphics.PreferredBackBufferWidth = _screenWidth;
            Graphics.PreferredBackBufferHeight = _screenHeight;
            Graphics.IsFullScreen = false; // กำหนดค่าเริ่มต้นเป็น Windowed

            Graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // กำหนดหน้าจอเริ่มต้น
            var menuScreen = new MenuScreen(this, Graphics, GraphicsDevice, Content);
            ChangeScreen(menuScreen);
        }

        /// <summary>
        /// เปลี่ยนหน้าจอ
        /// </summary>
        public void ChangeScreen(Screen newScreen)
        {
            _previousScreen = _currentScreen;
            _currentScreen = newScreen;

            // เรียก LoadContent() ของหน้าจอใหม่
            _currentScreen?.LoadContent();
        }

        /// <summary>
        /// เมธอดสำหรับเปลี่ยนโหมดหน้าจอ Fullscreen/Windowed
        /// </summary>
        public void SetFullScreen(bool fullScreen)
        {
            Graphics.IsFullScreen = fullScreen;
            Graphics.ApplyChanges();
        }

        protected override void Update(GameTime gameTime)
        {
            if (_currentScreen != null)
            {
                _currentScreen.Update(gameTime);
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // ล้างหน้าจอด้วยสีปกติเมื่อไม่มีหน้าจอพื้นหลังให้วาด
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // วาดหน้าจอพื้นหลังก่อน ถ้าหน้าจอปัจจุบันเป็น ExitScreen
            if (_currentScreen is ExitScreen && _previousScreen != null)
            {
                _previousScreen.Draw(gameTime, SpriteBatch);
            }

            // วาดหน้าจอปัจจุบัน
            _currentScreen?.Draw(gameTime, SpriteBatch);

            base.Draw(gameTime);
        }
    }
}