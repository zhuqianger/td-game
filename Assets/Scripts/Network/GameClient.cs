using UnityEngine;
using System.Net.Sockets;
using System.Threading.Tasks;
using System;
using System.Text;
using System.Collections.Generic;

namespace Network
{
    [System.Serializable]
    public class LoginData
    {
        public string username;
        public string password;
        
        public LoginData(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }

    public class GameClient : MonoBehaviour
    {
        #region 单例模式
        private static GameClient _instance;
        public static GameClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameClient");
                    _instance = go.AddComponent<GameClient>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        #endregion

        private TcpClient client;
        private NetworkStream stream;
        private string serverIp = "1.117.63.144"; // 服务器IP
        private int serverPort = 8888;         // 服务器端口

        // 消息监听字典
        private Dictionary<int, Action<byte[]>> messageHandlers = new Dictionary<int, Action<byte[]>>();

        // 连接状态属性
        public bool IsConnected => client?.Connected ?? false;

        public async Task ConnectToServer(string username, string password, string ip = null, int port = 0)
        {
            // 如果已经连接，先断开
            if (IsConnected)
            {
                Disconnect();
            }

            // 更新服务器配置
            if (!string.IsNullOrEmpty(ip))
                serverIp = ip;
            if (port > 0)
                serverPort = port;

            try
            {
                client = new TcpClient();
                await client.ConnectAsync(serverIp, serverPort);
                stream = client.GetStream();
            
                Debug.Log($"成功连接到游戏服务器 {serverIp}:{serverPort}");
                
                // 启动接收消息的循环
                _ = ReceiveMessages();
                
                // 发送登录信息
                var loginData = new LoginData(username, password);
                
                // 使用消息ID 1 作为登录消息
                SendJsonMessage(1, loginData);
                Debug.Log($"已发送登录信息: 用户名={username}");
            }
            catch (Exception e)
            {
                Debug.LogError($"连接服务器失败: {e.Message}");
                throw; // 向上抛出异常，让调用者处理
            }
        }

        public void Disconnect()
        {
            try
            {
                stream?.Close();
                client?.Close();
                stream = null;
                client = null;
                Debug.Log("已断开与服务器的连接");
            }
            catch (Exception e)
            {
                Debug.LogError($"断开连接时发生错误: {e.Message}");
            }
        }

        private async Task ReceiveMessages()
        {
            byte[] buffer = new byte[1024];
        
            while (client.Connected)
            {
                try
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        // 处理接收到的消息
                        ProcessMessage(buffer, bytesRead);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"接收消息时发生错误: {e.Message}");
                    break;
                }
            }
        }

        private void ProcessMessage(byte[] buffer, int length)
        {
            // 解析消息ID（前4字节）- 注意Java是大端序，需要转换
            int messageId = BitConverter.ToInt32(ReverseBytes(buffer, 0, 4), 0);
        
            // 解析消息长度（接下来4字节）- 同样需要转换
            int messageLength = BitConverter.ToInt32(ReverseBytes(buffer, 4, 4), 0);
        
            // 获取消息内容
            byte[] payload = new byte[messageLength];
            Array.Copy(buffer, 8, payload, 0, messageLength);
        
            string json = Encoding.UTF8.GetString(payload);
            // 处理消息
            Debug.Log($"收到消息ID: {messageId}, 内容: {json}");
            
            // 将消息传递给 NetworkManager 处理
            NetworkManager.HandleMessage(messageId, payload);
        }

        public async Task SendMessage(int messageId, byte[] payload)
        {
            if (!client.Connected) return;

            try
            {
                // 构建消息：消息ID(4字节) + 消息长度(4字节) + 消息内容
                // 注意：Java使用大端序，需要转换
                byte[] messageIdBytes = ReverseBytes(BitConverter.GetBytes(messageId));
                byte[] lengthBytes = ReverseBytes(BitConverter.GetBytes(payload.Length));
            
                byte[] message = new byte[8 + payload.Length];
                Array.Copy(messageIdBytes, 0, message, 0, 4);
                Array.Copy(lengthBytes, 0, message, 4, 4);
                Array.Copy(payload, 0, message, 8, payload.Length);
            
                await stream.WriteAsync(message, 0, message.Length);
            }
            catch (Exception e)
            {
                Debug.LogError($"发送消息失败: {e.Message}");
            }
        }

        /// <summary>
        /// 注册消息处理器
        /// </summary>
        /// <param name="messageId">消息ID</param>
        /// <param name="handler">处理函数</param>
        public void RegisterMessageHandler(int messageId, Action<byte[]> handler)
        {
            if (handler != null)
            {
                messageHandlers[messageId] = handler;
            }
        }

        /// <summary>
        /// 取消注册消息处理器
        /// </summary>
        /// <param name="messageId">消息ID</param>
        public void UnregisterMessageHandler(int messageId)
        {
            if (messageHandlers.ContainsKey(messageId))
            {
                messageHandlers.Remove(messageId);
            }
        }

        /// <summary>
        /// 发送字符串消息
        /// </summary>
        public async void SendStringMessage(int messageId, string content)
        {
            byte[] payload = Encoding.UTF8.GetBytes(content);
            
            await SendMessage(messageId, payload);
        }

        /// <summary>
        /// 发送JSON消息
        /// </summary>
        public async void SendJsonMessage(int messageId, object data)
        {
            string json = JsonUtility.ToJson(data);
            byte[] payload = Encoding.UTF8.GetBytes(json);
            await SendMessage(messageId, payload);
        }

        /// <summary>
        /// 反转字节数组（用于端序转换）
        /// </summary>
        private byte[] ReverseBytes(byte[] bytes)
        {
            byte[] result = new byte[bytes.Length];
            Array.Copy(bytes, result, bytes.Length);
            Array.Reverse(result);
            return result;
        }

        /// <summary>
        /// 反转字节数组的一部分（用于端序转换）
        /// </summary>
        private byte[] ReverseBytes(byte[] bytes, int offset, int length)
        {
            byte[] result = new byte[length];
            Array.Copy(bytes, offset, result, 0, length);
            Array.Reverse(result);
            return result;
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void OnApplicationQuit()
        {
            Disconnect();
        }
    }
}