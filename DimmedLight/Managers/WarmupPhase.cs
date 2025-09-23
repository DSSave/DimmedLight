using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.Managers
{
    public class WarmupPhase : Phase //Phase 2
    {
        public WarmupPhase(float startSpeed = 6f)
        {
            PlatformSpeed = startSpeed;
            MaxPlatformSpeed = 10f;
            SpeedIncreaseRate = 0.13f;
            spawnInterval = 2f; // ระยะเวลาระหว่างการเกิดศัตรู
            minSpacing = 300f; // ระยะห่างขั้นต่ำระหว่างศัตรู
        }
        public override List<EnemySpawnInfo> GetSpawns(float delta) //List ของศัตรูที่จะเกิด
        {
            List<EnemySpawnInfo> result = new List<EnemySpawnInfo>(); // รายการที่จะเก็บข้อมูลการเกิดศัตรู
            timeSinceLastSpawn += delta;
            if (timeSinceLastSpawn >= spawnInterval)
            {
                timeSinceLastSpawn = 0f;
                string[] types = new[] { "Guilt", "Trauma", "Judgement", "FloorTrauma" };
                string type = types[rnd.Next(types.Length)];
                float x = 1920 + rnd.Next(300, 800);
                float y = type == "Guilt" ? 695f : type == "Trauma" ? 380f : type == "Judgement" ? 763f : 655f;
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
