using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.DTOs;

namespace Piolax_WebApp.Services.Impl
{
    public class NotificacionService : INotificacionService
    {
        private readonly AppDbContext _context;

        public NotificacionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CrearNotificacion(NotificacionDTO dto)
        {
            var notificacion = new Notificacion
            {
                titulo = dto.titulo,
                mensaje = dto.mensaje,
                fechaEnvio = DateTime.Now,
                leido = false,
                idEmpleado = dto.idEmpleado,
                tipoNotificacion = dto.tipoNotificacion,
            };

            _context.Notificaciones.Add(notificacion);
            await _context.SaveChangesAsync();
        }

        public async Task CrearNotificacionPorRol(string titulo, string mensaje, string tipoNotificacion, int idRol)
        {
            var empleados = await _context.EmpleadoAreaRol
                .Where(e => e.idRol == idRol)
                .ToListAsync();

            var notificaciones = empleados.Select(e => new Notificacion
            {
                titulo = titulo,
                mensaje = mensaje,
                fechaEnvio = DateTime.Now,
                leido = false,
                idEmpleado = e.idEmpleado,
                tipoNotificacion = tipoNotificacion,
            });

            _context.Notificaciones.AddRange(notificaciones);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notificacion>> ObtenerNotificacionesPorUsuario(int idEmpleado)
        {
            return await _context.Notificaciones
                .Where(n => n.idEmpleado == idEmpleado)
                .OrderByDescending(n => n.fechaEnvio)
                .ToListAsync();
        }

        public async Task MarcarComoLeida(int id)
        {
            var notificacion = await _context.Notificaciones.FindAsync(id);
            if (notificacion != null)
            {
                notificacion.leido = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}