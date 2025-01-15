using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrQuote.Domain.Models.RequestModel
{
    public class LayoutReqModel
    {
        public string? Flag { get; set; }
        public int LayoutID { get; set; } = 0;
        public string? LayoutName { get; set; }
        public bool IsShared { get; set; }
        public int CompanyID { get; set; }
        public int CreatedBy { get; set; }
    }
}
