using System;

namespace Config.Operator
{
    [Serializable]
    public class OperatorBaseAttributeConfig
    {
        public int id;
        public int operatorId;
        public int eliteLevel;
        public int baseHP;
        public int baseAttack;
        public int baseDefense;
        public int baseMagicResistance;
        public int[][] attackRange;
        public int deployCost;
        public int blockCount;
    }
}
