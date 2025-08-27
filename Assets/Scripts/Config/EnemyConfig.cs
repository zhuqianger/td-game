using UnityEngine;
using System;

namespace Config
{
    [Serializable]
    public class EnemyConfig
    {
        public int id;
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

    // 使用示例
    public static class EnemyConfigHelper
    {
        public static EnemyConfig GetEnemy(int enemyId)
        {
            return ConfigManager.GetConfigById<EnemyConfig>("enemy_config", enemyId);
        }
        
        public static EnemyConfig[] GetAllEnemies()
        {
            var allIds = ConfigManager.GetAllConfigIds("enemy_config");
            var enemies = new EnemyConfig[allIds.Count];
            
            for (int i = 0; i < allIds.Count; i++)
            {
                enemies[i] = GetEnemy(allIds[i]);
            }
            
            return enemies;
        }
        
        public static bool IsEnemyExists(int enemyId)
        {
            return ConfigManager.HasConfig("enemy_config", enemyId);
        }
    }
}
