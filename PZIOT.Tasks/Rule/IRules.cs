using PZIOT.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Tasks.Rule
{
    /// <summary>
    /// 规则接口
    /// </summary>
    public interface IRules
    {
        /// <summary>
        /// 执行规则
        /// </summary>
        /// <returns></returns>
        Task<bool> ExecuteRule(EquipmentDataScada usedata);
        /// <summary>
        /// 执行String规则
        /// </summary>
        /// <returns></returns>
        Task<bool> ExecuteRule(EquipmentDataScada usedata, EquipmentMatesTriggerString rule);
    }
}
