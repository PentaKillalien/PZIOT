using PZIOT.Common;
using PZIOT.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Tasks.Rule
{
    /// <summary>
    /// 对比数据错误
    /// </summary>
    public class CompareDataError : IRules
    {
        public Task<bool> ExecuteRule(EquipmentDataScada usedata)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExecuteRule(EquipmentDataScada usedata, EquipmentMatesTriggerString rule)
        {
            if (rule.CompareValue.Equals(usedata.EquipmentDataItemValue))
            {
                ConsoleHelper.WriteSuccessLine("数据对比成功");
            }
            else {
                ConsoleHelper.WriteErrorLine("数据对比失败");
            }
            return Task.FromResult(true);
        }
    }
}
