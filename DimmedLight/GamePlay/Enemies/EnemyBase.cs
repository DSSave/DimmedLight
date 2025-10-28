using DimmedLight.GamePlay.Animated;
using DimmedLight.GamePlay.Isplayer;
using DimmedLight.GamePlay.UI;
using DimmedLight.MainMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.GamePlay.Enemies
{
    public abstract class EnemyBase //คลาสแม่ของศัตรูทุกตัว
    {
        public AnimatedTexture Idle { get; internal set; }
        public AnimatedTexture AttackAnim { get; internal set; }
        public AnimatedTexture Death { get; internal set; }

        public Vector2 Position;
        public Rectangle HurtBox;
        public bool IsDead { get; set; }
        public bool DeathAnimationStarted { get; protected set; }
        public bool IsFlipped { get; protected set; }
        public string EnemyType { get; protected set; }

        protected float speed;
        protected float deathTimer;
        protected const float DeathDuration = 0.8f;

        public static SoundEffect ParryHit { get; set; }
        public static SoundEffect EnemiesDead { get; set; }
        public static SoundEffect PlayerHit { get; set; }
        public static SoundEffect AmmoShoot { get; set; }
        protected EnemyBase()
        {
            Idle = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            AttackAnim = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            Death = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
        }
        public abstract void Load(ContentManager content);
        public abstract void Update(GameTime gameTime, float delta, Player player, ref bool globalFlip, Delisaster delisaster, ScoreManager scoreManager);
        public abstract void Draw(SpriteBatch sb, Texture2D hurtBoxTex, Texture2D hitBoxTex, bool flip);

        public virtual void HandleDeathAnimation(float delta)
        {
            if (DeathAnimationStarted)
            {
                deathTimer += delta;
                Death.UpdateFrame(delta);
                if (deathTimer >= DeathDuration)
                {
                    DeathAnimationStarted = false;
                }
            }
        }
        public virtual void OnKilled(ScoreManager scoreManager, string method)
        {
            scoreManager.AddPoints(EnemyType, method);
        }
        public virtual void SetSpeed(float spd) => speed = spd;
        protected void PlayParryHit()
        {
            ParryHit?.Play(0.3f * SoundManager.SfxVolume, 0f, 0f);
        }
    }
}