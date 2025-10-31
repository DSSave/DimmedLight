using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using DimmedLight.MainMenu;

namespace DimmedLight.GamePlay.UI
{
    public class TutorialEvent
    {
        private Texture2D tutorialImage;
        private GraphicsDevice graphics;
        public bool IsActive { get; private set; } = false;
        private Action onFinish;
        private Texture2D _pixelTexture;
        public TutorialEvent(GraphicsDevice graphics, Texture2D tutorialImage, Action onFinish)
        {
            this.graphics = graphics;
            this.tutorialImage = tutorialImage;
            this.onFinish = onFinish;

            _pixelTexture = new Texture2D(graphics, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }

        public void Start()
        {
            IsActive = true;
        }

        public void Update(KeyboardState keyboardState, KeyboardState previousKeyboardState)
        {
            if (!IsActive) return;

            if (keyboardState.IsKeyDown(Keys.Enter) && previousKeyboardState.IsKeyUp(Keys.Enter))
            {
                SoundManager.PlayUIClick();
                IsActive = false;
                onFinish?.Invoke();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsActive) return;

            spriteBatch.Draw(_pixelTexture, new Rectangle(0, 0, graphics.Viewport.Width, graphics.Viewport.Height), Color.Black * 0.7f);

            // รูป tutorial ขนาด 1427 x 757 ตรงกลาง
            int imageWidth = 1427;
            int imageHeight = 757;
            int x = (graphics.Viewport.Width - imageWidth) / 2;
            int y = (graphics.Viewport.Height - imageHeight) / 2;
            spriteBatch.Draw(tutorialImage, new Rectangle(x, y, imageWidth, imageHeight), Color.White);
        }
    }
}
