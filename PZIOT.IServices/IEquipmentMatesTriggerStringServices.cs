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
    public interface IEquipmentMatesTriggerStringServices : IBaseServices<EquipmentMatesTriggerString>
    {
        Task<EquipmentMatesTriggerString> QueryGlobeDic(Expression<Func<EquipmentMatesTriggerString, bool>> whereExpression, int mateid);
    }
}
