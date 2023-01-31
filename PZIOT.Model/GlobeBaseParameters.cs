
namespace PZIOT.Model
{
    /// <summary>
    /// 存放静态变量--不存常量
    /// 全局静态参数--按照系统的业务逻辑，这里的所有静态内容都写到配置文件
    /// </summary>
    public class GlobeBaseParameters
    {
        /// <summary>
        /// 当前系统版本-不应该写这里
        /// </summary>
        public static string SysVersion { get; set; } = "V1.0";
    }
}
