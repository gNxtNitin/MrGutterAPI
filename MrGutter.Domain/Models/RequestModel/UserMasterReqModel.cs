using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrGutter.Domain.Models.RequestModel
{
    public class UserMasterReqModel
    {
        public int UserId { get; set; } = 0;
        //public int GroupId { get; set; } = 0;
        public int RoleId { get; set; } = 2;
        // public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MobileNo { get; set; }
        public string? EmailID { get; set; }
        public string? DOB { get; set; }
        public string? Password { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PinCode { get; set; }
        public string? PanCardNo { get; set; }
        public string? FilePath { get; set; }
        public bool IsActive { get; set; } = true;
        public int CreatedBy { get; set; } = 0;
    }

}
