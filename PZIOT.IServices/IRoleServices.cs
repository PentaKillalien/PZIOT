using PZIOT.IServices.BASE;
using PZIOT.Model.Models;
using System.Threading.Tasks;

namespace PZIOT.IServices
{	
	/// <summary>
	/// RoleServices
	/// </summary>	
    public interface IRoleServices :IBaseServices<Role>
	{
        Task<Role> SaveRole(string roleName);
        Task<string> GetRoleNameByRid(int rid);

    }
}
