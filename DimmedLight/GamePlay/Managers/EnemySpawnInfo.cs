using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace DimmedLight.GamePlay.Managers
{
    public class EnemySpawnInfo // ข้อมูลการเกิดของศัตรู
    {
        public string EnemyType;
        public Vector2 Position;
        public float Speed;

        public EnemySpawnInfo(string type, Vector2 pos, float speed) // constructor
        {
            EnemyType = type; // ชนิดของศัตรู
            Position = pos; // ตำแหน่งที่เกิด
            Speed = speed; // ความเร็วของศัตรู
        }
    }
}
