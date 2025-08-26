using UnityEngine;
using System;
using System.Collections;
using Network;
using System.Collections.Generic;
using Util;

namespace Operator
{
    public static class OperatorNetwork
    {
        private static bool _isInitialized = false;
        
        // 示例：初始化方法
        public static void Initialize()
        {
            if (!_isInitialized)
            {
                // 初始化逻辑
                RegisterMessageHandlers();
                Debug.Log("OperatorNetwork initialized.");
                _isInitialized = true;
            }
        }
        
        // 注册消息处理器
        private static void RegisterMessageHandlers()
        {
            // 为每个消息ID注册处理器
            NetworkManager.RegisterMessageHandler((int)MessageId.RESP_GET_PLAYER_OPERATORS, HandleGetPlayerOperators);
            NetworkManager.RegisterMessageHandler((int)MessageId.RESP_ADD_PLAYER_OPERATOR, HandleAddPlayerOperator);
            NetworkManager.RegisterMessageHandler((int)MessageId.RESP_LEVEL_UP_OPERATOR, HandleLevelUpOperator);
            NetworkManager.RegisterMessageHandler((int)MessageId.RESP_ELITE_OPERATOR, HandleEliteOperator);
            NetworkManager.RegisterMessageHandler((int)MessageId.RESP_UPGRADE_SKILL, HandleUpgradeSkill);
            NetworkManager.RegisterMessageHandler((int)MessageId.RESP_MASTER_SKILL, HandleMasterSkill);
            NetworkManager.RegisterMessageHandler((int)MessageId.RESP_UPDATE_OPERATOR_HP, HandleUpdateOperatorHp);
        }
        
        // 处理消息的单独回调方法
        private static void HandleGetPlayerOperators(byte[] data)
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log($"Received player operators: {json}");
            //处理收到玩家列表的逻辑
        }
        
        private static void HandleAddPlayerOperator(byte[] data)
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log($"Received add player operator response: {json}");
            // 处理添加玩家干员的逻辑
        }
        
        private static void HandleLevelUpOperator(byte[] data)
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log($"Received level up operator response: {json}");
            // 处理干员升级的逻辑
        }
        
        private static void HandleEliteOperator(byte[] data)
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log($"Received elite operator response: {json}");
            // 处理干员精英化的逻辑
        }
        
        private static void HandleUpgradeSkill(byte[] data)
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log($"Received upgrade skill response: {json}");
            // 处理技能升级的逻辑
        }
        
        private static void HandleMasterSkill(byte[] data)
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log($"Received master skill response: {json}");
            // 处理技能专精的逻辑
        }
        
        private static void HandleUpdateOperatorHp(byte[] data)
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log($"Received update operator HP response: {json}");
            // 处理更新干员HP的逻辑
        }
        
        // 以下是与干员相关的请求方法
        
        public static void RequestGetPlayerOperators(int playerId)
        {
            Debug.Log($"Sending request to get player operators for Player ID: {playerId}");
            NetworkManager.SendJsonMessage((int)MessageId.REQ_GET_PLAYER_OPERATORS, null);
        }
        
        public static void RequestAddPlayerOperator(int playerId, int operatorId)
        {
            Debug.Log($"Sending request to add operator {operatorId} to player {playerId}");
            NetworkManager.SendJsonMessage((int)MessageId.REQ_ADD_PLAYER_OPERATOR, new { PlayerId = playerId, OperatorId = operatorId });
        }
        
        public static void RequestLevelUpOperator(int operatorId)
        {
            Debug.Log($"Sending request to level up operator {operatorId}");
            NetworkManager.SendJsonMessage((int)MessageId.REQ_LEVEL_UP_OPERATOR, new { OperatorId = operatorId });
        }
        
        public static void RequestEliteOperator(int operatorId)
        {
            Debug.Log($"Sending request to elite operator {operatorId}");
            NetworkManager.SendJsonMessage((int)MessageId.REQ_ELITE_OPERATOR, new { OperatorId = operatorId });
        }
        
        public static void RequestUpgradeSkill(int operatorId, int skillId)
        {
            Debug.Log($"Sending request to upgrade skill {skillId} for operator {operatorId}");
            NetworkManager.SendJsonMessage((int)MessageId.REQ_UPGRADE_SKILL, new { OperatorId = operatorId, SkillId = skillId });
        }
        
        public static void RequestMasterSkill(int operatorId, int skillId)
        {
            Debug.Log($"Sending request to master skill {skillId} for operator {operatorId}");
            NetworkManager.SendJsonMessage((int)MessageId.REQ_MASTER_SKILL, new { OperatorId = operatorId, SkillId = skillId });
        }
        
        public static void RequestUpdateOperatorHp(int operatorId, int hp)
        {
            Debug.Log($"Sending request to update HP to {hp} for operator {operatorId}");
            NetworkManager.SendJsonMessage((int)MessageId.REQ_UPDATE_OPERATOR_HP, new { OperatorId = operatorId, Hp = hp });
        }
    }
}