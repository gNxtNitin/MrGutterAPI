using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MrGutter.Domain.Models.RequestModel;
using MrGutter.Domain.Models;

namespace MrGutter.Services.IServices
{
    public interface IUserManager
    {
        public Task<ResponseModel> GetGroupMaster(string? encReq);
        public Task<ResponseModel> CreateOrSetGroupMaster(RequestModel rq,char flag);
        public Task<ResponseModel> GetRoleMaster(string? encReq);
        public Task<ResponseModel> GetRoleByUserId(string encReq);
        public Task<ResponseModel> CreateOrSetRoleMaster(RequestModel rq, char flag);
        public Task<ResponseModel> GetUsers(string? encReq);
        public Task<ResponseModel> CreateOrSetUser(UserMasterReqModel rq);
        public Task<ResponseModel> CreateLogHistory(RequestModel req);
        public Task<ResponseModel> GetLogHistory();
    }

}
