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
using Piolax_WebApp.BackgroundServices; 
using Piolax_WebApp.Hubs;
using Piolax_WebApp.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

//Para cargar el archivo de configuración
using Microsoft.AspNetCore.Http.Features;
using Piolax_WebApp.Services.BackgroundServices;



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
builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();

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

//MantenimientoPreventivo
builder.Services.AddScoped<IMantenimientoPreventivoRepository, MantenimientoPreventivoRepository>();
builder.Services.AddScoped<IMantenimientoPreventivoService, MantenimientoPreventivoService>();

//Frecuencia y Estatus de Mantenimiento Preventivo 
builder.Services.AddScoped<IFrecuenciaMPRepository, FrecuenciaMPRepository>();
builder.Services.AddScoped<IFrecuenciaMPService, FrecuenciaMPService>();
builder.Services.AddScoped<IEstatusPreventivoRepository, EstatusPreventivoRepository>();
builder.Services.AddScoped<IEstatusPreventivoService, EstatusPreventivoService>();

//MantenimientoPreventivoPDFs
builder.Services.AddScoped<IMantenimientoPreventivoPDFsRepository, MantenimientoPreventivoPDFsRepository>();
builder.Services.AddScoped<IMantenimientoPreventivoPDFsService, MantenimientoPreventivoPDFsService>();

//MantenimientoPreventivoEjecucion
builder.Services.AddScoped<IMantenimientoPreventivoEjecucionRepository, MantenimientoPreventivoEjecucionRepository>();
builder.Services.AddScoped<IMantenimientoPreventivoEjecucionService, MantenimientoPreventivoEjecucionService>();

//MantenimientoPreventivoRefacciones
builder.Services.AddScoped<IMantenimientoPreventivoRefaccionesRepository, MantenimientoPreventivoRefaccionesRepository>();
builder.Services.AddScoped<IMantenimientoPreventivoRefaccionesService, MantenimientoPreventivoRefaccionService>();

//MantenimientoPreventivoObservaciones
builder.Services.AddScoped<IObservacionesMPRepository, ObservacionesMPRepository>();
builder.Services.AddScoped<IObservacionesMPService, ObservacionesMPService>();

//KPI's 
builder.Services.AddScoped<IKPIRepository, KPIRepository>();
builder.Services.AddScoped<IKPIMantenimientoPreventivoService, KPIMantenimientoPreventivoService>();

//Dashboard
builder.Services.AddScoped<IKPIRepository, KPIRepository>();
builder.Services.AddScoped<IKPIDashboardService, KPIDashboardService>();

//Notificaciones
//builder.Services.AddScoped<NewRequestNotificationService>();
builder.Services.AddHostedService<LowStockNotificationService>();

// Añade el servicio de KPIs en tiempo real
builder.Services.AddHostedService<KPIRealTimeService>();

// Para cerrar orden pasados 15 minutos
builder.Services.AddHostedService<AutoApprovalService>();


builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();


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

    //options.OperationFilter<FileUploadOperation>();
});

string corsConfiguration = "_corsConfiguration";

// Conexión para exponer URL a la Red Empresarial

// Registrar la política CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsConfiguration, cors =>
    {
        cors.WithOrigins("http://192.168.1.95:83") // cambia a la IP real del servidor
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

//Configuración de JWT

// Agregado para Notificaciones de quien Solicito - Limpia el mapeo por defecto del handler
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var tokenKey = builder.Configuration["TokenKey"];
if (string.IsNullOrEmpty(tokenKey))
{
    throw new InvalidOperationException("TokenKey no está configurado en appsettings.json");

}
builder.Services.AddScoped<ITokenService, TokenService>();


//Exponer URL a internet
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        //var tokenKey = builder.Configuration["TokenKey"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            //Agregado
            NameClaimType = ClaimTypes.NameIdentifier,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ClockSkew = TimeSpan.Zero,

        
            IssuerValidator = (issuer, token, parameters) =>
            {
                if (issuer.StartsWith("http://localhost") || issuer.StartsWith("http://192.168.1.95"))
                    return issuer;

                throw new SecurityTokenInvalidIssuerException("Issuer no válido.");
            },

            AudienceValidator = (audiences, token, parameters) =>
            {
                foreach (var audience in audiences)
                {
                    if (audience.StartsWith("http://localhost") || audience.StartsWith("http://192.168.1.95"))
                        return true;
                }
                return false;
            }
        };

        //Agregado para Notificaciones

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Extrae el token de la query string "access_token"
                var accessToken = context.Request.Query["access_token"];

                // Solo para rutas de hubs de SignalR
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/NotificationHub") ||
                     path.StartsWithSegments("/AsignacionHub")))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });


//Configuración de roles

builder.Services.AddAuthorization(options =>
{
    //options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole(
            "Admin",         // tu rol original
            "Tecnico",        // tu rol original
            "Supervisor",     // rol adicional
            "Assistant Manager", // otro rol
            "Manager",         // cuantos roles necesites
            "Lider"
        )
    );
});

builder.Services.Configure<FormOptions>(opts =>
{
    opts.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50 MB
});


var app = builder.Build();



//Usar esta función unicamente para debugeo en desarrollo, NO DEJAR EN PRODUCCIÓN ==> app.UseDeveloperExceptionPage();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        var request = ctx.Context.Request;
        var response = ctx.Context.Response;

        // Tomamos el origen de la petición
        var origin = request.Headers["Origin"].FirstOrDefault();

        // Si viene de localhost:4200 o contiene "trycloudflare.com", lo permitimos
        if (!string.IsNullOrEmpty(origin) &&
            (origin.StartsWith("http://192.168.")))
        {
            response.Headers.Append("Access-Control-Allow-Origin", origin);
            response.Headers.Append("Access-Control-Allow-Methods", "GET, OPTIONS");
            response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization");
        }
    }
});


// Permitir servir archivos estáticos
app.UseStaticFiles();

//Para conexion con localhost y url publica
app.UseCors(corsConfiguration);

//Usar esta función solo si la aplicación acepta HTTPS ====> app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Endpoint de SignalR
app.MapHub<AsignacionHub>("/AsignacionHub");
app.MapHub<NotificationHub>("/NotificationHub");

app.Run();