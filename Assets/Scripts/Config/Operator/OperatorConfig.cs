using System;

namespace Config.Operator
{
    [Serializable]
    public class OperatorConfig
    {
        public int id;
        public string operatorName;
        public int professionId;
        public int rarity;
        public int[] skillIds;
        public int[] talentIds;
    }
}