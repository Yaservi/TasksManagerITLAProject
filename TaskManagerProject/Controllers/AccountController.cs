using AplicationLayer.Dtos.Account;
using AplicationLayer.Dtos.Account.Auth;
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
        [Authorize(Roles = "Professor")]
        public async Task<ActionResult<RegisterResponse>> RegisterProfessor([FromBody] RegisterRequest request, [FromQuery] Roles role)
        {

            var result = await _accountService.RegisterAccountAsync(request, Roles.Professor);
            return Ok(result);
        }

        [HttpPost("Register Students")]
        [Authorize(Roles = "Professor")]
        public async Task<ActionResult<RegisterResponse>> RegisterStudent([FromBody] RegisterRequest request, [FromQuery] Roles role)
        {

            var result = await _accountService.RegisterAccountAsync(request, Roles.Student);
            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogOutAsync();
            return NoContent();
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<ActionResult<UpdateAccountDto>> Update(string id, [FromBody] UpdateAccountDto dto)
        {
            var result = await _accountService.UpdateAccountDetailsAsync(dto, id);
            return Ok(result);
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = "Professor")] // Ajusta el rol según tu lógica
        public async Task<IActionResult> Delete(string userId)
        {
            await _accountService.RemoveAccountAsync(userId);
            return NoContent();
        }
    }
}
