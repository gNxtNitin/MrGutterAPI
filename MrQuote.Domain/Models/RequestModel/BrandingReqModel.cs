using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrQuote.Domain.Models.RequestModel
{
    public class BrandingReqModel
    {
        public int CompanyId { get; set; } = 0;
        public string? AccountName { get; set; }
        public string? CompanyEmail { get; set; }
        public string? CompanyPhone { get; set; }
        public string? BusinessNumber { get; set; }
        public string? WebAddress { get; set; }
        public string? CompanyLogoPath { get; set; }
        public string? AreaTitle { get; set; }
        public string? AreaName1 { get; set; }
        public string? AreaName2 { get; set; }
        public string? Primary { get; set; }
        public string? Secondary { get; set; }
        public List<ThemeModel>? ThemeList { get; set; }
        public int CreatedBy { get; set; } = 0;
    }

    public class ThemeModel
    {
        public int CompanyId { get; set; } = 1;
        public string? ThemeId { get; set; }
        public string? ThemePath { get; set; }
        public bool? IsActive { get; set; }
    }
}
