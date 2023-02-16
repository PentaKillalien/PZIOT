using Newtonsoft.Json;
using PZIOT.Model.Models;
using PZIOT.Model.PZIOTModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Common.EquipmentDriver
{
    /// <summary>
    /// 设备驱动描述操作类
    /// </summary>
    public class EquipmentDriverDescOper
    {
        /// <summary>
        /// 创建多个驱动的连接
        /// </summary>
        /// <param name="euqipmentDriverDescs"></param>
        public async Task CreatAllDriverConnection(List<EquipmentDriverDesc> euqipmentDriverDescs) {
            foreach (var item in euqipmentDriverDescs)
            {
                await CreatSingleDriverConnection(item);
            }
        }
        /// <summary>
        /// 创建一个驱动的连接
        /// </summary>
        /// <param name="euqipmentDriverDesc"></param>
        public async Task CreatSingleDriverConnection(EquipmentDriverDesc euqipmentDriverDesc)
        {
            int equipmentid = euqipmentDriverDesc.EquipmentId;
            string driverType = euqipmentDriverDesc.DriverType;
            string startJson = euqipmentDriverDesc.StartJsonInfo;
            try
            {
                switch (driverType)
                {
                    case "TcpClientDriver":
                        TcpClientDriver tcpClientDriver = new TcpClientDriver();
                        PZIOTEquipmentManager.EquipmentDriverDic.Add(equipmentid,tcpClientDriver);
                        await PZIOTEquipmentManager.EquipmentDriverDic[equipmentid].CreatConnect(JsonConvert.DeserializeObject<TcpClientConnectionModel>(startJson)); break;
                    case "ModbusRtuOverTcpClient":
                        ModbusRtuOverTcpClient mdbusRtuOverTcpClient = new ModbusRtuOverTcpClient();
                        PZIOTEquipmentManager.EquipmentDriverDic.Add(equipmentid, mdbusRtuOverTcpClient);
                        await PZIOTEquipmentManager.EquipmentDriverDic[equipmentid].CreatConnect(JsonConvert.DeserializeObject<ModbusMasterModel>(startJson)); break;
                    default:
                        ConsoleHelper.WriteErrorLine($"Id为{equipmentid}的设备的设备驱动配置字段{driverType}不匹配,请检查");
                        break;
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteErrorLine($"设备id为{equipmentid}的驱动启动失败，关键位置异常，程序终止" + ex.ToString());
                throw;
            }

        }
    }
}
