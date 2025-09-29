using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.GamePlay.UI
{
    public class PauseMenu
    {
        private Texture2D pauseMenu;
        private SpriteFont font;
        private GraphicsDevice graphics;
        private Rectangle hitboxRestart;
        public Action ClickRestart;
        
        public bool IsPaused { get; private set; } = false;
        public PauseMenu(GraphicsDevice graphics, SpriteFont font)
        {
            this.graphics = graphics;
            this.font = font;

            pauseMenu = new Texture2D(graphics, 1, 1);
            pauseMenu.SetData(new[] { Color.Black * 0.75f});
        }
        public void Update(KeyboardState keyboardState, KeyboardState previousKeyboardState)
        {
            hitboxRestart = new Rectangle((graphics.Viewport.Width - 200) / 2, (graphics.Viewport.Height - 300) / 2 + 150, 200, 60);
            MouseState mouseState = Mouse.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyUp(Keys.Escape))
            {
                IsPaused = !IsPaused;
            }
            if(IsPaused)
            {
                if (mouseState.LeftButton == ButtonState.Pressed && hitboxRestart.Contains(mouseState.Position))
                {
                    ClickRestart?.Invoke();
                    IsPaused = false;
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsPaused)
            {
                spriteBatch.Draw(pauseMenu, new Rectangle(660, 240, 600, 600), Color.Black);
                string pauseText = "Game Paused";
                Vector2 textSize = font.MeasureString(pauseText);
                Vector2 position = new Vector2((graphics.Viewport.Width - textSize.X) / 2, (graphics.Viewport.Height - textSize.Y) / 4 + 50);
                spriteBatch.DrawString(font, pauseText, position, Color.White);

                if (hitboxRestart.Contains(Mouse.GetState().Position))
                    spriteBatch.Draw(pauseMenu, hitboxRestart, Color.Red);
                else
                    spriteBatch.Draw(pauseMenu, hitboxRestart, Color.Gray * 0.8f);
                string resetText = "Reset";
                Vector2 resetSize = font.MeasureString(resetText);
                Vector2 resetPos = new Vector2(hitboxRestart.X + (hitboxRestart.Width - resetSize.X) / 2, hitboxRestart.Y + (hitboxRestart.Height - resetSize.Y) / 2);
                spriteBatch.DrawString(font, resetText, resetPos, Color.White);
            }
        }
    }
}
