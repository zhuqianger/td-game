using System.Collections;
using System.Collections.Generic;
using Common;
using Config.Stage;

namespace Stage
{
    public static class StageModel
    {
        public static Dictionary<int,Dictionary<int,Stage>> stageMap = new Dictionary<int,Dictionary<int,Stage>>();
        //收到关卡数据
        public static void OnStageDataReceive(List<Stage> stageList)
        {
            stageMap.Clear();
            List<StageConfig> configList = Config.ConfigManager.GetConfig<List<StageConfig>>("stage_config");
            
            
            EventManager.Send(EventType.OnStageDataReceive);
        }

        
        //关卡数据更新
        public static void OnStageDataUpdate(Stage stage)
        {
            List<StageConfig> configList = Config.ConfigManager.GetConfig<List<StageConfig>>("stage_config");
            
            EventManager.Send(EventType.OnStageDataUpdate);
        }
        
        
        

    }
}