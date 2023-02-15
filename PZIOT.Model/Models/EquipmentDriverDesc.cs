using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Model.Models
{
    /// <summary>
    /// 设备驱动描述
    /// </summary>
    public class EquipmentDriverDesc : RootEntityTkey<int>
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public int EquipmentId { get; set; }
        /// <summary>
        /// 驱动类型
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string DriverType { get; set; }
        /// <summary>
        /// 启动json信息
        /// </summary>
        [SugarColumn(Length = 1000, IsNullable = true)]
        public string StartJsonInfo { get; set; }
    }
}
