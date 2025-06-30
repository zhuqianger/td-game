using System;
using UnityEngine;

namespace Battle.Models
{
    [Serializable]
    public class GameMapData
    {
        public int Id;
        public string MapName;
        public string MapData;
        public DateTime CreatedAt;

        // 地图配置
        public int Width;
        public int Height;
        public int[] TerrainTypes;
        public Vector2Int[] SpawnPoints;
    }
} 