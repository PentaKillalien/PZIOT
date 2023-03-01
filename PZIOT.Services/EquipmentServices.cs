using AutoMapper;
using PZIOT.Common;
using PZIOT.IServices;
using PZIOT.Model.Models;
using PZIOT.Services.BASE;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PZIOT.Services
{
    public class EquipmentServices : BaseServices<Equipment>, IEquipmentServices
    {
        //IMapper _mapper;
        //public EquipmentServices(IMapper mapper)
        //{
        //    this._mapper = mapper;
        //}
        public EquipmentServices()
        {
        }
        
        /// <summary>
        /// 获取设备详情信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Equipment> GetEquipmentDetails(int id)
        {
            var eqps = await base.Query(a => a.Id == id);
            return eqps[0];

        }


        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <returns></returns>
        [Caching(AbsoluteExpiration = 10)]
        public async Task<List<Equipment>> GetEquipments()
        {
            var bloglist = await base.Query(a => a.Id > 0, a => a.Id);

            return bloglist;

        }
    }
}
