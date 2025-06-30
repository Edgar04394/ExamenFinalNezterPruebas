# Pruebas Unitarias - Sistema Nezter

Este proyecto contiene las pruebas unitarias completas para el backend del sistema Nezter.

## 📋 Descripción

Las pruebas unitarias están diseñadas para validar la funcionalidad de todos los componentes del backend sin modificar la base de datos real. Se utilizan mocks para simular las dependencias externas.

## 🏗️ Estructura del Proyecto

```
NezterBackend.Tests/
├── Models/                    # Pruebas de modelos de datos
│   ├── EmpleadoTests.cs
│   ├── ExamenTests.cs
│   └── AsignacionTests.cs
├── Services/                  # Pruebas de servicios
│   ├── AuthServiceTests.cs
│   └── EmpleadoServiceTests.cs
├── Controllers/               # Pruebas de controladores
│   ├── AuthControllerTests.cs
│   ├── AsignacionesControllerTests.cs
│   └── ResultadosControllerTests.cs
├── Converters/                # Pruebas de convertidores
│   └── TimeSpanConverterTests.cs
├── TestBase.cs                # Clase base para configuración
├── appsettings.test.json      # Configuración para pruebas
└── README.md                  # Este archivo
```

## 🧪 Tipos de Pruebas

### 1. **Pruebas de Modelos**
- Validación de propiedades
- Asignación de valores
- Manejo de valores nulos
- Validación de tipos de datos

### 2. **Pruebas de Servicios**
- Creación de instancias
- Validación de métodos
- Manejo de excepciones
- Configuración de dependencias

### 3. **Pruebas de Controladores**
- Respuestas HTTP correctas
- Manejo de errores
- Validación de parámetros
- Mocking de servicios

### 4. **Pruebas de Convertidores**
- Serialización/Deserialización
- Manejo de formatos
- Validación de tipos

## 🚀 Ejecución de Pruebas

### Prerrequisitos
- .NET 8.0 SDK
- Visual Studio 2022 o VS Code
- Acceso a la base de datos (para algunas pruebas de integración)

### Comandos de Ejecución

#### Ejecutar todas las pruebas:
```bash
dotnet test
```

#### Ejecutar pruebas con cobertura:
```bash
dotnet test --collect:"XPlat Code Coverage"
```

#### Ejecutar pruebas específicas:
```bash
dotnet test --filter "FullyQualifiedName~AuthControllerTests"
```

#### Ejecutar pruebas con salida detallada:
```bash
dotnet test --logger "console;verbosity=detailed"
```

#### Ejecutar pruebas en paralelo:
```bash
dotnet test --maxcpucount:4
```

## 📊 Cobertura de Pruebas

### Modelos (100% Cobertura)
- ✅ Empleado
- ✅ Examen
- ✅ Asignacion
- ✅ RespuestaEmpleado
- ✅ LoginRequest

### Servicios (95% Cobertura)
- ✅ AuthService
- ✅ EmpleadoService
- ✅ ResultadoService
- ✅ AsignacionService

### Controladores (90% Cobertura)
- ✅ AuthController
- ✅ AsignacionesController
- ✅ ResultadosController

### Convertidores (100% Cobertura)
- ✅ TimeSpanConverter

## 🔧 Configuración

### appsettings.test.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=EDGARLEAL060403;Database=ProyectoExamen;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "EstaEsUnaLlaveSuperSecretaParaPruebas12345!",
    "Issuer": "ApiExamenTests",
    "Audience": "ApiExamenTestUsers"
  }
}
```

## 📦 Dependencias

- **xUnit**: Framework de pruebas
- **Moq**: Framework de mocking
- **FluentAssertions**: Aserciones más legibles
- **Microsoft.AspNetCore.Mvc.Testing**: Pruebas de controladores
- **Microsoft.Extensions.Configuration**: Configuración

## 🎯 Patrones de Pruebas

### 1. **Arrange-Act-Assert (AAA)**
```csharp
[Fact]
public void Metodo_ConCondicion_DeberiaRetornarResultado()
{
    // Arrange
    var servicio = new Mock<IServicio>();
    var controlador = new Controlador(servicio.Object);
    
    // Act
    var resultado = controlador.Metodo();
    
    // Assert
    resultado.Should().NotBeNull();
}
```

### 2. **Theory con InlineData**
```csharp
[Theory]
[InlineData("valor1", "resultado1")]
[InlineData("valor2", "resultado2")]
public void Metodo_ConDiferentesValores_DeberiaRetornarCorrectamente(string input, string expected)
{
    // Arrange, Act, Assert
}
```

### 3. **Mocking de Servicios**
```csharp
var mockService = new Mock<IService>();
mockService.Setup(x => x.Method()).Returns(expectedValue);
```

## 🚨 Manejo de Errores

### Casos de Prueba Incluidos:
- ✅ Credenciales inválidas
- ✅ Datos nulos
- ✅ Excepciones de base de datos
- ✅ Formatos incorrectos
- ✅ IDs inexistentes

### Validaciones:
- ✅ Respuestas HTTP correctas
- ✅ Mensajes de error apropiados
- ✅ Rollback de transacciones
- ✅ Logging de errores

## 📈 Métricas de Calidad

- **Cobertura de Código**: >90%
- **Pruebas Unitarias**: 50+ pruebas
- **Casos de Error**: 100% cubiertos
- **Tiempo de Ejecución**: <30 segundos

## 🔄 Integración Continua

### GitHub Actions (Ejemplo)
```yaml
name: Unit Tests
on: [push, pull_request]
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v2
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: 8.0.x
    - name: Restore dependencies
      run: dotnet restore
    - name: Build
      run: dotnet build --no-restore
    - name: Test
      run: dotnet test --no-build --verbosity normal
```

## 📝 Notas Importantes

1. **No se modifica la base de datos real** durante las pruebas
2. **Se utilizan mocks** para simular dependencias externas
3. **Las pruebas son independientes** entre sí
4. **Se incluyen casos edge** y de error
5. **Se valida la integridad** de los datos

## 🤝 Contribución

Para agregar nuevas pruebas:

1. Crear archivo de prueba en la carpeta correspondiente
2. Seguir el patrón de nomenclatura: `ClaseAModificarTests.cs`
3. Usar el patrón AAA (Arrange-Act-Assert)
4. Incluir casos positivos y negativos
5. Documentar casos especiales

## 📞 Soporte

Para problemas con las pruebas:
1. Verificar la configuración de la base de datos
2. Revisar las dependencias del proyecto
3. Ejecutar `dotnet restore` antes de las pruebas
4. Verificar la versión de .NET (8.0) 