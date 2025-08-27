using System;

namespace Config.Operator
{
    [Serializable]
    public class TalentConfig
    {
        public int id;
        public string talentName;
        public string talentType;
        public string description;
        public object[] effects;
        public bool isAlwaysActive;
        public string activationCondition;
        public float activationChance;
        public int requiredEliteLevel;
        public int requiredLevel;
    }
}
