using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AplicationLayer.Dtos.Email;

namespace AplicationLayer.Interfaces.Service
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequestDto emailRequestDto);

    }
}
