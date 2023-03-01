
using PZIOT.IServices;
using PZIOT.Model;
using PZIOT.Model.Models;
using PZIOT.SwaggerHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static PZIOT.Extensions.CustomApiVersion;
using PZIOT.Extensions;
using PZIOT.Services;
using PZIOT.Model.RhMes;

namespace PZIOT.Controllers
{
    /// <summary>
    /// 设备管理
    /// </summary>
    [Produces("application/json")]
    [Route("api/Equipment")]
    public class EquipmentController : BaseApiController
    {
        readonly IEquipmentServices _equipmentServices;
        private readonly ILogger<EquipmentController> _logger;
        IRedisBasketRepository _redisBasketRepository;
        /// <summary>
        /// gz
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="equipmentServices"></param>
        /// <param name="redisBasketRepository"></param>
        public EquipmentController(ILogger<EquipmentController> logger,IEquipmentServices equipmentServices, IRedisBasketRepository redisBasketRepository)
        {
            _logger = logger;
            _redisBasketRepository = redisBasketRepository;
            _equipmentServices = equipmentServices;
        }


        /// <summary>
        /// 获取设备列表【无权限】
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="eqp"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DataResult<PageModel<Equipment>>> Get(int id, int page = 1, string eqp = "1", string key = "")
        {
            await _redisBasketRepository.ListLeftPushAsync("JJBO","ccc",0);
            //Console.WriteLine($"redis放入数据成功{await _redisBasketRepository.ListLeftPopAsync(RedisMqKey.Loging,0)}")
            //int intPageSize = 6;
            //await _equipmentStatusServices.Add(new EquipmentStatus() { 
            //     EquipmentId=id,
            //     Status=PZIOTEquipmnetStatus.Normal,
            //     ChildStatus=PZIOTChildEquipmentStatus.None,
            //     StatusEndTime=DateTime.Now,
            //     StatusStartTime=DateTime.Now,
            //     StatusKeepLength=0,
            //     Desc="测试"

            //});
            return SuccessPage(new PageModel<Equipment>() { 
            
            });
        }


        /// <summary>
        /// 获取设备详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        //[Authorize(Policy = "Scope_BlogModule_Policy")]
        [Authorize]
        public async Task<DataResult<Equipment>> Get(int id)
        {
            return Success(await _equipmentServices.GetEquipmentDetails(id));
        }



        
        /// <summary>
        /// 获取设备测试信息 v2版本
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        ////MVC自带特性 对 api 进行组管理
        //[ApiExplorerSettings(GroupName = "v2")]
        ////路径 如果以 / 开头，表示绝对路径，反之相对 controller 的想u地路径
        //[Route("/api/v2/blog/Blogtest")]
        //和上边的版本控制以及路由地址都是一样的

        [CustomRoute(ApiVersions.V2, "Blogtest")]
        public DataResult<string> V2_Blogtest()
        {
            return Success<string>("我是第二版的设备信息");
        }

        /// <summary>
        /// 添加设备【无权限】
        /// </summary>
        /// <param name="equipment"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(Policy = "Scope_BlogModule_Policy")]
        [Authorize]
        public async Task<DataResult<string>> Post([FromBody] Equipment equipment)
        {
            var id = (await _equipmentServices.Add(equipment));
            return id > 0 ? Success<string>(id.ObjToString()) : Failed("添加失败");
        }


        
        /// <summary>
        /// 更新设备信息
        /// </summary>
        /// <param name="equipment"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Update")]
        [Authorize(Permissions.Name)]
        public async Task<DataResult<string>> Put([FromBody] Equipment equipment)
        {
            if (equipment != null && equipment.Id > 0)
            {
                var model = await _equipmentServices.QueryById(equipment.Id);

                if (model != null)
                {
                    model.Name = equipment.Name;
                    model.UniqueCode = equipment.UniqueCode;

                    if (await _equipmentServices.Update(model))
                    {
                        return Success<string>(equipment?.Id.ObjToString());
                    }
                }
            }
            return Failed("更新失败");
        }



        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Permissions.Name)]
        [Route("Delete")]
        public async Task<DataResult<string>> Delete(int id)
        {
            if (id > 0)
            {
                var equipment = await _equipmentServices.QueryById(id);
                if (equipment == null)
                {
                    return Failed("查询无数据");
                }
                return await _equipmentServices.Update(equipment) ? Success(equipment?.Id.ObjToString(), "删除成功") : Failed("删除失败");
            }
            return Failed("入参无效");
        }
        /// <summary>
        /// apache jemeter 压力测试
        /// 更新接口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ApacheTestUpdate")]
        public async Task<DataResult<bool>> ApacheTestUpdate()
        {
            return Success(await _equipmentServices.Update(new { bsubmitter = $"laozhang{DateTime.Now.Millisecond}", bID = 1 }), "更新成功");
        }
    }
}