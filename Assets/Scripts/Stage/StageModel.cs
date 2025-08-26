using System.Collections;
using System.Collections.Generic;
using Common;

namespace Stage
{
    public static class StageModel
    {
        public static List<Stage> stageList = new List<Stage>();
        public static void OnStageListReceive(List<Stage> stageList)
        {
            StageModel.stageList = stageList;
            EventManager.Send(EventType.OnStageDataReceive);
        }
    }
}