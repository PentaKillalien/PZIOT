using System.Threading.Tasks;
using AutoMapper;
using PZIOT.IServices;
using PZIOT.Model;
using PZIOT.Model.Models;
using PZIOT.Model.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PZIOT.Model.RhMes;

namespace PZIOT.Controllers
{
    /// <summary>
    /// 用户角色关系
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class UserRoleController : Controller
    {
        private readonly ISysUserInfoServices _sysUserInfoServices;
        private readonly IUserRoleServices _userRoleServices;
        private readonly IRoleServices _roleServices;
        private readonly IMapper _mapper;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sysUserInfoServices"></param>
        /// <param name="userRoleServices"></param>
        /// <param name="mapper"></param>
        /// <param name="roleServices"></param>
        public UserRoleController(ISysUserInfoServices sysUserInfoServices, IUserRoleServices userRoleServices, IMapper mapper, IRoleServices roleServices)
        {
            _sysUserInfoServices = sysUserInfoServices;
            _userRoleServices = userRoleServices;
            _roleServices = roleServices;
            _mapper = mapper;
        }



        /// <summary>
        /// 新建用户
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="loginPwd"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DataResult<SysUserInfoDto>> AddUser(string loginName, string loginPwd)
        {
            var userInfo = await _sysUserInfoServices.SaveUserInfo(loginName, loginPwd);
            return new DataResult<SysUserInfoDto>()
            {
                Success = true,
                Message = "添加成功",
                Attach = _mapper.Map<SysUserInfoDto>(userInfo)
            };
        }

        /// <summary>
        /// 新建Role
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DataResult<Role>> AddRole(string roleName)
        {
            return new DataResult<Role>()
            {
                Success = true,
                Message = "添加成功",
                Attach = await _roleServices.SaveRole(roleName)
            };
        }

        /// <summary>
        /// 新建用户角色关系
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="rid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DataResult<UserRole>> AddUserRole(int uid, int rid)
        {
            return new DataResult<UserRole>()
            {
                Success = true,
                Message = "添加成功",
                Attach = await _userRoleServices.SaveUserRole(uid, rid)
            };
        }




    }
}
