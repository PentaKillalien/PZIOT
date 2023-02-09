using SqlSugar;
using System;

namespace PZIOT.Model.Models
{
    /// <summary>
    /// 博客文章
    /// </summary>
    public class Equipment: RootEntityTkey<int>
    {
        /// <summary>
        /// FactoryId
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string Name { get; set; }

        /// <summary>
        /// 唯一编号
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string UniqueCode { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string Type { get; set; }

    }
}
