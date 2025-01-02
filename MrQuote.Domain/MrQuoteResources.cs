using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrQuote.Domain
{
    public class MrQuoteResources
    {
        public static IConfiguration? configuration { get; set; }
        public static string GetConnectionString(string? name = null)
        {
            string constrName = name == null ? "ConnStr" : name;
            return configuration.GetConnectionString(constrName);
        }
        public static string GetEncryptionKey(string? name = null)
        {
            string constrName = name == null ? "encryptionKey" : name;
            return configuration.GetSection(constrName).ToString();
        }
    }

}
