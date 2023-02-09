using PZIOT.IServices.BASE;
using PZIOT.Model.Models;
using PZIOT.Model.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PZIOT.IServices
{
    public interface IEquipmentServices :IBaseServices<Equipment>
    {
        Task<List<Equipment>> GetEquipments();
        Task<Equipment> GetEquipmentDetails(int id);

    }

}
