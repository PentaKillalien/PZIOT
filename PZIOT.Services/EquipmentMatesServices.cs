using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
    public class EquipmentMatesServices:BaseServices<EquipmentMates>, IEquipmentMatesServices
    {
        public async Task<List<EquipmentMates>> QueryGlobeDic(Expression<Func<EquipmentMates, bool>> whereExpression,int eqpid)
        {
            if (!PZIOTEquipmentManager.EquipmentMatesDic.ContainsKey(eqpid))
            {
                var list = await BaseDal.Query(whereExpression);
                if (list.Any())
                {
                    PZIOTEquipmentManager.EquipmentMatesDic.Add(eqpid, new List<EquipmentMates>());
                    list.ForEach(t => PZIOTEquipmentManager.EquipmentMatesDic[eqpid].Add(t));
                }
                else {
                    return null;
                }
                
            }
            return PZIOTEquipmentManager.EquipmentMatesDic[eqpid];
        }
    }
}
