using DimmedLight.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.Managers
{
    public abstract class Phase
    {
        public float PlatformSpeed { get; protected set; }
        public float MaxPlatformSpeed { get; protected set; }
        public float SpeedIncreaseRate { get; protected set; }

        protected float spawnInterval; // ระยะเวลาระหว่างการเกิดศัตรู
        protected float timeSinceLastSpawn; // เวลาที่ผ่านไปตั้งแต่การเกิดครั้งล่าสุด
        protected float minSpacing; // ระยะห่างขั้นต่ำระหว่างศัตรู
        protected Random rnd = new Random();
        public List<EnemyBase> ActiveEnemies { get; set; } = new List<EnemyBase>();
        public abstract List<EnemySpawnInfo> GetSpawns(float delta); //List ของศัตรูที่จะเกิด
        public virtual void Initialize() //รีเซ็ตค่า
        {
            timeSinceLastSpawn = 0f;

        }
        public virtual bool IsPhaseComplete() // เฟสนี้จะสิ้นสุดเมื่อความเร็วของแพลตฟอร์มถึงความเร็วสูงสุด
        {
            return PlatformSpeed >= MaxPlatformSpeed; // เฟสนี้จะสิ้นสุดเมื่อความเร็วของแพลตฟอร์มถึงความเร็วสูงสุด
        }
    }
}
