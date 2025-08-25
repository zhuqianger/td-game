using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Net.Mime;
using System.Threading.Tasks;


namespace Network
{
    public class GameClientTest : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button connectButton;
        [SerializeField] private Button disconnectButton;
        [SerializeField] private Button sendTestButton;
        [SerializeField] private InputField messageInput;
        [SerializeField] private Text logText;
        [SerializeField] private Text connectionStatusText;
        [Header("Test Settings")]
        [SerializeField] private int testMessageId = 2001;
        [SerializeField] private string serverIp = "127.0.0.1";
        [SerializeField] private int serverPort = 8888;

        private void Start()
        {
            // 初始化按钮监听
            if (connectButton != null)
                connectButton.onClick.AddListener(OnConnectClick);
            if (disconnectButton != null)
                disconnectButton.onClick.AddListener(OnDisconnectClick);
            if (sendTestButton != null)
                sendTestButton.onClick.AddListener(OnSendTestClick);

            // 注册网络消息处理器
            GameClient.Instance.RegisterMessageHandler(testMessageId, OnTestResponse);
            
            // 更新UI状态
            UpdateConnectionStatus();
            AddLog("测试程序已启动");
        }

        private void OnDestroy()
        {
            // 取消注册消息处理器
            GameClient.Instance.UnregisterMessageHandler(testMessageId);
        }

        private async void OnConnectClick()
        {
            try
            {
                await GameClient.Instance.ConnectToServer("1","123456",serverIp, serverPort);
                UpdateConnectionStatus();
                AddLog($"正在连接服务器 {serverIp}:{serverPort}...");
            }
            catch (Exception e)
            {
                AddLog($"连接失败: {e.Message}");
            }
        }

        private void OnDisconnectClick()
        {
            try
            {
                GameClient.Instance.Disconnect();
                UpdateConnectionStatus();
                AddLog("已断开连接");
            }
            catch (Exception e)
            {
                AddLog($"断开连接失败: {e.Message}");
            }
        }

        private void OnSendTestClick()
        {
            if (!GameClient.Instance.IsConnected)
            {
                AddLog("错误：未连接到服务器");
                return;
            }

            try
            {
                string message = messageInput != null ? messageInput.text : "测试消息";
                if (string.IsNullOrEmpty(message))
                {
                    message = "测试消息";
                }

                // 发送测试消息
                GameClient.Instance.SendStringMessage(testMessageId, message);
                AddLog($"发送消息: {message}");
            }
            catch (Exception e)
            {
                AddLog($"发送消息失败: {e.Message}");
            }
        }

        private void OnTestResponse(byte[] payload)
        {
            try
            {
                string response = System.Text.Encoding.UTF8.GetString(payload);
                AddLog($"收到服务器响应: {response}");
            }
            catch (Exception e)
            {
                AddLog($"处理响应失败: {e.Message}");
            }
        }

        private void UpdateConnectionStatus()
        {
            if (connectionStatusText != null)
            {
                connectionStatusText.text = GameClient.Instance.IsConnected ? "已连接" : "未连接";
                connectionStatusText.color = GameClient.Instance.IsConnected ? Color.green : Color.red;
            }
        }

        private void AddLog(string message)
        {
            if (logText != null)
            {
                logText.text = $"[{DateTime.Now:HH:mm:ss}] {message}\n{logText.text}";
            }
            Debug.Log($"[NetworkTest] {message}");
        }

        // 用于在Inspector中测试的方法
        public void TestConnection()
        {
            OnConnectClick();
        }

        public void TestDisconnection()
        {
            OnDisconnectClick();
        }

        public void TestSendMessage()
        {
            OnSendTestClick();
        }

        // 用于测试特定消息ID的方法
        public void SendCustomMessage(int messageId, string message)
        {
            if (!GameClient.Instance.IsConnected)
            {
                AddLog("错误：未连接到服务器");
                return;
            }

            try
            {
                GameClient.Instance.SendStringMessage(messageId, message);
                AddLog($"发送自定义消息: ID={messageId}, Content={message}");
            }
            catch (Exception e)
            {
                AddLog($"发送自定义消息失败: {e.Message}");
            }
        }
    }
}