using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.GamePlay.Managers
{
    public class WarmupPhase : Phase
    {
        public WarmupPhase(float startSpeed = 6f)
        {
            PlatformSpeed = startSpeed;
            MaxPlatformSpeed = 10f;
            SpeedIncreaseRate = 0.13f;
            spawnInterval = 2f;
            MinSpacing = 300f;
        }
        public override List<EnemySpawnInfo> GetSpawns(float delta)
        {
            var result = new List<EnemySpawnInfo>();
            timeSinceLastSpawn += delta;
            if (timeSinceLastSpawn >= spawnInterval)
            {
                timeSinceLastSpawn = 0f;
                string[] types = { "Guilt", "Trauma", "Judgement", "FloorTrauma" };
                string type = types[rnd.Next(types.Length)];
                float x = 1920 + rnd.Next(300, 800);
                float y = type == "Guilt" ? 670f : type == "Trauma" ? 330f : type == "Judgement" ? 720f : 445f;
                result.Add(new EnemySpawnInfo(type, new Vector2(x, y), PlatformSpeed));
            }

            if (PlatformSpeed < MaxPlatformSpeed)
            {
                PlatformSpeed += SpeedIncreaseRate * delta;
                if (PlatformSpeed > MaxPlatformSpeed) PlatformSpeed = MaxPlatformSpeed;
            }
            return result;
        }
    }
}
