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
    public interface IEquipmentMatesServices : IBaseServices<EquipmentMates>
    {
         Task<List<EquipmentMates>> QueryGlobeDic(Expression<Func<EquipmentMates, bool>> whereExpression, int eqpid);
    }
}
