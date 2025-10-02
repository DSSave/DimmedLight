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
        private GraphicsDevice graphics;
        private Phase[] phases;
        private int currentIndex = 0;
        private Phase currentPhase;
        private EnemyFactory factory;
        private List<EnemyBase> activeEnemies = new List<EnemyBase>();
        private float lastSpawnX = 0f;

        private float minSpacing = 200f;
        public float PlatformSpeed => currentPhase.PlatformSpeed;
        private Random rng = new Random();
        private HellCloakEvent hellCloakEvent;
        private float phase3Timer = 0f;
        private float nextEventTime = 18f;
        public TutorialEvent TutorialEvent { get; private set; }
        public int CurrentPhaseIndex => currentIndex;
        private bool tutorialShown = false;
        public bool IsInEvent => hellCloakEvent.IsActive || hellCloakEvent.IsPreparing || (TutorialEvent != null && TutorialEvent.IsActive);

        public PhaseManager(EnemyFactory factory, Player player, Delisaster delisaster, Camera camera,
            Texture2D hellCloakTheme, Texture2D tutorialImage, Texture2D parryProjecTex, Texture2D attackProjecTex,
            SoundEffect parryHit, Song eventSound, Song bgm, GraphicsDevice graphicsDevice)
        {
            this.factory = factory;
            this.graphics = graphicsDevice;
            phases = new Phase[]
            {
                new TutorialPhase(6f),
                new WarmupPhase(6f),
                new FullPhase(10f)
            };
            currentIndex = 0; //ค่าเดิม 0
            currentPhase = phases[currentIndex];
            currentPhase.Initialize();

            hellCloakEvent = new HellCloakEvent(hellCloakTheme, player, delisaster, camera, parryProjecTex, attackProjecTex, parryHit, eventSound, bgm);
            TutorialEvent = new TutorialEvent(graphicsDevice, tutorialImage, () =>
            {
                hellCloakEvent.StartEvent();
            });
            hellCloakEvent.OnPrepareFinished = () =>
            {
                if (!tutorialShown)
                {
                    TutorialEvent.Start();
                    tutorialShown = true;
                }
                else
                {
                    hellCloakEvent.StartEvent();
                }
            };
        }
        public void Update(GameTime gameTime, float delta, Player player, ref bool isFlipped, Delisaster delisaster, ScoreManager scoreManager, KeyboardState keyboardState, KeyboardState previousKeyboardState)
        {
            if (TutorialEvent != null && TutorialEvent.IsActive)
            {
                TutorialEvent.Update(keyboardState, previousKeyboardState);
                return;
            }
            if (currentIndex == 2)
            {
                if (!hellCloakEvent.IsActive && !hellCloakEvent.IsPreparing)
                {
                    phase3Timer += delta;
                    if (phase3Timer >= nextEventTime)
                    {
                        hellCloakEvent.Prepare();
                        activeEnemies.Clear();
                        lastSpawnX = 0f;
                    }
                }
            }
            if (hellCloakEvent.IsActive || hellCloakEvent.IsPreparing)
            {
                hellCloakEvent.Update(gameTime, delta, currentPhase.PlatformSpeed, scoreManager);
                if (!hellCloakEvent.IsActive && !hellCloakEvent.IsPreparing)
                {
                    phase3Timer = 0f;
                    nextEventTime = rng.Next(50, 90); //random next eventTime
                    //nextEventTime = 50f;
                    lastSpawnX = 0f;
                }
                return;
            }
            if (!player.canWalk)
            {
                return;
            }
            var spawns = currentPhase.GetSpawns(delta);
            foreach (var s in spawns)
            {
                float spawnX = s.Position.X;
                float rightmost = activeEnemies.Count > 0 ? activeEnemies.Max(e => e.Position.X) : 0f; // ตำแหน่ง X ที่ขวาสุดของศัตรูที่กำลังทำงาน

                if (spawnX - rightmost < minSpacing) // ถ้าตำแหน่งที่จะเกิดศัตรูใกล้กับศัตรูที่ขวาสุดเกินไป
                    spawnX = rightmost + minSpacing;

                if (spawnX - lastSpawnX < minSpacing) // ถ้าตำแหน่งที่จะเกิดศัตรูใกล้กับศัตรูที่เกิดล่าสุดเกินไป
                    spawnX = lastSpawnX + minSpacing;

                lastSpawnX = spawnX; // อัปเดตตำแหน่ง X ของศัตรูที่เกิดล่าสุด

                EnemyBase created = s.EnemyType switch // สร้างศัตรูตามประเภทที่ระบุ
                {
                    "Guilt" => factory.CreateGuilt(new Vector2(spawnX, s.Position.Y), s.Speed),
                    "Trauma" => factory.CreateTrauma(new Vector2(spawnX, s.Position.Y), 6f),
                    "Judgement" => factory.CreateJudgement(new Vector2(spawnX, s.Position.Y), s.Speed),
                    "FloorTrauma" => factory.CreateFloorTrauma(new Vector2(spawnX, s.Position.Y), s.Speed),
                    _ => null // ถ้าประเภทไม่ตรงกับที่รู้จัก ให้คืนค่า null
                };
                if (created != null) // ถ้าสร้างศัตรูสำเร็จ
                {
                    activeEnemies.Add(created); // เพิ่มศัตรูที่สร้างลงในรายการศัตรูที่กำลังทำงาน
                    if (currentPhase is TutorialPhase tutorialPhase)
                    {
                        tutorialPhase.SetCurrentFixedEnemy(created);
                    }
                }
            }
            for (int i = activeEnemies.Count - 1; i >= 0; i--) // วนลูปย้อนกลับผ่านรายการศัตรูที่กำลังทำงาน
            {
                var e = activeEnemies[i]; // ดึงศัตรูตัวปัจจุบัน
                if (!(e is Trauma)) // ถ้าศัตรูไม่ใช่ประเภท Trauma
                {
                    e.SetSpeed(currentPhase.PlatformSpeed); // ตั้งค่าความเร็วของศัตรูให้ตรงกับความเร็วแพลตฟอร์มในเฟสปัจจุบัน
                }
                e.Update(gameTime, delta, player, ref isFlipped, delisaster, scoreManager); // อัปเดตสถานะของศัตรู
                if (e.IsDead && !e.DeathAnimationStarted)
                {
                    activeEnemies.RemoveAt(i); // ถ้าศัตรูตายและยังไม่เริ่มแอนิเมชันตาย ให้ลบศัตรูออกจากรายการ
                }

            }
            if (currentPhase.IsPhaseComplete() && currentIndex < phases.Length - 1) // ถ้าเฟสปัจจุบันจบแล้วและยังมีเฟสถัดไป
            {
                currentIndex++;
                currentPhase = phases[currentIndex];
                currentPhase.Initialize();
                //activeEnemies.Clear();
                //lastSpawnX = 0;

            }
        }
        public void Draw(SpriteBatch sp, Texture2D hurt, Texture2D hit, bool flip)
        {
            foreach (var e in activeEnemies)
            {
                e.Draw(sp, hurt, hit, flip);
            }

            if (hellCloakEvent.IsActive)
            {
                hellCloakEvent.Draw(sp);
            }
            if (TutorialEvent != null && TutorialEvent.IsActive)
            {
                TutorialEvent.Draw(sp);
                return;
            }
        }
        public void Reset()
        {
            phases = new Phase[]
            {
                new TutorialPhase(6f),
                new WarmupPhase(6f),
                new FullPhase(10f)
            };
            currentIndex = 0;
            currentPhase = phases[0];
            currentPhase.Initialize();
            activeEnemies.Clear();
            lastSpawnX = 0f;

            hellCloakEvent.Reset();
            tutorialShown = false;
            phase3Timer = 0f;
            nextEventTime = 30f;
        }
        public void ResetCurrentPhase(int phaseIndex)
        {
            if (hellCloakEvent.IsActive || hellCloakEvent.IsPreparing)
            {
                return;
            }
            if (phaseIndex >= 0 && phaseIndex < phases.Length)
            {
                phases = new Phase[]
                {
                    new TutorialPhase(6f),
                    new WarmupPhase(6f),
                    new FullPhase(10f)
                };
                currentIndex = phaseIndex;
                currentPhase = phases[currentIndex];

                currentPhase.Initialize();
                activeEnemies.Clear();
                lastSpawnX = 0f;

                hellCloakEvent.Reset();
                phase3Timer = 0f;
                nextEventTime = 30f;
            }
        }
    }
}
