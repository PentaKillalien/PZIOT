using Microsoft.Extensions.Hosting;
using PZIOT.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PZIOT.Extensions.IOT
{
    public class IotService : IHostedService, IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            ConsoleHelper.WriteSuccessLine("StartAsync: IOT服务已启动......");
            //开始加载设备内容
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
