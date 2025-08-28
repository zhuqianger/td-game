# 测试环境配置说明

## 问题说明

之前的测试代码只是模拟了网络连接，没有真正与后端服务器通信。现在修复后的代码会尝试建立真实的TCP连接。

## 测试场景

### 1. 后端服务器运行中
- 测试会成功连接到服务器
- 消息会真正发送到后端
- 可以测试完整的网络通信流程

### 2. 后端服务器未运行
- 连接测试会失败（这是正确的行为）
- 测试会跳过需要连接的测试用例
- 仍然可以测试基础功能和错误处理

## 运行测试前的准备

### 选项1：启动后端服务器
```bash
# 确保你的Java后端服务器在运行
# 默认端口：8888
# 默认IP：127.0.0.1 或 1.117.63.144
```

### 选项2：使用模拟模式（仅测试逻辑）
如果你只想测试逻辑而不需要真实网络连接，可以修改 `GameClientLogic.cs`：

```csharp
// 在 ConnectToServer 方法中添加模拟模式
public async Task<bool> ConnectToServer(string username, string password, string ip = null, int port = 0, bool simulationMode = false)
{
    if (simulationMode)
    {
        // 模拟模式代码
        await Task.Delay(100);
        _isConnected = true;
        return true;
    }
    
    // 真实连接代码
    // ... 现有代码
}
```

## 测试结果解释

### 连接成功的测试
- `TestConnection` - 基础连接测试
- `TestConnectionToLocalhost` - 本地连接测试
- `TestSendMessage` - 消息发送测试
- `TestJsonMessage` - JSON消息测试

### 连接失败的测试（正常情况）
- `TestInvalidServerConnection` - 无效服务器连接
- `TestNetworkTimeout` - 网络超时测试

### 始终成功的测试
- `TestGameClientSingleton` - 单例模式测试
- `TestInitialConnectionState` - 初始状态测试
- `TestServerConfig` - 服务器配置测试
- `TestLogs` - 日志功能测试

## 常见问题

### Q: 测试显示"连接失败"，这是正常的吗？
A: 是的，如果后端服务器没有运行，连接失败是完全正常的。测试会跳过需要连接的测试用例。

### Q: 如何让所有测试都通过？
A: 启动你的Java后端服务器，确保它在8888端口监听。

### Q: 可以只测试逻辑而不测试网络吗？
A: 可以，使用 `SimulateReceiveMessage` 方法来模拟消息接收，测试消息处理逻辑。

## 网络协议说明

测试使用与Unity版本相同的网络协议：
- 消息格式：消息ID(4字节) + 消息长度(4字节) + 消息内容
- 字节序：大端序（与Java兼容）
- 编码：UTF-8
- 消息ID 1：登录消息
