using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ApiExamen.Controllers;
using ApiExamen.Services;
using ApiExamen.Models;
using ApiExamen.Interfaces;

namespace NezterBackend.Tests.Controllers
{
    public class AuthControllerTests : TestBase
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _authController = new AuthController(_mockAuthService.Object, _mockConfiguration.Object);
        }

        [Fact]
        public void AuthController_DeberiaCrearseCorrectamente()
        {
            // Act & Assert
            _authController.Should().NotBeNull();
            _authController.Should().BeOfType<AuthController>();
        }

        [Fact]
        public async Task Login_ConCredencialesValidas_DeberiaRetornarOk()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Usuario = "admin",
                Contrasena = "admin123"
            };

            var expectedToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
            _mockAuthService.Setup(x => x.Login(loginRequest))
                .ReturnsAsync(expectedToken);

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
            
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().NotBeNull();
            okResult.Value!.GetType().GetProperty("token")!.GetValue(okResult.Value).Should().Be(expectedToken);
        }

        [Fact]
        public async Task Login_ConCredencialesInvalidas_DeberiaRetornarUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Usuario = "usuario_inexistente",
                Contrasena = "contrasena_incorrecta"
            };

            _mockAuthService.Setup(x => x.Login(loginRequest))
                .ReturnsAsync((string?)null);

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<UnauthorizedObjectResult>();
            
            var unauthorizedResult = result as UnauthorizedObjectResult;
            unauthorizedResult!.Value.Should().Be("Credenciales incorrectas");
        }

        [Fact]
        public async Task Login_ConUsuarioNulo_DeberiaRetornarUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Usuario = null,
                Contrasena = "contrasena"
            };

            _mockAuthService.Setup(x => x.Login(loginRequest))
                .ReturnsAsync((string?)null);

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Login_ConContrasenaNula_DeberiaRetornarUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Usuario = "usuario",
                Contrasena = null
            };

            _mockAuthService.Setup(x => x.Login(loginRequest))
                .ReturnsAsync((string?)null);

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Login_ConExcepcionEnServicio_DeberiaManejarError()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Usuario = "admin",
                Contrasena = "admin123"
            };

            _mockAuthService.Setup(x => x.Login(loginRequest))
                .ThrowsAsync(new Exception("Error de base de datos"));

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>();
            
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(500);
            objectResult.Value.Should().NotBeNull();
        }

        [Theory]
        [InlineData("admin", "admin123")]
        [InlineData("empleado1", "emp123")]
        [InlineData("testuser", "testpass")]
        public async Task Login_ConDiferentesUsuarios_DeberiaManejarCorrectamente(string usuario, string contrasena)
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Usuario = usuario,
                Contrasena = contrasena
            };

            var expectedToken = $"token_for_{usuario}";
            _mockAuthService.Setup(x => x.Login(loginRequest))
                .ReturnsAsync(expectedToken);

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
            
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().NotBeNull();
        }

        [Fact]
        public void LoginRequest_DeberiaTenerPropiedadesCorrectas()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Usuario = "testuser",
                Contrasena = "testpass"
            };

            // Act & Assert
            loginRequest.Should().NotBeNull();
            loginRequest.Usuario.Should().Be("testuser");
            loginRequest.Contrasena.Should().Be("testpass");
        }

        [Fact]
        public void LoginRequest_ConPropiedadesNulas_DeberiaSerValido()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Usuario = null,
                Contrasena = null
            };

            // Act & Assert
            loginRequest.Should().NotBeNull();
            loginRequest.Usuario.Should().BeNull();
            loginRequest.Contrasena.Should().BeNull();
        }
    }
} 