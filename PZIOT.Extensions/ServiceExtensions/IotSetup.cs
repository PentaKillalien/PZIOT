using Microsoft.Extensions.DependencyInjection;
using PZIOT.Extensions.IOT;
using PZIOT.Extensions.Mqtt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Extensions.ServiceExtensions
{
    /// <summary>
    /// iot
    /// </summary>
    public static class IotSetup
    {
        public static void AddIotSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddHostedService<IotService>();
        }
    }
}
