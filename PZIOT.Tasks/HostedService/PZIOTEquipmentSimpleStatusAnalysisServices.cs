using PZIOT.Common;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using PZIOT.IServices;

namespace PZIOT.Tasks
{
    /// <summary>
    /// 状态分析服务，进行统一的状态分析任务，数据插入状态数据库，定时查询采集表的数据进行分析，只能根据规则分析待机和运行和停机和异常时间
    /// 频率可控制在一小时执行一次
    /// 最开始可以糙一点，根据数据变化来进行分析
    /// 如果设备配置了状态字段，后期单独写服务进行分析
    /// </summary>
    public class PZIOTEquipmentSimpleStatusAnalysisServices : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IEquipmentServices _equipmentServices;
        private readonly IEquipmentStatusServices _equipmentStatusServices;
        private readonly IEquipmentDataScadaServices _equipmentDataScadaServices;
        private readonly IEquipmentMatesServices _equipmentMatesServices;
        private int AnalysisInterval = 120;//120秒
        // 这里可以注入
        public PZIOTEquipmentSimpleStatusAnalysisServices(IEquipmentMatesServices equipmentMatesServices,IEquipmentServices equipmentServices,IEquipmentStatusServices equipmentStatusServices, IEquipmentDataScadaServices equipmentDataScadaServices)
        {
            _equipmentServices = equipmentServices;
            _equipmentStatusServices= equipmentStatusServices;
            _equipmentDataScadaServices = equipmentDataScadaServices;
            _equipmentMatesServices = equipmentMatesServices;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("PZIOTEquipmentSimpleStatusAnalysisServices Job  is starting.");
            AnalysisInterval = Convert.ToInt16(AppSettings.app(new string[] { "ServiceConfig", "PZIOTEquipmentSimpleStatusAnalysisServicesInterval" }));
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(AnalysisInterval));//两个小时

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            try
            {
                ConsoleHelper.WriteWarningLine($"状态简单分析服务执行： {DateTime.Now}");
                DateTime endTime = DateTime.Now;//开始时间
                DateTime startTime = endTime.AddSeconds(-(AnalysisInterval));//结束时间
                Task.Run(async () => {
                    //获取到所有的设备
                    var eqps = await _equipmentServices.Query(t => t.IsAutoAnalysisStatus);
                    foreach (var item in eqps)
                    {
                        //单个设备的分析逻辑
                        //查询当前时间节点和设置的时间节点后的一段时间的数据
                        var datas = await _equipmentDataScadaServices.Query(t => t.EquipmentId == item.Id && t.EquipmentDataGatherTime > startTime && t.EquipmentDataGatherTime < endTime);
                        ConsoleHelper.WriteSuccessLine($"PZIOTEquipmentSimpleStatusAnalysisServices分析设备编号{item.Id}中，获取到最近{AnalysisInterval}秒的数据，总条数为{datas.Count}");
                        var mates = await _equipmentMatesServices.Query(t => t.EquipmentId == item.Id);
                        if (datas.Count > 0)
                        {
                            int samecount = 0;//待机
                            int notsamecount = 0;//正常
                            int nodatasorempty = 0;//异常
                                                   //如果有多个数据项咋办，岂不是一直运行
                            foreach (var mate in mates)
                            {
                                var scadadatas = datas.FindAll(t => t.EquipmentDataItemName.Equals(mate.MateName) && !string.IsNullOrEmpty(t.EquipmentDataItemValue));
                                //排序
                                //scadadatas.Sort((a,b)=>a.EquipmentDataItemValue.CompareTo(b.EquipmentDataItemValue));
                                int changeCount = 0;
                                for (int i = 1; i < scadadatas.Count; i++)
                                {
                                    if (scadadatas[i - 1].EquipmentDataItemValue != scadadatas[i].EquipmentDataItemValue)
                                    {
                                        changeCount++;
                                    }
                                }

                                Console.WriteLine($"相邻元素属性值变化了 {changeCount} 次");
                                notsamecount += changeCount;
                                samecount += (scadadatas.Count - notsamecount);

                            }
                            nodatasorempty = datas.Count - notsamecount - samecount;//异常
                            ConsoleHelper.WriteErrorLine($"设备{item.Id}的数据条数分析结果，待机{samecount},运行{notsamecount},异常{nodatasorempty}");
                            //计算时长占比，会有偏差 ，比较简易版
                            int standbytime = (int)((double)samecount / datas.Count * AnalysisInterval);
                            int runbytime = (int)((double)notsamecount / datas.Count * AnalysisInterval);
                            int ex = AnalysisInterval - standbytime - runbytime;
                            //存储
                            await _equipmentStatusServices.Add(new Model.Models.EquipmentStatus()
                            {
                                EquipmentId = item.Id,
                                ChildStatus = Model.Models.PZIOTChildEquipmentStatus.None,
                                Status = Model.Models.PZIOTEquipmnetStatus.Standby,
                                Desc = "简单状态分析服务分析设备待机,计算得出待机时间",
                                StatusEndTime = endTime,
                                StatusStartTime = startTime,
                                StatusKeepLength = standbytime
                            });
                            await _equipmentStatusServices.Add(new Model.Models.EquipmentStatus()
                            {
                                EquipmentId = item.Id,
                                ChildStatus = Model.Models.PZIOTChildEquipmentStatus.None,
                                Status = Model.Models.PZIOTEquipmnetStatus.Normal,
                                Desc = "简单状态分析服务分析设备运行,计算得出运行时间",
                                StatusEndTime = endTime,
                                StatusStartTime = startTime,
                                StatusKeepLength = runbytime
                            });
                            await _equipmentStatusServices.Add(new Model.Models.EquipmentStatus()
                            {
                                EquipmentId = item.Id,
                                ChildStatus = Model.Models.PZIOTChildEquipmentStatus.None,
                                Status = Model.Models.PZIOTEquipmnetStatus.Fault,
                                Desc = "简单状态分析服务分析设备异常行为，计算得出异常时间",
                                StatusEndTime = endTime,
                                StatusStartTime = startTime,
                                StatusKeepLength = ex
                            });
                        }
                        else
                        {
                            //直接判断为停机，都没有数据
                            var result = await _equipmentStatusServices.Add(new Model.Models.EquipmentStatus()
                            {
                                EquipmentId = item.Id,
                                ChildStatus = Model.Models.PZIOTChildEquipmentStatus.None,
                                Status = Model.Models.PZIOTEquipmnetStatus.Shutdown,
                                Desc = "简单状态分析服务检测到设备停机,原因为没有任何数据采集活动发生，可能是未开启数据采集配置",
                                StatusEndTime = endTime,
                                StatusStartTime = endTime,
                                StatusKeepLength = AnalysisInterval
                            });
                        }


                    }

                });
            }
            catch (Exception ex)
            {

                ConsoleHelper.WriteErrorLine(ex.ToString());
            }
            
           

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("PZIOTEquipmentSimpleStatusAnalysisServices Job  is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
