using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ApiExamen.Controllers;
using ApiExamen.Services;
using ApiExamen.Models;
using ApiExamen.Interfaces;

namespace NezterBackend.Tests.Controllers
{
    public class AsignacionesControllerTests : TestBase
    {
        private readonly Mock<IAsignacionService> _mockAsignacionService;
        private readonly AsignacionesController _asignacionesController;

        public AsignacionesControllerTests()
        {
            _mockAsignacionService = new Mock<IAsignacionService>();
            _asignacionesController = new AsignacionesController(_mockAsignacionService.Object);
        }

        [Fact]
        public void AsignacionesController_DeberiaCrearseCorrectamente()
        {
            // Act & Assert
            _asignacionesController.Should().NotBeNull();
            _asignacionesController.Should().BeOfType<AsignacionesController>();
        }

        [Fact]
        public async Task AsignarExamen_ConAsignacionValida_DeberiaRetornarOk()
        {
            // Arrange
            var asignacion = new Asignacion
            {
                idExamen = 1,
                codigoEmpleado = 100,
                fechaAsignacion = DateTime.Now
            };

            _mockAsignacionService.Setup(x => x.Asignar(asignacion))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _asignacionesController.AsignarExamen(asignacion);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
            
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().NotBeNull();
            okResult.Value!.GetType().GetProperty("mensaje")!.GetValue(okResult.Value).Should().Be("Examen asignado");
        }

        [Fact]
        public async Task EliminarAsignacion_ConIdValido_DeberiaRetornarOk()
        {
            // Arrange
            var idAsignacion = 1;

            _mockAsignacionService.Setup(x => x.EliminarAsignacion(idAsignacion))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _asignacionesController.EliminarAsignacion(idAsignacion);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
            
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().NotBeNull();
            okResult.Value!.GetType().GetProperty("mensaje")!.GetValue(okResult.Value).Should().Be("Asignación eliminada exitosamente");
        }

        [Fact]
        public async Task EliminarAsignacion_ConExcepcion_DeberiaRetornarBadRequest()
        {
            // Arrange
            var idAsignacion = 999;

            _mockAsignacionService.Setup(x => x.EliminarAsignacion(idAsignacion))
                .ThrowsAsync(new Exception("Asignación no encontrada"));

            // Act
            var result = await _asignacionesController.EliminarAsignacion(idAsignacion);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
            
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.Value.Should().NotBeNull();
            badRequestResult.Value!.ToString()!.Should().Contain("Error al eliminar asignación");
        }

        [Fact]
        public async Task ListarAsignacionesPorEmpleado_ConEmpleadoValido_DeberiaRetornarOk()
        {
            // Arrange
            var empleado = new Empleado { codigoEmpleado = 100 };
            var asignacionesEsperadas = new List<Asignacion>
            {
                new Asignacion { idAsignacion = 1, idExamen = 1, codigoEmpleado = 100 },
                new Asignacion { idAsignacion = 2, idExamen = 2, codigoEmpleado = 100 }
            };

            _mockAsignacionService.Setup(x => x.ConsultarPorEmpleado(empleado.codigoEmpleado))
                .ReturnsAsync(asignacionesEsperadas);

            // Act
            var result = await _asignacionesController.ListarAsignacionesPorEmpleado(empleado);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
            
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().NotBeNull();
            okResult.Value.Should().BeEquivalentTo(asignacionesEsperadas);
        }

        [Fact]
        public async Task ListarAsignacionesPorEmpleado_ConListaVacia_DeberiaRetornarOk()
        {
            // Arrange
            var empleado = new Empleado { codigoEmpleado = 999 };
            var asignacionesVacias = new List<Asignacion>();

            _mockAsignacionService.Setup(x => x.ConsultarPorEmpleado(empleado.codigoEmpleado))
                .ReturnsAsync(asignacionesVacias);

            // Act
            var result = await _asignacionesController.ListarAsignacionesPorEmpleado(empleado);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
            
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().NotBeNull();
            okResult.Value.Should().BeEquivalentTo(asignacionesVacias);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public async Task EliminarAsignacion_ConDiferentesIds_DeberiaManejarCorrectamente(int idAsignacion)
        {
            // Arrange
            _mockAsignacionService.Setup(x => x.EliminarAsignacion(idAsignacion))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _asignacionesController.EliminarAsignacion(idAsignacion);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void Asignacion_DeberiaTenerPropiedadesValidas()
        {
            // Arrange
            var asignacion = new Asignacion
            {
                idAsignacion = 1,
                idExamen = 1,
                codigoEmpleado = 100,
                fechaAsignacion = DateTime.Now,
                nombre_examen = "Examen de Programación",
                descripcion = "Evaluación de conocimientos",
                tiempoLimite = "01:30:00"
            };

            // Act & Assert
            asignacion.Should().NotBeNull();
            asignacion.idAsignacion.Should().Be(1);
            asignacion.idExamen.Should().Be(1);
            asignacion.codigoEmpleado.Should().Be(100);
            asignacion.nombre_examen.Should().Be("Examen de Programación");
            asignacion.descripcion.Should().Be("Evaluación de conocimientos");
            asignacion.tiempoLimite.Should().Be("01:30:00");
        }

        [Fact]
        public void Asignacion_ConPropiedadesNulas_DeberiaSerValido()
        {
            // Arrange
            var asignacion = new Asignacion
            {
                idAsignacion = 1,
                idExamen = 1,
                codigoEmpleado = 100,
                fechaAsignacion = DateTime.Now
            };

            // Act & Assert
            asignacion.Should().NotBeNull();
            asignacion.nombre_examen.Should().BeNull();
            asignacion.descripcion.Should().BeNull();
            asignacion.tiempoLimite.Should().BeNull();
        }
    }
} 