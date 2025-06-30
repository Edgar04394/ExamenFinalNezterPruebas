using Xunit;
using FluentAssertions;
using System.Text.Json;
using ApiExamen.Converters;

namespace NezterBackend.Tests.Converters
{
    public class TimeSpanConverterTests
    {
        private readonly TimeSpanConverter _converter;

        public TimeSpanConverterTests()
        {
            _converter = new TimeSpanConverter();
        }

        [Fact]
        public void TimeSpanConverter_DeberiaCrearseCorrectamente()
        {
            // Act & Assert
            _converter.Should().NotBeNull();
            _converter.Should().BeOfType<TimeSpanConverter>();
        }

        [Fact]
        public void Read_ConStringValido_DeberiaConvertirCorrectamente()
        {
            // Arrange
            var jsonString = "\"01:30:45\"";
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(jsonString));

            // Act
            reader.Read();
            var result = _converter.Read(ref reader, typeof(TimeSpan), new JsonSerializerOptions());

            // Assert
            result.Should().Be(new TimeSpan(1, 30, 45));
        }

        [Theory]
        [InlineData("00:00:00", 0, 0, 0)]
        [InlineData("00:30:00", 0, 30, 0)]
        [InlineData("01:00:00", 1, 0, 0)]
        [InlineData("02:15:30", 2, 15, 30)]
        [InlineData("12:45:20", 12, 45, 20)]
        public void Read_ConDiferentesFormatos_DeberiaConvertirCorrectamente(string timeString, int horas, int minutos, int segundos)
        {
            // Arrange
            var jsonString = $"\"{timeString}\"";
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(jsonString));

            // Act
            reader.Read();
            var result = _converter.Read(ref reader, typeof(TimeSpan), new JsonSerializerOptions());

            // Assert
            result.Should().Be(new TimeSpan(horas, minutos, segundos));
        }

        [Fact]
        public void Read_ConStringInvalido_DeberiaLanzarExcepcion()
        {
            // Arrange
            var jsonString = "\"formato_invalido\"";
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(jsonString));

            // Act & Assert
            reader.Read();
            try
            {
                _converter.Read(ref reader, typeof(TimeSpan), new JsonSerializerOptions());
                Assert.Fail("Se esperaba una excepción JsonException pero no se lanzó ninguna.");
            }
            catch (JsonException)
            {
                Assert.True(true);
            }
        }

        [Fact]
        public void Write_ConTimeSpanValido_DeberiaEscribirCorrectamente()
        {
            // Arrange
            var timeSpan = new TimeSpan(1, 30, 45);
            var stream = new MemoryStream();
            var writer = new Utf8JsonWriter(stream);

            // Act
            _converter.Write(writer, timeSpan, new JsonSerializerOptions());
            writer.Flush();

            // Assert
            var jsonString = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            jsonString.Should().Be("\"01:30:45\"");
        }

        [Theory]
        [InlineData(0, 0, 0, "00:00:00")]
        [InlineData(0, 30, 0, "00:30:00")]
        [InlineData(1, 0, 0, "01:00:00")]
        [InlineData(2, 15, 30, "02:15:30")]
        [InlineData(12, 45, 20, "12:45:20")]
        public void Write_ConDiferentesTimeSpans_DeberiaEscribirCorrectamente(int horas, int minutos, int segundos, string expectedString)
        {
            // Arrange
            var timeSpan = new TimeSpan(horas, minutos, segundos);
            var stream = new MemoryStream();
            var writer = new Utf8JsonWriter(stream);

            // Act
            _converter.Write(writer, timeSpan, new JsonSerializerOptions());
            writer.Flush();

            // Assert
            var jsonString = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            jsonString.Should().Be($"\"{expectedString}\"");
        }

        [Fact]
        public void Write_ConTimeSpanCero_DeberiaEscribirCorrectamente()
        {
            // Arrange
            var timeSpan = TimeSpan.Zero;
            var stream = new MemoryStream();
            var writer = new Utf8JsonWriter(stream);

            // Act
            _converter.Write(writer, timeSpan, new JsonSerializerOptions());
            writer.Flush();

            // Assert
            var jsonString = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            jsonString.Should().Be("\"00:00:00\"");
        }

        [Fact]
        public void Write_ConTimeSpanMaxValue_DeberiaEscribirCorrectamente()
        {
            // Arrange
            var timeSpan = TimeSpan.MaxValue;
            var stream = new MemoryStream();
            var writer = new Utf8JsonWriter(stream);

            // Act
            _converter.Write(writer, timeSpan, new JsonSerializerOptions());
            writer.Flush();

            // Assert
            var jsonString = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            jsonString.Should().NotBeEmpty();
            jsonString.Should().StartWith("\"");
            jsonString.Should().EndWith("\"");
        }

        [Fact]
        public void CanConvert_ConTipoTimeSpan_DeberiaRetornarTrue()
        {
            // Act
            var result = _converter.CanConvert(typeof(TimeSpan));

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(typeof(string))]
        [InlineData(typeof(int))]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(object))]
        public void CanConvert_ConOtrosTipos_DeberiaRetornarFalse(Type type)
        {
            // Act
            var result = _converter.CanConvert(type);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void CanConvert_ConTipoNull_DeberiaRetornarFalse()
        {
            // Act
            var result = _converter.CanConvert(null);

            // Assert
            result.Should().BeFalse();
        }
    }
} 