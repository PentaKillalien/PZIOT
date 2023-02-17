using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Model.Models
{
    public class EquipmentMates : RootEntityTkey<int>
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public int EquipmentId { get; set; }
        /// <summary>
        /// 仪表名称
        /// </summary>
        public string MateName { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActivation { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string MateUnit { get; set; }

        /// <summary>
        /// 标准值
        /// </summary>
        public string StandardValue { get; set; }

        /// <summary>
        /// 设置最大值
        /// </summary>
        public string MaxValue { get; set; }

        /// <summary>
        /// 设置最小值
        /// </summary>
        public string MinValue { get; set; }

        string m_Value = string.Empty;
        /// <summary>
        /// 实际值
        /// </summary>
        public string Value
        {
            get { return m_Value; }
            set
            {
                m_Value = value;
                this.UpDateTimeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        /// <summary>
        /// 数据地址 通过该地址解析数据通道中那个值是该仪表的值 
        /// </summary>
        public string DataAddress { get; set; }

        /// <summary>
        /// 值更新时间
        /// </summary>
        public string UpDateTimeStr { get; set; }

        /// <summary>
        /// 功能
        /// </summary>
        public string Function { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public string VarType { get; set; }
        /// <summary>
        /// 触发器
        /// </summary>
        public int TriggerId { get; set; }
        /// <summary>
        /// 采集间隔
        /// </summary>
        public int GatherInterval { get; set; }
    }
}
