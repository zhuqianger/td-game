using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Player
{
    public static class PlayerModel
    {
        // 玩家数据存储
        public static Player currentPlayer;
        
        // 初始化
        public static void Init()
        {
            currentPlayer = null;
        }
        
        // 接收玩家数据
        public static void OnPlayerDataReceive(Player player)
        {
            if (player == null)
            {
                Debug.LogWarning("Received null player data");
                return;
            }
            
            currentPlayer = player;
            Debug.Log($"Received player data: ID={player.id}, Name={player.playerName}");
        }
        
        // 更新玩家数据
        public static void OnPlayerDataUpdate(Player player)
        {
            if (player == null)
            {
                Debug.LogWarning("Cannot update null player data");
                return;
            }
            
            currentPlayer = player;
            Debug.Log($"Updated player data: ID={player.id}, Name={player.playerName}");
        }
        
        // 获取玩家ID
        public static int GetPlayerId()
        {
            return currentPlayer?.id ?? 0;
        }
        
        // 获取玩家名称
        public static string GetPlayerName()
        {
            return currentPlayer?.playerName ?? "";
        }
        
        // 检查玩家是否已登录
        public static bool IsPlayerLoggedIn()
        {
            return currentPlayer != null && currentPlayer.id > 0;
        }
    }
}