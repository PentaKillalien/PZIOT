using log4net.Util;
using PZIOT.Common;
using PZIOT.Common.EquipmentDriver;
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
                Console.WriteLine("创建TcpClientDriver示例");
                TcpClientDriver tcp = new TcpClientDriver();
                await tcp.CreatConnect(new Model.PZIOTModels.TcpClientConnectionModel() { 
                      Serverip="192.168.82.57",
                      Port=1987,
                      TimeOut=100
                });
                EquipmentReadResponseProtocol result = await tcp.ReadSingleParaFromEquipment("ggg");
                ConsoleHelper.WriteInfoLine($"发送ggg,接收到回复{result.ResponseValue}");
                ApiLock.IOTApiLock = true;
                Console.WriteLine("IOT启动成功");
                return true;
            });
            return result;
        }
    }
}
