using PZIOT.Common;
using PZIOT.IServices;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using PZIOT.Common.EquipmentDriver;
using PZIOT.Model.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.Linq.Expressions;

namespace PZIOT.Tasks
{
    /// <summary>
    ///  定时采集任务，重复数据不进行采集存储,需进行优化，目前所有的采集都和数据库挂钩，资源占用会较大
    /// </summary>
    public class PZIOTDataGatherServices : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IEquipmentServices _equipmentServices;//查询设备信息
        private readonly IEquipmentMatesServices _equipmentMatesServices;//查询设备数据项信息
        private readonly IEquipmentDataScadaServices _equipmentDataScadaServices;//设备采集数据
        // 这里可以注入
        public PZIOTDataGatherServices(IEquipmentServices equipmentServices,IEquipmentMatesServices equipmentMatesServices,IEquipmentDataScadaServices equipmentDataScadaServices)
        {
            _equipmentServices = equipmentServices;
            _equipmentMatesServices= equipmentMatesServices;
            _equipmentDataScadaServices = equipmentDataScadaServices;
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
                        {
                            var eqpinfo = await _equipmentServices.QueryById(item.Key);
                            var matesinfo = await _equipmentMatesServices.Query(it => it.EquipmentId == item.Key);
                            foreach (var item2 in matesinfo)
                            {
                                try
                                {
                                    if (item2.IsActivation) {

                                        //先获取采集的Mate项，绑定设备Id
                                        EquipmentReadResponseProtocol backinfo = await item.Value.RequestSingleParaFromEquipment(item2.DataAddress);
                                        ConsoleHelper.WriteWarningLine($"设备id为{item.Key}的驱动采集数据地址{item2.DataAddress}得到回复，结果为{backinfo.ResponseValue}!");
                                        //转换成对应的设备信息进行记录,查询设备信息
                                        item2.Value = backinfo.ResponseValue;
                                        //数据库值更新
                                        await _equipmentMatesServices.Update(item2);
                                        //建立数据采集存储对象
                                        EquipmentDataScada data = new EquipmentDataScada();
                                        data.EquipmentId = item.Key;
                                        data.EquipmentDataGatherTime = DateTime.Now;
                                        data.EquipmentDataItemValue = backinfo.ResponseValue;
                                        data.EquipmentDataItemName = item2.MateName;
                                        data.LastInterval = 0;
                                        await _equipmentDataScadaServices.Add(data);
                                    }
                                    
                                }
                                catch (Exception ex)
                                {

                                    ConsoleHelper.WriteWarningLine($"设备id为{item.Key}的采集异常!异常信息>{ex.Message}");
                                }
                                
                            }
                            
                            
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
