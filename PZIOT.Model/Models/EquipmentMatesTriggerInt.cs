using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Model.Models
{
    /// <summary>
    /// 设备数据项触发器,采集时数据值会经过触发器，这里面保存触发器的具体配置
    /// </summary>
    public class EquipmentMatesTriggerInt : RootEntityTkey<int>
    {

        /// <summary>
        /// 触发器绑定的数据项ID
        /// </summary>
        public int MateId { get; set; }
        /// <summary>
        /// 触发器类型
        /// </summary>
        public TriggerType TriggerType{get;set;}
        /// <summary>
        /// 最小值
        /// </summary>
        public double MinValue { get; set; }
        /// <summary>
        /// 最大值
        /// </summary>
        public double MaxValue { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 反射方法
        /// </summary>
        public string AssemblyMethod { get; set; }
    }
    /// <summary>
    /// 触发器类型
    /// </summary>
    public enum TriggerType { 
        /// <summary>
        /// 整数型
        /// </summary>
        INT
    }
}
