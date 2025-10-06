using DimmedLight.GamePlay.Animated;
using DimmedLight.GamePlay.Background;
using DimmedLight.GamePlay.Enemies;
using DimmedLight.GamePlay.ETC;
using DimmedLight.GamePlay.Isplayer;
using DimmedLight.GamePlay.Managers;
using DimmedLight.GamePlay.UI;
using DimmedLight.MainMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DimmedLight.GamePlay
{
    public class Gameplay
    {
        private Game1 game;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;

        #region Assets
        Texture2D bg1Tex, bg2Tex, bg3Tex, bg4Tex, bg5Tex;
        Texture2D platformTex, platformAsset;
        Texture2D projectileTex, attackProjecTex, parryProjecTex;

        Texture2D hurtBoxTex, hitBoxTex;
        Texture2D hellCloakTheme;
        Texture2D tutorialImage;
        Texture2D pauseImage;
        Texture2D bottonCursor;
        #endregion

        #region Background & Platforms
        BackgroundLayer bg1, bg2, bg3, bg4, bg5;
        PlatformManager platformManager;
        #endregion

        #region Player
        Player player;
        #endregion

        #region Enemies
        PhaseManager phaseManager;
        EnemyFactory enemyFactory;
        #endregion

        #region Disaster & HUD
        Delisaster delisaster;
        Camera camera;
        HUD hud;
        ScoreManager scoreManager;
        #endregion

        #region Control
        bool isFlipped = false;
        KeyboardState previousKeyState;
        GamePadState previousGamePadState;
        PauseMenu pauseMenu;
        #endregion

        private SoundEffect parryHit;
        private Song BMG;
        private Song EventSound;

        private Texture2D redOverlay;
        private bool delisasterHasDashed = false;
        private Color attack = new Color(37, 150, 190);
        private GameOver gameOverScreen;
        private bool showGameOver = false;
        public static class Texture2DHelper
        {
            public static Texture2D Pixel { get; set; }
        }

        public Gameplay(Game1 game, GraphicsDeviceManager graphics)
        {
            this.game = game;
            _graphics = graphics;
        }

        public void LoadContent()
        {
            _spriteBatch = new SpriteBatch(game.GraphicsDevice);
            
            #region Assets
            platformTex = game.Content.Load<Texture2D>("platform-remake");
            platformAsset = game.Content.Load<Texture2D>("floating-platform");
            bg1Tex = game.Content.Load<Texture2D>("Background/back1");
            bg2Tex = game.Content.Load<Texture2D>("Background/back2");
            bg3Tex = game.Content.Load<Texture2D>("Background/back3");
            bg4Tex = game.Content.Load<Texture2D>("Background/back4");
            bg5Tex = game.Content.Load<Texture2D>("Background/back5");

            projectileTex = game.Content.Load<Texture2D>("bullet4");
            attackProjecTex = game.Content.Load<Texture2D>("bullet2");
            parryProjecTex = game.Content.Load<Texture2D>("bullet1");

            font = game.Content.Load<SpriteFont>("gameFont");
            hellCloakTheme = game.Content.Load<Texture2D>("ThemeEvent");
            tutorialImage = game.Content.Load<Texture2D>("eventTutorial");
            pauseImage = game.Content.Load<Texture2D>("Frame");
            bottonCursor = game.Content.Load<Texture2D>("bottonCursor");

            parryHit = game.Content.Load<SoundEffect>("Audio/LOOP_SFX_ParrySuccess2");
            BMG = game.Content.Load<Song>("Audio/SunYaNaFon");
            EventSound = game.Content.Load<Song>("Audio/TaLayJai");
            if (MediaPlayer.State == MediaState.Playing && pauseMenu.IsPaused)
            {
                MediaPlayer.Stop();
            }
            MediaPlayer.Play(BMG);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.01f;
            gameOverScreen = new GameOver(game, _graphics);
            gameOverScreen.LoadContent();
            #endregion

            #region Hurt&HitBox
            hurtBoxTex = new Texture2D(game.GraphicsDevice, 1, 1);
            hurtBoxTex.SetData(new[] { Color.Red });
            hitBoxTex = new Texture2D(game.GraphicsDevice, 1, 1);
            hitBoxTex.SetData(new[] { attack });

            Texture2D pixel = new Texture2D(game.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            Texture2DHelper.Pixel = pixel;

            redOverlay = new Texture2D(game.GraphicsDevice, 1, 1);
            redOverlay.SetData(new[] { Color.Red });
            #endregion

            #region BG&Platform
            bg1 = new BackgroundLayer(bg1Tex, 3, 0.1f);
            bg2 = new BackgroundLayer(bg2Tex, 3, 0.3f);
            bg3 = new BackgroundLayer(bg3Tex, 3, 0.5f);
            bg4 = new BackgroundLayer(bg4Tex, 3, 0.7f);
            bg5 = new BackgroundLayer(bg5Tex, 3, 0.9f);
            platformManager = new PlatformManager(platformTex, platformAsset, 6);
            #endregion

            #region Player
            scoreManager = new ScoreManager();
            player = new Player(null, scoreManager);
            player.Load(game.Content);
            scoreManager.LoadContent(game.Content);
            #endregion

            #region Disaster&UI
            delisaster = new Delisaster();
            delisaster.Load(game.Content);
            hud = new HUD();
            camera = new Camera();
            pauseMenu = new PauseMenu(_graphics.GraphicsDevice, font, pauseImage, bottonCursor);
            pauseMenu.ClickExit = () =>
            {
                if (MediaPlayer.State == MediaState.Playing || MediaPlayer.State == MediaState.Paused)
                    MediaPlayer.Stop();

                game.ChangeScreen(new MenuScreen((Game1)game, _graphics, game.GraphicsDevice, game.Content));
            };
            pauseMenu.ClickOption = () =>
            {
                if (MediaPlayer.State == MediaState.Playing || MediaPlayer.State == MediaState.Paused)
                    MediaPlayer.Stop();

                game.ChangeScreen(new SettingScreen((Game1)game, _graphics, game.GraphicsDevice, game.Content));
            };
            #endregion

            #region Enemy&Factory
            var guiltIdle = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            var guiltAttack = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            var guiltDeath = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            guiltIdle.Load(game.Content, "Guilt_idle", 1, 1, 15);
            guiltAttack.Load(game.Content, "Guilt_idle", 1, 1, 15);
            guiltDeath.Load(game.Content, "guiltdead_Spritesheet", 7, 1, 8);

            var traumaIdle = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            var traumaAttack = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            var traumaDeath = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            traumaIdle.Load(game.Content, "trauma-re", 1, 1, 15);
            traumaAttack.Load(game.Content, "trauma-re", 1, 1, 15);
            traumaDeath.Load(game.Content, "traumadead_Spritesheet", 7, 1, 8);

            var judgementIdle = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            var judgementDeath = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            judgementIdle.Load(game.Content, "judgement_Spritesheet", 10, 1, 15);
            judgementDeath.Load(game.Content, "judgement_dead_Spritesheet", 8, 1, 9);

            enemyFactory = new EnemyFactory(
                guiltIdle, guiltAttack, guiltDeath,
                traumaIdle, traumaAttack, traumaDeath,
                judgementIdle, judgementDeath,
                projectileTex,
                parryProjecTex
            );

            phaseManager = new PhaseManager(enemyFactory, player, delisaster, camera, hellCloakTheme, tutorialImage,
                parryProjecTex, attackProjecTex, parryHit, EventSound, BMG, game.GraphicsDevice);
            #endregion

            player.SetPhaseManager(phaseManager);
            EnemyBase.ParryHit = parryHit;
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            GamePadState gpState = GamePad.GetState(PlayerIndex.One);

            pauseMenu.Update(keyState, previousKeyState);
            //float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //if (keyState.IsKeyDown(Keys.D5) && !previousKeyState.IsKeyDown(Keys.D5)) delisaster.DashForward(new Vector2(150, delisaster.Position.Y));
            
            if (keyState.IsKeyDown(Keys.D4) && !previousKeyState.IsKeyDown(Keys.D4)) ResetGame();

            if (pauseMenu.IsPaused && !player.IsDead)
            {
                if (MediaPlayer.State == MediaState.Playing)
                    MediaPlayer.Pause();
            }
            else if (!pauseMenu.IsPaused && !player.IsDead)
            {
                if (MediaPlayer.State == MediaState.Paused)
                    MediaPlayer.Resume();
            }
            else if (player.IsDead)
            {
                if (MediaPlayer.State == MediaState.Playing)
                    MediaPlayer.Stop();
            }

            if (!pauseMenu.IsPaused)
            {
                float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (phaseManager.TutorialEvent != null && phaseManager.TutorialEvent.IsActive)
                {
                    phaseManager.TutorialEvent.Update(keyState, previousKeyState);
                    previousKeyState = keyState;
                    previousGamePadState = gpState;
                    return;
                }
                if (!player.IsDead)
                {
                    float bgSpeed = player.canWalk ? phaseManager.PlatformSpeed : 0f;
                    bg1.Update(bgSpeed);
                    bg2.Update(bgSpeed);
                    bg3.Update(bgSpeed);
                    bg4.Update(bgSpeed);
                    bg5.Update(bgSpeed);
                    platformManager.Update(bgSpeed, delta);

                    player.Update(gameTime, keyState, gpState, previousKeyState, previousGamePadState, delta);
                    delisaster.Update(delta, player);

                    phaseManager.Update(gameTime, delta, player, ref isFlipped, delisaster, scoreManager, keyState, previousKeyState);

                    if (player.Health <= 0) player.IsDead = true;

                    if (!player.IsReturning && !player.IsInvincible)
                        player.LastSafePosition = player.Position;


                    scoreManager.Update(gameTime, player);
                }
                else if (player.IsDead)
                {
                    player.UpdateDeathAnimation(delta);
                    delisaster.Update(delta, player);
                    if (MediaPlayer.State == MediaState.Playing)
                        MediaPlayer.Stop();
                    if (!delisasterHasDashed)
                    {
                        delisaster.DashForward(new Vector2(0, delisaster.Position.Y));
                        delisasterHasDashed = true;
                    }
                    if (player.DeathDelayStarted && player.DeathDelayTimer >= player.PostDeathDelay)
                    {
                        showGameOver = true;
                    }
                }
                if (showGameOver)
                {
                    gameOverScreen.Update(gameTime);
                    if (gameOverScreen.RestartRequested)
                    {
                        ResetGame();
                        gameOverScreen.Reset();
                        showGameOver = false;
                    }
                    return;
                }

                camera.MoveCamTocenter(delta);
            }
            if (keyState.IsKeyDown(Keys.D5) && !previousKeyState.IsKeyDown(Keys.D5))
                showGameOver = true;
            previousKeyState = keyState;
            previousGamePadState = gpState;
            pauseMenu.ClickRestart = () => ResetGame();
        }

        public void Draw(GameTime gameTime)
        {
            game.GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(sortMode: SpriteSortMode.Deferred, blendState: BlendState.AlphaBlend, transformMatrix: camera.GetViewMatrix());

            bg1.Draw(_spriteBatch);
            bg2.Draw(_spriteBatch);
            bg3.Draw(_spriteBatch);
            bg4.Draw(_spriteBatch);
            bg5.Draw(_spriteBatch);

            platformManager.Draw(_spriteBatch);

            player.Draw(_spriteBatch, hurtBoxTex, hitBoxTex);
            hud.DrawHealth(_spriteBatch, hurtBoxTex, player.Health, player);

            phaseManager.Draw(_spriteBatch, hurtBoxTex, hitBoxTex, isFlipped);
            delisaster.Draw(_spriteBatch);

            
            scoreManager.Draw(_spriteBatch, font);

            pauseMenu.Draw(_spriteBatch);

            if (player.IsInvincible)
            {
                float alpha = player.InvincibilityTimer / player.InvincibilityTime;
                alpha = MathHelper.Clamp(alpha, 0f, 0.3f);

                _spriteBatch.Draw(
                    redOverlay,
                    new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
                    Color.Red * alpha
                );
            }
            _spriteBatch.End();
            if (showGameOver)
            {
                _spriteBatch.Begin();

                gameOverScreen.Draw(_spriteBatch);
                _spriteBatch.End();

                return;
            }
        }

        private void ResetGame()
        {
            phaseManager.Reset();
            isFlipped = false;
            player.Position = new Vector2(400, 650);
            player.Reset();

            delisaster.ResetPosition();
            delisasterHasDashed = false;

            scoreManager.Reset();

            platformManager.Reset();
            showGameOver = false;

            if (MediaPlayer.State == MediaState.Playing)
                MediaPlayer.Stop();
            MediaPlayer.Play(BMG);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.01f;
        }
    }
}
