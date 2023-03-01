using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Model.RhMes
{
    
    /// <summary>
    /// 结果基类
    /// </summary>
    public abstract class ResultBase<T>
    {
        
        /// <summary>
        /// 结果信息
        /// </summary>
        public virtual string Message { get; set; }
      
        /// <summary>
        /// 结果成功与否标志
        /// </summary>
        public virtual bool Success { get; set; }
       
        /// <summary>
        /// 附加对象，万能传值对象
        /// </summary>
        public T Attach { get; set; }

        protected ResultBase() { }

        protected ResultBase(bool success, string message)
        {
            this.Success = success;
            this.Message = message.AsProjectMessage();
        }

        protected ResultBase(bool success, string message, T attatc)
        {
            this.Success = success;
            this.Message = message.AsProjectMessage();
            this.Attach = attatc;
        }
    }
}
