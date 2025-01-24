using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrQuote.Domain.Models.RequestModel
{
    public class MeasurementCatReqModel
    {
        public string? Flag { get; set; }
        public int MCatID { get; set; } = 0;
        public int CompanyID { get; set; } = 1;
        public string? CategoryName { get; set; }
        public int OrderNo { get; set; }
        public bool IsActive { get; set; } = true;
        public int CreatedBy { get; set; } = 0;
    }
}
