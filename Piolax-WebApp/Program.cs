using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Piolax_WebApp.Services;
using Piolax_WebApp.Services.Impl;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Repositories.Impl;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MySQLContext");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
         connectionString ?? throw new InvalidOperationException("Connection string 'MySQLContext' not found."),
         ServerVersion.AutoDetect(connectionString),
         mysqlOptions => mysqlOptions.EnableRetryOnFailure()
    )
);

// Add services to the container.

builder.Services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();

builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


// Configure the HTTPS request pipeline.
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
string url = "http://localhost:7208";


builder.Services.AddCors(options =>
    options.AddPolicy(name: corsConfiguration,
        cors => cors.WithOrigins(url)
        .AllowAnyHeader()
        .AllowAnyMethod()
    )
);


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(option =>
                {
                    option.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TokenKey")),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

var app = builder.Build();


// Configure the HTTP request pipeline.
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
