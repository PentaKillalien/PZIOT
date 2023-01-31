using PZIOT.Common;
using PZIOT.IRepository.Base;
using PZIOT.IServices;
using PZIOT.Model.Models;
using PZIOT.Services.BASE;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PZIOT.Services
{
    public class TopicServices: BaseServices<Topic>, ITopicServices
    {
        /// <summary>
        /// 获取开Bug专题分类（缓存）
        /// </summary>
        /// <returns></returns>
        [Caching(AbsoluteExpiration = 60)]
        public async Task<List<Topic>> GetTopics()
        {
            return await base.Query(a => !a.tIsDelete && a.tSectendDetail == "tbug");
        }

    }
}
