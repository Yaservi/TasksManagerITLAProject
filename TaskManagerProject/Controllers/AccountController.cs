using AplicationLayer.Dtos.Account;
using AplicationLayer.Dtos.Account.Auth;
using AplicationLayer.Dtos.Account.Password.Forgot;
using AplicationLayer.Dtos.Account.Password.Reset;
using AplicationLayer.Dtos.Account.Register;
using AplicationLayer.Interfaces.Service;
using DomainLayer.Enums;
using InfrastructureLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] AuthRequest request)
        {
            var result = await _accountService.AuthAsync(request);
            if (result.StatusCode != 200)
                return Unauthorized(result);
            return Ok(result);
        }

        [HttpPost("Register Professor")]
        public async Task<ActionResult<RegisterResponse>> RegisterProfessor([FromBody] RegisterRequest request)
        {
            var response = await _accountService.RegisterAccountAsync(request, Roles.Professor);
            return Ok(response);
        }

        [HttpPost("Register Students")]
        public async Task<ActionResult<RegisterResponse>> RegisterStudent([FromBody] RegisterRequest request)
        {
            var response = await _accountService.RegisterAccountAsync(request, Roles.Student);
            return Ok(response);
        }

        

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<ActionResult<UpdateAccountDto>> Update(string id, [FromBody] UpdateAccountDto dto)
        {
            var result = await _accountService.UpdateAccountDetailsAsync(dto, id);
            return Ok(result);
        }


        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotRequest request)
        {
            var result = await _accountService.GetForgotPasswordAsync(request);
            if (!result.Successful)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _accountService.ResetPasswordAsync(request);
            if (!result.Successful)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("confirm-account")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmAccount([FromQuery] string userId, [FromQuery] string token)
        {
            var result = await _accountService.ConfirmAccountAsync(userId, token);
            if (!result.Successful)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogOutAsync();
            return NoContent();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(string userId)
        {
            await _accountService.RemoveAccountAsync(userId);
            return NoContent();
        }

    }
}
