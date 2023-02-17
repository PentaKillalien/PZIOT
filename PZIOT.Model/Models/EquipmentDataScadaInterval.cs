﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Model.Models
{
    /// <summary>
    /// 数据采集历史表，定时任务会根据此表做数据分析
    /// </summary>
    public class EquipmentDataScadaInterval : RootEntityTkey<int>
    {
        /// <summary>
        /// 设备id
        /// </summary>
        public int EquipmentId { get; set; }
        /// <summary>
        /// 设备采集数据项名
        /// </summary>
        public string EquipmentDataItemName { get; set; }
        /// <summary>
        /// 设备采集数据项值
        /// </summary>
        public string EquipmentDataItemValue { get; set; }
        /// <summary>
        /// 数据采集时间
        /// </summary>
        public DateTime EquipmentDataGatherTime { get; set; }
        /// <summary>
        /// 和上一条不同数据的时间间隔
        /// </summary>
        public int LastInterval { get; set; }
    }
}
