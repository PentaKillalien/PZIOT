using System;
using System.Collections.Generic;

namespace PZIOT.Model.RhMes
{
    public class OpResult : OpResult<object>
    {
        public OpResult()
        { }

        protected OpResult(bool success, string message) : base(success, message)
        {

        }

        protected OpResult(bool success, string message, int record) : base(success, message, record)
        {

        }
        protected OpResult(bool success, string okMessage, string failMessage) : base(success, okMessage, failMessage)
        {

        }
        protected OpResult(bool success, string okMessage, string failMessage, int record) : base(success, okMessage, failMessage)
        {
            this.Record = record;
        }
        /// <summary>
        /// 创建操作结果默认的成功实例
        /// </summary>
        /// <returns></returns>
        public static OpResult Default(object attach = null)
        {
            return new OpResult(true, "ok") { Attach = attach };
        }

        /// <summary>
        /// 创建操作结果默认的失败实例
        /// </summary>
        /// <param name="failMsg"></param>
        /// <returns></returns>
        public static OpResult Failure(string failMsg)
        {
            return new OpResult(false, failMsg);
        }
        /// <summary>
        /// 创建操作结果实例
        /// </summary>
        /// <param name="success"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static OpResult Create(bool success, string message)
        {
            return new OpResult(success, message);
        }
        /// <summary>
        /// 创建操作结果实例
        /// </summary>
        /// <param name="success"></param>
        /// <param name="message"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        public static OpResult Create(bool success, string message, int record)
        {
            return new OpResult(success, message, record);
        }
        /// <summary>
        /// 创建操作结果实例
        /// </summary>
        /// <param name="success"></param>
        /// <param name="okMessage"></param>
        /// <param name="failMessage"></param>
        /// <returns></returns>
        public static OpResult Create(bool success, string okMessage, string failMessage)
        {
            return new OpResult(success, okMessage, failMessage);
        }
        /// <summary>
        /// 创建操作结果实例
        /// </summary>
        /// <param name="success"></param>
        /// <param name="okMessage"></param>
        /// <param name="failMessage"></param>
        /// <param name="attach"></param>
        /// <returns></returns>
        public static OpResult Create(bool success, string okMessage, string failMessage, object attach)
        {
            return new OpResult(success, okMessage, failMessage) { Attach = attach };
        }
        public static OpResult Create(bool success, string okMessage, string failMessage, int record)
        {
            return new OpResult(success, okMessage, failMessage);
        }
    }

    /// <summary>
    /// 操作结果
    /// </summary>
    public class OpResult<T> : ResultBase<T>
    {
        /// <summary>
        /// 操作记录数
        /// </summary>
        public int Record { get; set; }
        public OpResult()
        { }
        internal OpResult(bool success, string message) : base(success, message)
        {

        }
        internal OpResult(bool success, string message, int record) : base(success, message)
        {
            this.Record = record;
        }
        internal OpResult(bool success, string message, T attach) : base(success, message, attach)
        {

        }
        internal OpResult(bool success, string message, int record, T attach) : base(success, message, attach)
        {
            this.Record = record;
        }
        internal OpResult(bool success, string okMessage, string failMessage)
        {
            this.Success = success;
            this.Message = success ? okMessage : failMessage;
        }
        internal OpResult(bool success, string okMessage, string failMessage, T attach)
        {
            this.Success = success;
            this.Message = success ? okMessage : failMessage;
            this.Attach = attach;
        }
    }

    public static class OpResultExtension
    {
        public static OpResult AsStoreResult(this bool success, int record)
        {
            return OpResult.Create(success, "存储数据成功！", "存储数据失败！", record);
        }
        public static OpResult AsStoreResult(this bool success, string message)
        {
            return OpResult.Create(success, message);
        }
        public static OpResult ToStoreResult<T>(this OpResult<T> result, string message = null)
        {
            string msg = string.Empty;
            if (message == null)
            {
                msg = result.Message;
            }
            else
            {
                if (!result.Success)
                {
                    msg = result.Message;
                }
                else
                {
                    msg = message;
                }
            }
            OpResult op = OpResult.Create(result.Success, msg, result.Record);
            op.Attach = result.Attach;
            return op;
        }
        public static OpResult ToOpResult<T>(this OpResult<T> result, string okMsg)
        {
            string msg = result.Message;
            if (result.Success)
            {
                msg = okMsg;
            }
            OpResult op = OpResult.Create(result.Success, msg, result.Record);
            op.Attach = result.Attach;
            return op;
        }
        public static OpResult<T> AsStoreResult<T>(this T attach, bool success, string msg, int record)
        {
            return new OpResult<T>(success, msg, record, attach);
        }
        public static OpResult<T> AsOpResult<T>(this T attach, bool success, string msg)
        {
            return new OpResult<T>(success, msg, attach);
        }
        public static OpResult<T> AsOpResult<T>(this T attach, bool success, string okMsg, string failMsg)
        {
            return new OpResult<T>(success, okMsg, failMsg, attach);
        }
        public static OpResult<T> AsOpResult<T>(this T attach, bool success)
        {
            return new OpResult<T>(success, "操作成功！", "操作失败！", attach);
        }
        public static OpResult<T> AsOpResult<T>(this T entity)
        {
            bool success = entity != null;
            string msg = success ? "操作成功！" : "操作失败！";
            return new OpResult<T>(success, msg, entity);
        }
        public static OpResult<T> AsExceptionOpResult<T>(this Exception ex, T data = default)
        {
            return new OpResult<T>(false, ex.Message, 0, data);
        }
        public static DataResult<T> ToDataResult<T>(this OpResult op, T data)
        {
            return new DataResult<T>(op.Success, op.Message, op.Record, data);
        }

        public static OpResult ToOpResult<T>(this OpResult<T> op)
        {
            return OpResult.Create(op.Success, op.Message, op.Message, op.Attach);
        }

        /// <summary>
        /// 转为目标类型对象的数据结果类型
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="dataResult"></param>
        /// <param name="ooMaper"></param>
        /// <returns></returns>
        public static OpResult<List<TDestination>> MapOpResult<TSource, TDestination>(this OpResult<List<TSource>> dataResult, Func<TSource, TDestination> ooMaper)
            where TSource : class, new()
            where TDestination : class, new()
        {
            List<TDestination> destination = new List<TDestination>();
            var datas = dataResult.Attach;
            if (datas == null || datas.Count == 0)
                return destination.AsOpResult();
            datas.ForEach(m =>
            {
                TDestination d = ooMaper(m);
                destination.Add(d);
            });
            return destination.AsOpResult();
        }
        public static OpResult AsOpResult(this bool success)
        {
            return OpResult.Create(success, "操作成功!", "操作失败!");
        }
        /// <summary>
        /// 检测集合列表，返回检测操作结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static OpResult CheckedToOpResult<T>(this List<T> datas)
        {
            if (datas == null || datas.Count == 0)
                return OpResult.Create(false, "yg_error:集合的值为null或者为空！");
            return OpResult.Create(true, "yg_info:OK!");
        }
        /// <summary>
        /// 返回操作异常的结果信息
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static OpResult AsExceptionOpResult(this Exception ex)
        {
            return OpResult.Create(false, ex.Message);
        }
        /// <summary>
        /// 处理列表数据，返回操作结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static OpResult HandleListData<T>(this List<T> datas, Func<T, OpResult> handler)
        {
            if (datas == null) return OpResult.Create(false, "传入的集合数据不能为null");
            var result = OpResult.Default();
            for (int i = 0; i < datas.Count; i++)
            {
                result = handler(datas[i]);
                if (!result.Success)
                    break;
            }
            return result;
        }

        /// <summary>
        /// 将数据Dto转为异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="opResult"></param>
        /// <returns></returns>
        public static Exception ToException<T>(this OpResult<T> opResult)
        {
            return new Exception(opResult.Message);
        }

        public static Exception ToException(this OpResult opResult)
        {
            return new Exception(opResult.Message);
        }
    }
    /// <summary>
    /// 存储消息类别
    /// </summary>
    public enum StoreMsgType
    {
        Insert,
        Update,
        Delete
    }
}
