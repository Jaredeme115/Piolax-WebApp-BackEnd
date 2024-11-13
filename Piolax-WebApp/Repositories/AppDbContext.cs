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
        public DbSet<usuario_area_rol> usuario_area_rol { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar la relación entre Empleado y StatusEmpleado
            modelBuilder.Entity<Empleado>()
                .HasOne(e => e.StatusEmpleado)
                .WithMany(s => s.Empleados)
                .HasForeignKey(e => e.idStatusEmpleado);

            modelBuilder.Entity<usuario_area_rol>()
                .HasOne(e => e.Empleado)
                .WithMany()
                .HasForeignKey(e => e.idEmpleado)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<usuario_area_rol>()
                .HasOne(e => e.Area)
                .WithMany()
                .HasForeignKey(e => e.idArea)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<usuario_area_rol>()
                .HasOne(e => e.Rol)
                .WithMany()
                .HasForeignKey(e => e.idRol)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }

    }
}
