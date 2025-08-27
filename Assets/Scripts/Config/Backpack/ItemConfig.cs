using System;

namespace Config.Backpack
{
    [Serializable]
    public class ItemConfig
    {
        public int id;
        public string name;
        public int quality;
        public int backpackTypeId;
        public string description;
    }
}
