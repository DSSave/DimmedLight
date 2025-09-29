using DimmedLight.GamePlay.Animated;
using DimmedLight.GamePlay.Isplayer;
using DimmedLight.GamePlay.UI;
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
        public AnimatedTexture Idle;
        public AnimatedTexture AttackAnim;
        public AnimatedTexture Death;

        public Vector2 Position;
        public Rectangle HurtBox;
        public bool IsDead = false;
        public bool DeathAnimationStarted = false;
        public float DeathTimer = 0f;
        public float DeathDuration = 0.8f;
        public bool IsFlipped;
        protected float speed;
        public static SoundEffect ParryHit;

        public EnemyBase()
        {
            Idle = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            AttackAnim = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
            Death = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f);
        }
        public string EnemyType { get; protected set; }
        public abstract void Load(ContentManager content);
        public abstract void Update(GameTime gameTime, float delta, Player player, ref bool globalFlip, Delisaster delisaster, ScoreManager scoreManager);
        public abstract void Draw(SpriteBatch sb, Texture2D hurtBoxTex, Texture2D hitBoxTex, bool flip);

        public virtual void HandleDeathAnimation(float delta)
        {
            if (DeathAnimationStarted)
            {
                DeathTimer += delta;
                Death.UpdateFrame(delta);
                if (DeathTimer >= DeathDuration)
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
            ParryHit?.Play();
        }
    }
}
