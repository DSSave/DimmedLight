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
    public class PauseMenu 
    {
        private Texture2D pauseMenu;
        private Texture2D holder;
        private SpriteFont font;
        private GraphicsDevice graphics;

        public Action ClickRestart;
        public Action ClickExit;
        public Action ClickOption;
        public bool IsPaused { get; private set; } = false;
        private bool inExitMenu = false;

        private List<(string text, Rectangle rect, Action onClick)> menuItems;
        private List<(string text, Rectangle rect, Action onClick)> exitMenuItems;

        public PauseMenu(GraphicsDevice graphics, SpriteFont font, Texture2D pauseImage, Texture2D bottonCursor)
        {
            this.graphics = graphics;
            this.font = font;
            pauseMenu = pauseImage;
            holder = bottonCursor;

            SetupMenus();

        }
        private void SetupMenus()
        {
            int centerX = graphics.Viewport.Width / 2;
            int startY = graphics.Viewport.Height / 2 - 100;

            int spacing = 90;

            // Pause menu items
            menuItems = new List<(string, Rectangle, Action)>
            {
                ("Resume", new Rectangle(centerX - 150, startY, 300, 50), () => IsPaused = false),
                ("Restart", new Rectangle(centerX - 150, startY + spacing, 300, 50), () => { ClickRestart?.Invoke(); IsPaused = false; }),
                ("Option", new Rectangle(centerX - 150, startY + spacing * 2, 300, 50), () => { ClickOption?.Invoke(); IsPaused = false; }),
                ("Exit", new Rectangle(centerX - 150, startY + spacing * 3, 300, 50), () => { inExitMenu = true; }),
            };

            // Exit menu items
            exitMenuItems = new List<(string, Rectangle, Action)>
            {
                ("Yes", new Rectangle(centerX - 150, startY + spacing - 25, 300, 50), () => { ClickExit?.Invoke(); IsPaused = false; }),
                ("No", new Rectangle(centerX - 150, startY + spacing * 2, 300, 50), () => { inExitMenu = false; }),
            };
        }
        public void Update(KeyboardState keyboardState, KeyboardState previousKeyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyUp(Keys.Escape))
            {
                if (!IsPaused)
                {
                    IsPaused = true;
                    inExitMenu = false;
                }
                else if (IsPaused && inExitMenu)
                {
                    inExitMenu = false;
                }
                else
                {
                    IsPaused = false;
                }
            }

            if (IsPaused)
            {
                MouseState mouse = Mouse.GetState();
                var mousePos = mouse.Position;

                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    if (!inExitMenu)
                    {
                        foreach (var item in menuItems)
                        {
                            if (item.rect.Contains(mousePos))
                            {
                                item.onClick?.Invoke();
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in exitMenuItems)
                        {
                            if (item.rect.Contains(mousePos))
                            {
                                item.onClick?.Invoke();
                                break;
                            }
                        }
                    }
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsPaused) return;

            Texture2D blackPixel = new Texture2D(graphics, 1, 1);
            blackPixel.SetData(new[] { Color.White });
            spriteBatch.Draw(blackPixel, new Rectangle(0, 0, graphics.Viewport.Width, graphics.Viewport.Height), Color.Black * 0.7f);

            spriteBatch.Draw(pauseMenu, new Rectangle(graphics.Viewport.Width / 2 - 576, graphics.Viewport.Height / 2 - 324, 1152, 648), Color.White);

            string title = inExitMenu ? "Exit" : "Game Paused";
            Vector2 titleSize = font.MeasureString(title) * 1.5f;
            Vector2 titlePos = new Vector2((graphics.Viewport.Width - titleSize.X) / 2, graphics.Viewport.Height / 2 - 200);
            spriteBatch.DrawString(font, title, titlePos, Color.Black, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0);

            MouseState mouse = Mouse.GetState();

            if (!inExitMenu)
            {
                foreach (var item in menuItems)
                {
                    if (item.rect.Contains(mouse.Position))
                    {
                        spriteBatch.Draw(holder, item.rect, Color.White);
                    }

                    Vector2 textSize = font.MeasureString(item.text);
                    Vector2 textPos = new Vector2(item.rect.X + (item.rect.Width - textSize.X) / 2, item.rect.Y + (item.rect.Height - textSize.Y) / 2);
                    spriteBatch.DrawString(font, item.text, textPos, Color.Black);
                }
            }
            else
            {
                foreach (var item in exitMenuItems)
                {
                    if (item.rect.Contains(mouse.Position))
                    {
                        spriteBatch.Draw(holder, item.rect, Color.White);
                    }

                    Vector2 textSize = font.MeasureString(item.text);
                    Vector2 textPos = new Vector2(item.rect.X + (item.rect.Width - textSize.X) / 2, item.rect.Y + (item.rect.Height - textSize.Y) / 2);
                    spriteBatch.DrawString(font, item.text, textPos, Color.Black);
                }
            }
        }
    }
}
