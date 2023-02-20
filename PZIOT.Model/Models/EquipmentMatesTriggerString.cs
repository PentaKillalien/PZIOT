using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Model.Models
{
    public class EquipmentMatesTriggerString : RootEntityTkey<int>
    {
        /// <summary>
        /// 触发器绑定的数据项ID
        /// </summary>
        public int MateId { get; set; }
        /// <summary>
        /// 触发器类型
        /// </summary>
        public TriggerType TriggerType { get; set; }
        /// <summary>
        /// 比较值
        /// </summary>
        public string CompareValue { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 反射方法
        /// </summary>
        public string AssemblyMethod { get; set; }
    }
}
