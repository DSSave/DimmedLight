using DimmedLight.GamePlay.Enemies;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.GamePlay.Managers
{
    public class TutorialPhase : Phase //Phase 1
    {
        private int _fixedSpawnIndex;
        private EnemyBase _currentFixedEnemy;

        private readonly (string type, Vector2 pos)[] _fixedEnemies =
        {
            ("Guilt", new Vector2(2100, 670)),
            ("Trauma", new Vector2(2100, 330)),
            ("Judgement", new Vector2(2100, 720))
        };
        public TutorialPhase(float startSpeed = 6f)
        {
            PlatformSpeed = startSpeed;
            MaxPlatformSpeed = 6f;
            SpeedIncreaseRate = 0.3f;
            spawnInterval = 2.5f;
            MinSpacing = 400f;
        }
        public override void Initialize()
        {
            base.Initialize();
            _fixedSpawnIndex = 0;
            _currentFixedEnemy = null;
        }
        public override List<EnemySpawnInfo> GetSpawns(float delta)
        {
            var result = new List<EnemySpawnInfo>();
            if (_fixedSpawnIndex < _fixedEnemies.Length)
            {
                if (_currentFixedEnemy == null || _currentFixedEnemy.IsDead)
                {
                    var (type, pos) = _fixedEnemies[_fixedSpawnIndex];
                    result.Add(new EnemySpawnInfo(type, pos, PlatformSpeed));
                    _fixedSpawnIndex++;
                }
            }
            return result;
        }
        public void SetCurrentFixedEnemy(EnemyBase enemy) => _currentFixedEnemy = enemy;

        public override bool IsPhaseComplete() => _fixedSpawnIndex >= _fixedEnemies.Length && (_currentFixedEnemy == null || _currentFixedEnemy.IsDead);
    }
}
