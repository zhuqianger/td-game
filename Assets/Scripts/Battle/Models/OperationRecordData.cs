using System;
using UnityEngine;

namespace Battle.Models
{
    [Serializable]
    public class OperationRecordData
    {
        public long Id;
        public long TurnId;
        public int OperationType;
        public long UnitId;
        public Vector2Int SourcePosition;
        public Vector2Int TargetPosition;
        public string OperationData; // JSON格式存储
        public DateTime CreatedAt;

        // 操作状态
        public bool IsValid;
        public string Result;
    }
} 