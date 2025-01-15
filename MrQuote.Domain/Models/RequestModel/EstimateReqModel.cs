using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrQuote.Domain.Models.RequestModel
{
    public class EstimateReqModel
    {
        public string? Flag { get; set; }
        public int EstimateID { get; set; } = 0;
        public int StatusID { get; set; } = 0;
        public int UserID { get; set; } = 0;    // Estimator ID
        public string? EstimateNo { get; set; }
        public string? EstimateCreatedDate { get; set; }
        public string? EstimateRevenue { get; set; }
        public string? NextCallDate { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Company { get; set; }
        public string? Email { get; set; }
        public string? PhoneNo { get; set; }
        public string? Addressline1 { get; set; }
        public string? Addressline2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public bool IsShared { get; set; }
        public int CompanyID { get; set; }
        public int CreatedBy { get; set; }
    }

    public class EstimateQueryParameters
    {
        public string? Flag { get; set; }
        public int UserId { get; set; }=0;
        public int CompanyID { get; set; } = 0;
        public int EstimateID { get; set; } = 0;
        //public string? Search { get; set; } // For keyword search
        //public string? FilterBy { get; set; } // Example: "Status", "Category"
        //public string? FilterValue { get; set; } // Example: "Active", "Electronics"
        //public string? SortBy { get; set; } // Example: "Name", "CreatedDate"
        //public bool SortDescending { get; set; } = false; // Sort direction
        //public int PageNumber { get; set; } = 1; // Pagination: Page number
        //public int PageSize { get; set; } = 10; // Pagination: Page size
    }
}
