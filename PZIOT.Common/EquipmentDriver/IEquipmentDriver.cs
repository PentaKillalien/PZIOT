using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Common.EquipmentDriver
{
    /// <summary>
    /// 驱动都采取主动链接设备的模式，Client,一个设备对应一个或多个Driver实例，不再搞服务端做成驱动
    /// 由于驱动可能会创建很多个，不像服务层具备唯一性，由AutoFac代理暂时不作为主要项
    /// </summary>
    public interface IEquipmentDriver
    {
        /// <summary>
        /// 创建和设备的链接
        /// </summary>
        /// <returns></returns>
        Task<bool> CreatConnect(object t);
        /// <summary>
        /// 断开和设备的链接
        /// </summary>
        /// <returns></returns>
        Task<bool> DisConnect();
        /// <summary>
        /// 获取链接状态
        /// </summary>
        /// <returns></returns>
        Task<bool> GetConnectionState();
        /// <summary>
        /// 从设备读取单个参数
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        /// <summary>
        /// 从设备请求单个参数
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<EquipmentReadResponseProtocol> RequestSingleParaFromEquipment(string para);
        /// <summary>
        /// 从设备请求多个参数
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        Task<List<EquipmentReadResponseProtocol>> RequestMultipleParasFromEquipment(List<string> readparas);
    }
}
