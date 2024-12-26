using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrGutter.Domain.Models.RequestModel
{
    public class LogHistoryReqModel
    {
        public int UserId { get; set; } = 0;
        public string? Actions { get; set; }
        public string? Details { get; set; }
    }
}
