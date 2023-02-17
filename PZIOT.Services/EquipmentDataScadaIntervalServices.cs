using PZIOT.IServices;
using PZIOT.Model.Models;
using PZIOT.Services.BASE;
using System;
using System.Threading.Tasks;

namespace PZIOT.Services
{
    public class EquipmentDataScadaIntervalServices : BaseServices<EquipmentDataScadaInterval>, IEquipmentDataScadaIntervalServices
    {
        /// <summary>
        /// 获取最大主键
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetMaxIndex(string mateName)
        {
            try
            {
                var temp = await BaseDal.QuerySql($"select * FROM equipmentdatascadainterval where equipmentdatascadainterval.equipmentdataitemname = '{mateName}' ORDER BY id DESC LIMIT 1;");
                return temp[0].Id;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;   
            }
            
        }
    }
}
