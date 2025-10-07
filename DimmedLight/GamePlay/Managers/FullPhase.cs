using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.GamePlay.Managers
{
    public class FullPhase : Phase //Phase 3 
    {
        public FullPhase(float startSpeed = 10f) // ความเร็วเริ่มต้นของแพลตฟอร์ม
        {
            PlatformSpeed = startSpeed;
            MaxPlatformSpeed = 15f; // ความเร็วสูงสุดของแพลตฟอร์ม
            SpeedIncreaseRate = 0.1f; // อัตราการเพิ่มความเร็วของแพลตฟอร์ม
            spawnInterval = 1.5f; // ระยะเวลาระหว่างการเกิดศัตรู
            minSpacing = 200f; // ระยะห่างขั้นต่ำระหว่างศัตรู
        }
        public override List<EnemySpawnInfo> GetSpawns(float delta) //List ของศัตรูที่จะเกิด
        {
            List<EnemySpawnInfo> result = new List<EnemySpawnInfo>(); // รายการที่จะเก็บข้อมูลการเกิดศัตรู
            timeSinceLastSpawn += delta; // เพิ่มเวลาที่ผ่านไปตั้งแต่การเกิดครั้งล่าสุด
            if (timeSinceLastSpawn >= spawnInterval) // ถ้าเวลาที่ผ่านไปเกินกว่าระยะเวลาที่กำหนด
            {
                timeSinceLastSpawn = 0f;
                int countToSpawn = 1 + rnd.Next(0, 2); // สุ่มจำนวนศัตรูที่จะเกิด (1-2 ตัว)
                for (int i = 0; i < countToSpawn; i++) // วนลูปตามจำนวนศัตรูที่จะเกิด
                {
                    string[] types = new[] { "Guilt", "Trauma", "Judgement", "FloorTrauma" }; // ประเภทของศัตรูที่สามารถเกิดได้
                    string type = types[rnd.Next(types.Length)]; // สุ่มเลือกประเภทของศัตรู
                    float x = 1920 + rnd.Next(200, 450) + i * 200; // ตำแหน่ง X ที่ศัตรูจะเกิด (นอกหน้าจอทางขวา)
                    float y = type == "Guilt" ? 670f : type == "Trauma" ? 330f : type == "Judgement" ? 720f : 515f; // ตำแหน่ง Y ที่ศัตรูจะเกิด (ขึ้นอยู่กับประเภทของศัตรู)
                    result.Add(new EnemySpawnInfo(type, new Vector2(x, y), PlatformSpeed)); // เพิ่มข้อมูลการเกิดศัตรูลงในรายการ
                }
            }
            if (PlatformSpeed < MaxPlatformSpeed) // ถ้าความเร็วของแพลตฟอร์มยังไม่ถึงความเร็วสูงสุด
            {
                PlatformSpeed += SpeedIncreaseRate * delta; // เพิ่มความเร็วของแพลตฟอร์มตามอัตราการเพิ่มความเร็วและเวลาที่ผ่านไป
                if (PlatformSpeed > MaxPlatformSpeed)
                {
                    PlatformSpeed = MaxPlatformSpeed;
                }
            }
            return result; // คืนค่ารายการการเกิดศัตรู
        }
        public override bool IsPhaseComplete() => false; // เฟสนี้จะไม่สิ้นสุด
    }
}
