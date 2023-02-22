using NModbus.IO;
using NModbus;
using PZIOT.Model.PZIOTModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using S7.Net;

namespace PZIOT.Common.EquipmentDriver
{
    /// <summary>
    /// 西门子S7的Driver
    /// </summary>
    public class S7NetClientDriver : BaseClientDriver
    {
        private S7NetModel _S7NetModel;
        private CpuType cpuType;
        private Plc plc;

        private bool _IsConnected = false;
        public override bool IsConnected => _IsConnected;
        public override async Task<bool> CreatConnect(object t)
        {
            bool result = await Task.Run(() =>
            {
                try
                {
                    _S7NetModel = (S7NetModel)t;
                    switch (_S7NetModel.PlcType)
                    {
                        case "S7200": cpuType = CpuType.S7200; break;
                        case "S7200Smart": cpuType = CpuType.S7200Smart; break;
                        case "S7300": cpuType = CpuType.S7300; break;
                        case "S7400": cpuType = CpuType.S7400; break;
                        case "S71200": cpuType = CpuType.S71200; break;
                        case "S71500": cpuType = CpuType.S71500; break;
                        default:
                            break;
                    }
                    plc = new Plc(cpuType, _S7NetModel.Address, _S7NetModel.Rack, _S7NetModel.Slot);
                    plc.Open();
                    _IsConnected = plc.IsConnected;
                    ConsoleHelper.WriteSuccessLine($"连接plc{_S7NetModel.Address}成功!");
                    return _IsConnected;
                }
                catch (Exception ex)
                {
                    _IsConnected = false;
                    ConsoleHelper.WriteErrorLine($"驱动创建连接失败，错误的连接字符串{t},{ex}");
                    return false;
                }

            });

            return result;
        }

        public override async Task<bool> DisConnect()
        {
            bool result = await Task.Run(() => {
                plc.Close();
                return true;
            });

            return result;
        }

        public override async Task<bool> GetConnectionState()
        {
            bool result = await Task.Run(() => {
                return plc.IsConnected;
            });

            return result;
        }



        public override async Task<EquipmentReadResponseProtocol> RequestSingleParaFromEquipment(string para)
        {
            ConsoleHelper.WriteWarningLine("S7Net驱动尝试读取");
            EquipmentReadResponseProtocol result = await Task.Run(() => {
                string back = ReadPlc(para);
                return new EquipmentReadResponseProtocol()
                {
                    RequestPara = para,
                    ResponseValue = back
                };

            });
            return result;
        }

        /// <summary>
        /// 读取plc地址
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        private string ReadPlc(string txt_read_addr)
        {
            try
            {
                try
                {
                    string[] arr = (txt_read_addr.ToUpper()).Split('.');
                    string valuetype = arr[1].Substring(0, 3);
                    if (valuetype == "DBX")
                    {
                        bool test1 = (bool)plc.Read(txt_read_addr.ToUpper());
                        return test1.ToString();
                    }

                    else if (valuetype == "DBW")
                    {
                        short test3 = ((ushort)plc.Read(txt_read_addr.ToUpper())).ConvertToShort();
                        return test3.ToString();
                    }

                    else if (valuetype == "DBD")
                    {
                        double test5 = ((uint)plc.Read(txt_read_addr.ToUpper())).ConvertToFloat();
                        return test5.ToString();
                    }

                    else
                    {
                        ConsoleHelper.WriteErrorLine("请检查地址是否输入错误！");
                    }
                }
                catch (Exception ex)
                {
                    ConsoleHelper.WriteErrorLine($"请检查地址是否输入错误！{ex}");
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteErrorLine($"读取Plc地址{txt_read_addr}异常{ex.Message}");
                return string.Empty;
            }
        }
    }
}
