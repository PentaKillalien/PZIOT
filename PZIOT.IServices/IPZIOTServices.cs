using PZIOT.IServices.BASE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.IServices
{
    /// <summary>
    /// IOT服务不集成Base业务，只管加载IOT示例等
    /// </summary>
    public interface IPZIOTServices
    {
        /// <summary>
        /// 启动IOT
        /// </summary>
        /// <returns></returns>
        Task<bool> StartPIOT();
    }
}
