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
        public DbSet<Maquinas> Maquinas { get; set; } = default!;
        public DbSet<usuario_area_rol> usuario_area_rol { get; set; } = default!;
        public DbSet<Turnos> Turnos { get; set; } = default!;
        public DbSet<StatusOrden> StatusOrden { get; set; } = default!;
        public DbSet<StatusAprobacionSolicitante> StatusAprobacionSolicitante { get; set; } = default!;
        public DbSet<Solicitudes> Solicitudes { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar la relación entre Empleado y StatusEmpleado
            modelBuilder.Entity<Empleado>()
                .HasOne(e => e.StatusEmpleado)
                .WithMany(s => s.Empleados)
                .HasForeignKey(e => e.idStatusEmpleado);

            modelBuilder.Entity<Maquinas>()
                .HasOne(m => m.Area)
                .WithMany(a => a.Maquinas)
                .HasForeignKey(m => m.idArea);

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

            // Configurar relaciones de Solicitudes (opcional)
            modelBuilder.Entity<Solicitudes>()
                .HasOne(s => s.Empleado)
                .WithMany(e => e.Solicitudes) // Relacion Empleado-Solicitudes 1:N
                .HasForeignKey(s => s.idEmpleado);

            modelBuilder.Entity<Solicitudes>()
                .HasOne(s => s.Maquina)
                .WithMany(m => m.Solicitudes) // Relacion Maquina-Solicitudes 1:N
                .HasForeignKey(s => s.idMaquina);

            modelBuilder.Entity<Solicitudes>()
                .HasOne(s => s.Turno)
                .WithMany(t => t.Solicitudes) // Relacion Turno-Solicitudes 1:N
                .HasForeignKey(s => s.idTurno);

            modelBuilder.Entity<Solicitudes>()
                .HasOne(s => s.StatusOrden)
                .WithMany(so => so.Solicitudes) // Relacion StatusOrden-Solicitudes 1:N
                .HasForeignKey(s => s.idStatusOrden);

            modelBuilder.Entity<Solicitudes>()
                .HasOne(s => s.StatusAprobacionSolicitante)
                .WithMany(sas => sas.Solicitudes) // Relacion StatusAprobacionSolicitante-Solicitudes 1:N
                .HasForeignKey(s => s.idStatusAprobacionSolicitante);

            base.OnModelCreating(modelBuilder);
        }

    }
}
