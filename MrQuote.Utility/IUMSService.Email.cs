using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrQuote.Utility
{
    public interface IUMSService
    {
        public Task QueueEmail(string toEmailIds, string subject, string body, string token, int isHTML = 1);
    }
}
