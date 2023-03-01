using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Model.RhMes
{
    /// <summary>
    /// 接口请求返回结果
    /// </summary>
    public class ReqResult<T>
    {
        /// <summary>
        /// 结果信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 异常类型
        /// </summary>
        public string ErrorType { get; set; }
        /// <summary>
        /// 结果成功与否标志
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 附加对象，万能传值对象
        /// </summary>
        public T Attach { get; set; }

        /// <summary>
        /// 请求上下文
        /// </summary>
        public ReqContext Context { get; set; }
    }

    /// <summary>
    /// 请求上下文
    /// </summary>
    public class ReqContext
    {
        /// <summary>
        /// 调用Url
        /// </summary>
        public string ApiUrl { get; set; }

        /// <summary>
        /// 调用方法名称
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// 输入的参数
        /// </summary>
        public string InputPara { get; set; }

        /// <summary>
        /// Http方法
        /// </summary>
        public string HttpMethod { get; set; }

        /// <summary>
        /// Http状态码
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// 请求头
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public static class ReqResultExtensions
    {
        /// <summary>
        /// 从数据结果中获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataResult"></param>
        /// <returns></returns>
        public static T FetchData<T>(this ReqResult<T> dataResult)
        {
            if (dataResult.Success)
                return dataResult.Attach;
            return default;
        }

        /// <summary>
        /// 转回正常的操作结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reqResult"></param>
        /// <param name="externalSystem"></param>
        /// <returns></returns>
        public static OpResult AsSuccessOpResult<T>(this ReqResult<T> reqResult, string externalSystem)
        {
            var result = OpResult.Create(reqResult.Success, $"invoke the external  interface from  {externalSystem} success!");
            if (reqResult.Attach != null)
                result.Attach = reqResult.Attach;
            return result;
        }

        /// <summary>
        /// 转回异常的操作结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reqResult"></param>
        /// <param name="externalSystem"></param>
        /// <returns></returns>
        public static OpResult AsFailOpResult<T>(this ReqResult<T> reqResult, string externalSystem)
        {
            var result = OpResult.Create(reqResult.Success, $"来自外部{externalSystem}系统的异常，异常类型:{reqResult.ErrorType}");
            if (reqResult.Attach != null)
                result.Attach = reqResult.Attach;
            return result;
        }

        /// <summary>
        /// 转回异常的数据结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reqResult"></param>
        /// <param name="externalSystem"></param>
        /// <returns></returns>
        public static DataResult<T> AsFailDataResult<T>(this ReqResult<T> reqResult, string externalSystem)
        {
            DataResult<T> dataResult = new DataResult<T>(reqResult.Success, $"来自外部{externalSystem}系统的异常，异常类型:{reqResult.ErrorType}");
            if (reqResult.Attach != null)
            {
                dataResult.Attach = reqResult.Attach;
            }
            return dataResult;
        }

        /// <summary>
        /// 转回正常的数据结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reqResult"></param>
        /// <param name="externalSystem"></param>
        /// <returns></returns>
        public static DataResult<T> AsSuccessDataResult<T>(this ReqResult<T> reqResult, string externalSystem)
        {
            DataResult<T> dataResult = new DataResult<T>(reqResult.Success, $"invoke the external  interface from  {externalSystem} success!");
            if (reqResult.Attach != null)
            {
                dataResult.Attach = reqResult.Attach;
            }
            return dataResult;
        }
    }
}
