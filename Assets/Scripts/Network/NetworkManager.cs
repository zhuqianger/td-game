using System;
using System.Collections.Generic;
using System.Threading.Tasks; // Added for Task

namespace Network
{
    public static class NetworkManager
    {
        private static bool isConnected = false;
        private static GameClient gameClient = GameClient.Instance;
        
        // 消息处理字典
        private static Dictionary<int, Action<byte[]>> messageHandlers = new Dictionary<int, Action<byte[]>>();
        
        // 初始化网络连接
        public static void Initialize()
        {
            // 初始化逻辑
        }

        // 断开连接
        public static void Disconnect()
        {
            gameClient.Disconnect();
            isConnected = false;
        }
        
        // 发送数据
        public static void SendData(int messageId, byte[] data)
        {
            if (!isConnected) return;
            gameClient.SendMessage(messageId, data);
        }
        
        // 发送字符串消息
        public static void SendStringMessage(int messageId, string content)
        {
            if (!isConnected) return;
            gameClient.SendStringMessage(messageId, content);
        }
        
        // 发送JSON消息
        public static void SendJsonMessage(int messageId, object data)
        {
            if (!isConnected) return;
            gameClient.SendJsonMessage(messageId, data);
        }
        
        // 获取连接状态
        public static bool IsConnected()
        {
            return isConnected;
        }
        
        // 注册消息处理器
        public static void RegisterMessageHandler(int messageId, Action<byte[]> handler)
        {
            if (handler != null)
            {
                messageHandlers[messageId] = handler;
            }
        }
        
        // 取消注册消息处理器
        public static void UnregisterMessageHandler(int messageId)
        {
            if (messageHandlers.ContainsKey(messageId))
            {
                messageHandlers.Remove(messageId);
            }
        }
        
        // 处理消息
        public static void HandleMessage(int messageId, byte[] payload)
        {
            if (messageHandlers.TryGetValue(messageId, out Action<byte[]> handler))
            {
                handler(payload);
            }
            else
            {
                UnityEngine.Debug.LogWarning($"未找到消息ID {messageId} 的处理器");
            }
        }
        
        // 连接到服务器
        public static async Task<bool> Connect(string serverAddress, int port, string username, string password)
        {
            try
            {
                await gameClient.ConnectToServer(username, password, serverAddress, port);
                isConnected = gameClient.IsConnected;
                return isConnected;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"连接服务器失败: {e.Message}");
                isConnected = false;
                return false;
            }
        }
    }
}