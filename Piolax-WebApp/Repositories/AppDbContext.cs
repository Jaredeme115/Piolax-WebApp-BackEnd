using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Piolax_WebApp.Models;
using System;

namespace Piolax_WebApp.Repositories
{
    public class AppDbContext (DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Empleado> Empleado { get; set; } = default!;
        public DbSet<Areas> Areas { get; set; } = default!;
        public DbSet<Roles> Roles { get; set; } = default!;
        public DbSet<StatusEmpleado> StatusEmpleado { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar la relación entre Empleado y StatusEmpleado
            modelBuilder.Entity<Empleado>()
                .HasOne(e => e.StatusEmpleado)
                .WithMany(s => s.Empleados)
                .HasForeignKey(e => e.idStatusEmpleado);

            base.OnModelCreating(modelBuilder);
        }

    }
}
