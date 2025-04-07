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
        public DbSet<categoriaTicket> categoriaTicket { get; set; } = default!;

        //Inventario
        public DbSet<Inventario> Inventario { get; set; } = default!;

        //InventarioCategorias
        public DbSet<InventarioCategorias> InventarioCategorias { get; set; } = default!;

        //Asignaciones
        public DbSet<Asignaciones> Asignaciones { get; set; } = default!;
        public DbSet<StatusAprobacionTecnico> StatusAprobacionTecnico { get; set; } = default!;
        public DbSet<asignacion_refacciones> asignacion_refacciones { get; set; } = default!;
        public DbSet<Asignacion_Tecnico> Asignacion_Tecnico { get; set; } = default!;
        public DbSet<StatusAsignacion> StatusAsignacion { get; set; } = default!;

        //KPI´s Mantenimiento
        public DbSet<KpisMantenimiento> KpisMantenimiento { get; set; } = default!;
        public DbSet<KpisDetalle> KpisDetalle { get; set; } = default!;



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

            // Configurar la relación entre Solicitudes y Empleado, Maquina, Turno, StatusOrden, StatusAprobacionSolicitante, CategoriaTicket
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

            modelBuilder.Entity<Solicitudes>()
                .HasOne(s => s.categoriaTicket)
                .WithMany(ct => ct.Solicitudes) // Relacion CategoriaTicket-Solicitudes 1:N
                .HasForeignKey(s => s.idCategoriaTicket);


            // Configurar la relación entre Asignaciones y Solicitud, StatusAsignacion
            modelBuilder.Entity<Asignaciones>()
                .HasOne(a => a.Solicitud)
                .WithMany(s => s.Asignaciones)
                .HasForeignKey(a => a.idSolicitud);

            modelBuilder.Entity<StatusAsignacion>()
                .HasMany(sa => sa.Asignaciones)
                .WithOne(a => a.StatusAsignacion)
                .HasForeignKey(a => a.idStatusAsignacion);


            // Configurar la relación entre Inventario y Areas, Maquinas, InventarioCategorias
            modelBuilder.Entity<Inventario>()
                .HasOne(i => i.Areas)
                .WithMany(a => a.Inventario)
                .HasForeignKey(i => i.idArea);

            modelBuilder.Entity<Inventario>()
                .HasOne(i => i.Maquinas)
                .WithMany(m => m.Inventario)
                .HasForeignKey(i => i.idMaquina);

            modelBuilder.Entity<Inventario>()
                .HasOne(i => i.InventarioCategorias)
                .WithMany(ic => ic.Inventario)
                .HasForeignKey(i => i.idInventarioCategoria);

            // Configurar la relación entre asignacion_refacciones y Asignaciones, Inventario, Asignacion_Tecnico
            modelBuilder.Entity<Inventario>()
                .HasMany(i => i.Asignacion_Refacciones)
                .WithOne(a => a.Inventario)
                .HasForeignKey(a => a.idRefaccion);

            modelBuilder.Entity<Asignaciones>()
                .HasMany(a => a.Asignacion_Refacciones)
                .WithOne(ar => ar.Asignaciones)
                .HasForeignKey(ar => ar.idAsignacion);

            modelBuilder.Entity<Asignacion_Tecnico>()
                .HasMany(at => at.Asignacion_Refacciones)
                .WithOne(ar => ar.Asignacion_Tecnicos)
                .HasForeignKey(ar => ar.idAsignacionTecnico);


            // Configurar la relación entre Asignacion_Tecnico y Asignaciones, Empleado, StatusAprobacionTecnico
            modelBuilder.Entity<Asignaciones>()
                .HasMany(a => a.Asignacion_Tecnico)
                .WithOne(at => at.Asignacion)
                .HasForeignKey(at => at.idAsignacion);

            modelBuilder.Entity<Empleado>()
                .HasMany(e => e.Asignacion_Tecnico)
                .WithOne(at => at.Empleado)
                .HasForeignKey(at => at.idEmpleado);

            modelBuilder.Entity<StatusAprobacionTecnico>()
                .HasMany(sat => sat.Asignacion_Tecnicos)
                .WithOne(at => at.StatusAprobacionTecnico)
                .HasForeignKey(at => at.idStatusAprobacionTecnico);


            // Configurar la relación entre EstatusInventario, Inventario
            modelBuilder.Entity<Inventario>()
                .Property(e => e.EstatusInventario)
                .HasConversion<string>()
                .HasColumnType("ENUM('Disponible', 'Pendiente', 'EnReparación')");


            // Configurar la propiedad fechaActualizacion de Inventario
            modelBuilder.Entity<Inventario>()
                .Property(i => i.fechaActualizacion)
                .HasColumnType("DATETIME")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAddOrUpdate(); // Indica que se genera tanto al crear como al actualizar

            // Configurar la relacion entre KpisMantenimiento y Empleados
            modelBuilder.Entity<KpisMantenimiento>()
                .HasOne(km => km.Empleado)
                .WithMany(e => e.KpisMantenimientos)
                .HasForeignKey(km => km.idEmpleado);

            // Configurar la relacion entre KpisMantenimiento y Areas
            modelBuilder.Entity<KpisMantenimiento>()
                .HasOne(km => km.Area)
                .WithMany(a => a.KpisMantenimientos)
                .HasForeignKey(km => km.idArea);

            // Configurar la relacion entre KpisMantenimiento y Maquinas   
            modelBuilder.Entity<KpisMantenimiento>()
                .HasOne(km => km.Maquina)
                .WithMany(m => m.KpisMantenimientos)
                .HasForeignKey(km => km.idMaquina);

            // Configurar la relacion entre KpisDetalles y KpisMantenimiento
            modelBuilder.Entity<KpisDetalle>()
                .HasOne(kd => kd.KpisMantenimiento)
                .WithMany(km => km.KpisDetalle)
                .HasForeignKey(kd => kd.idKPIMantenimiento);


            base.OnModelCreating(modelBuilder);

        }

    }
}
