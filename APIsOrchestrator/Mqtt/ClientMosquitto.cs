using Mqtt;
using MQTTnet;
using MQTTnet.Client;
using Microsoft.Extensions.Configuration;
using Mqtt.Client.AspNetCore.Settings;
using Serilog;

namespace Mqtt;

public class ClientMosquitto
{
    private IMqttClient? mqttClient { get; }

    public IConfiguration? configuration { get; }

    public static async Task PublishMessageAsync(IConfiguration configuration,string messaggio)
    {
        var mqttFactory1 = new MqttFactory();

        IMqttClient mqttClient = mqttFactory1.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithClientId(Guid.NewGuid().ToString())
            .WithTcpServer(
                configuration.GetSection("BrokerHostSettings").Get<BrokerHostSettings>().Host,
                configuration.GetSection("BrokerHostSettings").Get<BrokerHostSettings>().Port)
            .WithCredentials(
                configuration.GetSection("ClientSettings").Get<ClientSettings>().UserName,
                configuration.GetSection("ClientSettings").Get<ClientSettings>().Password)
            .WithCleanSession()
            .Build();

        await mqttClient.ConnectAsync(options);
        if (mqttClient.IsConnected)
            Log.Information("MQTT Connected");
        

        var message = new MqttApplicationMessageBuilder()
            .WithTopic("test-topic-in")
            .WithPayload(messaggio)
            .Build();

        if (mqttClient.IsConnected)
        {
            Log.Information("publish messaggio: "+ messaggio);
            await mqttClient.PublishAsync(message);
            await mqttClient.DisconnectAsync();
        }
    }
}

