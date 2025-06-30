using Xunit;
using FluentAssertions;
using ApiExamen.Models;

namespace NezterBackend.Tests.Models
{
    public class AsignacionTests
    {
        [Fact]
        public void Asignacion_DeberiaTenerTodasLasPropiedades()
        {
            // Arrange
            var asignacion = new Asignacion();

            // Act & Assert
            asignacion.Should().NotBeNull();
            asignacion.Should().BeOfType<Asignacion>();
        }

        [Fact]
        public void Asignacion_DeberiaPoderAsignarValores()
        {
            // Arrange
            var fechaAsignacion = DateTime.Now;
            var asignacion = new Asignacion
            {
                idAsignacion = 1,
                idExamen = 1,
                codigoEmpleado = 100,
                fechaAsignacion = fechaAsignacion,
                nombre_examen = "Examen de Programación",
                descripcion = "Evaluación de conocimientos en programación",
                tiempoLimite = "01:30:00"
            };

            // Act & Assert
            asignacion.idAsignacion.Should().Be(1);
            asignacion.idExamen.Should().Be(1);
            asignacion.codigoEmpleado.Should().Be(100);
            asignacion.fechaAsignacion.Should().Be(fechaAsignacion);
            asignacion.nombre_examen.Should().Be("Examen de Programación");
            asignacion.descripcion.Should().Be("Evaluación de conocimientos en programación");
            asignacion.tiempoLimite.Should().Be("01:30:00");
        }

        [Fact]
        public void Asignacion_DeberiaTenerPropiedadesNullables()
        {
            // Arrange
            var asignacion = new Asignacion();

            // Act & Assert
            asignacion.nombre_examen.Should().BeNull();
            asignacion.descripcion.Should().BeNull();
            asignacion.tiempoLimite.Should().BeNull();
        }

        [Fact]
        public void Asignacion_DeberiaTenerFechasPorDefecto()
        {
            // Arrange
            var asignacion = new Asignacion();

            // Act & Assert
            asignacion.fechaAsignacion.Should().Be(default(DateTime));
        }

        [Theory]
        [InlineData(1, 1, 100)]
        [InlineData(2, 3, 200)]
        [InlineData(10, 5, 500)]
        public void Asignacion_DeberiaAceptarDiferentesIDs(int idAsignacion, int idExamen, int codigoEmpleado)
        {
            // Arrange
            var asignacion = new Asignacion
            {
                idAsignacion = idAsignacion,
                idExamen = idExamen,
                codigoEmpleado = codigoEmpleado
            };

            // Act & Assert
            asignacion.idAsignacion.Should().Be(idAsignacion);
            asignacion.idExamen.Should().Be(idExamen);
            asignacion.codigoEmpleado.Should().Be(codigoEmpleado);
        }

        [Theory]
        [InlineData("00:30:00")]
        [InlineData("01:00:00")]
        [InlineData("02:15:30")]
        public void Asignacion_DeberiaAceptarDiferentesFormatosDeTiempo(string tiempoLimite)
        {
            // Arrange
            var asignacion = new Asignacion
            {
                tiempoLimite = tiempoLimite
            };

            // Act & Assert
            asignacion.tiempoLimite.Should().Be(tiempoLimite);
        }
    }
} 