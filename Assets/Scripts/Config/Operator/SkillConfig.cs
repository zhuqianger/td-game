using System;

namespace Config.Operator
{
    [Serializable]
    public class SkillConfig
    {
        public int id;
        public string skillName;
        public string skillType;
        public string description;
        public int spCost;
        public int duration;
        public float cooldown;
        public float triggerChance;
        public int spRecovery;
        public bool autoTrigger;
        public string triggerCondition;
        public object[] effects;
    }
}
