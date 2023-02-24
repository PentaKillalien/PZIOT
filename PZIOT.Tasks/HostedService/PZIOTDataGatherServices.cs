using PZIOT.Common;
using PZIOT.IServices;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using PZIOT.Common.EquipmentDriver;
using PZIOT.Model.Models;
using PZIOT.Tasks.Trigger;
using System.Collections.Generic;
using PZIOT.Common.Helper;
using PZIOT.Tasks.Rule;
using System.Linq;
using PZIOT.Tasks.Function;
using System.Diagnostics;

namespace PZIOT.Tasks
{
    /// <summary>
    ///  定时采集任务，目前所有的采集都和数据库挂钩，资源占用会较大
    /// </summary>
    public class PZIOTDataGatherServices : IHostedService, IDisposable
    {
        private Timer _timer;
        //private readonly IEquipmentServices _equipmentServices;//查询设备信息
        private readonly IEquipmentMatesServices _equipmentMatesServices;//查询设备数据项信息
        private readonly IEquipmentDataScadaServices _equipmentDataScadaServices;//设备采集数据
        private readonly IEquipmentMatesTriggerIntServices _equipmentMatesTriggerServices;
        private readonly IEquipmentMatesTriggerStringServices _equipmentMatesTriggerStringServices;
        private readonly IEquipmentMatesFunctionServices _equipmentMatesFunctionServices;
        private readonly Dictionary<int, TriggerData>  triggerDatas;
        private readonly Dictionary<int, IFunction> functions;
        private const string DefaultTriggerClass = "DefaultConsoleOk";
        private int GatherFrequency = 10;//秒
        // 这里可以注入
        public PZIOTDataGatherServices(IEquipmentMatesTriggerStringServices equipmentMatesTriggerStringServices,IEquipmentMatesTriggerIntServices equipmentMatesTriggerServices, IEquipmentMatesServices equipmentMatesServices, IEquipmentMatesFunctionServices equipmentMatesFunctionServices,IEquipmentDataScadaServices equipmentDataScadaServices)
        {
            //_equipmentServices = equipmentServices;
            _equipmentMatesServices= equipmentMatesServices;
            _equipmentDataScadaServices = equipmentDataScadaServices;
            _equipmentMatesTriggerServices = equipmentMatesTriggerServices;
            _equipmentMatesTriggerStringServices = equipmentMatesTriggerStringServices;
            _equipmentMatesFunctionServices = equipmentMatesFunctionServices;
            triggerDatas = new Dictionary<int, TriggerData>();
            functions = new Dictionary<int, IFunction>();   

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Job 1 is starting.");
            GatherFrequency = Convert.ToInt16(AppSettings.app(new string[] {"ServiceConfig", "PZIOTDataGatherServicesInterval" }));
            //_timer = new Timer(DoWork, null, TimeSpan.Zero,
            //    TimeSpan.FromSeconds(60 * 60));//一个小时
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(GatherFrequency));//优先写采集频率统一的版本，后期每个设备自定义频率
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
                            var matesinfo = await _equipmentMatesServices.QueryGlobeDic(it => it.EquipmentId == item.Key,item.Key);
                            foreach (var item2 in matesinfo)
                            {
                                try
                                {
                                    if (item2.IsActivation) {
                                        if (!triggerDatas.ContainsKey(item2.Id)) {
                                            if (item2.VarType.Equals("DOUBLE")) {
                                                var triggerData = new TriggerData();
                                                triggerData.ValueChanged += async (sender, e) =>
                                                {
                                                    //double类型的mate会有Function,String的只有触发器
                                                    Console.WriteLine($"数据项编号{e.MateId}数据发生变化,oldValue>{e.OldValue}=>newvalue>{e.NewValue}");
                                                    int mateid = Convert.ToInt16(e.MateId);
                                                    if (!functions.ContainsKey(mateid))
                                                    {
                                                        //查找function
                                                        var getresult2 = await _equipmentMatesFunctionServices.Query(t => t.MateId.Equals(item2.Id));
                                                        if (getresult2.Count > 0)
                                                        {
                                                            foreach (var func in getresult2)
                                                            {
                                                                Type[] implementingTypes = InterfaceImplementationHelper.GetImplementingTypes(typeof(IFunction));
                                                                var myClass = implementingTypes.FirstOrDefault(type => type.Name == func.AssemblyMethod);
                                                                if (myClass != null)
                                                                {
                                                                    var myObject = (IFunction)Activator.CreateInstance(myClass);
                                                                    functions.Add(mateid, myObject);
                                                                    await myObject.ExecuteRule(e.OldValue, e.NewValue);
                                                                    Console.WriteLine($"Trigger fired: {func.Description}");
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else {
                                                        await functions[mateid].ExecuteRule(e.OldValue, e.NewValue);
                                                    }
                                                    
                                                };
                                                triggerDatas.Add(item2.Id, triggerData);
                                            }
                                            
                                        }
                                        //先获取采集的Mate项，绑定设备Id
                                        EquipmentReadResponseProtocol backinfo = await item.Value.RequestSingleParaFromEquipment(item2.DataAddress);
                                        ConsoleHelper.WriteWarningLine($"设备id为{item.Key}的驱动采集数据地址{item2.DataAddress}得到回复，结果为{backinfo.ResponseValue}!");
                                        //转换成对应的设备信息进行记录,查询设备信息
                                        item2.Value = backinfo.ResponseValue;
                                        //数据库值更新
                                        await _equipmentMatesServices.Update(item2);
                                        //建立数据采集存储对象
                                        EquipmentDataScada data = new EquipmentDataScada();
                                        data.MateId = item2.Id;
                                        data.EquipmentId = item.Key;
                                        data.EquipmentDataGatherTime = DateTime.Now;
                                        data.EquipmentDataItemValue = backinfo.ResponseValue;
                                        data.EquipmentDataItemName = item2.MateName;
                                        //查询规则进行触发,其实不需要维护Triggerid
                                        switch (item2.VarType)
                                        {
                                            case "DOUBLE":
                                                double num = 0;
                                                if (double.TryParse(data.EquipmentDataItemValue, out num))
                                                {
                                                    //获取到当前Mate维护的触发器
                                                    var getresult = await _equipmentMatesTriggerServices.Query(t => t.TriggerType.Equals(item2.TriggerId) && t.MateId == item2.Id);
                                                    if (getresult.Count == 0)
                                                    {
                                                        triggerDatas[item2.Id].rulemethod = DefaultTriggerClass;//默认触发器
                                                    }
                                                    else
                                                    {

                                                        triggerDatas[item2.Id].rulemethod = getresult[0].AssemblyMethod;
                                                    }
                                                    triggerDatas[item2.Id].rules = getresult;
                                                    triggerDatas[item2.Id].mateId = item2.Id;
                                                    triggerDatas[item2.Id].usedata = data;//触发更多数据
                                                    triggerDatas[item2.Id].Value = num;//值变化触发判断
                                                                                       //InterfaceImplementationHelper(typeof(IRule));

                                                }
                                                break;
                                            case "STRING":
                                                var getresult2 = await _equipmentMatesTriggerStringServices.Query(t => t.TriggerType.Equals(item2.TriggerId) && t.MateId == item2.Id);
                                                //Console.WriteLine($"{getresult2[0].Description}");
                                                foreach (var rule in getresult2)
                                                {
                                                    Type[] implementingTypes = InterfaceImplementationHelper.GetImplementingTypes(typeof(IRules));
                                                    var myClass = implementingTypes.FirstOrDefault(type => type.Name == rule.AssemblyMethod);
                                                    if (myClass != null)
                                                    {
                                                        var myObject = (IRules)Activator.CreateInstance(myClass);
                                                        await myObject.ExecuteRule(data, rule);
                                                        Console.WriteLine($"Trigger fired: {rule.Description}");
                                                    }
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                        

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
