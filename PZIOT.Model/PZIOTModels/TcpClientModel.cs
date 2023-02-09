using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Model.PZIOTModels
{
    /// <summary>
    /// TcpClient链接信息模型
    /// </summary>
    public class TcpClientConnectionModel
    {
        /// <summary>
        /// Ip地址
        /// </summary>
        public string  Serverip{ get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port{ get; set; }
    }
}
