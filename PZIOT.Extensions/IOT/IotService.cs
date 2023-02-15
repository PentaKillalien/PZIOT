using log4net;
using Microsoft.Extensions.Hosting;
using PZIOT.Common;
using PZIOT.IServices;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PZIOT.Extensions.IOT
{
    public class IotService : IHostedService, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(IotService));
        public void Dispose()
        {
            this.Dispose();
        }
        public IotService(IEquipmentServices services)
        {
            Console.WriteLine("-----Iot服务启动前");
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            ConsoleHelper.WriteSuccessLine("StartedAsync: IOT服务已启动.");
            Log.Info("StartedAsync: IOT服务已启动.");
            //开始加载设备驱动,从数据库读取，读取时根据设备编号建立映射
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.StopAsync(cancellationToken);
            return Task.CompletedTask;
        }
    }
}
