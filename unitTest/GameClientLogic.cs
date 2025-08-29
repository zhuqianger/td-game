using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace tdTest
{
    [Serializable]
    public class LoginData
    {
        public string username { get; set; }
        public string password { get; set; }
        
        public LoginData(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }

    public class GameClientLogic
    {
        #region 单例模式
        private static GameClientLogic _instance;
        public static GameClientLogic Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameClientLogic();
                }
                return _instance;
            }
        }

        private GameClientLogic() { }
        #endregion

        // 真实的网络连接
        private TcpClient client;
        private NetworkStream stream;
        private string _serverIp = "127.0.0.1";
        private int _serverPort = 8888;

        // 消息监听字典
        private Dictionary<int, Action<byte[]>> messageHandlers = new Dictionary<int, Action<byte[]>>();

        // 连接状态属性
        public bool IsConnected => client?.Connected ?? false;

        // 真实连接方法
        public async Task<bool> ConnectToServer(string username, string password, string ip = null, int port = 0)
        {
            // 如果已经连接，先断开
            if (IsConnected)
            {
                Disconnect();
            }

            // 更新服务器配置
            if (!string.IsNullOrEmpty(ip))
                _serverIp = ip;
            if (port > 0)
                _serverPort = port;

            try
            {
                client = new TcpClient();
                await client.ConnectAsync(_serverIp, _serverPort);
                stream = client.GetStream();
            
                Console.WriteLine($"成功连接到游戏服务器 {_serverIp}:{_serverPort}");
                
                // 启动接收消息的循环
                _ = ReceiveMessages();
                
                // 发送登录信息
                var loginData = new LoginData(username, password);
                
                // 使用消息ID 1 作为登录消息
                await SendJsonMessage(1, loginData);
                Console.WriteLine($"已发送登录信息: 用户名={username}");
                
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"连接服务器失败: {e.Message}");
                return false;
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
                Console.WriteLine("已断开与服务器的连接");
            }
            catch (Exception e)
            {
                Console.WriteLine($"断开连接时发生错误: {e.Message}");
            }
        }

        // 真实接收消息
        private async Task ReceiveMessages()
        {
            byte[] buffer = new byte[1024];
        
            while (client?.Connected == true)
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
                    Console.WriteLine($"接收消息时发生错误: {e.Message}");
                    break;
                }
            }
        }

        private void ProcessMessage(byte[] buffer, int length)
        {
            try
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
                Console.WriteLine($"收到消息ID: {messageId}, 内容: {json}");
                
                // 调用注册的消息处理器
                if (messageHandlers.ContainsKey(messageId))
                {
                    messageHandlers[messageId]?.Invoke(payload);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"处理消息时发生错误: {e.Message}");
            }
        }

        public async Task<bool> SendMessage(int messageId, byte[] payload)
        {
            if (!IsConnected) return false;

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
                Console.WriteLine($"发送消息ID: {messageId}, 长度: {payload.Length}");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"发送消息失败: {e.Message}");
                return false;
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
        public async Task<bool> SendStringMessage(int messageId, string content)
        {
            byte[] payload = Encoding.UTF8.GetBytes(content);
            return await SendMessage(messageId, payload);
        }

        /// <summary>
        /// 发送JSON消息
        /// </summary>
        public async Task<bool> SendJsonMessage(int messageId, object data)
        {
            string json = JsonSerializer.Serialize(data);
            byte[] payload = Encoding.UTF8.GetBytes(json);
            return await SendMessage(messageId, payload);
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

        // 获取当前服务器配置
        public (string ip, int port) GetServerConfig()
        {
            return (_serverIp, _serverPort);
        }

        // 重置连接状态（用于测试）
        public void ResetConnection()
        {
            Disconnect();
            messageHandlers.Clear();
        }

        // 模拟接收消息（用于测试，当不需要真实网络时）
        public void SimulateReceiveMessage(int messageId, string content)
        {
            if (!IsConnected) return;

            byte[] payload = Encoding.UTF8.GetBytes(content);
            
            // 处理消息
            Console.WriteLine($"模拟收到消息ID: {messageId}, 内容: {content}");
            
            // 调用注册的消息处理器
            if (messageHandlers.ContainsKey(messageId))
            {
                messageHandlers[messageId]?.Invoke(payload);
            }
        }
    }
}
