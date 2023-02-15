using PZIOT.Common;
using PZIOT.IServices;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PZIOT.Tasks
{
    public class Job1TimedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IEquipmentServices _equipmentServices;

        // 这里可以注入
        public Job1TimedService(IEquipmentServices equipmentServices)
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
                ConsoleHelper.WriteSuccessLine("执行定时任务Dowork");
                var model = _equipmentServices.GetEquipmentDetails(1).Result;
                Console.WriteLine($"Job 1 启动成功，获取id=1的设备为:{model?.UniqueCode}");
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
