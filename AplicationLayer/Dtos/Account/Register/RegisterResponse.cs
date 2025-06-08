using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationLayer.Dtos.Account.Register
{
    public class RegisterResponse
    {
        public string? UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
    }
}
