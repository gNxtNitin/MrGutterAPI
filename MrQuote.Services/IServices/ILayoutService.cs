using MrQuote.Domain.Models;
using MrQuote.Domain.Models.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrQuote.Services.IServices
{
    public interface ILayoutService
    {
        Task<ResponseModel> CreateOrSetLayout(LayoutReqModel req);
    }
}
