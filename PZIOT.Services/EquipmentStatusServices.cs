using PZIOT.IServices;
using PZIOT.Model.Models;
using PZIOT.Services.BASE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Services
{
    /// <summary>
    /// 设备状态数据库访问层
    /// </summary>
    public class EquipmentStatusServices: BaseServices<EquipmentStatus>,IEquipmentStatusServices
    {
    }
}
