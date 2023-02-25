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
    public class EquipmentMatesTriggerIntServices : BaseServices<EquipmentMatesTriggerInt>, IEquipmentMatesTriggerIntServices
    {
        public async Task<List<EquipmentMatesTriggerInt>> QueryGlobeDic(Expression<Func<EquipmentMatesTriggerInt, bool>> whereExpression, int mateid)
        {
            if (!PZIOTEquipmentManager.MateTriggerIntDic.ContainsKey(mateid))
            {
                var list = await BaseDal.Query(whereExpression);
                if (list.Any())
                {
                    PZIOTEquipmentManager.MateTriggerIntDic.Add(mateid, list);
                }
                else { 
                    return null;
                }

            }
            var result = PZIOTEquipmentManager.MateTriggerIntDic[mateid];
            return result;
        }
    }
}
