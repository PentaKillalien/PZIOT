using Newtonsoft.Json;
using PZIOT.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Tasks.Rule
{
    /// <summary>
    /// 数据上传到WebApi
    /// </summary>
    public class UploadDataToTheWebapi : IRules
    {
        public Task<bool> ExecuteRule(EquipmentDataScada usedata)
        {
            Console.WriteLine($"执行UploadDataToTheWebapi推送触发器......内容{JsonConvert.SerializeObject(usedata)}");
            return Task.FromResult(true);
        }

        public Task<bool> ExecuteRule(EquipmentDataScada usedata, EquipmentMatesTriggerString rule)
        {
            throw new NotImplementedException();
        }
    }
}
