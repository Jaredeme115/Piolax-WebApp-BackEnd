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

//Lo agregue por problemas de referencias cíclicas en la serialización JSON
/*builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });*/


//Empleado
builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();
builder.Services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();

//Areas
builder.Services.AddScoped<IAreasService, AreasService>();
builder.Services.AddScoped<IAreasRepository, AreasRepository>();

//Roles
builder.Services.AddScoped<IRolesService, RolesService>();
builder.Services.AddScoped<IRolesRepository, RolesRepository>();

//StatusEmpleado
builder.Services.AddScoped<IStatusEmpleadoService, StatusEmpleadoService>();
builder.Services.AddScoped<IStatusEmpleadoRepository, StatusEmpleadoRepository>();

//Maquinas
builder.Services.AddScoped<IMaquinasService, MaquinasService>();
builder.Services.AddScoped<IMaquinasRepository, MaquinasRepository>();

//Turnos
builder.Services.AddScoped<ITurnosService, TurnosService>();
builder.Services.AddScoped<ITurnosRepository, TurnosRepository>();

//StatusOrden
builder.Services.AddScoped<IStatusOrdenService, StatusOrdenService>();
builder.Services.AddScoped<IStatusOrdenRepository, StatusOrdenRepository>();

//StatusAprobacionSolicitante
builder.Services.AddScoped<IStatusAprobacionSolicitanteService, StatusAprobacionSolicitanteService>();
builder.Services.AddScoped<IStatusAprobacionSolicitanteRepository, StatusAprobacionSolicitanteRepository>();

//Solicitudes
builder.Services.AddScoped<ISolicitudService, SolicitudService>();
builder.Services.AddScoped<ISolicitudesRepository, SolicitudRepository>();

//Empleado-Area-Rol
builder.Services.AddScoped<IEmpleadoAreaRolService, EmpleadoAreaRolService>();
builder.Services.AddScoped<IEmpleadoAreaRolRepository, EmpleadoAreaRolRepository>();

//Refresh Token
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IRefreshTokensService, RefreshTokensService>();

//Inventario
builder.Services.AddScoped<IInventarioRepository, InventarioRepository>();
builder.Services.AddScoped<IInventarioService, InventarioService>();

//InventarioCategorias
builder.Services.AddScoped<IInventarioCategoriasRepository, InventarioCategoriasRepository>();
builder.Services.AddScoped<IInventarioCategoriasService, InventarioCategoriasService>();

//CategoriasTicket
builder.Services.AddScoped<ICategoriaTicketRepository, CategoriaTicketRepository>();
builder.Services.AddScoped<ICategoriaTicketService, CategoriaTicketService>();

//StatusAsignacion
builder.Services.AddScoped<IStatusAsignacionRepository, StatusAsignacionRepository>();
builder.Services.AddScoped<IStatusAsignacionService, StatusAsignacionService>();

//Asignaciones
builder.Services.AddScoped<IAsignacionRepository, AsignacionRepository>();
builder.Services.AddScoped<IAsignacionService, AsignacionService>();

//AsignacionTecnico
builder.Services.AddScoped<IAsignacionTecnicosRepository, AsignacionTecnicosRepository>();
builder.Services.AddScoped<IAsignacionTecnicosService, AsignacionTecnicosService>();

//AsignacionRefacciones
builder.Services.AddScoped<IAsignacionRefaccionesRepository, AsignacionRefaccionesRepository>();
builder.Services.AddScoped<IAsignacionRefaccionesService, AsignacionRefaccionesService>();



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


//Conexion con Localhost

builder.Services.AddCors(options =>
    options.AddPolicy(name: corsConfiguration,
        cors => cors.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
    )
);


//Conexion con NGROK

/*builder.Services.AddCors(options =>
    options.AddPolicy(name: corsConfiguration,
        cors => cors.WithOrigins("http://localhost:4200", "https://5105-200-188-149-29.ngrok-free.app")
        .AllowAnyHeader()
        .AllowAnyMethod()
    )
);*/

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
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"], // Usar el Issuer configurado
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["Jwt:Audience"], // Usar el Audience configurado
                        ClockSkew = TimeSpan.Zero
                    };
});

//Configuración de roles

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
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
