using System;
using System.Threading.Tasks;
using PZIOT.Common;
using PZIOT.Common.DB;
using PZIOT.IRepository.Base;
using PZIOT.IServices;
using PZIOT.Model.Models;
using PZIOT.Services.BASE;

namespace PZIOT.Services
{
    public partial class PasswordLibServices : BaseServices<PasswordLib>, IPasswordLibServices
    {
        IBaseRepository<PasswordLib> _dal;

        public PasswordLibServices(IBaseRepository<PasswordLib> dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
        }

        [UseTran(Propagation = Propagation.Required)]
        public async Task<bool> TestTranPropagation2()
        {
            await _dal.Add(new PasswordLib()
            {
                IsDeleted = false,
                plAccountName = "aaa",
                plCreateTime = DateTime.Now
            });

            return true;
        }

        [UseTran(Propagation = Propagation.Mandatory)]
        public async Task<bool> TestTranPropagationNoTranError()
        {
            await _dal.Add(new PasswordLib()
            {
                IsDeleted = false,
                plAccountName = "aaa",
                plCreateTime = DateTime.Now
            });

            return true;
        }

        [UseTran(Propagation = Propagation.Nested)]
        public async Task<bool> TestTranPropagationTran2()
        {
            await _dal.Add(new PasswordLib()
            {
                IsDeleted = false,
                plAccountName = "aaa",
                plCreateTime = DateTime.Now
            });

            return true;
        }
    }
}