using System;
using UnityEngine;

namespace Battle.Models
{
    [Serializable]
    public class UnitStateRecordData
    {
        public long Id;
        public long TurnId;
        public long UnitId;
        public Vector2Int Position;
        public int Hp;
        public int MaxHp;
        public int Mp;
        public int[] BuffIds;
        public string StateData; // JSON格式存储
        public DateTime CreatedAt;

        // 单位状态
        public bool IsAlive;
        public bool CanMove;
        public bool CanAttack;
    }
} 