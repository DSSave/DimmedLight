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
        private readonly Game1 _game;
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;

        #region Assets
        private Texture2D _platformTex, _platformAsset;
        private Texture2D _projectileTex, _attackProjecTex, _parryProjecTex;
        private Texture2D _hurtBoxTex, _hitBoxTex;
        private Texture2D _hellCloakTheme;
        private Texture2D _tutorialImage;
        private Texture2D _pauseImage, _frame, _bottonCursor;
        private SoundEffect _parryHit, _enemiesDead, _playerHit, _ammoShoot;
        private Song _bmg, _eventSound;
        private Texture2D _redOverlay;
        #endregion

        #region Game Objects
        private BackgroundLayer _bg1, _bg2, _bg3, _bg4, _bg5;
        private PlatformManager _platformManager;
        private Player _player;
        private PhaseManager _phaseManager;
        private EnemyFactory _enemyFactory;
        private Delisaster _delisaster;
        private Camera _camera;
        private HUD _hud;
        private ScoreManager _scoreManager;
        #endregion

        #region Player
        Player player;
        #endregion

        #region Control
        private bool _isFlipped;
        private KeyboardState _previousKeyState;
        private GamePadState _previousGamePadState;
        private PauseMenu _pauseMenu;
        private GameOver _gameOverScreen;
        private bool _showGameOver;
        private bool _delisasterHasDashed;
        public bool SettingScreenWasOpen { get; set; }
        #endregion

        public Gameplay(Game1 game, GraphicsDeviceManager graphics)
        {
            _game = game;
            _graphics = graphics;
        }
        public void LoadContent()
        {
            _spriteBatch = new SpriteBatch(_game.GraphicsDevice);

            // Load assets
            LoadTextures();
            LoadSounds();
            LoadFonts();

            // Initialize game objects
            InitializeBackground();
            InitializeCoreGameplay();
            InitializeUI();
            InitializeEnemies();

            //MediaPlayer.Play(_bmg);
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

            // Create 1x1 textures for drawing primitives
            _hurtBoxTex = new Texture2D(_game.GraphicsDevice, 1, 1);
            _hurtBoxTex.SetData(new[] { Color.Red });
            _hitBoxTex = new Texture2D(_game.GraphicsDevice, 1, 1);
            _hitBoxTex.SetData(new[] { new Color(37, 150, 190) });
            _redOverlay = new Texture2D(_game.GraphicsDevice, 1, 1);
            _redOverlay.SetData(new[] { Color.Red });
        }
        private void LoadSounds()
        {
            _enemiesDead = _game.Content.Load<SoundEffect>("Audio/LOOP_SFX_EnemiesDead");
            _ammoShoot = _game.Content.Load<SoundEffect>("Audio/LOOP_SFX_EventParryAmmoAndGuilt");
            _playerHit = _game.Content.Load<SoundEffect>("Audio/LOOP_SFX_PlayerHit2");
            _parryHit = _game.Content.Load<SoundEffect>("Audio/LOOP_SFX_ParrySuccess2");
            _bmg = _game.Content.Load<Song>("Audio/TaLayJai");
            _eventSound = _game.Content.Load<Song>("Audio/SunYaNaFon");
        }
        private void LoadFonts()
        {
            _font = _game.Content.Load<SpriteFont>("gameFont");
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
            _player = new Player(null, _scoreManager);
            _player.Load(_game.Content);
            _delisaster = new Delisaster();
            _delisaster.Load(_game.Content);
            _camera = new Camera();
        }
        private void InitializeUI()
        {
            _hud = new HUD();
            _pauseMenu = new PauseMenu(_graphics.GraphicsDevice, _font, _pauseImage, _frame, _bottonCursor, _camera);
            _pauseMenu.LoadContent(_game.Content);
            _pauseMenu.ClickExit = () =>
            {
                MediaPlayer.Stop();
                _game.ChangeScreen(new MenuScreen(_game, _graphics, _game.GraphicsDevice, _game.Content));
            };
            _pauseMenu.ClickOption = () =>
            {
                //MediaPlayer.Stop();
                _game.ChangeScreen(new SettingScreen(_game, _graphics, _game.GraphicsDevice, _game.Content, SettingScreen.SettingSource.PauseMenu, this));
            };
            _gameOverScreen = new GameOver(_game, _graphics);
            _gameOverScreen.LoadContent();
        }
        private void InitializeEnemies()
        {
            // Guilt animations
            var guiltIdle = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            guiltIdle.Load(_game.Content, "Guilt_idle", 1, 1, 15);
            var guiltAttack = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            guiltAttack.Load(_game.Content, "Guilt_idle", 1, 1, 15);
            var guiltDeath = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            guiltDeath.Load(_game.Content, "guiltdead_Spritesheet", 7, 1, 8);

            // Trauma animations
            var traumaIdle = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            traumaIdle.Load(_game.Content, "trauma-re", 1, 1, 15);
            var traumaAttack = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            traumaAttack.Load(_game.Content, "trauma-re", 1, 1, 15);
            var traumaDeath = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            traumaDeath.Load(_game.Content, "traumadead_Spritesheet", 7, 1, 8);

            // Judgement animations
            var judgementIdle = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            judgementIdle.Load(_game.Content, "judgement_Spritesheet", 10, 1, 15);
            var judgementDeath = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            judgementDeath.Load(_game.Content, "judgement_dead_Spritesheet", 8, 1, 9);

            // Enemy Factory
            _enemyFactory = new EnemyFactory(
                guiltIdle, guiltAttack, guiltDeath,
                traumaIdle, traumaAttack, traumaDeath,
                judgementIdle, judgementDeath,
                _projectileTex,
                _parryProjecTex
            );

            // Phase Manager
            _phaseManager = new PhaseManager(_enemyFactory, _player, _delisaster, _camera, _hellCloakTheme, _tutorialImage,
                _parryProjecTex, _attackProjecTex, _parryHit, _eventSound, _bmg, _game.GraphicsDevice);

            _player.SetPhaseManager(_phaseManager);

            // Assign static sound effects
            EnemyBase.ParryHit = _parryHit;
            EnemyBase.EnemiesDead = _enemiesDead;
            EnemyBase.PlayerHit = _playerHit;
            EnemyBase.AmmoShoot = _ammoShoot;
        }
        public void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            GamePadState gpState = GamePad.GetState(PlayerIndex.One);

            _pauseMenu.Update(keyState, _previousKeyState);

            if (!_pauseMenu.IsPaused && SettingScreenWasOpen)
            {
                SettingScreenWasOpen = false;
            }

            HandleMusic();

            if (!_pauseMenu.IsPaused)
            {
                float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_phaseManager.TutorialEvent != null && _phaseManager.TutorialEvent.IsActive)
                {
                    _phaseManager.TutorialEvent.Update(keyState, _previousKeyState);
                }
                else if (!_player.IsDead)
                {
                    UpdateGameplay(gameTime, keyState, gpState, delta);
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

            _previousKeyState = keyState;
            _previousGamePadState = gpState;
            _pauseMenu.ClickRestart = ResetGame;
        }

        private void HandleMusic()
        {
            if (_pauseMenu.IsPaused && !_player.IsDead)
            {
                if (MediaPlayer.State == MediaState.Playing)
                    MediaPlayer.Pause();
            }
            else if (!_pauseMenu.IsPaused && !_player.IsDead)
            {
                if (MediaPlayer.State == MediaState.Paused)
                    MediaPlayer.Resume();
            }
            else if (_player.IsDead)
            {
                if (MediaPlayer.State == MediaState.Playing)
                    MediaPlayer.Stop();
            }
        }
        private void UpdateGameplay(GameTime gameTime, KeyboardState keyState, GamePadState gpState, float delta)
        {
            float bgSpeed = _player.canWalk ? _phaseManager.PlatformSpeed : 0f;
            _bg1.Update(bgSpeed);
            _bg2.Update(bgSpeed);
            _bg3.Update(bgSpeed);
            _bg4.Update(bgSpeed);
            _bg5.Update(bgSpeed);
            _platformManager.Update(bgSpeed, delta);

            _player.Update(gameTime, keyState, gpState, _previousKeyState, _previousGamePadState, delta);
            _delisaster.Update(delta, _player);

            _phaseManager.Update(gameTime, delta, _player, ref _isFlipped, _delisaster, _scoreManager, keyState, _previousKeyState);

            if (_player.Health <= 0)
            {
                _player.IsDead = true;
            }

            if (!_player.IsReturning && !_player.IsInvincible)
            {
                _player.LastSafePosition = _player.Position;
            }

            _scoreManager.Update(gameTime, _player);
        }

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
                _showGameOver = true;
            }
        }
        public void Draw(GameTime gameTime)
        {
            _game.GraphicsDevice.Clear(Color.Black);

            // Main gameplay draw
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());
            DrawBackground();
            _platformManager.Draw(_spriteBatch);
            _player.Draw(_spriteBatch, _hurtBoxTex, _hitBoxTex);
            _scoreManager.Draw(_spriteBatch, _font);
            _hud.DrawHealth(_spriteBatch, _hurtBoxTex, _player.Health, _player);
            _phaseManager.Draw(_spriteBatch, _hurtBoxTex, _hitBoxTex, _isFlipped);
            _delisaster.Draw(_spriteBatch);
            _pauseMenu.Draw(_spriteBatch);
            DrawPlayerInvincibilityOverlay();
            _spriteBatch.End();

            // Game over screen draw (if applicable)
            if (_showGameOver)
            {
                _spriteBatch.Begin();
                _gameOverScreen.Draw(_spriteBatch);
                _spriteBatch.End();
            }
        }
        private void DrawBackground()
        {
            _bg1.Draw(_spriteBatch);
            _bg2.Draw(_spriteBatch);
            _bg3.Draw(_spriteBatch);
            _bg4.Draw(_spriteBatch);
            _bg5.Draw(_spriteBatch);
        }

        private void DrawPlayerInvincibilityOverlay()
        {
            if (_player.IsInvincible)
            {
                float alpha = MathHelper.Clamp(_player.InvincibilityTimer / _player.InvincibilityTime, 0f, 0.3f);
                _spriteBatch.Draw(
                    _redOverlay,
                    new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
                    Color.Red * alpha
                );
            }
        }
        private void ResetGame()
        {
            _phaseManager.Reset();
            _isFlipped = false;
            _player.Reset();
            _delisaster.ResetPosition();
            _delisasterHasDashed = false;
            _scoreManager.Reset();
            _platformManager.Reset();
            _showGameOver = false;
            _gameOverScreen.Reset();

            if (MediaPlayer.State != MediaState.Playing)
            {
                MediaPlayer.Play(_bmg);
            }
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.01f * SoundManager.BgmVolume;
        }
    }
}
