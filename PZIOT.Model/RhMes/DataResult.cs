using System;
using System.Collections.Generic;

namespace PZIOT.Model.RhMes
{
    /// <summary>
    /// 数据结果
    /// </summary>
    public class DataResult : DataResult<object>
    {
        #region structure
        public DataResult()
        { }
        protected DataResult(bool success, string message) : base(success, message)
        {

        }

        protected DataResult(bool success, string message, int record) : base(success, message, record)
        {

        }
        protected DataResult(bool success, string okMessage, string failMessage) : base(success, okMessage, failMessage)
        {

        }
        protected DataResult(bool success, string okMessage, string failMessage, int record) : base(success, okMessage, failMessage)
        {
            this.Record = record;
        }
        #endregion

        #region Create
        /// <summary>
        /// 创建操作结果实例
        /// </summary>
        /// <param name="success"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static DataResult Create(bool success, string message)
        {

            return new DataResult(success, message);
        }
        /// <summary>
        /// 创建操作结果实例
        /// </summary>
        /// <param name="success"></param>
        /// <param name="message"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        public static DataResult Create(bool success, string message, int record)
        {
            return new DataResult(success, message, record);
        }
        /// <summary>
        /// 创建操作结果实例
        /// </summary>
        /// <param name="success"></param>
        /// <param name="okMessage"></param>
        /// <param name="failMessage"></param>
        /// <returns></returns>
        public static DataResult Create(bool success, string okMessage, string failMessage)
        {
            return new DataResult(success, okMessage, failMessage);
        }
        public static DataResult Create(bool success, string okMessage, string failMessage, int record)
        {
            return new DataResult(success, okMessage, failMessage, record);
        }
        #endregion
    }

    /// <summary>
    /// 操作结果
    /// </summary>
    public class DataResult<T>
    {
        /// <summary>
        /// 操作记录数
        /// </summary>
        public int Record { get; set; }

        public int Status { get; set; } 

        public string Message { get; set; }

        public bool Success { get; set; }

        public int? TotalCount { get; set; }

        /// <summary>
        /// 跳过数量
        /// </summary>
        public int SkipCount { get; set; }


        public T Attach { get; set; }

        /// <summary>
        /// 数据表头字段类别
        /// </summary>
        public List<DataFieldInfo> DataHeadFields { get; set; }
        public DataResult()
        { }
        internal DataResult(bool success, string message)
        {
            this.Success = success;
            this.Message = message.AsProjectMessage();
        }
        internal DataResult(bool success, string message, int record)
        {
            this.Success = success;
            this.Message = message.AsProjectMessage();
            this.Record = record;
        }
        internal DataResult(bool success, string message, T attach)
        {
            this.Success = success;
            this.Message = message.AsProjectMessage();
            this.Attach = attach;
        }
        internal DataResult(int status, bool success, string message, T attach)
        {
            this.Status = status;
            this.Success = success;
            this.Message = message.AsProjectMessage();
            this.Attach = attach;
        }
        internal DataResult(bool success, string message, int record, T attach)
        {
            this.Success = success;
            this.Message = message.AsProjectMessage();
            this.Attach = attach;
            this.Record = record;
        }
        internal DataResult(bool success, string message, int record, int? totalCount, T attach)
        {
            this.Success = success;
            this.Message = message.AsProjectMessage();
            this.Attach = attach;
            this.Record = record;
            this.TotalCount = totalCount;
        }
        internal DataResult(bool success, string message, int record, int? totalCount, T attach, int skipCount)
        {
            this.Success = success;
            this.Message = message.AsProjectMessage();
            this.Attach = attach;
            this.Record = record;
            this.TotalCount = totalCount;
            this.SkipCount = skipCount;
        }
        internal DataResult(bool success, string okMessage, string failMessage)
        {
            this.Success = success;
            this.Message = success ? okMessage.AsProjectMessage() : failMessage.AsProjectMessage();
        }
        internal DataResult(bool success, string okMessage, string failMessage, T attach)
        {
            this.Success = success;
            this.Message = success ? okMessage.AsProjectMessage() : failMessage.AsProjectMessage();
            this.Attach = attach;
        }
    }

    /// <summary>
    /// 扩展
    /// </summary>
    public static class DataResultExtension
    {

        /// <summary>
        /// 将泛型数据对象转换为数据结果类型，包含异常信息
        /// 考虑泛型对象为集合对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static DataResult<List<T>> AsDataListResult<T>(this List<T> datas, string exception = null)
        {
            if (datas == null || datas.Count == 0)
            {
                if (exception == null)
                    return new DataResult<List<T>>(false, "未获取到相关数据!", 0, 0, datas);
                else
                    return new DataResult<List<T>>(false, exception, 0, 0, datas);
            }
            return new DataResult<List<T>>(true, $"获取数据成功！", datas.Count, datas.Count, datas);
        }

        /// <summary>
        /// 将泛型分页数据对象转换为分页数据结果类型，包含异常信息
        /// 考虑泛型对象为集合对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas"></param>
        /// <param name="totalCount"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static DataResult<List<T>> AsPageDataListResult<T>(this List<T> datas, int totalCount, string exception = null)
        {
            if (datas == null || datas.Count == 0)
            {
                if (exception == null)
                    return new DataResult<List<T>>(false, "未获取到相关数据!", 0, totalCount, datas);
                else
                    return new DataResult<List<T>>(false, exception, 0, 0, datas);
            }
            return new DataResult<List<T>>(true, $"获取数据成功！", datas.Count, totalCount, datas);
        }

        /// <summary>
        /// 将泛型数据对象转换为数据结果类型，包含异常信息
        /// 为单个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static DataResult<T> AsDataSingleResult<T>(this T data, string exception = null)
        {
            if (data == null)
            {
                if (exception == null)
                    return new DataResult<T>(false, "未获取到相关数据!", 0);
                else
                    return new DataResult<T>(false, exception, 0);
            }
            return new DataResult<T>(true, $"获取数据成功！", 1, data);
        }
        /// <summary>
        /// 将泛型数据对象转换为失败类型的数据结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataResult<T> AsFailDResult<T>(this string msg, T data = default)
        {
            return new DataResult<T>(false, msg, 0, data);
        }

        /// <summary>
        /// 将泛型数据对象转换为异常类型的数据结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ex"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataResult<T> AsExceptionDResult<T>(this Exception ex, T data = default)
        {
            return new DataResult<T>(false, ex.Message, 0, data);
        }

        /// <summary>
        /// 转换为数据结果类型
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="success"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static DataResult<T> AsDataSingleResult<T>(this T data, bool success, string message)
        {
            var m = success == true ? data : default;
            return new DataResult<T>(success, message, 1, m);
        }

        /// <summary>
        /// 从数据结果中获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataResult"></param>
        /// <returns></returns>
        public static T FetchData<T>(this DataResult<T> dataResult)
        {
            if (dataResult.Success)
                return dataResult.Attach;
            return default;
        }

        /// <summary>
        /// 从数据结果中获取数据集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataResult"></param>
        /// <param name="isInit"></param>
        /// <returns></returns>
        public static List<T> FetchData<T>(this DataResult<List<T>> dataResult, bool isInit = false)
        {
            List<T> result;
            if (dataResult.Success)
            {
                result = dataResult.Attach;
            }
            else
            {
                result = null;
            }
            if (isInit)
            {
                result = result ?? new List<T>();
            }
            return result;
        }

        /// <summary>
        /// 设置表头字段数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataResult"></param>
        /// <param name="fieldInfos"></param>
        /// <returns></returns>
        public static DataResult<List<T>> SetDataHeadFields<T>(this DataResult<List<T>> dataResult, List<DataFieldInfo> fieldInfos)
        {
            dataResult.DataHeadFields = fieldInfos;
            return dataResult;
        }

        /// <summary>
        /// 设置表头字段数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataResult"></param>
        /// <param name="fieldInfos"></param>
        /// <returns></returns>
        public static DataResult<T> SetDataHeadFields<T>(this DataResult<T> dataResult, List<DataFieldInfo> fieldInfos)
        {
            dataResult.DataHeadFields = fieldInfos;
            return dataResult;
        }

        /// <summary>
        /// 将数据Dto转为操作Dto
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataResult"></param>
        /// <returns></returns>
        public static OpResult ToOpResult<T>(this DataResult<T> dataResult)
        {
            return OpResult.Create(dataResult.Success, dataResult.Message);
        }

        /// <summary>
        /// 将数据Dto转为异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataResult"></param>
        /// <returns></returns>
        public static Exception ToException<T>(this DataResult<T> dataResult)
        {
            return new Exception(dataResult.Message);
        }
    }


}
