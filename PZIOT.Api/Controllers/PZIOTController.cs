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
    [Route("api/Pziot")]
    public class PZIOTController : BaseApiController
    {
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
