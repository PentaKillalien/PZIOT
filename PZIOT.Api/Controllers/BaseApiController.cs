using PZIOT.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using PZIOT.Model.RhMes;
using PZIOT.Common.WebApiClients.HttpApis;

namespace PZIOT.Controllers
{
    /// <summary>
    /// Api的标准返回结构体的封装
    /// </summary>
    public class BaseApiController : Controller
    {
        
        [NonAction]
        public DataResult<T> Success<T>(T data, string msg = "成功")
        {
            return new DataResult<T>()
            {
                Success = true,
                Message = msg,
                Attach = data
            };
        }
        // [NonAction]
        //public MessageModel<T> Success<T>(T data, string msg = "成功",bool success = true)
        //{
        //    return new MessageModel<T>()
        //    {
        //        success = success,
        //        msg = msg,
        //        response = data,
        //    };
        //}
        [NonAction]
        public DataResult Success(string msg = "成功")
        {
            return new DataResult()
            {
                Success = true,
                Message = msg,
                Attach = null
            };
        }
        [NonAction]
        public DataResult<string> Failed(string msg = "失败", int status = 500)
        {
            return new DataResult<string>()
            {
                Success = true,
                Message = msg,
                Attach = null,
                Status= status
            };
        }
        [NonAction]
        public DataResult<T> Failed<T>(string msg = "失败", int status = 500)
        {
            return new DataResult<T>()
            {
                Success = true,
                Message = msg,
                Status = status
            };
        }
        [NonAction]
        public DataResult<PageModel<T>> SuccessPage<T>(int page, int dataCount, int pageSize, List<T> data, int pageCount, string msg = "获取成功")
        {

            return new DataResult<PageModel<T>>()
            {
                Success = true,
                Message = msg,
                Attach = new PageModel<T>(page, dataCount, pageSize, data)

            };
        }
        [NonAction]
        public DataResult<PageModel<T>> SuccessPage<T>(PageModel<T> pageModel, string msg = "获取成功")
        {

            return new DataResult<PageModel<T>>()
            {
                Success = true,
                Message = msg,
                Attach = pageModel
            };
        }
    }
}
