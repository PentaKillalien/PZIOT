using PZIOT.Common;
using PZIOT.IServices;
using PZIOT.Model.Models;
using PZIOT.Services.BASE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Services
{
    public class EquipmentMatesTriggerStringServices : BaseServices<EquipmentMatesTriggerString>, IEquipmentMatesTriggerStringServices
    {
        public async Task<EquipmentMatesTriggerString> QueryGlobeDic(Expression<Func<EquipmentMatesTriggerString, bool>> whereExpression, int mateid)
        {
            if (!PZIOTEquipmentManager.MateTriggerStringDic.ContainsKey(mateid))
            {
                var list = await BaseDal.Query(whereExpression);
                if (list.Any())
                {
                    if (list.Count > 1)
                    {
                        ConsoleHelper.WriteWarningLine($"Alarm:触发器为与数据项为一对一规则,Mate->{mateid}发现多个触发器，请即时处理!");
                        return null;
                    }
                    PZIOTEquipmentManager.MateTriggerStringDic.Add(mateid, list[0]);
                }

            }
            var result = PZIOTEquipmentManager.MateTriggerStringDic[mateid];
            return result;
        }
    }
}
