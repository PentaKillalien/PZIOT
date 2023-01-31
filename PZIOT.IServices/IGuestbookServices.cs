using PZIOT.IServices.BASE;
using PZIOT.Model;
using PZIOT.Model.Models;
using System.Threading.Tasks;

namespace PZIOT.IServices
{
    public partial interface IGuestbookServices : IBaseServices<Guestbook>
    {
        Task<MessageModel<string>> TestTranInRepository();
        Task<bool> TestTranInRepositoryAOP();

        Task<bool> TestTranPropagation();

        Task<bool> TestTranPropagationNoTran();

        Task<bool> TestTranPropagationTran();
    }
}