using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using Network;
using System.Collections.Generic;
using System.Text;
using Common;
using Network.Entity;
using Util;

namespace Player
{
    public static class PlayerNetwork
    {
        public static bool _isInitialized = false;
        
        // 初始化方法
        public static void Init()
        {
            if (!_isInitialized)
            {
                RegisterMessageHandlers();
                Debug.Log("PlayerNetwork initialized.");
                _isInitialized = true;
            }
        }
        
        // 注册消息处理器
        private static void RegisterMessageHandlers()
        {
            NetworkManager.RegisterMessageHandler((int)MessageId.RESP_LOGIN,HandleLogin);
            NetworkManager.RegisterMessageHandler((int)MessageId.RESP_CREATE_PLAYER, HandleCreatePlayer);
            NetworkManager.RegisterMessageHandler((int)MessageId.RESP_GET_PLAYER_INFO, HandleGetPlayerInfo);
            NetworkManager.RegisterMessageHandler((int)MessageId.RESP_UPDATE_PLAYER, HandleUpdatePlayer);
        }

        private static void HandleLogin(byte[] data)
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            CommonResponse<Player> response = GameUtil.Deserialize<CommonResponse<Player>>(data);
            if (response.success)
            {
                PlayerModel.OnPlayerDataReceive(response.data);
                SceneManager.LoadScene("GameScene");
            }
        }


        // 处理创建玩家响应
        private static void HandleCreatePlayer(byte[] data)
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log($"Received create player response: {json}");
            
            CommonResponse<Player> response = GameUtil.Deserialize<CommonResponse<Player>>(data);
            if (response.success)
            {
                PlayerModel.OnPlayerDataReceive(response.data);
            }
            else
            {
                Debug.LogError($"Create player failed: {response.message}");
            }
        }
        
        // 处理获取玩家信息响应
        private static void HandleGetPlayerInfo(byte[] data)
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log($"Received player info: {json}");
            
            CommonResponse<Player> response = GameUtil.Deserialize<CommonResponse<Player>>(data);
            if (response.success)
            {
                PlayerModel.OnPlayerDataReceive(response.data);
                
                // 登录成功，切换到GameScene
                Debug.Log("Login successful, switching to GameScene");
                
            }
            else
            {
                Debug.LogError($"Get player info failed: {response.message}");
            }
        }
        
        // 处理更新玩家响应
        private static void HandleUpdatePlayer(byte[] data)
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log($"Received update player response: {json}");
            
            CommonResponse<Player> response = GameUtil.Deserialize<CommonResponse<Player>>(data);
            if (response.success)
            {
                PlayerModel.OnPlayerDataUpdate(response.data);
            }
            else
            {
                Debug.LogError($"Update player failed: {response.message}");
            }
        }
        
        // 请求创建玩家
        public static void RequestCreatePlayer(string playerName)
        {
            if (!NetworkManager.IsConnected())
            {
                Debug.LogError("Not connected to server");
                return;
            }
            
            var request = new { playerName = playerName };
            NetworkManager.SendJsonMessage((int)MessageId.REQ_CREATE_PLAYER, request);
            Debug.Log($"Requested to create player: {playerName}");
        }
        
        // 请求获取玩家信息
        public static void RequestGetPlayerInfo()
        {
            if (!NetworkManager.IsConnected())
            {
                Debug.LogError("Not connected to server");
                return;
            }
            
            NetworkManager.SendStringMessage((int)MessageId.REQ_GET_PLAYER_INFO, "");
            Debug.Log("Requested player info");
        }
        
        // 请求更新玩家信息
        public static void RequestUpdatePlayer(string playerName)
        {
            if (!NetworkManager.IsConnected())
            {
                Debug.LogError("Not connected to server");
                return;
            }
            
            var request = new { playerName = playerName };
            NetworkManager.SendJsonMessage((int)MessageId.REQ_UPDATE_PLAYER, request);
            Debug.Log($"Requested to update player: {playerName}");
        }
    }
}