using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Model.PZIOTModels
{
    /// <summary>
    /// 西门子连接模型
    /// </summary>
    public class S7NetModel
    {
        /// <summary>
        /// plc类型
        /// </summary>
        public string PlcType { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 槽号
        /// </summary>
        public short Slot { get; set; }
        /// <summary>
        /// 架号
        /// </summary>
        public short Rack { get; set; }
    }
}
