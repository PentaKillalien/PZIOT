using NModbus;
using NModbus.IO;
using PZIOT.Model.PZIOTModels;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Common.EquipmentDriver
{
    /// <summary>
    /// 数据格式1-10-1-1*4*short
    /// </summary>
    public class ModbusRtuOverTcpClient : BaseClientDriver
    {
        private ModbusMasterModel modbusMasterModel;
        private bool _IsConnected = false;
        private int timeout = 2000;
        public override bool IsConnected => _IsConnected;
        private IModbusMaster modbusMaster = null;
        private TcpClient tcpClient = null;
        private ModbusFactory factory = null;
        public override async Task<bool> CreatConnect(object t)
        {
            bool result = await Task.Run(() =>
            {
                try
                {
                    modbusMasterModel = (ModbusMasterModel)t;
                    if (modbusMasterModel.Timeout > 20)
                        timeout = modbusMasterModel.Timeout;
                    tcpClient = new TcpClient(modbusMasterModel.Address, modbusMasterModel.Port);
                    factory = new ModbusFactory();
                    var adapter = new TcpClientAdapter(tcpClient);
                    modbusMaster = factory.CreateRtuMaster(adapter);
                    modbusMaster.Transport.ReadTimeout = timeout;
                    _IsConnected = true;
                    return true;
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
                modbusMaster.Dispose();
                return true;
            });

            return result;
        }

        public override async Task<bool> GetConnectionState()
        {
            bool result = await Task.Run(() => {
                return tcpClient.Connected;
            });

            return result;
        }



        public override async Task<EquipmentReadResponseProtocol> RequestSingleParaFromEquipment(string para)
        {
            EquipmentReadResponseProtocol result = await Task.Run( () => {
                string back = ReadModbusAddressHandler(para);
                return new EquipmentReadResponseProtocol()
                {
                    RequestPara = para,
                    ResponseValue = back
                };

            });
            return result;
        }
        /// <summary>
        /// 分解格式
        /// </summary>
        /// <param name="source"></param>
        private string ReadModbusAddressHandler(string source) {
            string[] datas = source.Split("*");
            //1-10-1-1*4*short
            return ReadModbusAddress(datas[0], datas[1], datas[2]);
        }
        /// <summary>
        /// 读取数据项modbus配置的地址值
        /// </summary>
        /// <param name="dataAddr">1-10-1-1</param>
        /// <param name="funcCode">4</param>
        /// <param name="dataType">short</param>
        /// <param name="oper">AB-CD</param>
        /// <returns></returns>
        private string ReadModbusAddress(string dataAddr, string funcCode, string dataType)
        {
            try
            {
                //地址分割
                //RedisLogHelper.LogInfo($"传入参数为：-dataaddr:{dataAddr}-funcode:{funcCode}-datatype:{dataType}");
                string[] datas = dataAddr.Split('-');
                string convertflag = string.Empty;
                if (datas.Length > 3)
                {
                    convertflag = datas[3];
                }
                string result = string.Empty;
                if (funcCode.Equals("4"))
                {
                    //read hold register
                    switch (dataType)
                    {
                        case "short":
                            ushort[] aa = modbusMaster.ReadInputRegisters(byte.Parse(datas[0]), ushort.Parse(datas[1]), ushort.Parse(datas[2]));
                            //RedisLogHelper.LogInfo("OverRtu--读取short值的个数为：" + aa.Length);
                            result = aa[ushort.Parse(datas[3])].ToString();
                            break;
                        case "int": result = ReadInt(modbusMaster.ReadInputRegisters(byte.Parse(datas[0]), ushort.Parse(datas[1]), ushort.Parse(datas[2])), convertflag); break;
                        case "float": result = ReadFloat(modbusMaster.ReadInputRegisters(byte.Parse(datas[0]), ushort.Parse(datas[1]), ushort.Parse(datas[2])), convertflag); break;
                        case "double": result = ReadDouble(modbusMaster.ReadInputRegisters(byte.Parse(datas[0]), ushort.Parse(datas[1]), ushort.Parse(datas[2])), convertflag); break;
                        default:
                            break;
                    }
                }
                Console.WriteLine(result);
                return result;
            }
            catch (Exception ex)
            {
       
                return string.Empty;
            }
        }
        /// <summary>
        /// 读取int
        /// </summary>
        /// <param name="shorts"></param>
        /// <returns></returns>
        private string ReadInt(ushort[] shorts, string flag)
        {
            if (string.IsNullOrEmpty(flag) || flag.Equals("ABCD"))
            {
                List<byte> result = new List<byte>();
                result.AddRange(BitConverter.GetBytes(shorts[1]));
                result.AddRange(BitConverter.GetBytes(shorts[0]));
                long value = BitConverter.ToInt32(result.ToArray(), 0);
                return value.ToString();
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// 读取float
        /// </summary>
        /// <param name="shorts"></param>
        /// <returns></returns>
        private string ReadFloat(ushort[] shorts, string flag)
        {
            if (string.IsNullOrEmpty(flag) || flag.Equals("ABCD"))
            {
                List<byte> result = new List<byte>();
                result.AddRange(BitConverter.GetBytes(shorts[1]));
                //result.AddRange(BitConverter.GetBytes('.'));
                result.AddRange(BitConverter.GetBytes(shorts[0]));
                float value = BitConverter.ToSingle(result.ToArray(), 0);
                return value.ToString();
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// 读取double
        /// </summary>
        /// <param name="shorts"></param>
        /// <returns></returns>
        private string ReadDouble(ushort[] shorts, string flag)
        {
            if (string.IsNullOrEmpty(flag) || flag.Equals("ABCD"))
            {
                List<byte> result = new List<byte>();
                result.AddRange(BitConverter.GetBytes(shorts[1]));
                result.AddRange(BitConverter.GetBytes(shorts[0]));
                //result.AddRange(BitConverter.GetBytes('.'));
                result.AddRange(BitConverter.GetBytes(shorts[2]));
                result.AddRange(BitConverter.GetBytes(shorts[3]));
                double value = BitConverter.ToDouble(result.ToArray(), 0);
                return value.ToString();
            }
            else
            {
                return "";
            }

        }
    }
}
