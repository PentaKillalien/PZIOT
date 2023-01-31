using PZIOT.IServices.BASE;
using PZIOT.Model.Models;
using System.Threading.Tasks;

namespace PZIOT.IServices
{	
	/// <summary>
	/// UserRoleServices
	/// </summary>	
    public interface IUserRoleServices :IBaseServices<UserRole>
	{

        Task<UserRole> SaveUserRole(int uid, int rid);
        Task<int> GetRoleIdByUid(int uid);
    }
}

