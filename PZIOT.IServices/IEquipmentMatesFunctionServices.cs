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
    public interface IEquipmentMatesFunctionServices : IBaseServices<EquipmentMatesFunction>
    {
        Task<EquipmentMatesFunction> QueryGlobeDic(Expression<Func<EquipmentMatesFunction, bool>> whereExpression, int mateid);
    }
}
