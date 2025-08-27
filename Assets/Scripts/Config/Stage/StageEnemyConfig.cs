using System;

namespace Config.Stage
{
    [Serializable]
    public class StageEnemyConfig
    {
        public int id;
        public MapConfig mapConfig;
        public SpawnConfig[] spawns;
    }

    [Serializable]
    public class MapConfig
    {
        public EntryPointConfig[] entryPoints;
        public TargetPointConfig targetPoint;
    }

    [Serializable]
    public class EntryPointConfig
    {
        public int id;
        public int[] position;
        public string name;
    }

    [Serializable]
    public class TargetPointConfig
    {
        public int[] position;
        public string name;
    }

    [Serializable]
    public class SpawnConfig
    {
        public int enemyId;
        public int spawnTime;
        public int entryPointId;
    }
}
