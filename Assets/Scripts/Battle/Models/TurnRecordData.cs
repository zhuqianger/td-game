using System;
using UnityEngine;

namespace Battle.Models
{
    [Serializable]
    public class TurnRecordData
    {
        public long Id;
        public long MatchId;
        public int TurnNumber;
        public long PlayerId;
        public string TurnState; // JSON格式存储
        public DateTime CreatedAt;

        // 回合状态
        public bool IsCompleted;
        public int ActionCount;
    }
} 