using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Model.Models
{
    public class EquipmentMatesFunction : RootEntityTkey<int>
    {

        /// <summary>
        /// 触发器绑定的数据项ID
        /// </summary>
        public int MateId { get; set; }
        /// <summary>
        /// 计算差值
        /// </summary>
        public double CalculationDifference { get; set; }
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
