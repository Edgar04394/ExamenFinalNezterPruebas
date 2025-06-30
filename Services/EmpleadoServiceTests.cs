using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Configuration;
using ApiExamen.Services;
using ApiExamen.Models;
using ApiExamen.Interfaces;

namespace NezterBackend.Tests.Services
{
    public class EmpleadoServiceTests : TestBase
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IEmpleadoService> _mockEmpleadoService;
        private readonly EmpleadoService _empleadoService;

        public EmpleadoServiceTests()
        {
            // Crear una configuración real para las pruebas
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"ConnectionStrings:DefaultConnection", "Server=localhost;Database=TestDB;Trusted_Connection=true;TrustServerCertificate=true"}
                })
                .Build();
            
            _empleadoService = new EmpleadoService(configuration);
        }

        private void SetupMockConfiguration()
        {
            _mockConfiguration.Setup(x => x.GetConnectionString("DefaultConnection"))
                .Returns("Server=EDGARLEAL060403;Database=ProyectoExamen;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True");
        }

        [Fact]
        public void EmpleadoService_DeberiaCrearseCorrectamente()
        {
            // Act & Assert
            _empleadoService.Should().NotBeNull();
            _empleadoService.Should().BeOfType<EmpleadoService>();
        }

        [Fact]
        public void Empleado_DeberiaTenerPropiedadesValidas()
        {
            // Arrange
            var empleado = new Empleado
            {
                codigoEmpleado = 1,
                nombre = "Juan",
                apellidoPaterno = "Pérez",
                apellidoMaterno = "García",
                fechaNacimiento = new DateTime(1990, 1, 1),
                fechaInicioContrato = new DateTime(2023, 1, 1),
                idPuesto = 1
            };

            // Act & Assert
            empleado.Should().NotBeNull();
            empleado.codigoEmpleado.Should().Be(1);
            empleado.nombre.Should().Be("Juan");
            empleado.apellidoPaterno.Should().Be("Pérez");
            empleado.apellidoMaterno.Should().Be("García");
            empleado.fechaNacimiento.Should().Be(new DateTime(1990, 1, 1));
            empleado.fechaInicioContrato.Should().Be(new DateTime(2023, 1, 1));
            empleado.idPuesto.Should().Be(1);
        }

        [Fact]
        public void EmpleadoUsuarioDTO_DeberiaTenerPropiedadesValidas()
        {
            // Arrange
            var dto = new EmpleadoUsuarioDTO
            {
                Nombre = "María",
                ApellidoPaterno = "López",
                ApellidoMaterno = "Martínez",
                FechaNacimiento = new DateTime(1985, 5, 15),
                FechaInicioContrato = new DateTime(2022, 6, 1),
                IdPuesto = 2,
                Usuario = "maria.lopez",
                Contrasena = "maria123"
            };

            // Act & Assert
            dto.Should().NotBeNull();
            dto.Nombre.Should().Be("María");
            dto.ApellidoPaterno.Should().Be("López");
            dto.ApellidoMaterno.Should().Be("Martínez");
            dto.FechaNacimiento.Should().Be(new DateTime(1985, 5, 15));
            dto.FechaInicioContrato.Should().Be(new DateTime(2022, 6, 1));
            dto.IdPuesto.Should().Be(2);
            dto.Usuario.Should().Be("maria.lopez");
            dto.Contrasena.Should().Be("maria123");
        }

        [Theory]
        [InlineData("Juan", "Pérez", "García")]
        [InlineData("María", "López", "Martínez")]
        [InlineData("Carlos", "González", "Hernández")]
        public void Empleado_DeberiaAceptarDiferentesNombres(string nombre, string apellidoPaterno, string apellidoMaterno)
        {
            // Arrange
            var empleado = new Empleado
            {
                nombre = nombre,
                apellidoPaterno = apellidoPaterno,
                apellidoMaterno = apellidoMaterno
            };

            // Act & Assert
            empleado.nombre.Should().Be(nombre);
            empleado.apellidoPaterno.Should().Be(apellidoPaterno);
            empleado.apellidoMaterno.Should().Be(apellidoMaterno);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void Empleado_DeberiaAceptarDiferentesPuestos(int idPuesto)
        {
            // Arrange
            var empleado = new Empleado
            {
                idPuesto = idPuesto
            };

            // Act & Assert
            empleado.idPuesto.Should().Be(idPuesto);
        }

        [Fact]
        public void Empleado_ConFechasValidas_DeberiaSerValido()
        {
            // Arrange
            var fechaNacimiento = new DateTime(1990, 1, 1);
            var fechaInicioContrato = new DateTime(2023, 1, 1);

            var empleado = new Empleado
            {
                fechaNacimiento = fechaNacimiento,
                fechaInicioContrato = fechaInicioContrato
            };

            // Act & Assert
            empleado.fechaNacimiento.Should().Be(fechaNacimiento);
            empleado.fechaInicioContrato.Should().Be(fechaInicioContrato);
            empleado.fechaInicioContrato.Should().BeAfter(empleado.fechaNacimiento);
        }

        [Fact]
        public void Empleado_ConCodigoEmpleadoValido_DeberiaSerValido()
        {
            // Arrange
            var codigoEmpleado = 1001;

            var empleado = new Empleado
            {
                codigoEmpleado = codigoEmpleado
            };

            // Act & Assert
            empleado.codigoEmpleado.Should().Be(codigoEmpleado);
            empleado.codigoEmpleado.Should().BeGreaterThan(0);
        }

        [Fact]
        public void EmpleadoUsuarioDTO_ConCredencialesValidas_DeberiaSerValido()
        {
            // Arrange
            var dto = new EmpleadoUsuarioDTO
            {
                Usuario = "usuario.test",
                Contrasena = "contrasena123"
            };

            // Act & Assert
            dto.Usuario.Should().Be("usuario.test");
            dto.Contrasena.Should().Be("contrasena123");
            dto.Usuario.Should().NotBeEmpty();
            dto.Contrasena.Should().NotBeEmpty();
        }
    }
} 