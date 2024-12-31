using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrGutter.Domain.Models.RequestModel
{
    public class CompanyReqModel
    {
        public int CompanyID { get; set; }
        public string? Flag { get; set; }
        public string? CompanyName { get; set; }
        public string? PointOfContact { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;
        public int CreatedBy { get; set; } = 0;
    }
}
