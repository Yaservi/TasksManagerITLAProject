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
using DomainLayer.Dto;
using AplicationLayer.Dtos.Account.Password.Forgot;
using AplicationLayer.Dtos.Account.Password.Reset;
using AplicationLayer.Dtos.Account.JWT;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using Azure.Core;
using AplicationLayer.Dtos.Email;

namespace InfrastructureLayer.Services
{
    public class AccountService(UserManager<User> userManager,
            SignInManager<User> signInManager,
            IdentityContext context,
            IOptions<JWTSetting> jwtSettings, IEmailService emailSender) : IAccountService
    {
       
        private readonly JWTSetting _jwtSettings = jwtSettings.Value; 
        private string RandomTokenString()
        {
            using var rng = RandomNumberGenerator.Create();
            var randomBytes = new Byte[40];
            rng.GetBytes(randomBytes);
            return BitConverter.ToString(randomBytes);
        }
        private RefreshToken GenerateRefreshToken()
        {
            return new RefreshToken
            {
                Token = RandomTokenString(),
                Expired = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow
            };
        }

        private async Task<JwtSecurityToken> GenerateTokenAsync(User user)
        {
            var userClaims = await userManager.GetClaimsAsync(user);
            var roles = await userManager.GetRolesAsync(user);

            List<Claim> rolesClaims = new List<Claim>();

            foreach (var role in roles)
            {
                rolesClaims.Add(new Claim("roles", role));
            }

            var claim = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("Id", user.Id)
            }
                .Union(userClaims)
                .Union(rolesClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken
            (
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claim,
                expires: DateTime.Now.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials
            );
            return jwtSecurityToken;
        }

        private async Task<string> SendVerificationEmailUrlAsync(User user)
        {
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            return code;
        }
        private async Task<string> SendForgotPasswordAsync(User user)
        {
            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            return code;
        }




        public async Task<AuthResponse> AuthAsync(AuthRequest authRequest)
        {
            AuthResponse response = new();

            var user = await userManager.FindByEmailAsync(authRequest.Email);
            if (user == null)
            {
                response.StatusCode = 404;
                return response;
            }
            var result = await signInManager.PasswordSignInAsync(user.UserName, authRequest.Password, false, false);

            if (!result.Succeeded)
            {
                response.StatusCode = 401;
                return response;
            }

            if (!user.EmailConfirmed)
            {
                response.StatusCode = 400;
                return response;
            }

            JwtSecurityToken jwtSecurityToken = await GenerateTokenAsync(user);

            response.UserId = user.Id;
            response.Username = user.UserName;
            response.FirstName = user.FirstName;
            response.LastName = user.LastName;
            response.Email = user.Email;

            var rolesList = await userManager.GetRolesAsync(user).ConfigureAwait(false);

            response.Roles = rolesList.ToList();
            response.IsVerified = user.EmailConfirmed;
            response.PhoneNumber = user.PhoneNumber;
            response.JwtToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            var refreshToken = GenerateRefreshToken();
            response.RefreshToken = refreshToken.Token;

            return response;
        }

        public async Task<Response<string>> ConfirmAccountAsync(string userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Response<string>.ErrorResponse($"No account registered with this {userId} user id");
            }

            try
            {
                token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            }
            catch
            {
                return Response<string>.ErrorResponse("Invalid token format.");
            }

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Response<string>.SuccessResponse("Your account has been successfully confirmed!");
            }
            else
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return Response<string>.ErrorResponse($"An error occurred trying to confirm your account: {errors}");
            }
        }

        public async Task<Response<ForgotResponse>> GetForgotPasswordAsync(ForgotRequest forgotRequest)
        {
            ForgotResponse response = new();

            var account = await userManager.FindByEmailAsync(forgotRequest.Email);

            if (account == null)
            {
                return Response<ForgotResponse>.ErrorResponse("No accounts registered with this email");
            }

            var verification = await SendForgotPasswordAsync(account);
            var resetLink = $"http://localhost:4200/reset-password?email={Uri.EscapeDataString(forgotRequest.Email)}&token={Uri.EscapeDataString(verification)}";

            await emailSender.SendAsync(new EmailRequestDto
            {
                To = forgotRequest.Email,
                Body = $"Para restablecer tu contraseña, haz clic en el siguiente enlace: {resetLink}",
                Subject = "Recuperación de contraseña"
            });


            response.Message = "Se envió el email, chequea tu inbox";
           
            return Response<ForgotResponse>.SuccessResponse(response);
        }


        public async Task LogOutAsync()
        {
            await signInManager.SignOutAsync();
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

            var result = await userManager.CreateAsync(user, registerRequest.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

            await userManager.AddToRoleAsync(user, roles.ToString());
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            await emailSender.SendAsync(new EmailRequestDto
            {
                To = user.Email,
                Subject = "Verifica tu cuenta",
                Body = $"Tu código de verificación es: {code}"
            });

            return new RegisterResponse
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email
            };
        }

        public async Task RemoveAccountAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
                await userManager.DeleteAsync(user);
        }

        public async Task<Response<ResetPasswordResponse>> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest)
        {

            ResetPasswordResponse response = new();
            var account = await userManager.FindByEmailAsync(resetPasswordRequest.Email);

            if (account == null)
            {
                return Response<ResetPasswordResponse>.ErrorResponse("No accounts registered with this email");
            }

            resetPasswordRequest.Token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetPasswordRequest.Token));

            var result = await userManager.ResetPasswordAsync(account, resetPasswordRequest.Token, resetPasswordRequest.Password);

            if (!result.Succeeded)
            {
                return Response<ResetPasswordResponse>.ErrorResponse("An Error has occured trying to reset your password");
            }
            response.Message = "Your password has been reset";

            return Response<ResetPasswordResponse>.SuccessResponse(response);
        }

        public async Task<UpdateAccountDto> UpdateAccountDetailsAsync(UpdateAccountDto updateAccountDto, string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                throw new Exception("Usuario no encontrado");

            user.FirstName = updateAccountDto.FirstName;
            user.LastName = updateAccountDto.LastName;
            user.UserName = updateAccountDto.Username;

            await userManager.UpdateAsync(user);

            return new UpdateAccountDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName
            };
        }
    }
}
