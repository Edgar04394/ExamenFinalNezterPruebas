using Xunit;
using FluentAssertions;
using ApiExamen.Models;

namespace NezterBackend.Tests.Models
{
    public class ExamenTests
    {
        [Fact]
        public void Examen_DeberiaTenerTodasLasPropiedades()
        {
            // Arrange
            var examen = new Examen();

            // Act & Assert
            examen.Should().NotBeNull();
            examen.Should().BeOfType<Examen>();
        }

        [Fact]
        public void Examen_DeberiaPoderAsignarValores()
        {
            // Arrange
            var tiempoLimite = new TimeSpan(1, 30, 0); // 1 hora 30 minutos
            var examen = new Examen
            {
                idExamen = 1,
                titulo = "Examen de Programación",
                descripcion = "Evaluación de conocimientos en programación",
                tiempoLimite = tiempoLimite
            };

            // Act & Assert
            examen.idExamen.Should().Be(1);
            examen.titulo.Should().Be("Examen de Programación");
            examen.descripcion.Should().Be("Evaluación de conocimientos en programación");
            examen.tiempoLimite.Should().Be(tiempoLimite);
        }

        [Fact]
        public void Examen_DeberiaTenerPropiedadesNullables()
        {
            // Arrange
            var examen = new Examen();

            // Act & Assert
            examen.titulo.Should().BeNull();
            examen.descripcion.Should().BeNull();
        }

        [Fact]
        public void Examen_DeberiaTenerTiempoLimitePorDefecto()
        {
            // Arrange
            var examen = new Examen();

            // Act & Assert
            examen.tiempoLimite.Should().Be(default(TimeSpan));
        }

        [Theory]
        [InlineData(0, 30, 0)] // 30 minutos
        [InlineData(1, 0, 0)]  // 1 hora
        [InlineData(2, 15, 30)] // 2 horas 15 minutos 30 segundos
        public void Examen_DeberiaAceptarDiferentesTiemposLimite(int horas, int minutos, int segundos)
        {
            // Arrange
            var tiempoLimite = new TimeSpan(horas, minutos, segundos);
            var examen = new Examen
            {
                tiempoLimite = tiempoLimite
            };

            // Act & Assert
            examen.tiempoLimite.Should().Be(tiempoLimite);
            examen.tiempoLimite.TotalSeconds.Should().Be(horas * 3600 + minutos * 60 + segundos);
        }
    }
} 