using Newtonsoft.Json;
using PZIOT.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Tasks.Rule
{
    public class DefaultConsoleOk : IRules
    {
        public Task<bool> ExecuteRule(EquipmentDataScada usedata)
        {
            Console.WriteLine($"执行触发器默认触发方法....内容{JsonConvert.SerializeObject(usedata)}");
           return Task.FromResult(true);
        }
    }
}
