using AplicationLayer.Dtos.Account.Auth;
using AplicationLayer.Dtos.Account.Register;
using AplicationLayer.Dtos.Account;
using AplicationLayer.Interfaces.Service;
using DomainLayer.Enums;
using DomainLayer.Settings;
using InfrastructureLayer.Data;
using InfrastructureLayer.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IdentityContext _context;
        private readonly JWTSetting _jwtSettings;

        public AccountService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IdentityContext context,
            IOptions<JWTSetting> jwtSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<AuthResponse> AuthAsync(AuthRequest authRequest)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == authRequest.Email);
            if (user == null)
                return new AuthResponse { StatusCode = 404,};

            var result = await _signInManager.CheckPasswordSignInAsync(user, authRequest.Password, false);
            if (!result.Succeeded)
                return new AuthResponse { StatusCode = 401 };

            var roles = await _userManager.GetRolesAsync(user);

            // Generar JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName ?? ""),
                    new Claim(ClaimTypes.Email, user.Email ?? "")
                }.Concat(roles.Select(r => new Claim(ClaimTypes.Role, r)))),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthResponse
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName,
                Roles = roles.ToList(),
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                IsVerified = user.EmailConfirmed,
                StatusCode = 200,
                JwtToken = tokenHandler.WriteToken(token)
            };
        }

        public async Task LogOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<RegisterResponse> RegisterAccountAsync(RegisterRequest registerRequest, Roles roles)
        {
            var user = new User
            {
                UserName = registerRequest.Username,
                Email = registerRequest.Email,
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                PhoneNumber = registerRequest.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, registerRequest.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, roles.ToString());

            return new RegisterResponse
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email
            };
        }

        public Task<RegisterResponse> RegisterProfessorAsync(RegisterRequest registerRequest, Roles roles)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveAccountAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
                await _userManager.DeleteAsync(user);
        }

        public async Task<UpdateAccountDto> UpdateAccountDetailsAsync(UpdateAccountDto updateAccountDto, string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new Exception("Usuario no encontrado");

            user.FirstName = updateAccountDto.FirstName;
            user.LastName = updateAccountDto.LastName;
            user.UserName = updateAccountDto.Username;

            await _userManager.UpdateAsync(user);

            return new UpdateAccountDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName
            };
        }
    }
}
