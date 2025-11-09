using DimmedLight.GamePlay.Background;
using DimmedLight.GamePlay.Enemies;
using DimmedLight.GamePlay.ETC;
using DimmedLight.GamePlay.Isplayer;
using DimmedLight.GamePlay.Managers;
using DimmedLight.GamePlay.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.GamePlay.Managers
{
    public class PhaseManager
    {
        public float PlatformSpeed => _currentPhase.PlatformSpeed;
        public int CurrentPhaseIndex => _currentIndex;
        public bool IsInEvent => _hellCloakEvent.IsActive || _hellCloakEvent.IsPreparing || (TutorialEvent?.IsActive == true);
        public TutorialEvent TutorialEvent { get; private set; }
        public IReadOnlyList<EnemyBase> ActiveEnemies => _activeEnemies;

        private readonly EnemyFactory _factory;
        private readonly List<EnemyBase> _activeEnemies = new List<EnemyBase>();
        public readonly HellCloakEvent _hellCloakEvent;
        private Phase[] _phases;
        private int _currentIndex;
        private Phase _currentPhase;
        private float _lastSpawnX;
        private float _phase3Timer;
        private float _nextEventTime;
        private bool _tutorialShown;
        private readonly Random _rng = new Random();
        private readonly ScoreManager _scoreManager;
        private PlatformManager _platformManager;
        private readonly Gameplay _gameplay;

        public PhaseManager(Gameplay gameplay, EnemyFactory factory, Player player, Delisaster delisaster, Camera camera,
            Texture2D hellCloakTheme, Texture2D tutorialImage, Texture2D parryProjecTex, Texture2D attackProjecTex,
            SoundEffect parryHit, GraphicsDevice graphicsDevice, ScoreManager scoreManager,
            PlatformManager platformManager)
        {
            _gameplay = gameplay;
            _factory = factory;
            _phases = new Phase[]
            {
                new TutorialPhase(),
                new WarmupPhase(),
                new FullPhase()
            };
            _currentIndex = 0;
            _currentPhase = _phases[_currentIndex];
            _currentPhase.Initialize();

            _hellCloakEvent = new HellCloakEvent(hellCloakTheme, player, delisaster, camera, parryProjecTex, attackProjecTex, parryHit, scoreManager, _platformManager);
            _scoreManager = scoreManager;
            TutorialEvent = new TutorialEvent(graphicsDevice, tutorialImage, _hellCloakEvent.StartEvent);
            _hellCloakEvent.OnPrepareFinished = () =>
            {
                if (!_tutorialShown)
                {
                    TutorialEvent.Start();
                    _tutorialShown = true;
                }
                else
                {
                    _hellCloakEvent.StartEvent();
                }
            };

            ResetEventTimers();
        }
        public void Update(GameTime gameTime, float delta, Player player, ref bool isFlipped, Delisaster delisaster, ScoreManager scoreManager, KeyboardState keyboardState, KeyboardState previousKeyboardState)
        {
            if(_gameplay != null && _gameplay.IsEventEndingAnimationPlaying)
            {
                UpdateEnemies(gameTime, delta, player, ref isFlipped, delisaster, scoreManager);
                return;
            }
            if (TutorialEvent?.IsActive == true)
            {
                TutorialEvent.Update(keyboardState, previousKeyboardState);
                return;
            }

            if (_hellCloakEvent.IsActive || _hellCloakEvent.IsPreparing)
            {
                _hellCloakEvent.Update(gameTime, delta, _currentPhase.PlatformSpeed, scoreManager);
                if (!_hellCloakEvent.IsActive && !_hellCloakEvent.IsPreparing)
                {
                    ResetEventTimers();
                }
                return;
            }

            if (!player.canWalk) return;

            UpdatePhase3EventTimer(delta);

            if (_gameplay == null || !_gameplay.IsEventEndingAnimationPlaying)
            {
                SpawnEnemies(delta);
            }
            UpdateEnemies(gameTime, delta, player, ref isFlipped, delisaster, scoreManager);

            CheckForPhaseCompletion();
        }
        private void UpdatePhase3EventTimer(float delta)
        {
            if (_currentIndex == 2 && !_hellCloakEvent.IsActive && !_hellCloakEvent.IsPreparing)
            {
                _phase3Timer += delta;
                if (_phase3Timer >= _nextEventTime)
                {
                    _hellCloakEvent.Prepare();
                    _activeEnemies.Clear();
                    _lastSpawnX = 0f;
                }
            }
        }
        private void SpawnEnemies(float delta)
        {
            var spawns = _currentPhase.GetSpawns(delta);
            foreach (var s in spawns)
            {
                float spawnX = s.Position.X;
                float rightmost = _activeEnemies.Any() ? _activeEnemies.Max(e => e.Position.X) : 0f;

                spawnX = Math.Max(spawnX, rightmost + _currentPhase.MinSpacing);
                spawnX = Math.Max(spawnX, _lastSpawnX + _currentPhase.MinSpacing);
                _lastSpawnX = spawnX;

                EnemyBase created = s.EnemyType switch
                {
                    "Guilt" => _factory.CreateGuilt(new Vector2(spawnX, s.Position.Y), s.Speed),
                    "Trauma" => _factory.CreateTrauma(new Vector2(spawnX, s.Position.Y), 6f),
                    "Judgement" => _factory.CreateJudgement(new Vector2(spawnX, s.Position.Y), s.Speed),
                    "FloorTrauma" => _factory.CreateFloorTrauma(new Vector2(spawnX, s.Position.Y), s.Speed),
                    _ => null
                };

                if (created != null)
                {
                    _activeEnemies.Add(created);
                    if (_currentPhase is TutorialPhase tutorialPhase)
                    {
                        tutorialPhase.SetCurrentFixedEnemy(created);
                    }
                }
            }
        }

        private void UpdateEnemies(GameTime gameTime, float delta, Player player, ref bool isFlipped, Delisaster delisaster, ScoreManager scoreManager)
        {
            for (int i = _activeEnemies.Count - 1; i >= 0; i--)
            {
                var e = _activeEnemies[i];
                if (!(e is Trauma))
                {
                    e.SetSpeed(_currentPhase.PlatformSpeed);
                }
                e.Update(gameTime, delta, player, ref isFlipped, delisaster, scoreManager);

                if (e.IsDead && !e.DeathAnimationStarted)
                {
                    _activeEnemies.RemoveAt(i);
                }
                else if (e.IsDead && e.DeathAnimationStarted && e.deathTimer >= EnemyBase.DeathDuration)
                {
                    _activeEnemies.RemoveAt(i);
                }
            }
        }

        private void CheckForPhaseCompletion()
        {
            if (_currentPhase.IsPhaseComplete() && _currentIndex < _phases.Length - 1)
            {
                _currentIndex++;
                _currentPhase = _phases[_currentIndex];
                _currentPhase.Initialize();
            }
        }
        public void Draw(SpriteBatch sp, Texture2D hurt, Texture2D hit, bool flip)
        {
            foreach (var e in _activeEnemies)
            {
                e.Draw(sp, hurt, hit, flip);
            }

            if (_hellCloakEvent.IsActive)
            {
                _hellCloakEvent.Draw(sp);
            }
            if (TutorialEvent?.IsActive == true)
            {
                TutorialEvent.Draw(sp);
            }
        }
        public void Reset()
        {
            _phases = new Phase[]
            {
                new TutorialPhase(),
                new WarmupPhase(),
                new FullPhase()
            };
            _currentIndex = 0;
            _currentPhase = _phases[0];
            _currentPhase.Initialize();
            _activeEnemies.Clear();
            _lastSpawnX = 0f;

            _hellCloakEvent.Reset();
            _tutorialShown = false;
            ResetEventTimers();
        }

        public void ResetCurrentPhase(int phaseIndex)
        {
            if (IsInEvent) return;

            if (phaseIndex >= 0 && phaseIndex < _phases.Length)
            {
                _phases = new Phase[]
            {
                new TutorialPhase(),
                new WarmupPhase(),
                new FullPhase()
            };
                _currentIndex = phaseIndex;
                _currentPhase = _phases[_currentIndex];
                _currentPhase.Initialize();
                _activeEnemies.Clear();
                _lastSpawnX = 0f;

                _hellCloakEvent.Reset();
                ResetEventTimers();
            }
        }

        private void ResetEventTimers()
        {
            _phase3Timer = 0f;
            _nextEventTime = _rng.Next(20, 40);
        }
    }
}
