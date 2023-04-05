using PZIOT.IServices.BASE;
using PZIOT.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.IServices
{
    public interface IEquipmentMatesTriggerIntServices : IBaseServices<EquipmentMatesTriggerInt>
    {
        Task<List<EquipmentMatesTriggerInt>> QueryGlobeDic(Expression<Func<EquipmentMatesTriggerInt, bool>> whereExpression, int mateid);
    }
}
