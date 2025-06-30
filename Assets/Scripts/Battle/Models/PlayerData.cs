using System;
using UnityEngine;

namespace Battle.Models
{
    [Serializable]
    public class PlayerData
    {
        public long Id;
        public string Username;
        public DateTime CreatedAt;
        public DateTime LastLoginAt;

        // 游戏相关数据
        public int TeamId;
        public Vector2Int[] UnitPositions;
        public int[] UnitIds;
    }
} 