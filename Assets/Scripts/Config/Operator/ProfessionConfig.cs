using System;

namespace Config.Operator
{
    [Serializable]
    public class ProfessionConfig
    {
        public int id;
        public string name;
        public string description;
        public float attackInterval;
        public int redeployTime;
        public int sortOrder;
    }
}
