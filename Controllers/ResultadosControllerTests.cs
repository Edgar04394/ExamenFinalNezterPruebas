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
    public class ResultadosControllerTests : TestBase
    {
        private readonly Mock<IResultadoService> _mockResultadoService;
        private readonly ResultadosController _resultadosController;

        public ResultadosControllerTests()
        {
            _mockResultadoService = new Mock<IResultadoService>();
            
            // Crear una configuración real para las pruebas
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"ConnectionStrings:DefaultConnection", "Server=localhost;Database=TestDB;Trusted_Connection=true;TrustServerCertificate=true"}
                })
                .Build();
            
            _resultadosController = new ResultadosController(_mockResultadoService.Object, configuration);
        }

        [Fact]
        public void ResultadosController_DeberiaCrearseCorrectamente()
        {
            // Act & Assert
            _resultadosController.Should().NotBeNull();
            _resultadosController.Should().BeOfType<ResultadosController>();
        }

        [Fact]
        public async Task GuardarRespuestaEmpleado_ConRespuestaValida_DeberiaRetornarOk()
        {
            // Arrange
            var respuestaEmpleado = new RespuestaEmpleado
            {
                idAsignacion = 1,
                idPregunta = 1,
                idRespuesta = 1
            };

            _mockResultadoService.Setup(x => x.GuardarRespuestaEmpleado(It.IsAny<RespuestaEmpleado>())).Returns(Task.CompletedTask);

            // Act
            var result = await _resultadosController.GuardarRespuestaEmpleado(respuestaEmpleado);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
            
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().NotBeNull();
            okResult.Value!.GetType().GetProperty("mensaje")!.GetValue(okResult.Value).Should().Be("Respuesta guardada");
            okResult.Value!.GetType().GetProperty("success")!.GetValue(okResult.Value).Should().Be(true);
        }

        [Fact]
        public async Task GuardarRespuestaEmpleado_ConExcepcion_DeberiaRetornarBadRequest()
        {
            // Arrange
            var respuestaEmpleado = new RespuestaEmpleado
            {
                idAsignacion = 1,
                idPregunta = 1,
                idRespuesta = 1
            };

            _mockResultadoService.Setup(x => x.GuardarRespuestaEmpleado(It.IsAny<RespuestaEmpleado>())).ThrowsAsync(new Exception("Error al guardar respuesta"));

            // Act
            var result = await _resultadosController.GuardarRespuestaEmpleado(respuestaEmpleado);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
            
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.Value.Should().NotBeNull();
            badRequestResult.Value!.ToString()!.Should().Contain("Error al guardar respuesta");
        }

        [Fact]
        public async Task ReportePromedioPorCompetencia_ConAsignacionValida_DeberiaRetornarOk()
        {
            // Arrange
            var asignacion = new Asignacion { idAsignacion = 1 };
            var reporteEsperado = new List<ReportePromedioCompetencia>
            {
                new ReportePromedioCompetencia { Competencia = "Programación", Promedio = 85.5 },
                new ReportePromedioCompetencia { Competencia = "Lógica", Promedio = 90.0 }
            };

            _mockResultadoService.Setup(x => x.ReportePromedioPorCompetencia(It.IsAny<int>())).ReturnsAsync(reporteEsperado);

            // Act
            var result = await _resultadosController.ReportePromedioPorCompetencia(asignacion);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
            
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().NotBeNull();
            okResult.Value.Should().BeEquivalentTo(reporteEsperado);
        }

        [Fact]
        public async Task ReportePromedioPorCompetencia_ConListaVacia_DeberiaRetornarOk()
        {
            // Arrange
            var asignacion = new Asignacion { idAsignacion = 999 };
            var reporteVacio = new List<ReportePromedioCompetencia>();

            _mockResultadoService.Setup(x => x.ReportePromedioPorCompetencia(It.IsAny<int>())).ReturnsAsync(reporteVacio);

            // Act
            var result = await _resultadosController.ReportePromedioPorCompetencia(asignacion);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
            
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().NotBeNull();
            okResult.Value.Should().BeEquivalentTo(reporteVacio);
        }

        [Fact]
        public async Task ResultadosHistoricos_ConCodigoEmpleadoValido_DeberiaRetornarOk()
        {
            // Arrange
            var codigoEmpleado = 100;
            var resultadosEsperados = new List<object>
            {
                new { NombreExamen = "Examen de Programación", Competencia = "Programación", Promedio = 85.5 },
                new { NombreExamen = "Examen de Lógica", Competencia = "Lógica", Promedio = 90.0 }
            };

            _mockResultadoService.Setup(x => x.ObtenerResultadosHistoricos(codigoEmpleado))
                .ReturnsAsync(resultadosEsperados);

            // Act
            var result = await _resultadosController.ResultadosHistoricos(codigoEmpleado);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
            
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().NotBeNull();
            
            // Verificar que la respuesta es un objeto con las propiedades esperadas
            var response = okResult.Value;
            response.Should().NotBeNull();
            
            // Verificar que tiene las propiedades esperadas usando reflexión
            var mensaje = response!.GetType().GetProperty("mensaje")?.GetValue(response);
            var success = response!.GetType().GetProperty("success")?.GetValue(response);
            var codigoEmpleadoResponse = response!.GetType().GetProperty("codigoEmpleado")?.GetValue(response);
            var resultados = response!.GetType().GetProperty("resultados")?.GetValue(response);
            
            mensaje.Should().Be("Resultados históricos obtenidos exitosamente");
            success.Should().Be(true);
            codigoEmpleadoResponse.Should().Be(codigoEmpleado);
            resultados.Should().NotBeNull();
        }

        [Fact]
        public async Task ResultadosPorExamen_ConParametrosValidos_DeberiaRetornarOk()
        {
            // Arrange
            var idExamen = 1;
            var codigoEmpleado = 100;
            var request = new { idExamen, codigoEmpleado };
            var resultadosEsperados = new List<object>
            {
                new { NombreExamen = "Examen de Programación", Competencia = "Programación", Promedio = 85.5 }
            };

            _mockResultadoService.Setup(x => x.ObtenerResultadosPorExamen(idExamen, codigoEmpleado))
                .ReturnsAsync(resultadosEsperados);

            // Act
            var result = await _resultadosController.ResultadosPorExamen(request);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
            
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().NotBeNull();
            okResult.Value.Should().BeEquivalentTo(resultadosEsperados);
        }

        [Fact]
        public async Task GuardarEnHistorial_ConIdAsignacionValido_DeberiaRetornarOk()
        {
            // Arrange
            var idAsignacion = 1;

            _mockResultadoService.Setup(x => x.GuardarEnHistorial(It.IsAny<int>())).Returns(Task.CompletedTask);

            // Act
            var result = await _resultadosController.GuardarEnHistorial(idAsignacion);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
            
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().NotBeNull();
            okResult.Value!.GetType().GetProperty("mensaje")!.GetValue(okResult.Value).Should().Be("Resultados guardados en historial exitosamente");
            okResult.Value!.GetType().GetProperty("success")!.GetValue(okResult.Value).Should().Be(true);
        }

        [Fact]
        public void RespuestaEmpleado_DeberiaTenerPropiedadesValidas()
        {
            // Arrange
            var respuestaEmpleado = new RespuestaEmpleado
            {
                idRespuestaEmpleado = 1,
                idAsignacion = 1,
                idPregunta = 1,
                idRespuesta = 1
            };

            // Act & Assert
            respuestaEmpleado.Should().NotBeNull();
            respuestaEmpleado.idRespuestaEmpleado.Should().Be(1);
            respuestaEmpleado.idAsignacion.Should().Be(1);
            respuestaEmpleado.idPregunta.Should().Be(1);
            respuestaEmpleado.idRespuesta.Should().Be(1);
        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(5, 10, 15)]
        [InlineData(100, 200, 300)]
        public void RespuestaEmpleado_ConDiferentesIds_DeberiaSerValido(int idAsignacion, int idPregunta, int idRespuesta)
        {
            // Arrange
            var respuestaEmpleado = new RespuestaEmpleado
            {
                idAsignacion = idAsignacion,
                idPregunta = idPregunta,
                idRespuesta = idRespuesta
            };

            // Act & Assert
            respuestaEmpleado.Should().NotBeNull();
            respuestaEmpleado.idAsignacion.Should().Be(idAsignacion);
            respuestaEmpleado.idPregunta.Should().Be(idPregunta);
            respuestaEmpleado.idRespuesta.Should().Be(idRespuesta);
        }
    }
} 