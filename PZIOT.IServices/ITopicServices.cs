using PZIOT.IServices.BASE;
using PZIOT.Model.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PZIOT.IServices
{
    public interface ITopicServices : IBaseServices<Topic>
    {
        Task<List<Topic>> GetTopics();
    }
}
