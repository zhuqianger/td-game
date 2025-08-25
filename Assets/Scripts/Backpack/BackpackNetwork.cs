namespace Backpack
{
    using UnityEngine;
    using System;
    using Network;
    using Util;

    public static class BackpackNetwork
    {
        private static bool _isInitialized = false;

        // 初始化方法
        public static void Initialize()
        {
            if (!_isInitialized)
            {
                // 初始化逻辑
                RegisterMessageHandlers();
                Debug.Log("BackpackNetwork initialized.");
                _isInitialized = true;
            }
        }

        // 注册消息处理器
        private static void RegisterMessageHandlers()
        {
            // 为每个消息ID注册处理器
            NetworkManager.RegisterMessageHandler((int)MessageId.RESP_GET_BACKPACK, HandleGetBackpackItems);
            NetworkManager.RegisterMessageHandler((int)MessageId.RESP_ADD_ITEM, HandleAddItem);
            NetworkManager.RegisterMessageHandler((int)MessageId.RESP_USE_ITEM, HandleUseItem);
            NetworkManager.RegisterMessageHandler((int)MessageId.RESP_GET_BACKPACK_BY_TYPE, HandleGetBackpackByType);
        }

        // 处理消息的单独回调方法
        private static void HandleGetBackpackItems(byte[] data)
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log($"Received backpack items: {json}");
            // 处理获取背包物品的逻辑
        }

        private static void HandleAddItem(byte[] data)
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log($"Received add item response: {json}");
        }

        private static void HandleUseItem(byte[] data)
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log($"Received use item response: {json}");
            // 处理使用物品的逻辑
        }

        private static void HandleGetBackpackByType(byte[] data)
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log($"Received backpack items by type: {json}");
            // 处理按类型获取背包物品的逻辑
        }

        // 以下是与背包相关的请求方法
        public static void RequestGetBackpackItems(int playerId)
        {
            Debug.Log($"Sending request to get backpack items for Player ID: {playerId}");
            NetworkManager.SendJsonMessage((int)MessageId.REQ_GET_BACKPACK, new { PlayerId = playerId });
        }

        public static void RequestAddItem(int playerId, int itemId, int quantity)
        {
            Debug.Log($"Sending request to add item {itemId} with quantity {quantity} to player {playerId}");
            NetworkManager.SendJsonMessage((int)MessageId.REQ_ADD_ITEM, new { PlayerId = playerId, ItemId = itemId, Quantity = quantity });
        }

        public static void RequestUseItem(int playerId, int itemId, int quantity)
        {
            Debug.Log($"Sending request to use item {itemId} with quantity {quantity} for player {playerId}");
            NetworkManager.SendJsonMessage((int)MessageId.REQ_USE_ITEM, new { PlayerId = playerId, ItemId = itemId, Quantity = quantity });
        }

        public static void RequestGetBackpackByType(int playerId, string type)
        {
            Debug.Log($"Sending request to get backpack items of type {type} for Player ID: {playerId}");
            NetworkManager.SendJsonMessage((int)MessageId.REQ_GET_BACKPACK_BY_TYPE, new { PlayerId = playerId, Type = type });
        }
    }
}