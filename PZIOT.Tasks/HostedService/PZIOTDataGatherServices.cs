using PZIOT.Common;
using PZIOT.IServices;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using PZIOT.Common.EquipmentDriver;

namespace PZIOT.Tasks
{
    /// <summary>
    ///  定时采集任务，重复数据不进行采集存储
    /// </summary>
    public class PZIOTDataGatherServices : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IEquipmentServices _equipmentServices;//查询设备信息

        // 这里可以注入
        public PZIOTDataGatherServices(IEquipmentServices equipmentServices)
        {
            _equipmentServices = equipmentServices;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Job 1 is starting.");
            //_timer = new Timer(DoWork, null, TimeSpan.Zero,
            //    TimeSpan.FromSeconds(60 * 60));//一个小时
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(10));//10秒
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            try
            {
                if (PZIOTEquipmentManager.EquipmentDriverDic.Count == 0) {
                    ConsoleHelper.WriteSuccessLine("PZIOTDataGatherServices执行定时采集任务Dowork成功，但无驱动数据!");
                    return;
                }
                //item.key则是设备id
                foreach (var item in PZIOTEquipmentManager.EquipmentDriverDic)
                {
                    //驱动处于连接状态则进行采集
                    if (item.Value.IsConnected)
                    {
                        //采集所有的驱动并进行数据存储，目前的模式只能设置整体的采集频率 不支持单个
                        Task.Run(async () =>
                        {   //先获取采集的Mate项，绑定设备Id
                            EquipmentReadResponseProtocol backinfo = await item.Value.RequestSingleParaFromEquipment("这个是需要获取的参数");
                            //转换成对应的设备信息进行记录
                        });
                    }
                    else {
                        ConsoleHelper.WriteWarningLine($"设备id为{item.Key}的驱动未连接，无法进行采集!");
                    }
                    

                }
                ConsoleHelper.WriteSuccessLine("PZIOTDataGatherServices执行定时采集任务Dowork成功!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{ex.Message}");
            }

            ConsoleHelper.WriteSuccessLine($"Job 1： {DateTime.Now}");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Job 1 is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
