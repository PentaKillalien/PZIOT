using System.Threading.Tasks;
using PZIOT.IServices.BASE;
using PZIOT.Model.Models;

namespace PZIOT.IServices
{
    public partial interface IPasswordLibServices :IBaseServices<PasswordLib>
    {
        Task<bool> TestTranPropagation2();
        Task<bool> TestTranPropagationNoTranError();
        Task<bool> TestTranPropagationTran2();
    }
}
