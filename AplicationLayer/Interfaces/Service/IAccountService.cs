using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AplicationLayer.Dtos.Account;
using AplicationLayer.Dtos.Account.Auth;
using AplicationLayer.Dtos.Account.Register;
using DomainLayer.Enums;

namespace AplicationLayer.Interfaces.Service
{
    public interface IAccountService
    {
        Task<AuthResponse> AuthAsync(AuthRequest authRequest);
        Task<UpdateAccountDto> UpdateAccountDetailsAsync(UpdateAccountDto updateAccountDto, string id);
        Task<RegisterResponse> RegisterAccountAsync(RegisterRequest registerRequest, Roles roles);
        Task<RegisterResponse> RegisterProfessorAsync(RegisterRequest registerRequest, Roles roles);
        Task LogOutAsync();
        Task RemoveAccountAsync(string userId);
    }
}
