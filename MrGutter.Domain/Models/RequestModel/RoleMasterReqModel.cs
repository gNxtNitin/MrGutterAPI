using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrGutter.Domain.Models.RequestModel
{
    public class RoleMasterReqModel
    {
        public int RoleId { get; set; } = 0;
        public string? RoleName { get; set; }
        public string? Description { get; set; }
        public int CreatedBy { get; set; } = 0;
    }
}
