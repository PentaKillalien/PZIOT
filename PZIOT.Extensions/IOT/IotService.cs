using log4net;
using Microsoft.Extensions.Hosting;
using PZIOT.Common;
using PZIOT.Common.EquipmentDriver;
using PZIOT.IServices;
using PZIOT.Model.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PZIOT.Extensions.IOT
{
    public class IotService : IHostedService, IDisposable
    {
        private IEquipmentDriverDescServices _euqipmentDriverDescServices;
        private static readonly ILog Log = LogManager.GetLogger(typeof(IotService));
        public void Dispose()
        {
            this.Dispose();
        }
        public IotService(IEquipmentDriverDescServices services)
        {
            Console.WriteLine("-----Iot服务启动前");
            _euqipmentDriverDescServices = services;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            ConsoleHelper.WriteSuccessLine("StartedAsync: IOT服务已启动.");
            Log.Info("StartedAsync: IOT服务已启动.");
            List<EquipmentDriverDesc> tempAllDrivers = await _euqipmentDriverDescServices.Query();
            if (tempAllDrivers.Count == 0)
                ConsoleHelper.WriteWarningLine("数据库无任何驱动配置信息");
            else
                await new EquipmentDriverDescOper().CreatAllDriverConnection(tempAllDrivers);
            //开始加载设备驱动,从数据库读取，读取时根据设备编号建立映射
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.StopAsync(cancellationToken);
            return Task.CompletedTask;
        }
    }
}
