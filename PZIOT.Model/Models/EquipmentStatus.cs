using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Model.Models
{
    /// <summary>
    /// 设备状态
    /// </summary>
    public class EquipmentStatus : RootEntityTkey<int>
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public int EquipmentId { get; set; }
        /// <summary>
        /// 设备状态
        /// </summary>
        public PZIOTEquipmnetStatus Status { get; set; }
        /// <summary>
        /// 设备二级状态--第三方对接状态
        /// </summary>
        public PZIOTChildEquipmentStatus ChildStatus { get; set; }
        /// <summary>
        /// 状态保持时长
        /// </summary>
        public int StatusKeepLength { get; set; }
        /// <summary>
        /// 状态开始时间
        /// </summary>
        public DateTime StatusStartTime { get; set; }
        /// <summary>
        /// 状态结束时间
        /// </summary>
        public DateTime StatusEndTime { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(Length = 500, IsNullable = true)]
        public string Desc { get; set; }
    }

    /// <summary>
    /// PZIOT的设备状态枚举
    /// </summary>
    public enum PZIOTEquipmnetStatus {
        /// <summary>
        /// 正常
        /// </summary>
        Normal,
        /// <summary>
        /// 故障
        /// </summary>
        Fault,
        /// <summary>
        /// 停机
        /// </summary>
        Shutdown,
        /// <summary>
        /// 离线
        /// </summary>
        Offline,
        /// <summary>
        /// 待机
        /// </summary>
        Standby,
        /// <summary>
        /// 升级中
        /// </summary>
        Upgrading,
        /// <summary>
        /// 低电量
        /// </summary>
        LowBattery,
        /// <summary>
        /// 报警
        /// </summary>
        Alarm
    }

    /// <summary>
    /// PZIOT子状态枚举-第三方扩展行为
    /// </summary>
    public enum PZIOTChildEquipmentStatus { 
        /// <summary>
        /// 调模 - 某车企
        /// </summary>
        ModuleAdjustment
    }
}
