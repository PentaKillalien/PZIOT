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
    public class EquipmentMatesFunctionServices:BaseServices<EquipmentMatesFunction>, IEquipmentMatesFunctionServices
    {
        public async Task<EquipmentMatesFunction> QueryGlobeDic(Expression<Func<EquipmentMatesFunction, bool>> whereExpression, int mateid)
        {
            if (!PZIOTEquipmentManager.MateFunctionDic.ContainsKey(mateid))
            {
                var list = await BaseDal.Query(whereExpression);
                if (list.Any()) {
                    PZIOTEquipmentManager.MateFunctionDic.Add(mateid, list[0]);
                }
                    
            }
            var result = PZIOTEquipmentManager.MateFunctionDic[mateid];
            return result;
        }
    }
}
