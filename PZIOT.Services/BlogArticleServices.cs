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
        IMapper _mapper;
        public EquipmentServices(IMapper mapper)
        {
            this._mapper = mapper;
        }
        /// <summary>
        /// 获取视图博客详情信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Equipment> GetEquipmentDetails(int id)
        {
            // 此处想获取上一条下一条数据，因此将全部数据list出来，有好的想法请提出
            //var bloglist = await base.Query(a => a.IsDeleted==false, a => a.bID);
            var blogArticle = await base.Query(a => a.Id == id);
            return blogArticle[0];

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
