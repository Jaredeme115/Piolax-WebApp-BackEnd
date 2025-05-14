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

// A�ade el servicio de KPIs en tiempo real
builder.Services.AddHostedService<KPIRealTimeService>();

//Obtener URL dinamicamente
builder.Services.AddHttpContextAccessor();



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


//Conexion con Localhost

/*builder.Services.AddCors(options =>
    options.AddPolicy(name: corsConfiguration,
        cors => cors.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
    )
);*/


//Conexion para exponer URL a internet  

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsConfiguration, cors =>
    {
        cors
            .SetIsOriginAllowed(origin =>
            {
                // Permite localhost y cualquier t�nel de Cloudflare (trycloudflare.com)
                return origin == "http://localhost:4200" ||
                       new Uri(origin).Host.EndsWith("trycloudflare.com");
            })
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

//Configuraci�n de JWT

// Agregado para Notificaciones de quien Solicito - Limpia el mapeo por defecto del handler
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var tokenKey = builder.Configuration["TokenKey"];
if (string.IsNullOrEmpty(tokenKey))
{
    throw new InvalidOperationException("TokenKey no est� configurado en appsettings.json");
}

builder.Services.AddScoped<ITokenService, TokenService>();

//Localhost
/*builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
});*/

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

            // Validaci�n din�mica de Issuer
            IssuerValidator = (issuer, token, parameters) =>
            {
                if (issuer.StartsWith("https://localhost") || issuer.Contains("trycloudflare.com"))
                {
                    return issuer;
                }

                throw new SecurityTokenInvalidIssuerException("Issuer no v�lido.");
            },

            // Validaci�n din�mica de Audience
            AudienceValidator = (audiences, token, parameters) =>
            {
                foreach (var audience in audiences)
                {
                    if (audience.StartsWith("https://localhost") || audience.Contains("trycloudflare.com"))
                    {
                        return true;
                    }
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


//Configuraci�n de roles

builder.Services.AddAuthorization(options =>
{
    //options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole(
            "Tecnico",        // tu rol original
            "Supervisor",     // rol adicional
            "Assistant Manager", // otro rol
            "Manager",         // cuantos roles necesites
            "Lider"
        )
    );
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Permitir al CORS para archivos est�ticos
/*app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:4200");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, OPTIONS");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization");
    }
});*/

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        var request = ctx.Context.Request;
        var response = ctx.Context.Response;

        // Tomamos el origen de la petici�n
        var origin = request.Headers["Origin"].FirstOrDefault();

        // Si viene de localhost:4200 o contiene "trycloudflare.com", lo permitimos
        if (!string.IsNullOrEmpty(origin) &&
            (origin == "http://localhost:4200" || origin.Contains("trycloudflare.com")))
        {
            response.Headers.Append("Access-Control-Allow-Origin", origin);
            response.Headers.Append("Access-Control-Allow-Methods", "GET, OPTIONS");
            response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization");
        }
    }
});



// Permitir servir archivos est�ticos
app.UseStaticFiles();

app.UseHttpsRedirection();

//Para conexion con localhost y url publica
app.UseCors(corsConfiguration);

//Para exponer URL publica
//app.UseCors("AllowLocalTunnel");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Endpoint de SignalR
app.MapHub<AsignacionHub>("/AsignacionHub");
app.MapHub<NotificationHub>("/NotificationHub");

app.Run();
