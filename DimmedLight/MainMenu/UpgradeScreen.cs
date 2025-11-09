using Microsoft.Xna.Framework;
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
    public class UpgradeNode
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int CurrentLevel { get; set; }
        public int MaxLevel { get; set; }
        public Rectangle Bounds { get; set; }
        public Func<int, string> StatDisplayFunc { get; set; }
    }

    public class UpgradeScreen : Screen
    {
        private Texture2D _background;
        private SpriteFont _menuFont;
        private Texture2D _pixelTexture;

        private List<UpgradeNode> _nodes = new List<UpgradeNode>();
        private int _selectedIndex = 0;

        private MouseState _previousMouseState;
        private KeyboardState _previousKeyboardState;
        private GamePadState _previousGamePadState;

        private float[] _nodeScales = new float[4] { 1f, 1f, 1f, 1f };
        private const float SCALE_SPEED = 5f;

        public UpgradeScreen(Game1 game, GraphicsDeviceManager graphicsDeviceManager, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game, graphicsDeviceManager, graphicsDevice, content)
        {
        }

        public override void LoadContent()
        {
            _background = Content.Load<Texture2D>("UX_UIAsset/mainmenu_page/Background");
            //_menuFont = Content.Load<SpriteFont>("UX_UIAsset/Font/TextFont");
            _menuFont = Content.Load<SpriteFont>("gameFont");

            _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });

            SoundManager.PlayMainMenuMusic();
            InitializeUpgradeNodes();

            _previousMouseState = Mouse.GetState();
            _previousKeyboardState = Keyboard.GetState();
            _previousGamePadState = GamePad.GetState(PlayerIndex.One);
        }

        private void InitializeUpgradeNodes()
        {
            int screenWidth = GraphicsDevice.Viewport.Width;
            int screenHeight = GraphicsDevice.Viewport.Height;
            int totalWidth = 1000;
            int startX = (screenWidth - totalWidth) / 2;

            // --- CHANGE HERE: Adjust Y position to be relative to the screen center ---
            int yPos = screenHeight / 2 + 150;

            int spacing = 250;
            int iconSize = 128;

            _nodes.Add(new UpgradeNode
            {
                Name = "Toughness",
                Description = "Increase Max Health",
                CurrentLevel = 5,
                MaxLevel = 10,
                StatDisplayFunc = level => $"Gauge: {100 + (level - 1) * 10} -> {100 + level * 10}"
            });
            _nodes.Add(new UpgradeNode
            {
                Name = "Recovery",
                Description = "Improve Health Regen",
                CurrentLevel = 3,
                MaxLevel = 5,
                StatDisplayFunc = level => $"Regen: {1 + (level - 1) * 0.2f:F1}/s -> {1 + level * 0.2f:F1}/s"
            });
            _nodes.Add(new UpgradeNode
            {
                Name = "Light cap",
                Description = "Increase Max Ultimate Gauge",
                CurrentLevel = 1,
                MaxLevel = 3,
                StatDisplayFunc = level => $"Gauge: {100 * level} -> {100 * (level + 1)}"
            });
            _nodes.Add(new UpgradeNode
            {
                Name = "Sol gate",
                Description = "Reduce Skill Cooldown",
                CurrentLevel = 7,
                MaxLevel = 7,
                StatDisplayFunc = level => $"Cooldown: -{level * 2}% -> -{(level + 1) * 2}%"
            });

            for (int i = 0; i < _nodes.Count; i++)
            {
                _nodes[i].Bounds = new Rectangle(startX + i * spacing, yPos, iconSize, iconSize);
            }
        }

        public override void Update(GameTime gameTime)
        {
            var mouse = Mouse.GetState();
            var keyboard = Keyboard.GetState();
            var gamePad = GamePad.GetState(PlayerIndex.One);

            var mousePos = new Point(mouse.X, mouse.Y);

            // Handle navigation with keyboard and gamepad
            bool moveRight = keyboard.IsKeyDown(Keys.Right) && _previousKeyboardState.IsKeyUp(Keys.Right) ||
                             gamePad.ThumbSticks.Left.X > 0.5f && _previousGamePadState.ThumbSticks.Left.X <= 0.5f ||
                             gamePad.IsButtonDown(Buttons.DPadRight) && _previousGamePadState.IsButtonUp(Buttons.DPadRight);

            bool moveLeft = keyboard.IsKeyDown(Keys.Left) && _previousKeyboardState.IsKeyUp(Keys.Left) ||
                            gamePad.ThumbSticks.Left.X < -0.5f && _previousGamePadState.ThumbSticks.Left.X >= -0.5f ||
                            gamePad.IsButtonDown(Buttons.DPadLeft) && _previousGamePadState.IsButtonUp(Buttons.DPadLeft);

            if (moveRight)
            {
                _selectedIndex = (_selectedIndex + 1) % _nodes.Count;
            }
            if (moveLeft)
            {
                _selectedIndex = (_selectedIndex - 1 + _nodes.Count) % _nodes.Count;
            }

            // Mouse hover overrides selection index
            if (mouse.X != _previousMouseState.X || mouse.Y != _previousMouseState.Y)
            {
                for (int i = 0; i < _nodes.Count; i++)
                {
                    var scaledBounds = GetScaledBounds(_nodes[i].Bounds, _nodeScales[i]);
                    if (scaledBounds.Contains(mousePos))
                    {
                        _selectedIndex = i;
                        break;
                    }
                }
            }

            // Handle upgrade and downgrade logic
            var selectedNode = _nodes[_selectedIndex];

            // ----- THIS LINE IS CORRECTED -----
            bool upgradePressed = keyboard.IsKeyDown(Keys.Enter) && _previousKeyboardState.IsKeyUp(Keys.Enter) ||
                                  gamePad.IsButtonDown(Buttons.A) && _previousGamePadState.IsButtonUp(Buttons.A) ||
                                  mouse.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released && GetScaledBounds(selectedNode.Bounds, _nodeScales[_selectedIndex]).Contains(mousePos);

            bool downgradePressed = keyboard.IsKeyDown(Keys.B) && _previousKeyboardState.IsKeyUp(Keys.B) ||
                                    gamePad.IsButtonDown(Buttons.B) && _previousGamePadState.IsButtonUp(Buttons.B);

            if (upgradePressed && selectedNode.CurrentLevel < selectedNode.MaxLevel)
            {
                selectedNode.CurrentLevel++;
            }

            if (downgradePressed && selectedNode.CurrentLevel > 0)
            {
                selectedNode.CurrentLevel--;
            }

            // Back to Menu logic
            bool goBack = keyboard.IsKeyDown(Keys.Escape) && _previousKeyboardState.IsKeyUp(Keys.Escape);

            if (goBack)
            {
                Game.ChangeScreen(new MenuScreen(Game, Game._graphics, GraphicsDevice, Content));
            }

            // Update scale based on selection
            for (int i = 0; i < _nodes.Count; i++)
            {
                float targetScale = i == _selectedIndex ? 1.25f : 1.0f;
                _nodeScales[i] += (targetScale - _nodeScales[i]) * SCALE_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            _previousMouseState = mouse;
            _previousKeyboardState = keyboard;
            _previousGamePadState = gamePad;
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            spriteBatch.Draw(_background, GraphicsDevice.Viewport.Bounds, Color.White);

            Texture2D blackPixel = new Texture2D(GraphicsDevice, 1, 1);
            blackPixel.SetData(new[] { Color.White });
            spriteBatch.Draw(blackPixel, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.Black * 0.5f);

            Vector2 screenCenter = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            Color textColor = Color.White;
            Color shadowColor = Color.Black;
            Vector2 shadowOffset = new Vector2(2, 2);

            var selectedNode = _nodes[_selectedIndex];

            // --- วาดข้อมูลของ Node ที่เลือก (พร้อมเงา) ---
            // หัวข้อ "UPGRADE"
            spriteBatch.DrawString(_menuFont, "UPGRADE", new Vector2(50, 50) + shadowOffset, shadowColor);
            spriteBatch.DrawString(_menuFont, "UPGRADE", new Vector2(50, 50), textColor);

            // คำอธิบาย (Description)
            Vector2 descriptionSize = _menuFont.MeasureString(selectedNode.Description);
            Vector2 descriptionPosition = new Vector2(screenCenter.X - descriptionSize.X / 2, screenCenter.Y - 150);
            spriteBatch.DrawString(_menuFont, selectedNode.Description, descriptionPosition + shadowOffset, shadowColor);
            spriteBatch.DrawString(_menuFont, selectedNode.Description, descriptionPosition, textColor);

            // ค่าสถานะ (Stat Text)
            string statText = selectedNode.StatDisplayFunc(selectedNode.CurrentLevel);
            if (selectedNode.CurrentLevel >= selectedNode.MaxLevel) statText = "Max Level Reached";
            else if (selectedNode.CurrentLevel == 0) statText = "Not Unlocked";

            Vector2 statTextSize = _menuFont.MeasureString(statText);
            Vector2 statTextPosition = new Vector2(screenCenter.X - statTextSize.X / 2, screenCenter.Y - 50);
            spriteBatch.DrawString(_menuFont, statText, statTextPosition + shadowOffset, shadowColor);
            spriteBatch.DrawString(_menuFont, statText, statTextPosition, textColor);


            // --- วาด Node ทั้งหมด (พร้อมเงาสำหรับชื่อ) ---
            for (int i = 0; i < _nodes.Count; i++)
            {
                var node = _nodes[i];
                var scale = _nodeScales[i];
                Rectangle scaledBounds = GetScaledBounds(node.Bounds, scale);

                Color nodeColor = i == _selectedIndex ? Color.Gold : Color.DarkGray;
                spriteBatch.Draw(_pixelTexture, scaledBounds, nodeColor);

                string placeholderText = $"[{node.Name.First()}]";
                Vector2 textSize = _menuFont.MeasureString(placeholderText);
                Vector2 textPosition = new Vector2(scaledBounds.Center.X - textSize.X / 2, scaledBounds.Center.Y - textSize.Y / 2);
                spriteBatch.DrawString(_menuFont, placeholderText, textPosition, Color.White);

                // --- CHANGE HERE: Added a manual horizontal adjustment ---
                // เพิ่มค่าบวกเพื่อขยับไปทางขวา, ลดค่าลบเพื่อขยับไปทางซ้าย
                // ลองปรับค่าเลข 5 ดูจนกว่าจะพอดี
                int horizontalAdjust = 5;

                Vector2 nameSize = _menuFont.MeasureString(node.Name);
                Vector2 namePosition = new Vector2(scaledBounds.Center.X - nameSize.X / 2 + horizontalAdjust, scaledBounds.Bottom + 10);

                spriteBatch.DrawString(_menuFont, node.Name, namePosition + shadowOffset, shadowColor);
                spriteBatch.DrawString(_menuFont, node.Name, namePosition, textColor);
            }

            spriteBatch.End();
        }

        private Rectangle GetScaledBounds(Rectangle originalBounds, float scale)
        {
            return new Rectangle(
                (int)(originalBounds.Center.X - originalBounds.Width * scale / 2),
                (int)(originalBounds.Center.Y - originalBounds.Height * scale / 2),
                (int)(originalBounds.Width * scale),
                (int)(originalBounds.Height * scale));
        }
    }
}
