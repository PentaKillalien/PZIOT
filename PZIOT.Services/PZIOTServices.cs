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
        /// <summary>
        /// 需创建一些IOT的静态管理类来托载IOT驱动和其它业务运行的内容
        /// 待建立全局的功能模块
        /// </summary>
        /// <returns></returns>
        public async Task<bool> StartPIOT()
        {
            ApiLock.IOTApiLock = true;
            ConsoleHelper.WriteSuccessLine("IOT服务启动成功");
            bool result = await Task.Run(async () => {
                Console.WriteLine("创建TcpClientDriver测试示例中");
                TcpClientDriver tcp = new TcpClientDriver();
                await tcp.CreatConnect(new Model.PZIOTModels.TcpClientConnectionModel() { 
                      Serverip="192.168.82.57",
                      Port=1987,
                      TimeOut=100
                });
                EquipmentReadResponseProtocol result = await tcp.RequestSingleParaFromEquipment("ggg");
                ConsoleHelper.WriteInfoLine($"发送ggg,接收到回复{result.ResponseValue}");
                
                return true;
            });
            return result;
        }
    }
}
