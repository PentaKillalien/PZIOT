using PZIOT.Common;
using PZIOT.Common.Helper;
using PZIOT.IRepository.Base;
using PZIOT.IServices;
using PZIOT.Model;
using PZIOT.Model.Models;
using PZIOT.Model.ViewModels;
using PZIOT.Services.BASE;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PZIOT.Repository.UnitOfWorks;

namespace PZIOT.Services
{
    /// <summary>
	/// WeChatCompanyServices
	/// </summary>
    public class WeChatCompanyServices : BaseServices<WeChatCompany>, IWeChatCompanyServices
    {
        readonly IUnitOfWorkManage _unitOfWorkManage;
        readonly ILogger<WeChatCompanyServices> _logger;
        public WeChatCompanyServices(IUnitOfWorkManage unitOfWorkManage, ILogger<WeChatCompanyServices> logger)
        {
            this._unitOfWorkManage = unitOfWorkManage;
            this._logger = logger;
        }  
        
    }
}