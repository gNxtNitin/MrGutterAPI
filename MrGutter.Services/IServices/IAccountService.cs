using System;
using MrGutter.Domain.Models;
using MrGutter.Domain.Models.RequestModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrGutter.Services.IServices
{
    public interface IAccountService
    {
        public Task<ResponseModel> AuthenticateUser(LoginReqModel rq);
        public Task<ResponseModel> SetOTP(RequestModel rq);
        public Task<ResponseModel> ValidateOTP(RequestModel req);
        public Task<ResponseModel> SendForgotPasswordEmail(RequestModel req, string? userType);
        public Task<ResponseModel> ResetPassword(RequestModel req);
    }

}
