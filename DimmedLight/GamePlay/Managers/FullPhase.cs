using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.GamePlay.Managers
{
    public class FullPhase : Phase
    {
        public FullPhase(float startSpeed = 10f)
        {
            PlatformSpeed = startSpeed;
            MaxPlatformSpeed = 15f;
            SpeedIncreaseRate = 0.1f;
            spawnInterval = 1.5f;
            MinSpacing = 200f;
        }
        public override List<EnemySpawnInfo> GetSpawns(float delta)
        {
            List<EnemySpawnInfo> result = new List<EnemySpawnInfo>();
            timeSinceLastSpawn += delta;
            if (timeSinceLastSpawn >= spawnInterval)
            {
                timeSinceLastSpawn = 0f;
                int countToSpawn = 1 + rnd.Next(0, 2);
                for (int i = 0; i < countToSpawn; i++)
                {
                    string[] types = new[] { "Guilt", "Trauma", "Judgement", "FloorTrauma" };
                    string type = types[rnd.Next(types.Length)];
                    float x = 1920 + rnd.Next(200, 450) + i * 200;
                    float y = type == "Guilt" ? 670f : type == "Trauma" ? 330f : type == "Judgement" ? 720f : 445f;
                    result.Add(new EnemySpawnInfo(type, new Vector2(x, y), PlatformSpeed));
                }
            }
            if (PlatformSpeed < MaxPlatformSpeed) 
            {
                PlatformSpeed += SpeedIncreaseRate * delta; 
                if (PlatformSpeed > MaxPlatformSpeed)
                {
                    PlatformSpeed = MaxPlatformSpeed;
                }
            }
            return result; 
        }
        public override bool IsPhaseComplete() => false; 
    }
}
