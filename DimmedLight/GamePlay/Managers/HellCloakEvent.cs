using DimmedLight.GamePlay.Animated;
using DimmedLight.GamePlay.Background;
using DimmedLight.GamePlay.Enemies;
using DimmedLight.GamePlay.ETC;
using DimmedLight.GamePlay.Isplayer;
using DimmedLight.GamePlay.UI;
using DimmedLight.MainMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DimmedLight.GamePlay.Managers
{
    public class HellCloakEvent
    {
        private Texture2D HellCloakTheme;
        private Player player;
        private Delisaster delisaster;
        private Camera camera;

        private float duration = 25f; //ค่าเดิม 45 วินาที
        private float eventElapsed = 0f;

        public bool IsPreparing { get; private set; }
        public bool IsActive { get; private set; }
        private float prepareTime = 3f;
        private float prepareTimer = 0f;

        private List<Projectile> projectiles = new List<Projectile>();
        private Texture2D parryProjecTex;
        private Texture2D attackProjecTex;
        private float delayShoot = 5f;
        private float shootTimer = 0f;
        private float timer = 0f;
        private float spawnCooldown;
        private float spawnTimer = 0f;

        private Random rng = new Random();
        private bool playerHit = false;
        private readonly SoundEffect _parryHit;
        private readonly Song _eventSong;
        private readonly Song _bmg;
        private PlatformManager _platformManager;
        public Action OnPrepareFinished;
        private ScoreManager _scoreManager;

        private bool playerhit = false;
        private readonly List<string> _failMessages = new List<string>
        {
            "It's all your FAULT!!!",
            "Who was the one who killed her?",
            "Run away!!, Run away from your Mistakes...",
            "Leave Everything Behind"
        };
        private readonly List<string> _successMessages = new List<string>
        {
            "You did it...",
            "She is at Peace now.",
            "Light is brightest in the dark",
            "Keep moving forward."
        };
        public Action<string, bool> OnEventFinishTrigger;
        public void Prepare()
        {
            IsPreparing = true;
            prepareTimer = 0f;
            camera.StartShake(prepareTime, 20f);
        }
        public HellCloakEvent(Texture2D hellCloakTheme, Player player, Delisaster delisaster, Camera camera,
            Texture2D parryProjecTex, Texture2D attackProjecTex, SoundEffect parryHit, Song eventSong, Song bmg,
            ScoreManager scoreManager, PlatformManager platformManager)
        {
            HellCloakTheme = hellCloakTheme;
            this.player = player;
            this.delisaster = delisaster;
            this.camera = camera;
            this.parryProjecTex = parryProjecTex;
            this.attackProjecTex = attackProjecTex;
            _parryHit = parryHit;
            _eventSong = eventSong;
            _bmg = bmg;
            _scoreManager = scoreManager;
            _platformManager = platformManager;
        }

        public HellCloakEvent(Delisaster delisaster, Camera camera)
        {
            this.delisaster = delisaster;
            this.camera = camera;
        }

        public void StartEvent()
        {
            IsActive = true;
            IsPreparing = false;
            prepareTimer = 0f;
            shootTimer = 0f;

            player.SetEvent(true);
            delisaster.IsInEvent = true;
            delisaster.movetToRight(new Vector2(1220, -50));
            camera.MoveTo(new Vector2(0, 130));

            player.canWalk = false;
        }

        public void Update(GameTime gameTime, float delta, float currentPhaseSpeed, ScoreManager scoreManager)
        {
            if (IsPreparing)
            {
                prepareTimer += delta;
                camera.MoveCamTocenter(delta);
                if (prepareTimer >= prepareTime)
                {
                    IsPreparing = false;
                    OnPrepareFinished?.Invoke();
                    return;
                }
                return;
            }
            if (!IsActive) return;

            eventElapsed += delta;
            timer += delta;
            spawnTimer += delta;
            camera.MoveCamTocenter(delta);
            if (!IsPreparing)
            {
                shootTimer += delta;
                if (delayShoot <= shootTimer)
                {
                    if (spawnTimer >= spawnCooldown)
                    {
                        spawnTimer = 0f;
                        FireProjectile(currentPhaseSpeed * 60);
                        if (duration >= 16f)
                        {
                            spawnCooldown = (float)(rng.NextDouble() * 0.4 + 0.5);
                        }else if(duration >= 10f)
                        {
                            spawnCooldown = (float)(rng.NextDouble() * 0.2 + 0.3);
                        }else if(duration >= 4f)
                        {
                            spawnCooldown = (float)(rng.NextDouble() * 0.1 + 0.2);
                        }
                        /*else
                        {
                            spawnCooldown = (float)(rng.NextDouble() * 0.2 + 0.25);
                        }*/
                    }

                }

            }
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update(delta);
                bool projectileRemoved = false;
                if (projectiles[i].canParry && projectiles[i].HitBox.Intersects(player.HitBoxParry) && player.IsParrying)
                {
                    _parryHit?.Play(0.3f * SoundManager.SfxVolume, 0f, 0f);
                    _scoreManager?.IncreaseEventCombo();
                    projectiles.RemoveAt(i);
                    projectileRemoved = true;
                    continue;
                }
                if (!projectileRemoved && projectiles[i].canAttack && projectiles[i].HitBox.Intersects(player.HitBoxAttack) && player.IsAttacking)
                {
                    _scoreManager?.IncreaseEventCombo();
                    projectiles.RemoveAt(i);
                    projectileRemoved = true;
                    continue;
                }
                if (!projectileRemoved && projectiles[i].HitBox.Intersects(player.HurtBox) && !player.IsInvincible)
                {
                    playerHit = true;
                    _scoreManager?.ResetEventCombo();
                    End(false);
                    return;
                }
                if (!projectileRemoved && (!projectiles[i].Active || projectiles[i].Position.X < -projectiles[i].Texture.Width))
                {
                    projectiles.RemoveAt(i);
                }

            }

            if (eventElapsed >= duration)
            {
                scoreManager.EventBonus();
                End(true);
            }
            if (playerHit)
            {
                _scoreManager?.ResetEventCombo();
                End(false);
                return;
            }
        }
        private void FireProjectile(float speed)
        {
            bool parryable = rng.Next(2) == 0;
            Texture2D tex = parryable ? parryProjecTex : attackProjecTex;

            Projectile p = new Projectile(tex)
            {
                Position = delisaster.ProjectileSpawn(),
                Speed = speed,
                Direction = new Vector2(-1, 0),
                Active = true,
                canAttack = !parryable,
                canParry = parryable
            };
            projectiles.Add(p);
        }
        public void Draw(SpriteBatch sb)
        {
            foreach (var p in projectiles)
            {
                p.Draw(sb);
            }

            if (HellCloakTheme != null)
            {
                sb.Draw(HellCloakTheme, new Rectangle(0, 0, 1920, 1080), Color.White);
            }
        }

        public void End(bool success)
        {
            if(!IsActive && !IsPreparing) 
                return;
            IsActive = false;
            IsPreparing = false;

            string message;
            if(success)
            {
                int index = rng.Next(_successMessages.Count);
                message = _successMessages[index];
            }
            else
            {
                int index = rng.Next(_failMessages.Count);
                message = _failMessages[index];
            }
            OnEventFinishTrigger?.Invoke(message, success);
        }
        public void PostTextEnd()
        {
            eventElapsed = 0f;
            prepareTime = 0f;
            shootTimer = 0f;
            spawnTimer = 0f;
            projectiles.Clear();
            playerHit = false;

            if(player != null)
                player.SetEvent(false);
            if (delisaster != null)
                delisaster.ResetPosition();
            if (camera != null)
                camera.ResetPosition();
            _scoreManager.ResetEventCombo();
            _platformManager?.ResetAssetTimer();
        }
        public void Reset()
        {
            IsActive = false;
            IsPreparing = false;
            eventElapsed = 0f;
            prepareTimer = 0f;
            shootTimer = 0f;
            spawnTimer = 0f;
            projectiles.Clear();
            playerHit = false;

            if (player != null)
                player.SetEvent(false);
            if (delisaster != null)
                delisaster.ResetPosition();
            if (camera != null)
                camera.ResetPosition();
            _scoreManager.ResetEventCombo();
            _platformManager?.ResetAssetTimer();
        }
    }
}
