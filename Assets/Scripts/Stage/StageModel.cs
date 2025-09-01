using System.Collections;
using System.Collections.Generic;
using Common;
using Config;
using Config.Stage;
using UnityEngine;
using EventType = Common.EventType;


namespace Stage
{
    public static class StageModel
    {
        public static Dictionary<int,Dictionary<int,Stage>> stageMap = new Dictionary<int,Dictionary<int,Stage>>();
        
        
        public static void Init()
        {
            stageMap = new Dictionary<int, Dictionary<int, Stage>>();
        }
        
        //收到关卡数据
        public static void OnStageDataReceive(List<Stage> stageList)
        {
            if (stageList == null || stageList.Count == 0)
            {
                Debug.LogWarning("Received empty stage list");
                return;
            }
            
            stageMap.Clear();
            
            // 遍历关卡列表，按章节分组存储
            foreach (var stage in stageList)
            {
                if (stage != null)
                {
                    // 获取关卡配置信息
                    StageConfig stageConfig = ConfigManager.GetConfigById<StageConfig>("stage_config", stage.stageId);
                    if (stageConfig != null)
                    {
                        int chapter = stageConfig.chapter;
                        
                        // 如果章节不存在，创建新的章节字典
                        if (!stageMap.ContainsKey(chapter))
                        {
                            stageMap[chapter] = new Dictionary<int, Stage>();
                        }
                        
                        // 将关卡添加到对应章节
                        stageMap[chapter][stage.stageId] = stage;
                    }
                    else
                    {
                        Debug.LogWarning($"Stage config not found for stage ID: {stage.stageId}");
                    }
                }
            }
            
            Debug.Log($"Successfully stored {stageList.Count} stages in {stageMap.Count} chapters");
            
            EventManager.Send(EventType.OnStageDataReceive);
        }

        //关卡数据更新
        public static void OnStageDataUpdate(Stage stage)
        {
            if (stage == null)
            {
                Debug.LogWarning("Cannot update null stage");
                return;
            }
            
            // 获取关卡配置信息
            StageConfig stageConfig = ConfigManager.GetConfigById<StageConfig>("stage_config", stage.stageId);
            if (stageConfig == null)
            {
                Debug.LogWarning($"Stage config not found for stage ID: {stage.stageId}");
                return;
            }
            
            int chapter = stageConfig.chapter;
            
            // 确保章节存在
            if (!stageMap.ContainsKey(chapter))
            {
                stageMap[chapter] = new Dictionary<int, Stage>();
            }
            
            // 更新或添加关卡数据
            stageMap[chapter][stage.stageId] = stage;
            
            Debug.Log($"Updated stage {stage.stageId} in chapter {chapter} (Star: {stage.star})");
            
            EventManager.Send(EventType.OnStageDataUpdate);
        }
    }
}