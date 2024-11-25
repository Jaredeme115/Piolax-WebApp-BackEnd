using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Piolax_WebApp.Services;
using Piolax_WebApp.Services.Impl;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Repositories.Impl;
using System.Collections.Specialized;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MySQLContext");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
         connectionString ?? throw new InvalidOperationException("Connection string 'MySQLContext' not found."),
         ServerVersion.AutoDetect(connectionString),
         mysqlOptions => mysqlOptions.EnableRetryOnFailure()
    )
);

builder.Services.AddControllers();

builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();
builder.Services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
builder.Services.AddScoped<IAreasService, AreasService>();
builder.Services.AddScoped<IAreasRepository, AreasRepository>();
builder.Services.AddScoped<IRolesService, RolesService>();
builder.Services.AddScoped<IRolesRepository, RolesRepository>();
builder.Services.AddScoped<IStatusEmpleadoService, StatusEmpleadoService>();
builder.Services.AddScoped<IStatusEmpleadoRepository, StatusEmpleadoRepository>();
builder.Services.AddScoped<IMaquinasService, MaquinasService>();
builder.Services.AddScoped<IMaquinasRepository, MaquinasRepository>();
builder.Services.AddScoped<ITurnosService, TurnosService>();
builder.Services.AddScoped<ITurnosRepository, TurnosRepository>();
builder.Services.AddScoped<IStatusOrdenService, StatusOrdenService>();
builder.Services.AddScoped<IStatusOrdenRepository, StatusOrdenRepository>();
builder.Services.AddScoped<IStatusAprobacionSolicitanteService, StatusAprobacionSolicitanteService>();
builder.Services.AddScoped<IStatusAprobacionSolicitanteRepository, StatusAprobacionSolicitanteRepository>();
builder.Services.AddScoped<ISolicitudService, SolicitudService>();
builder.Services.AddScoped<ISolicitudesRepository, SolicitudRepository>();
builder.Services.AddScoped<IEmpleadoAreaRolService, EmpleadoAreaRolService>();
builder.Services.AddScoped<IEmpleadoAreaRolRepository, EmpleadoAreaRolRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IRefreshTokensService, RefreshTokensService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingresar Bearer [Espacio] token \r\n\r\n " +
                        "Ejemplo: Bearer ejoy<8878845468451418",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

string corsConfiguration = "_corsConfiguration";

builder.Services.AddCors(options =>
    options.AddPolicy(name: corsConfiguration,
        cors => cors.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
    )
);

//Configuración de JWT

var tokenKey = builder.Configuration["TokenKey"];
if (string.IsNullOrEmpty(tokenKey))
{
    throw new InvalidOperationException("TokenKey no está configurado en appsettings.json");
}

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(option =>
                {
                    option.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero // Elimina la tolerancia de tiempo
                    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(corsConfiguration);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
