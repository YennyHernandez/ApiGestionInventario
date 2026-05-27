using ApiTableroPowerBiFepep.Domain.Models;
using ApiTableroPowerBiFepep.Infrastructure.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ApiTableroPowerBIFepep.Utils.Security;

var builder = WebApplication.CreateBuilder(args);

#region CONFIG

IConfigurationSection configSection =
    builder.Configuration.GetSection("SectionConfiguration");

builder.Services.Configure<SectionConfiguration>(configSection);

IEncryptionService encrypt = new EncryptionService();

#endregion

#region DB (SOLO UNA)

string connectionEncrypted =
    builder.Configuration.GetValue<string>("ConnectionStrings:ConnetionVQualityFepep") ?? "";

string connectionString = encrypt.Decrypt(connectionEncrypted);

builder.Services.AddDbContext<ContextSql>(options =>
    options.UseSqlServer(connectionString));

#endregion

#region JWT

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

builder.Services.AddAuthorization();

#endregion

#region CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

#endregion

#region API CONTROLLERS

builder.Services.AddControllers();

#endregion

var app = builder.Build();

#region PIPELINE (IMPORTANTE)

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();