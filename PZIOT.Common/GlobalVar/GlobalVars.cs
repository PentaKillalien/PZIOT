using PZIOT.Common.EquipmentDriver;
using PZIOT.Model.Models;
using System.Collections.Generic;

namespace PZIOT
{
    /// <summary>
    /// 权限变量配置
    /// </summary>
    public static class Permissions
    {
        public const string Name = "Permission";

        /// <summary>
        /// 测试网关授权
        /// 可以使用PZIOT项目中的test用户
        /// 账号：test
        /// 密码：test
        /// </summary>
        public const string GWName = "GW";

        /// <summary>
        /// 当前项目是否启用IDS4权限方案
        /// true：表示启动IDS4
        /// false：表示使用JWT
        public static bool IsUseIds4 = false;
    }

    /// <summary>
    /// 路由变量前缀配置
    /// </summary>
    public static class RoutePrefix
    {
        /// <summary>
        /// 前缀名
        /// 如果不需要，尽量留空，不要修改
        /// 除非一定要在所有的 api 前统一加上特定前缀
        /// 前缀在appsettings.json中配置
        /// </summary>
        public static string Name = "";
    }

    /// <summary>
    /// RedisMqKey
    /// </summary>
    public static class RedisMqKey
    {
        public const string Loging = "Loging";
    }
    /// <summary>
    /// Api能否执行成功的系统锁
    /// </summary>
    public static class ApiLock {
        /// <summary>
        /// 此变量为True时，说明IOT业务已经启动,相关Api才可以访问
        /// </summary>
        public static bool IOTApiLock = false;
        /// <summary>
        /// Mqtt服务运行锁
        /// </summary>
        public static bool MqttRunLock = false;
    }
    /// <summary>
    /// PzIot设备管理模块
    /// </summary>
    public static class PZIOTEquipmentManager {
        /// <summary>
        /// 设备驱动对应关系
        /// </summary>
        public static Dictionary<int, IEquipmentDriver> EquipmentDriverDic{ get; set; }=new Dictionary<int, IEquipmentDriver>();
        /// <summary>
        /// 设备数据项对应关系,很少会有写的情况，基本就是处理读
        /// </summary>
        public static Dictionary<int, List<EquipmentMates>> EquipmentMatesDic { get; set; } = new Dictionary<int, List<EquipmentMates>>();
        /// <summary>
        /// MatesIntTrigger对应关系，很少会有写的情况，基本就是处理读
        /// </summary>
        public static Dictionary<int, List<EquipmentMatesTriggerInt>> MateTriggerIntDic { get; set; } = new Dictionary<int, List<EquipmentMatesTriggerInt>>();
        /// <summary>
        /// MatesStringTrigger对应关系，很少会有写的情况，基本就是处理读
        /// </summary>
        public static Dictionary<int, EquipmentMatesTriggerString> MateTriggerStringDic { get; set; } = new Dictionary<int, EquipmentMatesTriggerString>();
        /// <summary>
        /// MatesFunction对应关系，很少会有写的情况，基本就是处理读
        /// </summary>
        public static Dictionary<int, EquipmentMatesFunction> MateFunctionDic { get; set; } = new Dictionary<int, EquipmentMatesFunction>();
    }
}
