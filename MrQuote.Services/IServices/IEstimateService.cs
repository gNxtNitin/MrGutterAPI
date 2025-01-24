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
        Task<ResponseModel> CreateOrSetMeasurementCat(MeasurementCatReqModel req);
        Task<ResponseModel> CreateOrSetMeasurementToken(MeasurementTokenReqModel req);
        Task<ResponseModel> GetEstimate(EstimateQueryParameters req);
        Task<ResponseModel> GetStatus(string? statusId);
        Task<ResponseModel> GetMeasurementCat(int mCatId, int companyId);
        Task<ResponseModel> UnitOfMeasurement(int uMId, int companyId);
        Task<ResponseModel> GetMeasurementToken(int estimateId, int companyId, int mTokenId);
    }
}
