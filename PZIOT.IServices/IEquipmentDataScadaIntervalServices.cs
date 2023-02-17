using PZIOT.IServices.BASE;
using PZIOT.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.IServices
{
    public interface IEquipmentDataScadaIntervalServices : IBaseServices<EquipmentDataScadaInterval>
    {
        /// <summary>
        /// 获取最后一条数据的索引值
        /// </summary>
        /// <returns></returns>
        Task<int> GetMaxIndex(string mateName);
    }
}
