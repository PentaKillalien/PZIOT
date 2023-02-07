// See https://aka.ms/new-console-template for more information
//创建Mqtt服务器
//创建规则引擎
//权限Jwt验证，通过WebApi,设备连接Redis创建Session信息
//数据交互需带密钥，不然不接收存储
using MQTTnet;
using MQTTnet.Server;

Console.WriteLine("Hello, World!");
var options = new MqttServerOptions
{
};
var mqttServer = new MqttFactory().CreateMqttServer(options);
await mqttServer.StartAsync();
Console.ReadLine();
