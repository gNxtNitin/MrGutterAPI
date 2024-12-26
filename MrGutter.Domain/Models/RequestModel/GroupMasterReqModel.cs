using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrGutter.Domain.Models.RequestModel
{
    public class GroupMasterReqModel
    {
        public int GroupId { get; set; } = 0;
        public string? GroupName { get; set; }
        public string? Description { get; set; }
        public int CreatedBy { get; set; } = 0;
    }

}
