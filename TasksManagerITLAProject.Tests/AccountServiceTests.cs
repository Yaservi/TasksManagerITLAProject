using Xunit;
using Moq;
using AplicationLayer.Dtos.Account.Auth;
using AplicationLayer.Dtos.Account.Register;
using AplicationLayer.Interfaces.Service;
using DomainLayer.Enums;
using AplicationLayer.Dtos.Account.Password.Forgot;
using AplicationLayer.Dtos.Account.Password.Reset;
using System.Threading.Tasks;
using AplicationLayer.Dtos.Account;
using DomainLayer.Dto;

namespace TasksManagerITLAProject.Tests
{
    public class AccountServiceTests
    {
        [Fact]
        public async Task RegisterWithValidDataReturnsUserId()
        {
            var mockService = new Mock<IAccountService>();
            var request = new RegisterRequest { Email = "test@mail.com", Password = "Password123!" };
            var expectedResponse = new RegisterResponse { UserId = "123", Email = "test@mail.com" };

            mockService.Setup(s => s.RegisterAccountAsync(request, Roles.Professor))
                .ReturnsAsync(expectedResponse);

            var result = await mockService.Object.RegisterAccountAsync(request, Roles.Professor);

            Assert.NotNull(result.UserId);
            Assert.Equal("test@mail.com", result.Email);
        }

        [Fact]
        public async Task RegisterStudentWithValidDataReturnsUserId()
        {
            var mockService = new Mock<IAccountService>();
            var request = new RegisterRequest { Email = "student@mail.com", Password = "Password123!" };
            var expectedResponse = new RegisterResponse { UserId = "456", Email = "student@mail.com" };

            mockService.Setup(s => s.RegisterAccountAsync(request, Roles.Student))
                .ReturnsAsync(expectedResponse);

            var result = await mockService.Object.RegisterAccountAsync(request, Roles.Student);

            Assert.NotNull(result.UserId);
            Assert.Equal("student@mail.com", result.Email);
        }

        [Fact]
        public async Task RegisterWithExistingEmailThrowsException()
        {
            var mockService = new Mock<IAccountService>();
            var request = new RegisterRequest { Email = "existing@mail.com", Password = "Password123!" };

            mockService.Setup(s => s.RegisterAccountAsync(request, Roles.Professor))
                .ThrowsAsync(new System.Exception("Email ya existe"));

            await Assert.ThrowsAsync<System.Exception>(() =>
                mockService.Object.RegisterAccountAsync(request, Roles.Professor));
        }

        [Fact]
        public async Task AuthWithCorrectCredentialsReturnsJwtToken()
        {
            var mockService = new Mock<IAccountService>();
            var request = new AuthRequest { Email = "test@mail.com", Password = "Password123!" };
            var expectedResponse = new AuthResponse { JwtToken = "jwt_token" };

            mockService.Setup(s => s.AuthAsync(request))
                .ReturnsAsync(expectedResponse);

            var result = await mockService.Object.AuthAsync(request);

            Assert.NotNull(result.JwtToken);
        }

        [Fact]
        public async Task AuthWithIncorrectCredentialsReturnsNullToken()
        {
            var mockService = new Mock<IAccountService>();
            var request = new AuthRequest { Email = "wrong@mail.com", Password = "wrongpass" };
            var expectedResponse = new AuthResponse { JwtToken = null, StatusCode = 401 };

            mockService.Setup(s => s.AuthAsync(request))
                .ReturnsAsync(expectedResponse);

            var result = await mockService.Object.AuthAsync(request);

            Assert.Null(result.JwtToken);
            Assert.Equal(401, result.StatusCode);
        }

      

        [Fact]
        public async Task ConfirmAccountWithInvalidTokenReturnsError()
        {
            var mockService = new Mock<IAccountService>();
            var userId = "123";
            var token = "invalidtoken";
            var expectedResponse = Response<string>.ErrorResponse("Invalid token");

            mockService.Setup(s => s.ConfirmAccountAsync(userId, token))
                .ReturnsAsync(expectedResponse);

            var result = await mockService.Object.ConfirmAccountAsync(userId, token);

            Assert.False(result.Successful);
            Assert.Equal("Invalid token", result.Message);
        }

        [Fact]
        public async Task ForgotPasswordWithValidEmailSendsToken()
        {
            var mockService = new Mock<IAccountService>();
            var request = new ForgotRequest { Email = "test@mail.com" };
            var expectedResponse = Response<ForgotResponse>.SuccessResponse(new ForgotResponse { Message = "Email enviado" });

            mockService.Setup(s => s.GetForgotPasswordAsync(request))
                .ReturnsAsync(expectedResponse);

            var result = await mockService.Object.GetForgotPasswordAsync(request);

            Assert.True(result.Successful);
            Assert.Equal("Email enviado", result.SingleData.Message);
        }

        [Fact]
        public async Task ForgotPasswordWithInvalidEmailReturnsError()
        {
            var mockService = new Mock<IAccountService>();
            var request = new ForgotRequest { Email = "notfound@mail.com" };
            var expectedResponse = Response<ForgotResponse>.ErrorResponse("No cuentas registradas con este email");

            mockService.Setup(s => s.GetForgotPasswordAsync(request))
                .ReturnsAsync(expectedResponse);

            var result = await mockService.Object.GetForgotPasswordAsync(request);

            Assert.False(result.Successful);
            Assert.Equal("No cuentas registradas con este email", result.Message);
        }

        [Fact]
        public async Task ResetPasswordWithValidTokenReturnsSuccess()
        {
            var mockService = new Mock<IAccountService>();
            var request = new ResetPasswordRequest { Email = "waos@mail.com", Token = "validtoken", Password = "NewPassword123!" };
            var expectedResponse = Response<ResetPasswordResponse>.SuccessResponse(new ResetPasswordResponse { Message = "Tu contraseña ha sido reseteada" });

            mockService.Setup(s => s.ResetPasswordAsync(request))
                .ReturnsAsync(expectedResponse);

            var result = await mockService.Object.ResetPasswordAsync(request);

            Assert.True(result.Successful);
            Assert.Equal("Tu contraseña ha sido reseteada", result.SingleData.Message);
        }

        [Fact]
        public async Task LogoutExecutesSuccessfully()
        {
            var mockService = new Mock<IAccountService>();
            mockService.Setup(s => s.LogOutAsync()).Returns(Task.CompletedTask);

            await mockService.Object.LogOutAsync();
            mockService.Verify(s => s.LogOutAsync(), Times.Once);
        }

    }
}
