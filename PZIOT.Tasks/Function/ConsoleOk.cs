using PZIOT.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Tasks.Function
{
    /// <summary>
    /// 控制台输出OK的Function
    /// </summary>
    public class ConsoleOk : IFunction
    {
        public Task<bool> ExecuteRule(double Oldvalue, double NewValue)
        {
            ConsoleHelper.WriteSuccessLine($"进行减法运算,默认方法执行...差值{NewValue-Oldvalue}");
            return Task.FromResult(false);
        }
    }
}
