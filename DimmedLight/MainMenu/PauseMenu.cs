using DimmedLight.GamePlay.ETC;
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
        private Texture2D frameExit;
        private Texture2D holder;
        private SpriteFont font;
        private GraphicsDevice graphics;
        private Texture2D Resume;
        private Texture2D Restart;
        private Texture2D Option;
        private Texture2D Exit;

        public Action ClickRestart;
        public Action ClickExit;
        public Action ClickOption;
        private int selectedIndex = 0;
        private int exitSelectedIndex = 0;
        private int _previousSelectedIndex = 0;
        private int _previousExitSelectedIndex = 0;

        private MouseState _previousMouseState;
        private KeyboardState _previousKeyboardState;
        private GamePadState _previousGamePadState;
        public bool IsPaused { get; set; } = false;
        private bool inExitMenu = false;

        private List<(Texture2D tex, Rectangle rect, Action onClick)> menuItems;
        private List<(string text, Rectangle rect, Action onClick)> exitMenuItems;

        private Camera camera;
        public PauseMenu(GraphicsDevice graphics, SpriteFont font, Texture2D pauseImage, Texture2D frame,Texture2D bottonCursor, Camera camera)
        {
            this.graphics = graphics;
            this.font = font;
            this.camera = camera;
            pauseMenu = pauseImage;
            frameExit = frame;
            holder = bottonCursor;
        }
        public void LoadContent(ContentManager content)
        {
            Resume = content.Load<Texture2D>("MenuAsset/resume");
            Restart = content.Load<Texture2D>("MenuAsset/restart");
            Option = content.Load<Texture2D>("MenuAsset/option");
            Exit = content.Load<Texture2D>("MenuAsset/mainMenu");
            SetupMenus();
        }
        private void SetupMenus()
        {
            int centerX = graphics.Viewport.Width / 2;
            int startY = graphics.Viewport.Height / 2 - 100;

            int spacing = 90;

            // Pause menu items
            menuItems = new List<(Texture2D, Rectangle, Action)>
            {
                (Resume, new Rectangle(centerX - 150, startY, 300, 50), () => IsPaused = false),
                (Restart, new Rectangle(centerX - 150, startY + spacing, 300, 50), () => { ClickRestart?.Invoke(); IsPaused = false; }),
                (Option, new Rectangle(centerX - 150, startY + spacing * 2, 300, 50), () => { ClickOption?.Invoke(); }),
                (Exit, new Rectangle(centerX - 150, startY + spacing * 3, 300, 50), () => { inExitMenu = true; }),
            };

            // Exit menu items
            exitMenuItems = new List<(string, Rectangle, Action)>
            {
                ("Yes", new Rectangle(centerX - 150, startY + spacing, 300, 75), () => { ClickExit?.Invoke(); IsPaused = false; }),
                ("No", new Rectangle(centerX - 150, startY + spacing * 2, 300, 75), () => { inExitMenu = false; }),
            };

        }
        public void Update(KeyboardState keyboardState, KeyboardState previousKeyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyUp(Keys.Escape))
            {
                SoundManager.PlayUIHover();
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
                _previousExitSelectedIndex = exitSelectedIndex;
                _previousSelectedIndex = selectedIndex;

                if (!inExitMenu)
                {
                    for (int i = 0; i < menuItems.Count; i++)
                    {
                        if (menuItems[i].rect.Contains(mousePos))
                        {
                            selectedIndex = i; // เปลี่ยนตำแหน่ง holder ตามปุ่มที่เมาส์อยู่
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < exitMenuItems.Count; i++)
                    {
                        if (exitMenuItems[i].rect.Contains(mousePos))
                        {
                            exitSelectedIndex = i;
                            break;
                        }
                    }
                }
                if (!inExitMenu)
                {
                    if (keyboardState.IsKeyDown(Keys.Up) && previousKeyboardState.IsKeyUp(Keys.Up))
                    {
                        selectedIndex--;
                        if (selectedIndex < 0) selectedIndex = menuItems.Count - 1;
                    }
                    if (keyboardState.IsKeyDown(Keys.Down) && previousKeyboardState.IsKeyUp(Keys.Down))
                    {
                        selectedIndex++;
                        if (selectedIndex >= menuItems.Count) selectedIndex = 0;
                    }
                    if (keyboardState.IsKeyDown(Keys.Enter) && previousKeyboardState.IsKeyUp(Keys.Enter))
                    {
                        SoundManager.PlayUIClick();
                        menuItems[selectedIndex].onClick?.Invoke();
                    }
                    if( _previousSelectedIndex != selectedIndex)
                    {
                        SoundManager.PlayUIHover();
                    }
                }
                else
                {
                    if (keyboardState.IsKeyDown(Keys.Up) && previousKeyboardState.IsKeyUp(Keys.Up))
                    {
                        exitSelectedIndex--;
                        if (exitSelectedIndex < 0) exitSelectedIndex = exitMenuItems.Count - 1;
                    }
                    if (keyboardState.IsKeyDown(Keys.Down) && previousKeyboardState.IsKeyUp(Keys.Down))
                    {
                        exitSelectedIndex++;
                        if (exitSelectedIndex >= exitMenuItems.Count) exitSelectedIndex = 0;
                    }
                    if (keyboardState.IsKeyDown(Keys.Enter) && previousKeyboardState.IsKeyUp(Keys.Enter))
                    {
                        SoundManager.PlayUIClick();
                        exitMenuItems[exitSelectedIndex].onClick?.Invoke();
                    }
                    if(_previousExitSelectedIndex != exitSelectedIndex)
                    {
                        SoundManager.PlayUIHover();
                    }
                }
                if (mouse.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
                {
                    if (!inExitMenu)
                    {
                        foreach (var item in menuItems)
                        {
                            if (item.rect.Contains(mouse.Position))
                            {
                                SoundManager.PlayUIClick();
                                item.onClick?.Invoke();
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in exitMenuItems)
                        {
                            if (item.rect.Contains(mouse.Position))
                            {
                                SoundManager.PlayUIClick();
                                item.onClick?.Invoke();
                                break;
                            }
                        }
                    }
                }
                _previousMouseState = mouse;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsPaused) return;

            Vector2 cameraOffset = camera != null ? camera.CurrentPosition + camera.ShakeOffset : Vector2.Zero;

            Texture2D blackPixel = new Texture2D(graphics, 1, 1);
            blackPixel.SetData(new[] { Color.White });
            spriteBatch.Draw(blackPixel, new Rectangle(0, 0, graphics.Viewport.Width, graphics.Viewport.Height), Color.Black * 0.7f);

            spriteBatch.Draw(pauseMenu, new Rectangle((int)(graphics.Viewport.Width / 2 - 576 + cameraOffset.X), (int)(graphics.Viewport.Height / 2 - 324 + cameraOffset.Y), 1152, 648), Color.White);

            //string title = inExitMenu ? "Exit" : "Game Paused";
            string title = inExitMenu ? "Exit" : "";
            Vector2 titleSize = font.MeasureString(title) * 1.5f;
            Vector2 titlePos = new Vector2((graphics.Viewport.Width - titleSize.X + cameraOffset.X) / 2, graphics.Viewport.Height / 2 - 100 + cameraOffset.Y);
            spriteBatch.DrawString(font, title, titlePos, Color.Black, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, title, titlePos, Color.White, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
            MouseState mouse = Mouse.GetState();

            if (!inExitMenu)
            {
                for (int i = 0; i < menuItems.Count; i++)
                {
                    var item = menuItems[i];

                    // แสดง holder เฉพาะปุ่มที่เลือก
                    /*if (i == selectedIndex)
                        spriteBatch.Draw(holder, new Rectangle((int)(item.rect.X + cameraOffset.X), 
                            (int)(item.rect.Y + cameraOffset.Y), 
                            item.rect.Width, item.rect.Height), 
                            Color.White);*/

                    spriteBatch.Draw(item.tex, new Rectangle(
                        (int)(item.rect.X + cameraOffset.X),
                        (int)(item.rect.Y + cameraOffset.Y),
                        item.rect.Width, item.rect.Height),
                        Color.White);
                }
            }
            else // วาด Exit Menu
            {
                for (int i = 0; i < exitMenuItems.Count; i++)
                {
                    var item = exitMenuItems[i];

                    // วาด holder เฉพาะปุ่มที่เลือกใน Exit Menu
                    if (i == exitSelectedIndex)
                        spriteBatch.Draw(holder, new Rectangle((int)(item.rect.X + cameraOffset.X),
                            (int)(item.rect.Y + 5 + cameraOffset.Y),
                            item.rect.Width, item.rect.Height),
                            Color.White);

                    Vector2 textSize = font.MeasureString(item.text);
                    Vector2 textPos = new Vector2(item.rect.X + (item.rect.Width - textSize.X) / 2 + cameraOffset.X,
                                                  item.rect.Y + (item.rect.Height - textSize.Y) / 2 + cameraOffset.Y);
                    spriteBatch.DrawString(font, item.text, textPos, Color.White);
                }
            }
        }
    }
}
