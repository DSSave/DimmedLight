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
        private AnimatedTexture guiltIdleProto, guiltAttackProto, guiltDeathProto;
        private AnimatedTexture traumaIdleProto, traumaAttackProto, traumaDeathProto;
        private AnimatedTexture judgementIdleProto, judgementDeathProto;
        private Texture2D projectileTex;
        private Texture2D bullet4Tex;
        private Texture2D bullet1Tex;
        public EnemyFactory(AnimatedTexture guiltIdle, AnimatedTexture guiltAttack, AnimatedTexture guiltDeath,
                            AnimatedTexture traumaIdle, AnimatedTexture traumaAttack, AnimatedTexture traumaDeath,
                            AnimatedTexture judgementIdle, AnimatedTexture judgementDeath,
                            Texture2D bullet4, Texture2D bullet1)
        {
            guiltIdleProto = guiltIdle;
            guiltAttackProto = guiltAttack;
            guiltDeathProto = guiltDeath;
            traumaIdleProto = traumaIdle;
            traumaAttackProto = traumaAttack;
            traumaDeathProto = traumaDeath;
            judgementIdleProto = judgementIdle;
            judgementDeathProto = judgementDeath;
            bullet4Tex = bullet4;
            bullet1Tex = bullet1;
        }

        public Guilt CreateGuilt(Vector2 pos, float speed) // สร้างศัตรู Guilt
        {
            var g = new Guilt(); //ส่ง texture ของโปรเจกไทล์ไปยัง Constructor ของ Guilt
            g.Idle = guiltIdleProto.Clone();
            g.Attack = guiltAttackProto.Clone();
            g.Death = guiltDeathProto.Clone();
            g.Position = pos;
            g.SetSpeed(speed);
            g.IsDead = false;
            g.DeathAnimationStarted = false;
            g.DeathTimer = 0f;
            return g; // คืนค่า instance ของ Guilt
        }
        public Trauma CreateTrauma(Vector2 pos, float speed) // สร้างศัตรู Trauma
        {
            var t = new Trauma(bullet4Tex); // ส่ง texture ของโปรเจกไทล์ไปยัง Constructor ของ Trauma
            t.Idle = traumaIdleProto.Clone();
            t.AttackAnim = traumaAttackProto.Clone();
            t.Death = traumaDeathProto.Clone();
            t.Position = pos;
            t.ProjectileObj = new Projectile(bullet4Tex);
            t.SetSpeed(speed);
            t.IsDead = false;
            t.DeathAnimationStarted = false;
            t.DeathTimer = 0f;
            return t; // คืนค่า instance ของ Trauma
        }
        public FloorTrauma CreateFloorTrauma(Vector2 pos, float speed) // สร้างศัตรู FloorTrauma
        {
            var ft = new FloorTrauma(bullet1Tex);
            ft.Idle = traumaIdleProto.Clone();
            ft.AttackAnim = traumaAttackProto.Clone();
            ft.Death = traumaDeathProto.Clone();
            ft.Position = pos;
            ft.ProjectileObj = new Projectile(bullet1Tex);
            ft.SetSpeed(speed);
            ft.IsDead = false;
            ft.DeathAnimationStarted = false;
            ft.DeathTimer = 0f;
            return ft;

        }
        public Judgement CreateJudgement(Vector2 pos, float speed) // สร้างศัตรู Judgement
        {
            var j = new Judgement();
            j.Idle = judgementIdleProto.Clone();
            j.Death = judgementDeathProto.Clone();
            j.Position = pos;
            j.SetSpeed(speed);
            j.IsDead = false;
            j.DeathAnimationStarted = false;
            j.DeathTimer = 0f;
            return j;
        }
    }
}
