using log4net.Util;
using PZIOT.IServices;
using PZIOT.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Services
{
    public class PZIOTServices : IPZIOTServices
    {
        public async Task<bool> StartPIOT()
        {
            bool result = await Task.Run(async () => {
                Console.WriteLine("加载IOT设备实例中");
                await Task.Delay(1000);
                Console.WriteLine("加载IOT内置服务中");
                await Task.Delay(1000);
                Console.WriteLine("移除IOT接口限制");
                ApiLock.IOTApiLock = true;
                Console.WriteLine("IOT启动成功");
                return true;
            });
            return result;
        }
    }
}
