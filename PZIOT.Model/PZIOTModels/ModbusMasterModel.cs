using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Model.PZIOTModels
{
    /// <summary>
    /// 连接modbus需要的参数
    /// </summary>
    public class ModbusMasterModel
    {
        /// <summary>
        /// 用于指定 Modbus 服务器的 IP 地址或者设备地址。
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 用于指定 Modbus 服务器的端口号，通常为 502
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 用于指定读取数据的超时时间，单位为毫秒
        /// </summary>
        public int ReadTimeout { get; set; }
        /// <summary>
        /// 用于指定读取数据的超时时间，单位为毫秒
        /// </summary>
        public int WriteTimeout { get; set; }
        /// <summary>
        /// 用于指定重试的次数
        /// </summary>
        public int Retries { get; set; }

        /// <summary>
        /// 用于指定读取数据的超时时间，单位为毫秒
        /// </summary>
        public int Timeout { get; set; }
    }
}
