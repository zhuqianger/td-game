using System.Collections.Generic;
using UnityEngine;
using Config;
using Config.Backpack;
using Common;
using EventType = Common.EventType;

namespace Backpack
{
    public static class BackpackModel
    {
        // 玩家背包数据：外层键为背包类型ID，内层键为物品ID，值为物品对象
        private static Dictionary<int, Dictionary<int, Item>> itemMap = new Dictionary<int, Dictionary<int, Item>>();
        
        /// <summary>
        /// 初始化背包模型
        /// </summary>
        public static void Init()
        {
            itemMap = new Dictionary<int, Dictionary<int, Item>>();
            Debug.Log("BackpackModel initialized");
        }
        
        /// <summary>
        /// 接收物品列表数据（通常是从服务器获取的完整背包数据）
        /// </summary>
        /// <param name="itemList">物品列表</param>
        public static void OnItemListDataReceive(List<Item> itemList)
        {
            if (itemList == null || itemList.Count == 0)
            {
                Debug.LogWarning("Received empty item list");
                return;
            }
            
            // 清空现有数据
            itemMap.Clear();
            
            // 遍历物品列表，按背包类型分组存储
            foreach (var item in itemList)
            {
                if (item != null && item.itemId > 0)
                {
                    // 获取物品配置信息
                    ItemConfig itemConfig = ConfigManager.GetConfigById<ItemConfig>("items_config", item.itemId);
                    if (itemConfig != null)
                    {
                        int backpackTypeId = itemConfig.backpackTypeId;
                        
                        // 如果背包类型不存在，创建新的背包字典
                        if (!itemMap.ContainsKey(backpackTypeId))
                        {
                            itemMap[backpackTypeId] = new Dictionary<int, Item>();
                        }
                        
                        // 将物品添加到对应背包类型
                        itemMap[backpackTypeId][item.itemId] = item;
                    }
                    else
                    {
                        Debug.LogWarning($"Item config not found for item ID: {item.itemId}");
                    }
                }
            }
            
            Debug.Log($"Successfully stored {itemList.Count} items in {itemMap.Count} backpack types");
            EventManager.Send(EventType.OnBagItemDataReceive);
        }

        /// <summary>
        /// 更新物品列表数据（通常是部分物品的更新）
        /// </summary>
        /// <param name="itemList">更新的物品列表</param>
        public static void OnItemListDataUpdate(List<Item> itemList)
        {
            if (itemList == null || itemList.Count == 0)
            {
                Debug.LogWarning("Received empty item update list");
                return;
            }
            
            Debug.Log($"Updating {itemList.Count} items in backpack");
            
            // 遍历更新的物品列表
            foreach (var item in itemList)
            {
                if (item != null && item.itemId > 0)
                {
                    // 获取物品配置信息
                    ItemConfig itemConfig = ConfigManager.GetConfigById<ItemConfig>("items_config", item.itemId);
                    if (itemConfig != null)
                    {
                        int backpackTypeId = itemConfig.backpackTypeId;
                        
                        // 确保背包类型存在
                        if (!itemMap.ContainsKey(backpackTypeId))
                        {
                            itemMap[backpackTypeId] = new Dictionary<int, Item>();
                        }
                        
                        // 更新或添加物品数据
                        itemMap[backpackTypeId][item.itemId] = item;
                        
                        Debug.Log($"Updated item {item.itemId} in backpack type {backpackTypeId} (Quantity: {item.quantity})");
                    }
                    else
                    {
                        Debug.LogWarning($"Item config not found for item ID: {item.itemId}");
                    }
                }
            }
            EventManager.Send(EventType.OnBagItemDataUpdate);
        }
    }
}