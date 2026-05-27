# Sistema de Gestión de Inventario y Reportes

API REST construida con ASP.NET Core 8 y Entity Framework Core que implementa un sistema completo de gestión de inventario con arquitectura limpia, seguridad corporativa, gestión transaccional atómica y pruebas automatizadas.

## Objetivo del Proyecto

Este proyecto demuestra la capacidad de implementar:
- **Seguridad corporativa** con JWT y encriptación de credenciales
- **Gestión de transacciones atómicas** mediante el patrón Unit of Work
- **Calidad de código** mediante pruebas unitarias automatizadas
- **Arquitectura limpia** siguiendo principios SOLID y separación de responsabilidades

## Requisitos Previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) o superior
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (LocalDB o instancia completa)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [Visual Studio Code](https://code.visualstudio.com/) (opcional)

## Estructura del Proyecto

```
ApiInventario/
├── Inventario.WebAPI/          # Capa de presentación (Controllers, JWT, Swagger)
├── Inventario.Application/     # Capa de aplicación (Servicios, Validadores, Interfaces)
├── Inventario.Domain/          # Capa de dominio (Entidades, DTOs, Modelos)
├── Inventario.Infrastructure/  # Capa de infraestructura (Repositorios, UoW, Context, Migraciones)
├── Inventario.Utils/           # Utilidades compartidas (Encriptación)
├── Inventario.Tests/           # Pruebas unitarias (xUnit, Moq)
└── Inventario.sln              # Solución principal
```

## Configuración Inicial

### 1. Configurar la Base de Datos

El proyecto utiliza Entity Framework Core con enfoque Code-First. Las migraciones se aplican automáticamente al iniciar la aplicación.

**Configurar la cadena de conexión:**

Edita el archivo `Inventario.WebAPI/appsettings.json` y actualiza la sección `ConnectionStrings`:

```json
{
  "ConnectionStrings": {
    "ConnetionToken": "TU_CADENA_DE_CONEXION_ENCRIPTADA",
    "ConnetionGenerico": "TU_CADENA_DE_CONEXION_ENCRIPTADA"
  }
}
```

### 2. Encriptar la Cadena de Conexión

Por seguridad, las cadenas de conexión deben estar encriptadas. Utiliza el servicio de encriptación incluido en el proyecto:

**Opción A: Usar el servicio de encriptación programáticamente**

Crea un archivo temporal `encrypt.csx` con el siguiente contenido:

```csharp
#r "Inventario.Utils/bin/Debug/net9.0/Inventario.Utils.dll"
using Inventario.Utils.Security;

var encryptionService = new EncryptionService();
var connectionString = "Server=localhost;Database=InventarioDB;Trusted_Connection=True;TrustServerCertificate=True;";
var encrypted = encryptionService.Encrypt(connectionString);

Console.WriteLine($"Cadena encriptada: {encrypted}");
```

Ejecuta con:
```bash
dotnet script encrypt.csx
```

**Opción B: Crear una aplicación de consola temporal**

```csharp
using Inventario.Utils.Security;

var encryptionService = new EncryptionService();
var connectionString = "Server=localhost;Database=InventarioDB;Trusted_Connection=True;TrustServerCertificate=True;";
var encrypted = encryptionService.Encrypt(connectionString);

Console.WriteLine($"Cadena encriptada: {encrypted}");
```

Copia el resultado encriptado en `appsettings.json`.

### 3. Configurar Credenciales de Autenticación

Edita `appsettings.json` para configurar las credenciales del administrador:

```json
{
  "Auth": {
    "AdminUsername": "admin",
    "AdminPassword": "yeniadmin"
  },
  "Authentication": {
    "SecretKey": "hfA2IfgOFRnDI+wj9Z7FDT6Y0jko3KlOnP3RdiLR1YfjxDSPFiwakQ==",
    "Issuer": "https://localhost:7147",
    "Audience": "https://localhost:7147"
  }
}
```

**Credenciales por defecto:**
- **Usuario**: `admin`
- **Contraseña**: `yeniadmin`

## Ejecutar el Proyecto

### Desde la línea de comandos

```bash
# Restaurar paquetes NuGet
dotnet restore

# Compilar el proyecto
dotnet build

# Ejecutar la aplicación
dotnet run --project Inventario.WebAPI
```

La API estará disponible en:
- **HTTPS**: `https://localhost:7147`
- **HTTP**: `http://localhost:5000`
- **Swagger UI**: `https://localhost:7147/swagger`

### Desde Visual Studio

1. Abre `Inventario.sln` en Visual Studio 2022
2. Establece `Inventario.WebAPI` como proyecto de inicio
3. Presiona `F5` o haz clic en "Iniciar"

## Ejecutar Pruebas Unitarias

El proyecto incluye pruebas unitarias completas utilizando **xUnit** y **Moq** para simular dependencias.

### Ejecutar todas las pruebas

```bash
dotnet test
```

### Ejecutar pruebas con cobertura de código

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Ejecutar pruebas con detalle

```bash
dotnet test --verbosity normal
```

### Ejecutar pruebas específicas

```bash
# Pruebas de ProductoService
dotnet test --filter "FullyQualifiedName~ProductoServiceTests"

# Pruebas de CategoriaService
dotnet test --filter "FullyQualifiedName~CategoriaServiceTests"

# Pruebas de InventoryReportService
dotnet test --filter "FullyQualifiedName~InventoryReportServiceTests"
```

## Uso de la API

### 1. Autenticación

Obtén un token JWT para acceder a endpoints protegidos:

**Credenciales por defecto:**
- Usuario: `admin`
- Contraseña: `yeniadmin`

```bash
curl -X POST "https://localhost:7147/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "yeniadmin"
  }'
```

**Respuesta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expiresIn": 3600,
  "tokenType": "Bearer"
}
```

### 2. Gestión de Productos

**Crear producto:**
```bash
curl -X POST "https://localhost:7147/api/productos" \
  -H "Authorization: Bearer TU_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "sku": "PROD-001",
    "nombre": "Producto Ejemplo",
    "descripcion": "Descripción del producto",
    "precio": 99.99,
    "stock": 100,
    "stockMinimo": 10,
    "categoriaId": 1
  }'
```

**Obtener todos los productos:**
```bash
curl -X GET "https://localhost:7147/api/productos" \
  -H "Authorization: Bearer TU_TOKEN"
```

**Actualizar producto:**
```bash
curl -X PUT "https://localhost:7147/api/productos/1" \
  -H "Authorization: Bearer TU_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "id": 1,
    "sku": "PROD-001",
    "nombre": "Producto Actualizado",
    "descripcion": "Nueva descripción",
    "precio": 109.99,
    "stock": 150,
    "stockMinimo": 15,
    "categoriaId": 1
  }'
```

**Eliminar producto:**
```bash
curl -X DELETE "https://localhost:7147/api/productos/1" \
  -H "Authorization: Bearer TU_TOKEN"
```

### 3. Gestión de Categorías

**Crear categoría:**
```bash
curl -X POST "https://localhost:7147/api/categorias" \
  -H "Authorization: Bearer TU_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Nueva Categoría",
    "descripcion": "Descripción de la categoría"
  }'
```

**Obtener todas las categorías:**
```bash
curl -X GET "https://localhost:7147/api/categorias" \
  -H "Authorization: Bearer TU_TOKEN"
```

### 4. Reportes e Indicadores

**Obtener resumen del inventario:**
```bash
curl -X GET "https://localhost:7147/api/inventory/summary" \
  -H "Authorization: Bearer TU_TOKEN"
```

**Respuesta:**
```json
{
  "stateOperation": true,
  "result": {
    "valorTotalInventario": 150000.00,
    "productosPorCategoria": [
      {
        "categoriaNombre": "Electronica",
        "cantidadProductos": 5,
        "valorTotal": 75000.00
      }
    ],
    "productosStockCritico": [
      {
        "sku": "PROD-003",
        "nombre": "Producto Bajo",
        "stock": 2,
        "stockMinimo": 10,
        "categoriaNombre": "Electronica"
      }
    ],
    "porcentajeOcupacion": 75.50
  }
}
```

## Cumplimiento de Requisitos de la Prueba Técnica

### 1. Infraestructura y Seguridad de Datos

#### a) Base de Datos con Entity Framework Core (Code-First)

**Implementación:**
- **Contexto**: `Inventario.Infrastructure/Context/ContextInventory.cs`
- **Migraciones**: `Inventario.Infrastructure/Migrations/`
- **Seeder**: `Inventario.Infrastructure/Seeders/InventorySeeder.cs`

**Evidencia:**
```csharp
// Program.cs - Líneas 177-182
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ContextInventory>();
    await context.Database.MigrateAsync();
    await InventorySeeder.SeedAsync(context);
}
```

**Datos maestros incluidos:**
- 5 categorías: Electrónica, Ropa, Alimentos, Hogar, Deportes
- 10 productos de ejemplo con diferentes niveles de stock

#### b) Encriptación de Credenciales

**Implementación:**
- **Servicio**: `Inventario.Utils/Security/EncryptionService.cs`
- **Algoritmo**: TripleDES con clave y vector de inicialización
- **Uso**: `Inventario.WebAPI/Program.cs` - Líneas 38-41

**Evidencia:**
```csharp
// Program.cs - Desencriptación automática al inicio
string DecryptConnectionString(string encryptedConnectionString)
{
    return string.IsNullOrEmpty(encryptedConnectionString) 
        ? null 
        : Encrypt.Decrypt(encryptedConnectionString);
}

// Uso en configuración de DbContext
string ConnetionGenerico = DecryptConnectionString(
    configuracionConnectionStrings.ConnetionGenerico);
builder.Services.AddDbContext<ContextInventory>(
    opt => opt.UseSqlServer(ConnetionGenerico));
```

**Cadenas encriptadas en appsettings.json:**
```json
"ConnectionStrings": {
  "ConnetionToken": "oHRNWhqAbAEFlea4+xqNoJweblKYWx21...",
  "ConnetionGenerico": "oHRNWhqAbAEFlea4+xqNoJweblKYWx21..."
}
```

#### c) Gestión Transaccional con Unit of Work

**Implementación:**
- **Interfaz**: `Inventario.Infrastructure/Repositories/_UnitOfWork/IUnitOfWorkInventory.cs`
- **Implementación**: `Inventario.Infrastructure/Repositories/_UnitOfWork/UnitOfWorkInventory.cs`
- **Uso en servicios**: Todos los servicios usan `IUnitOfWorkInventory`

**Evidencia:**
```csharp
// IUnitOfWorkInventory.cs
public interface IUnitOfWorkInventory : IDisposable
{
    IProductoRepository ProductoRepository { get; }
    ICategoriaRepository CategoriaRepository { get; }
    Task BeginAsync();
    Task CommitAsync();
    Task RollbackAsync();
}

// UnitOfWorkInventory.cs - Implementación completa
public async Task BeginAsync()
{
    _transaction = await _context.Database.BeginTransactionAsync();
}

public async Task CommitAsync()
{
    if (_transaction != null)
    {
        await _transaction.CommitAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }
}

// ProductoService.cs - Uso del Unit of Work
public async Task<ResultOperation<ProductoDto>> CreateAsync(ProductoCreateDto dto)
{
    var skuExists = await _unitOfWork.ProductoRepository.ExistsBySkuAsync(dto.Sku);
    var categoriaExists = await _unitOfWork.CategoriaRepository.GetByIdAsync(dto.CategoriaId);
    // ... validaciones y lógica de negocio
    await _unitOfWork.ProductoRepository.CreateAsync(producto);
}
```

**Garantía de atomicidad:**
- Ningún controlador interactúa directamente con `DbContext`
- Todos los repositorios se acceden a través del Unit of Work
- Las transacciones se gestionan de forma centralizada

### 2. Desarrollo de la API

#### a) Endpoints de Gestión con Validaciones de Negocio

**Endpoints implementados:**

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/productos` | Listar todos los productos |
| GET | `/api/productos/{id}` | Obtener producto por ID |
| POST | `/api/productos` | Crear nuevo producto |
| PUT | `/api/productos/{id}` | Actualizar producto |
| DELETE | `/api/productos/{id}` | Eliminar producto |
| GET | `/api/categorias` | Listar todas las categorías |
| GET | `/api/categorias/{id}` | Obtener categoría por ID |
| POST | `/api/categorias` | Crear nueva categoría |
| PUT | `/api/categorias/{id}` | Actualizar categoría |
| DELETE | `/api/categorias/{id}` | Eliminar categoría |

**Validaciones de negocio implementadas:**

1. **SKU único:**
```csharp
// ProductoService.cs - Líneas 23-29
var skuExists = await _unitOfWork.ProductoRepository.ExistsBySkuAsync(dto.Sku);
if (skuExists)
{
    result.stateOperation = false;
    result.MessageResult = $"Ya existe un producto con el SKU '{dto.Sku}'.";
    return result;
}
```

2. **Stock no negativo:**
```csharp
// ProductoService.cs - Líneas 39-44
if (dto.Stock < 0)
{
    result.stateOperation = false;
    result.MessageResult = "El stock no puede ser negativo.";
    return result;
}
```

3. **Validación de categoría activa:**
```csharp
// ProductoService.cs - Líneas 31-37
var categoriaExists = await _unitOfWork.CategoriaRepository.GetByIdAsync(dto.CategoriaId);
if (categoriaExists == null || !categoriaExists.Activo)
{
    result.stateOperation = false;
    result.MessageResult = "La categoria seleccionada no existe o esta inactiva.";
    return result;
}
```

4. **Validadores FluentValidation:**
```csharp
// CategoriaCreateValidator.cs
public class CategoriaCreateValidator : AbstractValidator<CategoriaCreateDto>
{
    public CategoriaCreateValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");
        
        RuleFor(x => x.Descripcion)
            .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");
    }
}
```

#### b) Módulo de Reportes

**Endpoint:** `GET /api/inventory/summary`

**Implementación:**
- **Controlador**: `Inventario.WebAPI/Controllers/InventoryController.cs`
- **Servicio**: `Inventario.Application/Services/InventoryReportService.cs`

**Datos procesados:**

1. **Valor total del inventario:**
```csharp
var valorTotalInventario = productos.Sum(p => p.Precio * p.Stock);
```

2. **Valor total por categoría:**
```csharp
var productosPorCategoria = productos
    .GroupBy(p => p.Categoria.Nombre)
    .Select(g => new CategoriaResumenDto
    {
        CategoriaNombre = g.Key,
        CantidadProductos = g.Count(),
        ValorTotal = g.Sum(p => p.Precio * p.Stock)
    }).ToList();
```

3. **Productos con stock crítico:**
```csharp
var productosStockCritico = productos
    .Where(p => p.Stock <= p.StockMinimo)
    .Select(p => new ProductoStockCriticoDto
    {
        Sku = p.Sku,
        Nombre = p.Nombre,
        Stock = p.Stock,
        StockMinimo = p.StockMinimo,
        CategoriaNombre = p.Categoria.Nombre
    }).ToList();
```

4. **Porcentaje de ocupación:**
```csharp
var porcentajeOcupacion = productos.Any()
    ? (double)productos.Count(p => p.Stock > p.StockMinimo) / productos.Count * 100
    : 0;
```

#### c) Seguridad con JWT

**Implementación:**
- **Controlador**: `Inventario.WebAPI/Controllers/AuthController.cs`
- **Configuración**: `Inventario.WebAPI/Program.cs` - Líneas 78-95

**Configuración de autenticación:**
```csharp
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretKey"]))
    };
});
```

**Generación de token JWT:**
```csharp
private string GenerateJwtToken(string username)
{
    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, username),
        new Claim(ClaimTypes.Role, "Admin")
    };

    var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _configuration["Authentication:Issuer"],
        audience: _configuration["Authentication:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

**Protección de endpoints:**
```csharp
[Authorize]  // Aplicado a todos los controladores protegidos
[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    // ... endpoints protegidos
}
```

### 3. Pruebas Unitarias

#### a) Framework de Pruebas

**Tecnologías utilizadas:**
- **xUnit**: Framework de pruebas unitarias
- **Moq**: Framework de mocking para simular dependencias
- **FluentAssertions** (opcional): Para aserciones más legibles

**Proyecto de pruebas:** `Inventario.Tests/Inventario.Tests.csproj`

#### b) Cobertura de Servicios de Lógica de Negocio

**Archivos de pruebas:**
- `ProductoServiceTests.cs` - 157 líneas de pruebas
- `CategoriaServiceTests.cs` - Pruebas de gestión de categorías
- `InventoryReportServiceTests.cs` - Pruebas del módulo de reportes

**Ejemplo de prueba con Moq:**
```csharp
[Fact]
public async Task Create_Product_Valid_ReturnsSuccess()
{
    // Arrange
    var dto = new ProductoCreateDto
    {
        Sku = "PROD-001",
        Nombre = "Producto Test",
        Descripcion = "Descripcion",
        Precio = 99.99m,
        Stock = 100,
        StockMinimo = 10,
        CategoriaId = 1
    };

    _mockProductoRepo.Setup(r => r.ExistsBySkuAsync(dto.Sku))
        .ReturnsAsync(false);
    _mockCategoriaRepo.Setup(r => r.GetByIdAsync(dto.CategoriaId))
        .ReturnsAsync(new Categoria { Id = 1, Nombre = "Test", Activo = true });
    _mockProductoRepo.Setup(r => r.CreateAsync(It.IsAny<Producto>()))
        .ReturnsAsync((Producto p) => { p.Id = 1; return p; });

    // Act
    var result = await _service.CreateAsync(dto);

    // Assert
    Assert.True(result.stateOperation);
    Assert.NotNull(result.Result);
    Assert.Equal(dto.Sku, result.Result.Sku);
}
```

**Escenarios de prueba cubiertos:**

1. **ProductoServiceTests:**
   - Crear producto válido
   - Crear producto con SKU duplicado
   - Crear producto con categoría inexistente
   - Crear producto con stock negativo
   - Actualizar producto válido
   - Actualizar producto inexistente
   - Eliminar producto válido
   - Obtener producto por ID
   - Listar todos los productos

2. **CategoriaServiceTests:**
   - Crear categoría válida
   - Crear categoría con nombre duplicado
   - Actualizar categoría válida
   - Eliminar categoría con productos asociados
   - Listar categorías activas

3. **InventoryReportServiceTests:**
   - Obtener resumen con datos válidos
   - Calcular valor total del inventario
   - Identificar productos con stock crítico
   - Calcular porcentaje de ocupación

**Simulación de Unit of Work:**
```csharp
public ProductoServiceTests()
{
    _mockUnitOfWork = new Mock<IUnitOfWorkInventory>();
    _mockProductoRepo = new Mock<IProductoRepository>();
    _mockCategoriaRepo = new Mock<ICategoriaRepository>();

    _mockUnitOfWork.Setup(u => u.ProductoRepository)
        .Returns(_mockProductoRepo.Object);
    _mockUnitOfWork.Setup(u => u.CategoriaRepository)
        .Returns(_mockCategoriaRepo.Object);

    _service = new ProductoService(_mockUnitOfWork.Object);
}
```

## Características Adicionales

### Documentación con Swagger

La API incluye documentación interactiva con Swagger UI:
- **URL**: `https://localhost:7147/swagger`
- **Características**:
  - Documentación automática de endpoints
  - Ejemplos de request/response
  - Soporte para autenticación JWT
  - Comentarios XML en el código

### CORS Configurado

El proyecto incluye configuración de CORS para permitir orígenes específicos:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowPolicySecureDomains", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

### Inyección de Dependencias Dinámica

Los servicios se registran automáticamente mediante reflexión:
```csharp
var generalServices = typeof(_Service).Assembly.GetTypes()
    .Where(type => !type.Name.StartsWith("_") && type.Name.EndsWith("Service"))
    .ToList();

foreach (var implementation in serviceImplementations)
{
    var interfaceName = $"I{implementation.Name}";
    var serviceInterface = serviceInterfaces.FirstOrDefault(i => i.Name == interfaceName);
    if (serviceInterface != null)
    {
        builder.Services.AddScoped(serviceInterface, implementation);
    }
}
```

## Tecnologías Utilizadas

- **ASP.NET Core 8.0** - Framework web
- **Entity Framework Core 8.0** - ORM
- **SQL Server** - Base de datos
- **JWT Bearer** - Autenticación
- **FluentValidation** - Validaciones
- **xUnit** - Framework de pruebas
- **Moq** - Mocking
- **Swagger/OpenAPI** - Documentación

## Autor
**Yeni Fernanda Hernandez**  
