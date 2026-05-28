using Inventario.Application.Services;
using Inventario.Application.Services.Interfaces;
using Inventario.Application.Validators;
using Inventario.Domain.Models;
using Inventario.Infrastructure.Context;
using Inventario.Infrastructure.Repositories;
using Inventario.Infrastructure.Repositories.Interfaces;
using Inventario.Infrastructure.Repositories._UnitOfWork;
using Inventario.Infrastructure.Seeders;
using Inventario.Utils.Security;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
var builder = WebApplication.CreateBuilder(args);
IEncryptionService Encrypt = new EncryptionService();
IConfigurationSection seccionConfiguracion = builder.Configuration.GetSection("SectionConfiguration");
IConfigurationSection seccionConnectionStrings = builder.Configuration.GetSection("ConnectionStrings");

builder.Services.Configure<SectionConfiguration>(seccionConfiguracion);
builder.Services.Configure<ConnectionStrings>(seccionConnectionStrings);
var configuracionAppSettings = seccionConfiguracion.Get<SectionConfiguration>();
var configuracionConnectionStrings = seccionConnectionStrings.Get<ConnectionStrings>();
var allowedOrigins = (configuracionAppSettings?.SecureDomains ?? Array.Empty<string>())
    .Where(origin => !string.IsNullOrWhiteSpace(origin))
    .Select(origin => origin.Trim().TrimEnd('/'))
    .Concat(new[]
    {
        "http://localhost:5173",
        "http://127.0.0.1:5173"
    })
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray();

string DecryptConnectionString(string encryptedConnectionString)
{
    return string.IsNullOrEmpty(encryptedConnectionString) ? null : Encrypt.Decrypt(encryptedConnectionString);
}

if (builder.Configuration.GetSection("ConnectionStrings:ConnetionToken").Exists())
{
    string GetConnetionToken = DecryptConnectionString(configuracionConnectionStrings.ConnetionToken);
}

if (builder.Configuration.GetSection("ConnectionStrings:ConnetionGenerico").Exists())
{
    string ConnetionGenerico = DecryptConnectionString(configuracionConnectionStrings.ConnetionGenerico);
    if (!string.IsNullOrEmpty(ConnetionGenerico))
    {
        builder.Services.AddDbContext<ContextSql>(opt => opt.UseSqlServer(ConnetionGenerico));
    }
}

builder.Services.AddScoped<IEncryptionService, EncryptionService>();

#region Registro dinámico de servicios (Dynamic Services Injection)
var generalServices = typeof(_Service).Assembly.GetTypes()
    .Where(type => !type.Name.StartsWith("_") && type.Name.EndsWith("Service"))
    .ToList();

var serviceInterfaces = generalServices.Where(type => type.IsInterface);
var serviceImplementations = generalServices.Where(type => type.IsClass);

foreach (var implementation in serviceImplementations)
{
    var interfaceName = $"I{implementation.Name}";
    var serviceInterface = serviceInterfaces.FirstOrDefault(i => i.Name == interfaceName);
    if (serviceInterface != null)
    {
        builder.Services.AddScoped(serviceInterface, implementation);
    }
}
#endregion

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretKey"]))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("V1", new OpenApiInfo { Title = "Gestión de inventario", Version = "V1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        In = ParameterLocation.Header,
        Description = "Enter JWT with bearer format like 'Bearer [Token]'"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    opt.CustomSchemaIds(type => type.FullName);
    opt.DocInclusionPredicate((docName, apiDesc) =>
    {
        return apiDesc.GroupName == null || !apiDesc.GroupName.Equals("Hidden", StringComparison.OrdinalIgnoreCase);
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    opt.IncludeXmlComments(xmlPath);
});

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

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddEndpointsApiExplorer();

#region Inventory API Registrations

if (builder.Configuration.GetSection("ConnectionStrings:ConnetionGenerico").Exists())
{
    string ConnetionGenerico = DecryptConnectionString(configuracionConnectionStrings.ConnetionGenerico);
    if (!string.IsNullOrEmpty(ConnetionGenerico))
    {
        builder.Services.AddDbContext<ContextInventory>(opt => opt.UseSqlServer(ConnetionGenerico));
    }
}

builder.Services.AddScoped<IUnitOfWorkInventory, UnitOfWorkInventory>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IInventoryReportService, InventoryReportService>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

#endregion

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ContextInventory>();
    await context.Database.MigrateAsync();
    await InventorySeeder.SeedAsync(context);
}

app.UseCors("AllowPolicySecureDomains");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(opt => opt.RouteTemplate = "swagger/{documentName}/swagger.json");
    app.UseSwaggerUI(opt => opt.SwaggerEndpoint("V1/swagger.json", "Gestion Inventario"));
}

// Solo usar HTTPS redirection en producción
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
