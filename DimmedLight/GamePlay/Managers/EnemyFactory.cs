using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimmedLight.GamePlay.Animated;
using DimmedLight.GamePlay.Enemies;

namespace DimmedLight.GamePlay.Managers
{
    public class EnemyFactory
    {
        private readonly AnimatedTexture _guiltIdleProto, _guiltAttackProto, _guiltDeathProto;
        private readonly AnimatedTexture _traumaIdleProto, _traumaAttackProto, _traumaDeathProto;
        private readonly AnimatedTexture _judgementIdleProto, _judgementDeathProto;
        private readonly Texture2D _bullet4Tex;
        private readonly Texture2D _bullet1Tex;

        public EnemyFactory(AnimatedTexture guiltIdle, AnimatedTexture guiltAttack, AnimatedTexture guiltDeath,
                            AnimatedTexture traumaIdle, AnimatedTexture traumaAttack, AnimatedTexture traumaDeath,
                            AnimatedTexture judgementIdle, AnimatedTexture judgementDeath,
                            Texture2D bullet4, Texture2D bullet1)
        {
            _guiltIdleProto = guiltIdle;
            _guiltAttackProto = guiltAttack;
            _guiltDeathProto = guiltDeath;
            _traumaIdleProto = traumaIdle;
            _traumaAttackProto = traumaAttack;
            _traumaDeathProto = traumaDeath;
            _judgementIdleProto = judgementIdle;
            _judgementDeathProto = judgementDeath;
            _bullet4Tex = bullet4;
            _bullet1Tex = bullet1;
        }

        private T InitializeEnemy<T>(T enemy, Vector2 pos, float spd, AnimatedTexture idle, AnimatedTexture death, AnimatedTexture attack = null) where T : EnemyBase
        {
            enemy.Position = pos;
            enemy.SetSpeed(spd);
            enemy.Idle = idle.Clone();
            enemy.Death = death.Clone();
            if (attack != null)
            {
                enemy.AttackAnim = attack.Clone();
            }
            enemy.IsDead = false;
            // enemy.DeathAnimationStarted = false; // This is handled in EnemyBase constructor/reset
            return enemy;
        }

        public Guilt CreateGuilt(Vector2 pos, float speed)
        {
            var g = new Guilt();
            InitializeEnemy(g, pos, speed, _guiltIdleProto, _guiltDeathProto, _guiltAttackProto);
            g.Attack = _guiltAttackProto.Clone(); // Guilt has a specific 'Attack' property
            return g;
        }
        public Trauma CreateTrauma(Vector2 pos, float speed)
        {
            var t = new Trauma(_bullet4Tex);
            InitializeEnemy(t, pos, speed, _traumaIdleProto, _traumaDeathProto, _traumaAttackProto);
            t.ProjectileObj = new Projectile(_bullet4Tex);
            return t;
        }
        public FloorTrauma CreateFloorTrauma(Vector2 pos, float speed)
        {
            var ft = new FloorTrauma(_bullet1Tex);
            InitializeEnemy(ft, pos, speed, _traumaIdleProto, _traumaDeathProto, _traumaAttackProto);
            ft.ProjectileObj = new Projectile(_bullet1Tex);
            return ft;
        }
        public Judgement CreateJudgement(Vector2 pos, float speed)
        {
            var j = new Judgement();
            InitializeEnemy(j, pos, speed, _judgementIdleProto, _judgementDeathProto);
            return j;
        }
    }
}
