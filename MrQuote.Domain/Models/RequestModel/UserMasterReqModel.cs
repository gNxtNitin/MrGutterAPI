using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MrQuote.Domain.Models.RequestModel
{
    public class UserMasterReqModel
    {
        public string UserId { get; set; }
        public List<UserCompany>? CompanyList { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyEmail { get; set; }
        public string? CompanyPhone { get; set; }
        public string? ContactPerson { get; set; }
        public List<UserRole>? RoleList { get; set; } 
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public DateTime? DOB { get; set; }
        public string? Password { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? Flag { get; set; }
        public string? State { get; set; }
        public string? PinCode { get; set; }
        public string? UserType { get; set; }
        public string? UserStatus { get; set; }
        public string? PanCardNo { get; set; }
        public string? FilePath { get; set; }
        public bool isActive { get; set; } = true;
        public int CreatedBy { get; set; } = 0;
    }
    public class UserRole
    {
        public int UserId { get; set; } = 0;
        public int RoleId { get; set; } = 0;
        public string? RoleName { get; set; } 
        public bool IsActive { get; set; }=false;
    }

    public class UserCompany
    {
        public int UserId { get; set; } = 0;
        public int CompanyId { get; set; } = 0;
        public string? CompanyName { get; set; }
        public bool IsActive { get; set; } = false;
    }
}
