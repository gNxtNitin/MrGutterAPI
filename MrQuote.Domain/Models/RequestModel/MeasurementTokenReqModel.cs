using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrQuote.Domain.Models.RequestModel
{
    public class MeasurementTokenReqModel
    {
        public string? Flag { get; set; }
        public int CompanyID { get; set; } = 0;
        public int EstimateID { get; set; } = 0;
        public int MTokenID { get; set; } = 0;
        public int MCatID { get; set; } = 0;
        public int UmID { get; set; } = 1;
        public string? TokenName { get; set; }
        public string? TokenValue { get; set; }
        public int OrderNo { get; set; }
        public bool IsActive { get; set; } = true;
        public int CreatedBy { get; set; } = 0;
    }
}
