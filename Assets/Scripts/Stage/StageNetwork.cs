using UnityEngine;
using System;
using System.Collections.Generic;
using Common;
using Network;
using Util;

namespace Stage
{
    public static class StageNetwork
    {
        public static bool _isInitialized = false;
        
        public static void Init()
        {
            if (!_isInitialized)
            {
                // 初始化逻辑
                RegisterMessageHandlers();
                Debug.Log("StageNetwork initialized.");
                _isInitialized = true;
            }
        }
        
        // 注册消息处理器
        private static void RegisterMessageHandlers()
        {
            // 为每个消息ID注册处理器
            NetworkManager.RegisterMessageHandler((int)MessageId.RESP_GET_PLAYER_STAGES, HandleGetPlayerStages);
            NetworkManager.RegisterMessageHandler((int)MessageId.RESP_SAVE_STAGE_RECORD, HandleSaveStageRecord);
        }

        // 处理消息的单独回调方法
        private static void HandleGetPlayerStages(byte[] data)
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log($"Received player stages: {json}");
            // 处理获取玩家关卡的逻辑
            CommonResponse<List<Stage>> response = GameUtil.Deserialize<CommonResponse<List<Stage>>>(data);
            StageModel.OnStageDataReceive(response.data);
        }

        private static void HandleSaveStageRecord(byte[] data)
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log($"Received save stage record response: {json}");
            // 处理保存关卡记录的逻辑
            CommonResponse<Stage> response = GameUtil.Deserialize<CommonResponse<Stage>>(data);
            StageModel.OnStageDataUpdate(response.data);
        }
        
        // 以下是与关卡相关的请求方法
        public static void RequestGetPlayerStages()
        {
            NetworkManager.SendJsonMessage((int)MessageId.REQ_GET_PLAYER_STAGES,null);
        }

        public static void RequestSaveStageRecord(int stageId,int star,int[] operatorIds)
        { 
            NetworkManager.SendJsonMessage((int)MessageId.REQ_SAVE_STAGE_RECORD, new { stageId = stageId,star = star,operatorIds = operatorIds});
        }
    }
}