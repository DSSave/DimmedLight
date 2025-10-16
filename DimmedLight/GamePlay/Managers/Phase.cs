using DimmedLight.GamePlay.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.GamePlay.Managers
{
    public abstract class Phase
    {
        public float PlatformSpeed { get; protected set; }
        public float MaxPlatformSpeed { get; protected set; }
        public float SpeedIncreaseRate { get; protected set; }
        public float MinSpacing { get; protected set; }

        protected float spawnInterval;
        protected float timeSinceLastSpawn;
        protected Random rnd = new Random();
        public abstract List<EnemySpawnInfo> GetSpawns(float delta);

        public virtual void Initialize()
        {
            timeSinceLastSpawn = 0f;
        }

        public virtual bool IsPhaseComplete() => PlatformSpeed >= MaxPlatformSpeed;
    }
}
