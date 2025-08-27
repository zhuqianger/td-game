using System;

namespace Config.Backpack
{
    [Serializable]
    public class BackpackTypeConfig
    {
        public int id;
        public string name;
        public string description;
        public int sortOrder;
    }
}
