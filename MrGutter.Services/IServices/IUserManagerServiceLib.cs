using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MrQuote.Domain.Models.RequestModel;
using MrQuote.Domain.Models;

namespace MrQuote.Services.IServices
{
    public interface IUserManagerServiceLib
    {
        public Task<ResponseModel> GetGroupMaster(string? encReq);
        public Task<ResponseModel> CreateOrSetGroupMaster(RequestModel rq,char flag);
        public Task<ResponseModel> GetRoleMaster(string? encReq);
        public Task<ResponseModel> GetRoleByUserId(string encReq);
        public Task<ResponseModel> CreateOrSetRoleMaster(RequestModel rq, char flag);
        public Task<ResponseModel> GetUsers(string? encReq);
        public Task<ResponseModel> CreateOrSetUser(RequestModel rq, char flag);
        public Task<ResponseModel> CreateLogHistory(RequestModel req);
        public Task<ResponseModel> GetLogHistory();
    }

}
