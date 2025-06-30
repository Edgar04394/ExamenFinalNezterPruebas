using Xunit;
using FluentAssertions;
using ApiExamen.Models;

namespace NezterBackend.Tests.Models
{
    public class EmpleadoTests
    {
        [Fact]
        public void Empleado_DeberiaTenerTodasLasPropiedades()
        {
            // Arrange
            var empleado = new Empleado();

            // Act & Assert
            empleado.Should().NotBeNull();
            empleado.Should().BeOfType<Empleado>();
        }

        [Fact]
        public void Empleado_DeberiaPoderAsignarValores()
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
            empleado.codigoEmpleado.Should().Be(1);
            empleado.nombre.Should().Be("Juan");
            empleado.apellidoPaterno.Should().Be("Pérez");
            empleado.apellidoMaterno.Should().Be("García");
            empleado.fechaNacimiento.Should().Be(new DateTime(1990, 1, 1));
            empleado.fechaInicioContrato.Should().Be(new DateTime(2023, 1, 1));
            empleado.idPuesto.Should().Be(1);
        }

        [Fact]
        public void Empleado_DeberiaTenerPropiedadesNullables()
        {
            // Arrange
            var empleado = new Empleado();

            // Act & Assert
            empleado.nombre.Should().BeNull();
            empleado.apellidoPaterno.Should().BeNull();
            empleado.apellidoMaterno.Should().BeNull();
        }

        [Fact]
        public void Empleado_DeberiaTenerFechasPorDefecto()
        {
            // Arrange
            var empleado = new Empleado();

            // Act & Assert
            empleado.fechaNacimiento.Should().Be(default(DateTime));
            empleado.fechaInicioContrato.Should().Be(default(DateTime));
        }
    }
} 