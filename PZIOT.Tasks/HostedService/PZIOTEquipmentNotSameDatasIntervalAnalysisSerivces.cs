using PZIOT.Common;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using PZIOT.IServices;
using System.Collections.Generic;
using PZIOT.Model.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace PZIOT.Tasks
{
    /// <summary>
    /// 不同数数据时间间隔计算服务
    /// 如果数据为空则移除，不写入异常数据
    /// </summary>
    public class PZIOTEquipmentNotSameDatasIntervalAnalysisSerivces : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IEquipmentServices _equipmentServices;
        private readonly IEquipmentDataScadaServices _equipmentDataScadaServices;
        private readonly IEquipmentDataScadaIntervalServices _equipmentDataScadaIntervalServices;
        private readonly IEquipmentMatesServices _equipmentMatesServices;
        private int AnalysisInterval = 30;//120秒
        private Dictionary<int, Dictionary<string,DateTime>> keyValuePairs = new Dictionary<int, Dictionary<string, DateTime>>();
        private Dictionary<int, Dictionary<string, string>> mateskeyValuePairs = new Dictionary<int, Dictionary<string, string>>();
        // 这里可以注入
        public PZIOTEquipmentNotSameDatasIntervalAnalysisSerivces(IEquipmentMatesServices equipmentMatesServices, IEquipmentServices equipmentServices, IEquipmentDataScadaServices equipmentDataScadaServices,IEquipmentDataScadaIntervalServices equipmentDataScadaIntervalServices)
        {
            _equipmentServices = equipmentServices;
            _equipmentDataScadaServices = equipmentDataScadaServices;
            _equipmentDataScadaIntervalServices= equipmentDataScadaIntervalServices;
            _equipmentMatesServices = equipmentMatesServices;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("PZIOTEquipmentNotSameDatasIntervalAnalysisSerivces Job  is starting.");
            AnalysisInterval = Convert.ToInt16(AppSettings.app(new string[] { "ServiceConfig", "PZIOTEquipmentNotSameDatasIntervalAnalysisSerivcesInterval" }));
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(AnalysisInterval));//120

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            try
            {
                ConsoleHelper.WriteWarningLine($"不同数据时间间隔分析服务执行： {DateTime.Now}");
                DateTime endTime = DateTime.Now;//开始时间
                DateTime startTime = endTime.AddSeconds(-(AnalysisInterval));//结束时间
                Task.Run(async () => {
                    //获取到所有的设备
                    var eqps = await _equipmentServices.Query();
                    foreach (var item in eqps)
                    {
                        if (!keyValuePairs.ContainsKey(item.Id)) {
                            keyValuePairs.Add(item.Id,new Dictionary<string, DateTime>());
                        }
                        if (!mateskeyValuePairs.ContainsKey(item.Id))
                        {
                            mateskeyValuePairs.Add(item.Id, new Dictionary<string, string>());
                        }
                        //单个设备的分析逻辑
                        //查询当前时间节点和设置的时间节点后的一段时间的数据
                        var datas = await _equipmentDataScadaServices.Query(t => t.EquipmentId == item.Id && t.EquipmentDataGatherTime > startTime && t.EquipmentDataGatherTime < endTime && !string.IsNullOrEmpty(t.EquipmentDataItemValue));
                        ConsoleHelper.WriteSuccessLine($"PZIOTEquipmentNotSameDatasIntervalAnalysisSerivces分析设备编号{item.Id}中，获取到最近{AnalysisInterval}秒的数据，总条数为{datas.Count}");
                        var mates = await _equipmentMatesServices.Query(t => t.EquipmentId == item.Id);
                        
                        if (datas.Count > 0)
                        {
                            foreach (var mate in mates)
                            {
                                if (!keyValuePairs[item.Id].ContainsKey(mate.MateName)) {
                                    keyValuePairs[item.Id].Add(mate.MateName, DateTime.Now);
                                }
                                if (!mateskeyValuePairs[item.Id].ContainsKey(mate.MateName))
                                {
                                    mateskeyValuePairs[item.Id].Add(mate.MateName, "");
                                }
                                List<EquipmentDataScadaInterval> list = new List<EquipmentDataScadaInterval>();
                                var scadadatas = datas.FindAll(t => t.EquipmentDataItemName.Equals(mate.MateName));
                                //按采集时间排序，不然会错乱
                                scadadatas.Sort((a,b)=>a.EquipmentDataGatherTime.CompareTo(b.EquipmentDataGatherTime));
                                for (int i = 0; i < scadadatas.Count; i++)
                                {
                                    if (i == 0)
                                    {
                                        if (scadadatas[i].EquipmentDataGatherTime != keyValuePairs[item.Id][mate.MateName]&& scadadatas[i].EquipmentDataItemValue != mateskeyValuePairs[item.Id][mate.MateName])
                                        {
                                            int interval = (int)(scadadatas[i].EquipmentDataGatherTime - keyValuePairs[item.Id][mate.MateName]).TotalMilliseconds;
                                            list.Add(new EquipmentDataScadaInterval()
                                            {
                                                EquipmentDataGatherTime = scadadatas[i].EquipmentDataGatherTime,
                                                EquipmentDataItemName = scadadatas[i].EquipmentDataItemName,
                                                EquipmentDataItemValue = scadadatas[i].EquipmentDataItemValue,
                                                EquipmentId = scadadatas[i].EquipmentId,
                                                LastInterval = interval,
                                            });
                                            keyValuePairs[item.Id][mate.MateName] = scadadatas[i].EquipmentDataGatherTime;
                                            mateskeyValuePairs[item.Id][mate.MateName] = scadadatas[i].EquipmentDataItemValue;
                                        }
                                    }
                                    else {
                                        if (scadadatas[i - 1].EquipmentDataItemValue != scadadatas[i].EquipmentDataItemValue)
                                        {

                                            //变换的两条数据时间差计算
                                            int interval = (int)(scadadatas[i].EquipmentDataGatherTime - keyValuePairs[item.Id][mate.MateName]).TotalMilliseconds;
                                            list.Add(new EquipmentDataScadaInterval()
                                            {
                                                EquipmentDataGatherTime = scadadatas[i].EquipmentDataGatherTime,
                                                EquipmentDataItemName = scadadatas[i].EquipmentDataItemName,
                                                EquipmentDataItemValue = scadadatas[i].EquipmentDataItemValue,
                                                EquipmentId = scadadatas[i].EquipmentId,
                                                LastInterval = interval,
                                            });
                                            //变换后更新该值
                                            keyValuePairs[item.Id][mate.MateName] = scadadatas[i].EquipmentDataGatherTime;
                                            mateskeyValuePairs[item.Id][mate.MateName] = scadadatas[i].EquipmentDataItemValue;
                                        }

                                    }
                                    
                                }
                                //统一插入
                                //list.RemoveAt(0);
                                if (list.Count > 0)
                                {
                                    //需要移除第一个数据
                                    int result = await _equipmentDataScadaIntervalServices.Add(list);
                                    Console.WriteLine($"数据项{mate.MateName}共计{result}条数据插入成功.");
                                }

                            }

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
            Console.WriteLine("PZIOTEquipmentNotSameDatasIntervalAnalysisSerivces Job  is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
