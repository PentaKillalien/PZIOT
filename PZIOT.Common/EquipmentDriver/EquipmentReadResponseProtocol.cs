using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Common.EquipmentDriver
{
    /// <summary>
    /// 设备读取响应协议
    /// </summary>
    public class EquipmentReadResponseProtocol
    {
        /// <summary>
        /// 请求参数
        /// </summary>
        public string RequestPara { get; set; }
        /// <summary>
        /// 响应值
        /// </summary>
        public string ResponseValue { get; set; }
    }
}
