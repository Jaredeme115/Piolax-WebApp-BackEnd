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
        public DbSet<Turnos> Turnos { get; set; } = default!;
        public DbSet<StatusOrden> StatusOrden { get; set; } = default!;
        public DbSet<StatusAprobacionSolicitante> StatusAprobacionSolicitante { get; set; } = default!;
        public DbSet<Solicitudes> Solicitudes { get; set; } = default!;

        public DbSet<RefreshTokens> RefreshTokens { get; set; } = default!;

        public DbSet<EmpleadoAreaRol> EmpleadoAreaRol { get; set; } = default!;

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

            modelBuilder.Entity<EmpleadoAreaRol>()
            .HasKey(uar => new { uar.idEmpleado, uar.idArea, uar.idRol });

            modelBuilder.Entity<EmpleadoAreaRol>()
                .HasOne(uar => uar.Empleado)
                .WithMany(e => e.EmpleadoAreaRol)
                .HasForeignKey(uar => uar.idEmpleado);

            modelBuilder.Entity<EmpleadoAreaRol>()
                .HasOne(uar => uar.Area)
                .WithMany(a => a.EmpleadoAreaRol)
                .HasForeignKey(uar => uar.idArea);

            modelBuilder.Entity<EmpleadoAreaRol>()
                .HasOne(uar => uar.Rol)
                .WithMany(r => r.EmpleadoAreaRol)
                .HasForeignKey(uar => uar.idRol);

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

            // Configurar la relación entre RefreshTokens y Empleado

            modelBuilder.Entity<RefreshTokens>()
               .HasOne(rt => rt.Empleado)
               .WithMany(e => e.RefreshTokens)
               .HasForeignKey(rt => rt.idEmpleado);

            base.OnModelCreating(modelBuilder);

        }

    }
}
