using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestWebApplication1.DTOs
{
    public class RegisterRequest
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}