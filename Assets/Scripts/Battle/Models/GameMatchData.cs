using System;
using UnityEngine;

namespace Battle.Models
{
    [Serializable]
    public class GameMatchData
    {
        public long Id;
        public int MatchType;
        public int Status;
        public long Player1Id;
        public long Player2Id;
        public long WinnerId;
        public int TotalTurns;
        public int MapId;
        public string InitialState;

        // 游戏状态
        public int CurrentTurn;
        public long CurrentPlayerId;
        public bool IsGameOver;
    }
} 