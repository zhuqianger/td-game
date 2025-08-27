using System;

namespace Config.Operator
{
    [Serializable]
    public class OperatorGrowthConfig
    {
        public int id;
        public int operatorId;
        public int eliteLevel;
        public int hpGrowthPerLevel;
        public int attackGrowthPerLevel;
        public int defenseGrowthPerLevel;
        public int magicResistanceGrowthPerLevel;
    }
}
