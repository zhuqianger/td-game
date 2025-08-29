using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace tdTest
{
    public class GameClientTestLogic
    {
        // 测试设置
        private int testMessageId = 1;
        private string serverIp = "127.0.0.1";
        private int serverPort = 8888;
        
        // 测试日志
        private List<string> testLogs = new List<string>();
        
        // 测试结果
        public bool LastConnectionResult { get; private set; }
        public string LastReceivedMessage { get; private set; }
        public int MessageCount { get; private set; }

        public GameClientTestLogic()
        {
            // 注册网络消息处理器
            GameClientLogic.Instance.RegisterMessageHandler(testMessageId, OnTestResponse);
            
            AddLog("测试程序已启动");
        }

        public void Dispose()
        {
            // 取消注册消息处理器
            GameClientLogic.Instance.UnregisterMessageHandler(testMessageId);
        }

        public async Task<bool> TestConnection(string username = "1", string password = "123456")
        {
            try
            {
                AddLog($"正在连接服务器 {serverIp}:{serverPort}...");
                
                bool result = await GameClientLogic.Instance.ConnectToServer(username, password, serverIp, serverPort);
                LastConnectionResult = result;
                
                if (result)
                {
                    AddLog("连接成功");
                }
                else
                {
                    AddLog("连接失败");
                }
                
                return result;
            }
            catch (Exception e)
            {
                AddLog($"连接失败: {e.Message}");
                LastConnectionResult = false;
                return false;
            }
        }

        public bool TestDisconnection()
        {
            try
            {
                GameClientLogic.Instance.Disconnect();
                AddLog("已断开连接");
                LastConnectionResult = false;
                return true;
            }
            catch (Exception e)
            {
                AddLog($"断开连接失败: {e.Message}");
                return false;
            }
        }

        public async Task<bool> TestSendMessage(string message = "测试消息", int? customMessageId = null)
        {
            if (!GameClientLogic.Instance.IsConnected)
            {
                AddLog("错误：未连接到服务器");
                return false;
            }

            try
            {
                int messageId = customMessageId ?? testMessageId;
                
                if (string.IsNullOrEmpty(message))
                {
                    message = "测试消息";
                }

                // 发送测试消息
                bool result = await GameClientLogic.Instance.SendStringMessage(messageId, message);
                
                if (result)
                {
                    AddLog($"发送消息成功: ID={messageId}, Content={message}");
                    MessageCount++;
                }
                else
                {
                    AddLog($"发送消息失败: ID={messageId}, Content={message}");
                }
                
                return result;
            }
            catch (Exception e)
            {
                AddLog($"发送消息失败: {e.Message}");
                return false;
            }
        }

        private void OnTestResponse(byte[] payload)
        {
            try
            {
                string response = System.Text.Encoding.UTF8.GetString(payload);
                LastReceivedMessage = response;
                AddLog($"收到服务器响应: {response}");
            }
            catch (Exception e)
            {
                AddLog($"处理响应失败: {e.Message}");
            }
        }

        // 模拟接收消息（用于测试）
        public void SimulateReceiveMessage(int messageId, string content)
        {
            GameClientLogic.Instance.SimulateReceiveMessage(messageId, content);
        }

        private void AddLog(string message)
        {
            string logEntry = $"[{DateTime.Now:HH:mm:ss}] {message}";
            testLogs.Add(logEntry);
            Console.WriteLine($"[NetworkTest] {message}");
        }

        // 获取所有测试日志
        public List<string> GetTestLogs()
        {
            return new List<string>(testLogs);
        }

        // 获取最新的日志
        public string GetLatestLog()
        {
            return testLogs.Count > 0 ? testLogs[testLogs.Count - 1] : string.Empty;
        }

        // 清空测试日志
        public void ClearLogs()
        {
            testLogs.Clear();
        }

        // 用于测试特定消息ID的方法
        public async Task<bool> SendCustomMessage(int messageId, string message)
        {
            if (!GameClientLogic.Instance.IsConnected)
            {
                AddLog("错误：未连接到服务器");
                return false;
            }

            try
            {
                bool result = await GameClientLogic.Instance.SendStringMessage(messageId, message);
                
                if (result)
                {
                    AddLog($"发送自定义消息成功: ID={messageId}, Content={message}");
                    MessageCount++;
                }
                else
                {
                    AddLog($"发送自定义消息失败: ID={messageId}, Content={message}");
                }
                
                return result;
            }
            catch (Exception e)
            {
                AddLog($"发送自定义消息失败: {e.Message}");
                return false;
            }
        }

        // 测试连接状态
        public bool TestConnectionStatus()
        {
            bool status = GameClientLogic.Instance.IsConnected;
            AddLog($"当前连接状态: {(status ? "已连接" : "未连接")}");
            return status;
        }

        // 测试服务器配置
        public (string ip, int port) TestServerConfig()
        {
            var config = GameClientLogic.Instance.GetServerConfig();
            AddLog($"当前服务器配置: {config.ip}:{config.port}");
            return config;
        }

        // 重置测试状态
        public void ResetTestState()
        {
            LastConnectionResult = false;
            LastReceivedMessage = string.Empty;
            MessageCount = 0;
            ClearLogs();
            GameClientLogic.Instance.ResetConnection();
            AddLog("测试状态已重置");
        }
    }
}
