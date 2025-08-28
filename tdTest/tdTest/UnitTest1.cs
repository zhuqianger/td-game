using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace tdTest;

public class Tests
{
    private GameClientTestLogic testLogic;

    [SetUp]
    public void Setup()
    {
        testLogic = new GameClientTestLogic();
    }

    [TearDown]
    public void TearDown()
    {
        testLogic?.Dispose();
    }

    [Test]
    public async Task TestConnection()
    {
        // 测试连接功能
        bool result = await testLogic.TestConnection("1", "123456");
        
        Console.WriteLine($"连接测试结果: {result}");
        
        if (result)
        {
            Assert.That(testLogic.LastConnectionResult, Is.True);
            Assert.That(GameClientLogic.Instance.IsConnected, Is.True);
        }
        else
        {
            Console.WriteLine("连接失败 - 请确保后端服务器在运行");
        }
    }

    [Test]
    public async Task TestSendMessage()
    {
        // 先连接
        bool connected = await testLogic.TestConnection("1", "123456");
        
        if (connected)
        {
            // 测试发送消息
            bool result = await testLogic.TestSendMessage("Hello World");
            
            Assert.That(result, Is.True);
            Assert.That(testLogic.MessageCount, Is.EqualTo(1));
        }
        else
        {
            Console.WriteLine("跳过消息发送测试，因为连接失败");
        }
    }
}