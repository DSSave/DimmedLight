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
<<<<<<< Updated upstream
        Texture2D bg1Tex, bg2Tex, bg3Tex, bg4Tex, bg5Tex;
        Texture2D platformTex, platformAsset;
        Texture2D projectileTex, attackProjecTex, parryProjecTex;

        Texture2D hurtBoxTex, hitBoxTex;
        Texture2D hellCloakTheme;
        Texture2D tutorialImage;
        Texture2D pauseImage, frame;
        Texture2D bottonCursor;
=======
        private Texture2D _platformTex, _platformAsset;
        private Texture2D _projectileTex, _attackProjecTex, _parryProjecTex;
        private Texture2D _hurtBoxTex, _hitBoxTex, _pixelTexture;
        private Texture2D _hellCloakTheme;
        private Texture2D _tutorialImage;
        private Texture2D _pauseImage, _frame, _bottonCursor;
        private SoundEffect _parryHit, _enemiesDead, _playerHit, _ammoShoot;
        private Song _bmg, _eventSound;
        private Song _gameOverSound;
        private Texture2D _redOverlay;
        private Texture2D _healthPlayer;
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
        bool isFlipped = false;
        KeyboardState previousKeyState;
        GamePadState previousGamePadState;
        PauseMenu pauseMenu;
=======
        private bool _isFlipped;
        private KeyboardState _previousKeyState;
        private GamePadState _previousGamePadState;
        private PauseMenu _pauseMenu;
        private GameOver _gameOverScreen;
        private bool _showGameOver;
        private bool _delisasterHasDashed;
        public bool SettingScreenWasOpen { get; set; }
        private bool _gameOverSoundPlayed = false;
>>>>>>> Stashed changes
        #endregion

        private SoundEffect parryHit,enemiesDead,playerHit,ammoShoot;
        private Song BMG;
        private Song EventSound;

        private Texture2D redOverlay;
        private bool delisasterHasDashed = false;
        private Color attack = new Color(37, 150, 190);
        private GameOver gameOverScreen;
        private bool showGameOver = false;
        public bool SettingScreenWasOpen { get; set; } = false;

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

<<<<<<< Updated upstream
            font = game.Content.Load<SpriteFont>("gameFont");
            hellCloakTheme = game.Content.Load<Texture2D>("ThemeEvent");
=======
            _eventTextSlide = new EventTextSlide(_game.GraphicsDevice, Stepalange, _camera);
            MediaPlayer.Play(_bmg);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.01f * SoundManager.BgmVolume;
        }
        private void LoadTextures()
        {
            _platformTex = _game.Content.Load<Texture2D>("platform-remake");
            _platformAsset = _game.Content.Load<Texture2D>("floating-platform");
            _projectileTex = _game.Content.Load<Texture2D>("bullet4");
            _attackProjecTex = _game.Content.Load<Texture2D>("bullet2");
            _parryProjecTex = _game.Content.Load<Texture2D>("bullet1");
            _hellCloakTheme = _game.Content.Load<Texture2D>("ThemeEvent");
            _tutorialImage = _game.Content.Load<Texture2D>("eventTutorial");
            _pauseImage = _game.Content.Load<Texture2D>("PauseNew");
            _frame = _game.Content.Load<Texture2D>("Frame");
            _bottonCursor = _game.Content.Load<Texture2D>("bottonCursor");
>>>>>>> Stashed changes

            pauseImage = game.Content.Load<Texture2D>("PauseNew");
            tutorialImage = game.Content.Load<Texture2D>("eventTutorial");
            frame = game.Content.Load<Texture2D>("Frame");
            bottonCursor = game.Content.Load<Texture2D>("bottonCursor");

<<<<<<< Updated upstream
            enemiesDead = game.Content.Load<SoundEffect>("Audio/LOOP_SFX_EnemiesDead");
            ammoShoot = game.Content.Load<SoundEffect>("Audio/LOOP_SFX_EventParryAmmoAndGuilt");
            playerHit = game.Content.Load<SoundEffect>("Audio/LOOP_SFX_PlayerHit2");
            parryHit = game.Content.Load<SoundEffect>("Audio/LOOP_SFX_ParrySuccess2");
            BMG = game.Content.Load<Song>("Audio/TaLayJai");
            EventSound = game.Content.Load<Song>("Audio/SunYaNaFon");
            if (MediaPlayer.State == MediaState.Playing && pauseMenu.IsPaused)
=======
            _healthPlayer = _game.Content.Load<Texture2D>("MenuAsset/Hearts");
        }
        private void LoadSounds()
        {
            _enemiesDead = _game.Content.Load<SoundEffect>("Audio/LOOP_SFX_EnemiesDead");
            _ammoShoot = _game.Content.Load<SoundEffect>("Audio/LOOP_SFX_EventParryAmmoAndGuilt");
            _playerHit = _game.Content.Load<SoundEffect>("Audio/LOOP_SFX_PlayerHit2");
            _parryHit = _game.Content.Load<SoundEffect>("Audio/LOOP_SFX_ParrySuccess2");
            _bmg = _game.Content.Load<Song>("Audio/MainTheme ");
            _eventSound = _game.Content.Load<Song>("Audio/Event");
            _gameOverSound = _game.Content.Load<Song>("Audio/EasyGameOver");
        }
        private void LoadFonts()
        {
            Stepalange = _game.Content.Load<SpriteFont>("Fonts/StepalangeFont");
            StepalangeShort = _game.Content.Load<SpriteFont>("Fonts/StepalangeShortFont");
        }
        private void InitializeBackground()
        {
            var bg1Tex = _game.Content.Load<Texture2D>("Background/back1");
            var bg2Tex = _game.Content.Load<Texture2D>("Background/back2");
            var bg3Tex = _game.Content.Load<Texture2D>("Background/back3");
            var bg4Tex = _game.Content.Load<Texture2D>("Background/back4");
            var bg5Tex = _game.Content.Load<Texture2D>("Background/back5");

            _bg1 = new BackgroundLayer(bg1Tex, 3, 0.1f);
            _bg2 = new BackgroundLayer(bg2Tex, 3, 0.3f);
            _bg3 = new BackgroundLayer(bg3Tex, 3, 0.5f);
            _bg4 = new BackgroundLayer(bg4Tex, 3, 0.7f);
            _bg5 = new BackgroundLayer(bg5Tex, 3, 0.9f);
            _platformManager = new PlatformManager(_platformTex, _platformAsset, 6);
        }
        private void InitializeCoreGameplay()
        {
            _scoreManager = new ScoreManager();
            _scoreManager.LoadContent(_game.Content);
            _player = new Player(_phaseManager, _scoreManager);
            _player.Load(_game.Content);
            _delisaster = new Delisaster();
            _delisaster.Load(_game.Content);
            _camera = new Camera();
        }
        private void InitializeUI()
        {
            _hud = new HUD();
            _pauseMenu = new PauseMenu(_graphics.GraphicsDevice, Stepalange, _pauseImage, _frame, _bottonCursor, _camera);
            _pauseMenu.LoadContent(_game.Content);
            _pauseMenu.ClickExit = () =>
>>>>>>> Stashed changes
            {
                MediaPlayer.Stop();
            }
            //MediaPlayer.Play(BMG);
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
            pauseMenu = new PauseMenu(_graphics.GraphicsDevice, font, pauseImage, frame, bottonCursor, camera);
            pauseMenu.ClickExit = () =>
            {
<<<<<<< Updated upstream
                if (MediaPlayer.State == MediaState.Playing || MediaPlayer.State == MediaState.Paused)
                    MediaPlayer.Stop();

                game.ChangeScreen(new MenuScreen((Game1)game, _graphics, game.GraphicsDevice, game.Content));
=======
                if (MediaPlayer.State == MediaState.Playing)
                    MediaPlayer.Pause();
                _game.ChangeScreen(new SettingScreen(_game, _graphics, _game.GraphicsDevice, _game.Content, SettingScreen.SettingSource.PauseMenu, this));
>>>>>>> Stashed changes
            };
            pauseMenu.ClickOption = () =>
            {
                if (MediaPlayer.State == MediaState.Playing || MediaPlayer.State == MediaState.Paused)
                    MediaPlayer.Stop();

                game.ChangeScreen(new SettingScreen((Game1)game, _graphics, game.GraphicsDevice, game.Content, SettingScreen.SettingSource.PauseMenu, this));
            };
            pauseMenu.LoadContent(game.Content);
            #endregion
            
            #region Enemy&Factory
            var guiltIdle = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            var guiltAttack = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
<<<<<<< Updated upstream
=======
            guiltAttack.Load(_game.Content, "guilt_attack_spritesheet", 7, 1, 15);
>>>>>>> Stashed changes
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
            EnemyBase.EnemiesDead = enemiesDead;
            EnemyBase.PlayerHit = playerHit;
            EnemyBase.AmmoShoot = ammoShoot;
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            GamePadState gpState = GamePad.GetState(PlayerIndex.One);

            pauseMenu.Update(keyState, previousKeyState);
            if (!pauseMenu.IsPaused && SettingScreenWasOpen)
            {
                pauseMenu.IsPaused = false;
                SettingScreenWasOpen = false;
            }
            //float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //if (keyState.IsKeyDown(Keys.D5) && !previousKeyState.IsKeyDown(Keys.D5)) delisaster.DashForward(new Vector2(150, delisaster.Position.Y));

            //if (keyState.IsKeyDown(Keys.D4) && !previousKeyState.IsKeyDown(Keys.D4)) ResetGame();

<<<<<<< Updated upstream
            if (pauseMenu.IsPaused && !player.IsDead)
=======
            if (!_pauseMenu.IsPaused)
            {
                float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_phaseManager.TutorialEvent != null && _phaseManager.TutorialEvent.IsActive)
                {
                    _phaseManager.TutorialEvent.Update(keyState, _previousKeyState);
                }
                else if (!_player.IsDead)
                {
                    UpdateGameplay(gameTime, keyState, gpState, delta, _phaseManager);
                }
                else
                {
                    UpdateGameOver(delta);
                }

                _camera.MoveCamTocenter(delta);
            }
            if (_showGameOver)
            {
                _gameOverScreen.Update(gameTime);
                if (_gameOverScreen.RestartRequested)
                {
                    ResetGame();
                }
                return;
            }
            if(_isEventEndingAnimationPlaying)
            {
                float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
                _eventTextSlide.Update(delta);
                if (!_eventTextSlide.IsActive)
                {
                    _isEventEndingAnimationPlaying = false;
                    _phaseManager._hellCloakEvent.PostTextEnd();
                    _player.canWalk = true;

                    if (MediaPlayer.State != MediaState.Playing || MediaPlayer.Queue.ActiveSong != _bmg)
                    {
                        MediaPlayer.Play(_bmg);
                        MediaPlayer.IsRepeating = true;
                        MediaPlayer.Volume = 0.1f * SoundManager.BgmVolume;
                    }
                    _platformManager?.Update(0f, delta);
                    _bg1?.Update(0f); _bg2?.Update(0f); _bg3?.Update(0f); _bg4?.Update(0f); _bg5?.Update(0f);
                    _camera?.MoveCamTocenter(delta);
                    _player?.UpdateAnimations(delta);
                    _delisaster?.Update(delta, _player);
                    _scoreManager?.Update(gameTime, _player);

                    _previousKeyState = keyState;
                    _previousGamePadState = gpState;
                    return;
                }
            }

            _previousKeyState = keyState;
            _previousGamePadState = gpState;
            _pauseMenu.ClickRestart = ResetGame;
        }

        private void HandleMusic()
        {
            if (_pauseMenu.IsPaused && !_player.IsDead)
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
=======
        private void UpdateGameOver(float delta)
        {
            _player.UpdateDeathAnimation(delta);
            _delisaster.Update(delta, _player);

            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Stop();
            }

            if (!_delisasterHasDashed)
            {
                _delisaster.DashForward(new Vector2(0, _delisaster.Position.Y));
                _delisasterHasDashed = true;
            }

            if (_player.DeathDelayStarted && _player.DeathDelayTimer >= _player.PostDeathDelay)
            {
                if(!_gameOverSoundPlayed)
                {
                    MediaPlayer.Play(_gameOverSound);
                    MediaPlayer.IsRepeating = false;
                    MediaPlayer.Volume = SoundManager.BgmVolume;
                }
                _showGameOver = true;
            }
        }
>>>>>>> Stashed changes
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
            scoreManager.Draw(_spriteBatch, font);
            hud.DrawHealth(_spriteBatch, hurtBoxTex, player.Health, player);

            phaseManager.Draw(_spriteBatch, hurtBoxTex, hitBoxTex, isFlipped);
            delisaster.Draw(_spriteBatch);


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
<<<<<<< Updated upstream
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
=======
            _phaseManager.Reset();
            _isFlipped = false;
            _player.Reset();
            _delisaster.ResetPosition();
            _delisasterHasDashed = false;
            _scoreManager.Reset();
            _platformManager.Reset();
            _showGameOver = false;
            _gameOverScreen.Reset();
            _eventTextSlide.StartAnimation("", Color.White);
            _gameOverSoundPlayed = false;
            if (MediaPlayer.State != MediaState.Playing)
            {
                MediaPlayer.Play(_bmg);
            }
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.1f * SoundManager.BgmVolume;
>>>>>>> Stashed changes
        }
    }
}
