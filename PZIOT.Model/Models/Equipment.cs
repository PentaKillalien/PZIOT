using SqlSugar;

namespace PZIOT.Model.Models
{
    /// <summary>
    /// 设备基础信息
    /// </summary>
    public class Equipment: RootEntityTkey<int>
    {
        /// <summary>
        /// 设备名
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
        /// <summary>
        /// 车间
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string WorkSpace { get; set; }
        /// <summary>
        /// 工厂
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string Factory { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string Position { get; set; }
        /// <summary>
        /// 产线
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string ProductLine { get; set; }
        /// <summary>
        /// 注册用户 --哪个用户注册的
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string RegisterUser { get; set; }
        /// <summary>
        /// 注册用Id --唯一，作为键
        /// </summary>
        public int RegisterUserId { get; set; }
        /// <summary>
        /// Qccid
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string Qccid { get; set; }
        /// <summary>
        /// 是否自动分析状态
        /// </summary>
        public bool IsAutoAnalysisStatus { get; set; }
    }
}
