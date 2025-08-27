using System;

namespace Config.Enemy
{
    [Serializable]
    public class EnemyConfig
    {
        public int enemyId;
        public string enemyName;
        public int enemyType;
        public int hp;
        public int attack;
        public int defense;
        public int magicResistance;
        public float moveSpeed;
        public int blockCount;
        public int[][] attackRange;
    }
}
