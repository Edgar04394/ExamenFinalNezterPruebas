using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Configuration;
using ApiExamen.Services;
using ApiExamen.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Collections.Generic;
using ApiExamen.Interfaces;

namespace NezterBackend.Tests.Services
{
    public class AuthServiceTests : TestBase
    {
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUsuarioRepository = new Mock<IUsuarioRepository>();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"ConnectionStrings:DefaultConnection", "Server=localhost;Database=TestDB;Trusted_Connection=true;TrustServerCertificate=true"},
                    {"Jwt:Key", "test_jwt_key_for_testing_purposes_only"},
                    {"Jwt:Issuer", "test_issuer"},
                    {"Jwt:Audience", "test_audience"}
                })
                .Build();
            _authService = new AuthService(_mockUsuarioRepository.Object, configuration);
        }

        [Fact]
        public void AuthService_DeberiaCrearseCorrectamente()
        {
            // Act & Assert
            _authService.Should().NotBeNull();
            _authService.Should().BeOfType<AuthService>();
        }

        [Fact]
        public async Task Login_ConCredencialesValidas_DeberiaGenerarToken()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Usuario = "admin",
                Contrasena = "admin123"
            };
            var usuarioMock = new Usuario { usuario = "admin", contrasena = "admin123", rol = "Administrador" };
            _mockUsuarioRepository.Setup(x => x.ObtenerUsuarioPorCredenciales("admin", "admin123")).ReturnsAsync(usuarioMock);

            // Act
            var token = await _authService.Login(loginRequest);

            // Assert
            token.Should().NotBeNull();
        }

        [Fact]
        public async Task Login_ConCredencialesInvalidas_DeberiaRetornarNull()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Usuario = "noexiste",
                Contrasena = "incorrecta"
            };
            _mockUsuarioRepository.Setup(x => x.ObtenerUsuarioPorCredenciales("noexiste", "incorrecta")).ReturnsAsync((Usuario?)null);

            // Act
            var token = await _authService.Login(loginRequest);

            // Assert
            token.Should().BeNull();
        }

        [Fact]
        public async Task Login_ConUsuarioNulo_DeberiaRetornarNull()
        {
            // Arrange
            var loginRequest = new LoginRequest { Usuario = null, Contrasena = "algo" };
            _mockUsuarioRepository.Setup(x => x.ObtenerUsuarioPorCredenciales("", "algo")).ReturnsAsync((Usuario?)null);

            // Act
            var token = await _authService.Login(loginRequest);

            // Assert
            token.Should().BeNull();
        }

        [Fact]
        public async Task Login_ConContrasenaNula_DeberiaRetornarNull()
        {
            // Arrange
            var loginRequest = new LoginRequest { Usuario = "alguien", Contrasena = null };
            _mockUsuarioRepository.Setup(x => x.ObtenerUsuarioPorCredenciales("alguien", "")).ReturnsAsync((Usuario?)null);

            // Act
            var token = await _authService.Login(loginRequest);

            // Assert
            token.Should().BeNull();
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
            var usuarioMock = new Usuario { usuario = usuario, contrasena = contrasena, rol = "Administrador" };
            _mockUsuarioRepository.Setup(x => x.ObtenerUsuarioPorCredenciales(usuario, contrasena)).ReturnsAsync(usuarioMock);

            // Act
            var token = await _authService.Login(loginRequest);

            // Assert
            token.Should().NotBeNull();
        }
    }
} 