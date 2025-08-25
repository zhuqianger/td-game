namespace Backpack
{
    using System;
    using System.Collections.Generic;

    public static class BackpackModel
    {
        // 背包物品数据模型
        [Serializable]
        public class ItemData
        {
            public int id;
            public string name;
            public int quantity;
            public string type; // 物品类型，例如 "material", "equipment", "consumable"
            // 添加其他物品相关的字段
        }
        
        // 玩家背包数据
        private static List<ItemData> playerItems = new List<ItemData>();

        // 获取玩家背包物品列表
        public static List<ItemData> GetPlayerItems()
        {
            return playerItems;
        }

        // 更新玩家背包物品列表
        public static void UpdatePlayerItems(List<ItemData> items)
        {
            playerItems = items;
        }

        // 添加物品到背包
        public static void AddItem(ItemData item)
        {
            var existingItem = playerItems.Find(i => i.id == item.id);
            if (existingItem != null)
            {
                existingItem.quantity += item.quantity;
            }
            else
            {
                playerItems.Add(item);
            }
        }

        // 从背包移除物品
        public static bool RemoveItem(int itemId, int quantity)
        {
            var item = playerItems.Find(i => i.id == itemId);
            if (item != null)
            {
                item.quantity -= quantity;
                if (item.quantity <= 0)
                {
                    playerItems.Remove(item);
                }
                return true;
            }
            return false;
        }
    }
}