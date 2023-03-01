using PZIOT.Common.Helper;
using PZIOT.Controllers;
using PZIOT.IServices;
using PZIOT.Model;
using PZIOT.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using PZIOT.Model.RhMes;

namespace PZIOT.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class DepartmentController : BaseApiController
    {
        private readonly IDepartmentServices _departmentServices;
        private readonly IWebHostEnvironment _env;

        public DepartmentController(IDepartmentServices departmentServices, IWebHostEnvironment env)
        {
            _departmentServices = departmentServices;
            _env = env;
        }

        [HttpGet]
        public async Task<DataResult<PageModel<Department>>> Get(int page = 1, string key = "", int intPageSize = 50)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                key = "";
            }

            Expression<Func<Department, bool>> whereExpression = a => true;

            return new DataResult<PageModel<Department>>()
            {
                Message = "获取成功",
                Success = true,
                Attach = await _departmentServices.QueryPage(whereExpression, page, intPageSize)
            };

        }

        [HttpGet("{id}")]
        public async Task<DataResult<Department>> Get(string id)
        {
            return new DataResult<Department>()
            {
                Message = "获取成功",
                Success = true,
                Attach = await _departmentServices.QueryById(id)
            };
        }

        /// <summary>
        /// 查询树形 Table
        /// </summary>
        /// <param name="f">父节点</param>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<DataResult<List<Department>>> GetTreeTable(long f = 0, string key = "")
        {
            List<Department> departments = new List<Department>();
            var departmentList = await _departmentServices.Query(d => d.IsDeleted == false);
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                key = "";
            }

            if (key != "")
            {
                departments = departmentList.Where(a => a.Name.Contains(key)).OrderBy(a => a.OrderSort).ToList();
            }
            else
            {
                departments = departmentList.Where(a => a.Pid == f).OrderBy(a => a.OrderSort).ToList();
            }

            foreach (var item in departments)
            {
                List<int> pidarr = new() { };
                var parent = departmentList.FirstOrDefault(d => d.Id == item.Pid);

                while (parent != null)
                {
                    pidarr.Add(parent.Id);
                    parent = departmentList.FirstOrDefault(d => d.Id == parent.Pid);
                }

                pidarr.Reverse();
                pidarr.Insert(0, 0);
                item.PidArr = pidarr;

                item.hasChildren = departmentList.Where(d => d.Pid == item.Id).Any();
            }


            return Success(departments, "获取成功");
        }

        /// <summary>
        /// 获取部门树
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DataResult<DepartmentTree>> GetDepartmentTree(int pid = 0)
        {
            var departments = await _departmentServices.Query(d => d.IsDeleted == false);
            var departmentTrees = (from child in departments
                                   where child.IsDeleted == false
                                   orderby child.Id
                                   select new DepartmentTree
                                   {
                                       value = child.Id,
                                       label = child.Name,
                                       Pid = child.Pid,
                                       order = child.OrderSort,
                                   }).ToList();
            DepartmentTree rootRoot = new DepartmentTree
            {
                value = 0,
                Pid = 0,
                label = "根节点"
            };

            departmentTrees = departmentTrees.OrderBy(d => d.order).ToList();


            RecursionHelper.LoopToAppendChildren(departmentTrees, rootRoot, pid);

            return Success(rootRoot, "获取成功");
        }

        [HttpPost]
        public async Task<MessageModel<string>> Post([FromBody] Department request)
        {
            var data = new MessageModel<string>();

            var id = await _departmentServices.Add(request);
            data.success = id > 0;
            if (data.success)
            {
                data.response = id.ObjToString();
                data.msg = "添加成功";
            }

            return data;
        }

        [HttpPut]
        public async Task<DataResult<string>> Put([FromBody] Department request)
        {
            var data = new DataResult<string>();
            data.Success = await _departmentServices.Update(request);
            if (data.Success)
            {
                data.Message = "更新成功";
                data.Attach = request?.Id.ObjToString();
            }

            return data;
        }

        [HttpDelete]
        public async Task<DataResult<string>> Delete(int id)
        {
            var data = new DataResult<string>();
            var model = await _departmentServices.QueryById(id);
            model.IsDeleted = true;
            data.Success = await _departmentServices.Update(model);
            if (data.Success)
            {
                data.Message = "删除成功";
                data.Attach = model?.Id.ObjToString();
            }
             

            return data;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<DataResult<string>> SaveData2Tsv()
        {
            var data = new DataResult<string>() { Success = true, Message = "" };
            if (_env.IsDevelopment())
            {

                JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
                };

                var rolesJson = JsonConvert.SerializeObject(await _departmentServices.Query(d => d.IsDeleted == false), microsoftDateFormatSettings);
                FileHelper.WriteFile(Path.Combine(_env.WebRootPath, "BlogCore.Data.json", "Department_New.tsv"), rolesJson, Encoding.UTF8);

                data.Success = true;
                data.Message = "生成成功！";
            }
            else
            {
                data.Success = false;
                data.Message = "当前不处于开发模式，代码生成不可用！";
            }

            return data;
        }
    }
}