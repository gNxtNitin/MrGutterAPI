using MrQuote.Domain.Models;
using MrQuote.Domain.Models.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrQuote.Services.IServices
{
    public interface IEstimateService
    {
        Task<ResponseModel> CreateOrSetEstimate(EstimateReqModel req);
        Task<ResponseModel> GetEstimate(EstimateQueryParameters req);
        Task<ResponseModel> GetStatus(string? statusId);
    }
}
