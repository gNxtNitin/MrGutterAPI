using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MrQuote.Domain.Models.RequestModel;
using MrQuote.Domain.Models;

namespace MrQuote.Services.IServices
{
    public interface IUserManager
    {
        public Task<ResponseModel> GetGroupMaster(string? encReq);
        public Task<ResponseModel> CreateOrSetGroupMaster(RequestModel rq,char flag);
        public Task<ResponseModel> GetRoleMaster(string? encReq);
        public Task<ResponseModel> GetRoleByUserId(string encReq);
        public Task<ResponseModel> CreateOrSetRoleMaster(RequestModel rq, char flag);
        public Task<ResponseModel> GetUsers(string? userId);
        public Task<ResponseModel> CreateOrSetUser(UserMasterReqModel rq);
        public Task<ResponseModel> GetCompany(string? userId, string? companyId);
        public Task<ResponseModel> CreateOrSetCompany(CompanyReqModel rq);
        public Task<ResponseModel> CreateLogHistory(RequestModel req);
        public Task<ResponseModel> GetLogHistory();
    }

}
