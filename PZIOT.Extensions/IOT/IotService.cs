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
    /// <summary>
    /// 此服务支持的是私有部署，不上云，走现场网络的改造，上云走mqtt,设置权限等，Iot不启动，后面加入配置
    /// 上云用不到驱动描述表，这是前提
    /// </summary>
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
            //需要定义个容器挂载所有的驱动连接，配置数据项后统一调用接口读取相关数据进行采集活动
            ConsoleHelper.WriteSuccessLine("StartedAsync: IOT服务已启动.");
            Log.Info("StartedAsync: IOT服务已启动.");
            //开始加载设备驱动,从数据库读取，读取时根据设备编号建立映射
            List<EquipmentDriverDesc> tempAllDrivers = await _euqipmentDriverDescServices.Query();
            if (tempAllDrivers.Count == 0)
                ConsoleHelper.WriteWarningLine("数据库无任何驱动配置信息");
            else
                await new EquipmentDriverDescOper().CreatAllDriverConnection(tempAllDrivers);
            //开始加载设备数据项读取相关的内容，设备数据项的表怎么建立，然后建立定时任务，读取设备信息
            
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.StopAsync(cancellationToken);
            return Task.CompletedTask;
        }
    }
}
