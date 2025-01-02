using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrQuote.Services.IServices
{
    public interface IJwtAuthService
    {
        public Task<string> GenerateJwtToken(string name, string role);
        //public Task<string> GenerateJwtToken();
    }
}
