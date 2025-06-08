using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationLayer.Dtos.Account.Auth
{
    public class AuthRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
