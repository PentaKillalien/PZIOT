using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Common.EquipmentDriver
{
    public class ModbusRtuOverTcpClient : IEquipmentDriver
    {
        public bool IsConnected => throw new NotImplementedException();

        public Task<bool> CreatConnect(object t)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DisConnect()
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetConnectionState()
        {
            throw new NotImplementedException();
        }

        public Task<List<EquipmentReadResponseProtocol>> RequestMultipleParasFromEquipment(List<string> readparas)
        {
            throw new NotImplementedException();
        }

        public Task<EquipmentReadResponseProtocol> RequestSingleParaFromEquipment(string para)
        {
            throw new NotImplementedException();
        }
    }
}
