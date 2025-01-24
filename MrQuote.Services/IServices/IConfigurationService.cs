using MrQuote.Domain.Models;
using MrQuote.Domain.Models.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrQuote.Services.IServices
{
    public interface IConfigurationService
    {
        Task<ResponseModel> CreateOrSetBranding(BrandingReqModel req);
        Task<ResponseModel> GetBrandings(int companyId);
        Task<ResponseModel> GetCompanyTheme(int companyId);
    }
}
