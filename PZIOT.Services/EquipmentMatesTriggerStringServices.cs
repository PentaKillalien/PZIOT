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
                    PZIOTEquipmentManager.MateTriggerStringDic.Add(mateid, list[0]);
                }

            }
            var result = PZIOTEquipmentManager.MateTriggerStringDic[mateid];
            return result;
        }
    }
}
