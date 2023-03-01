using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PZIOT.Common.EquipmentDriver
{
    public class BaseClientDriver:IEquipmentDriver
    {
        virtual public bool IsConnected => throw new NotImplementedException();

        virtual public Task<bool> CreatConnect(object t)
        {
            throw new NotImplementedException();
        }

        virtual public Task<bool> DisConnect()
        {
            throw new NotImplementedException();
        }

        virtual public Task<bool> GetConnectionState()
        {
            throw new NotImplementedException();
        }

        public async Task<List<EquipmentReadResponseProtocol>> RequestMultipleParasFromEquipment(List<string> readparas)
        {
            List<EquipmentReadResponseProtocol> listinfos = new List<EquipmentReadResponseProtocol>();
            foreach (string parasItem in readparas)
            {
                EquipmentReadResponseProtocol temp = await RequestSingleParaFromEquipment(parasItem);
                listinfos.Add(temp);
            }
            return listinfos;
        }

        virtual public Task<EquipmentReadResponseProtocol> RequestSingleParaFromEquipment(string para)
        {
            throw new NotImplementedException();
        }
    }
}
