using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Piolax_WebApp.Models;
using System;

namespace Piolax_WebApp.Repositories
{
    public class AppDbContext (DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Empleado> Empleado { get; set; } = default!;
        
    }
}
