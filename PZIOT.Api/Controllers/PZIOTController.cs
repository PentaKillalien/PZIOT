using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PZIOT.Controllers;
using PZIOT.IServices;
using PZIOT.Model;
using PZIOT.Services;

namespace PZIOT.Api.Controllers
{
    /// <summary>
    /// IOT相关服务启动
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class PZIOTController : BaseApiController
    {
        //IOT只需要一个入口启动就行了，不需要太多的内容，如果需要把报表什么的数据结构暴露出来，应该建立其它的服务进行操作
        readonly IPZIOTServices _PZIOTServices;
        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="pZIOTServices"></param>
        public PZIOTController(IPZIOTServices pZIOTServices)
        {
            _PZIOTServices = pZIOTServices;
        }
        /// <summary>
        /// 启动IOT服务
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<MessageModel<string>> StartPIot()
        {
            if (ApiLock.IOTApiLock) {
                return Failed<string>("IOT主服务启动后无法再次启动");
            }
            await _PZIOTServices.StartPIOT();
            return Success<string>("IOT主服务启动成功");

        }
    }
}
