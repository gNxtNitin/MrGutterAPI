﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrQuote.Domain.Models.RequestModel
{
    public class LoginReqModel
    {
        public string? MobileOrEmail { get; set; }
        public string? Password { get; set; }
        public string? VerificationCode { get; set; }
        public bool IsResendOTP { get; set; } = false;
    }
}
