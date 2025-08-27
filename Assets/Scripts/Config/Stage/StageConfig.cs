using System;

namespace Config.Stage
{
    [Serializable]
    public class StageConfig
    {
        public int id;
        public string stageName;
        public int stageType;
        public int difficulty;
        public int chapter;
    }
}
