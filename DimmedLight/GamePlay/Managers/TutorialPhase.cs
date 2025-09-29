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
        //private bool initialSpawnd = false; // ตรวจสอบว่าศัตรูเริ่มต้นถูกสร้างแล้วหรือไม่
        private int fixedSpawnIndex = 0; // index ของศัตรูที่ต้อง spawn ต่อไป
        private EnemyBase currentFixedEnemy = null; // เก็บ enemy ที่ spawn ออกมาแล้ว

        private (string type, Vector2 pos)[] fixedEnemies = new (string, Vector2)[]
        {
            ("Guilt", new Vector2(2100, 695)),
            ("Trauma", new Vector2(2100, 380)),
            ("Judgement", new Vector2(2100, 763))
        };
        public TutorialPhase(float startSpeed = 6f)
        {
            PlatformSpeed = startSpeed;
            MaxPlatformSpeed = 6f;
            SpeedIncreaseRate = 0.3f;
            spawnInterval = 2.5f;
            minSpacing = 400f;
        }
        public override void Initialize()
        {
            base.Initialize();
            //initialSpawnd = false; // รีเซ็ตสถานะการเกิดศัตรูเริ่มต้น
            timeSinceLastSpawn = 0;
            PlatformSpeed = 6f;
            fixedSpawnIndex = 0;
            currentFixedEnemy = null;
        }
        public override List<EnemySpawnInfo> GetSpawns(float delta) //List ของศัตรูที่จะเกิด
        {
            List<EnemySpawnInfo> result = new List<EnemySpawnInfo>(); // รายการที่จะเก็บข้อมูลการเกิดศัตรู
            if (fixedSpawnIndex < fixedEnemies.Length)
            {
                if (currentFixedEnemy == null || currentFixedEnemy.IsDead)
                {
                    var (type, pos) = fixedEnemies[fixedSpawnIndex];
                    result.Add(new EnemySpawnInfo(type, pos, PlatformSpeed));
                    fixedSpawnIndex++;
                }
                return result;
            }
            /*timeSinceLastSpawn += delta;
            if (timeSinceLastSpawn >= spawnInterval) // ถ้าเวลาที่ผ่านไปเกินกว่าระยะเวลาที่กำหนด
            {
                timeSinceLastSpawn = 0f;
                string[] types = new[] { "Guilt", "Trauma", "Judgement", "FloorTrauma" }; // ประเภทของศัตรูที่สามารถเกิดได้
                string type = types[rnd.Next(types.Length)]; // สุ่มเลือกประเภทของศัตรู
                float x = 1920 + rnd.Next(200, 450); // ตำแหน่ง X ที่ศัตรูจะเกิด (นอกหน้าจอทางขวา)
                float y = type == "Guilt" ? 695f : type == "Trauma" ? 380f : type == "Judgement" ? 763f : 655f; // ตำแหน่ง Y ที่ศัตรูจะเกิด (ขึ้นอยู่กับประเภทของศัตรู)
                result.Add(new EnemySpawnInfo(type, new Vector2(x, y), PlatformSpeed)); // เพิ่มข้อมูลการเกิดศัตรูลงในรายการ
            }
            if (PlatformSpeed < MaxPlatformSpeed)
            {
                PlatformSpeed += SpeedIncreaseRate * delta;
                if (PlatformSpeed > MaxPlatformSpeed)
                {
                    PlatformSpeed = MaxPlatformSpeed;
                }
            }*/
           
            return result;
        }
        public void SetCurrentFixedEnemy(EnemyBase enemy)
        {
            currentFixedEnemy = enemy;
        }
        public override bool IsPhaseComplete()
        {
            return fixedSpawnIndex >= fixedEnemies.Length && (currentFixedEnemy == null || currentFixedEnemy.IsDead);
        }
    }
}
