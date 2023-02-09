using Microsoft.Extensions.DependencyInjection;
using MQTTnet.Server;
using PZIOT.Common;
using PZIOT.Extensions.Mqtt;
using System;

namespace PZIOT.Extensions.ServiceExtensions
{
    /// <summary>
    /// mqtt
    /// </summary>
    public static class MqttSetup
    {
        public static void AddMqttSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddHostedService<MqttHostService>();
        }
    }
}
