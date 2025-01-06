using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Piolax_WebApp.Models;
using System;

namespace Piolax_WebApp.Repositories
{
    public class AppDbContext (DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        //Empleado
        public DbSet<Empleado> Empleado { get; set; } = default!;
        public DbSet<RefreshTokens> RefreshTokens { get; set; } = default!;
        public DbSet<StatusEmpleado> StatusEmpleado { get; set; } = default!;
        public DbSet<EmpleadoAreaRol> EmpleadoAreaRol { get; set; } = default!;

        //Areas
        public DbSet<Areas> Areas { get; set; } = default!;

        //Roles
        public DbSet<Roles> Roles { get; set; } = default!;

        //Maquinas
        public DbSet<Maquinas> Maquinas { get; set; } = default!;

        //Turnos
        public DbSet<Turnos> Turnos { get; set; } = default!;

        //Solicitudes
        public DbSet<StatusOrden> StatusOrden { get; set; } = default!;
        public DbSet<StatusAprobacionSolicitante> StatusAprobacionSolicitante { get; set; } = default!;
        public DbSet<Solicitudes> Solicitudes { get; set; } = default!;

        //Inventario
        public DbSet<Inventario> Inventario { get; set; } = default!;

        //Asignaciones
        public DbSet<Asignaciones> Asignaciones { get; set; } = default!;
        public DbSet<StatusAprobacionTecnico> StatusAprobacionTecnico { get; set; } = default!;
        public DbSet<asignacion_refacciones> asignacion_refacciones { get; set; } = default!;
        public DbSet<CategoriaAsignacion> CategoriaAsignacion { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar la relación entre Empleado y StatusEmpleado
            modelBuilder.Entity<Empleado>()
                .HasOne(e => e.StatusEmpleado)
                .WithMany(s => s.Empleados)
                .HasForeignKey(e => e.idStatusEmpleado);

            // Configurar la relación entre RefreshTokens y Empleado
            modelBuilder.Entity<RefreshTokens>()
               .HasOne(rt => rt.Empleado)
               .WithMany(e => e.RefreshTokens)
               .HasForeignKey(rt => rt.idEmpleado);

            // Configurar la relación entre Maquinas y Areas
            modelBuilder.Entity<Maquinas>()
                .HasOne(m => m.Area)
                .WithMany(a => a.Maquinas)
                .HasForeignKey(m => m.idArea);

            // Configurar la relación entre EmpleadoAreaRol y Empleado, Area y Rol
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

            // Configurar la relación entre Solicitudes y Empleado, Maquina, Turno, StatusOrden, StatusAprobacionSolicitante
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


            // Configurar la relación entre Asignaciones y Solicitud, Empleado, Inventario, CategoriaAsignacion, StatusAprobacionTecnico
            modelBuilder.Entity<Asignaciones>()
                .HasOne(a => a.Solicitud)
                .WithMany(s => s.Asignaciones)
                .HasForeignKey(a => a.idSolicitud);

            modelBuilder.Entity<Asignaciones>()
                .HasOne(a => a.Empleado)
                .WithMany(e => e.Asignaciones)
                .HasForeignKey(a => a.idEmpleado);

            modelBuilder.Entity<Asignaciones>()
                .HasOne(a => a.Inventario)
                .WithMany(i => i.Asignaciones)
                .HasForeignKey(a => a.idRefaccion);

            modelBuilder.Entity<Asignaciones>()
                .HasOne(a => a.CategoriaAsignacion)
                .WithMany(ca => ca.Asignaciones)
                .HasForeignKey(a => a.idCategoriaAsignacion);

            modelBuilder.Entity<Asignaciones>()
                .HasOne(a => a.StatusAprobacionTecnico)
                .WithMany(sat => sat.Asignaciones)
                .HasForeignKey(a => a.idStatusAprobacionTecnico);

            // Configurar la relación entre Inventario y Areas, Maquinas
            modelBuilder.Entity<Inventario>()
                .HasOne(i => i.Areas)
                .WithMany(a => a.Inventario)
                .HasForeignKey(i => i.idArea);

            modelBuilder.Entity<Inventario>()
                .HasOne(i => i.Maquinas)
                .WithMany(m => m.Inventario)
                .HasForeignKey(i => i.idMaquina);

            // Configurar la relación entre asignacion_refacciones y Asignaciones, Inventario
            modelBuilder.Entity<Inventario>()
                .HasMany(i => i.Asignaciones)
                .WithOne(a => a.Inventario)
                .HasForeignKey(a => a.idRefaccion);

            modelBuilder.Entity<Asignaciones>()
                .HasMany(a => a.Asignacion_Refacciones)
                .WithOne(ar => ar.Asignaciones)
                .HasForeignKey(ar => ar.idAsignacion);


            base.OnModelCreating(modelBuilder);

        }

    }
}
