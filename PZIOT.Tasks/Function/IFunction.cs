using PZIOT.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Tasks.Function
{
    /// <summary>
    /// 正对于double值所建立的比较方法,比如运算报警 两个值大于多少报警
    /// </summary>
    public interface IFunction
    {
        /// <summary>
        /// 执行function
        /// </summary>
        /// <returns></returns>
        Task<bool> ExecuteRule(double Oldvalue,double NewValue);
    }
}
