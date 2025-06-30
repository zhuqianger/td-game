using System;
using UnityEngine;

namespace Battle.Models
{
    [Serializable]
    public class ReplayBookmarkData
    {
        public long Id;
        public long MatchId;
        public string Name;
        public int TurnNumber;
        public string Description;
        public DateTime CreatedAt;

        // 书签状态
        public bool IsValid;
        public string BookmarkData; // JSON格式存储
    }
} 