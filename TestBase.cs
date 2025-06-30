using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace NezterBackend.Tests
{
    public abstract class TestBase
    {
        protected IConfiguration Configuration { get; private set; }
        protected IServiceProvider ServiceProvider { get; private set; }

        protected TestBase()
        {
            SetupConfiguration();
            SetupServices();
        }

        private void SetupConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.test.json", optional: false)
                .AddEnvironmentVariables();

            Configuration = configurationBuilder.Build();
        }

        private void SetupServices()
        {
            var services = new ServiceCollection();

            // Configurar logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            // Configurar servicios de la aplicación
            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {
            // Los servicios específicos se configurarán en las clases derivadas
        }

        protected T GetService<T>() where T : class
        {
            return ServiceProvider.GetRequiredService<T>();
        }

        protected Mock<T> CreateMock<T>() where T : class
        {
            return new Mock<T>();
        }
    }
} 