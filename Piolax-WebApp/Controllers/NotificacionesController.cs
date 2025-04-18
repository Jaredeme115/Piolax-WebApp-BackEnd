using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificacionesController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Obtener notificaciones no leídas por usuario

        [HttpGet("usuario/{idEmpleado}/{rol}")]
        public async Task<IActionResult> ObtenerPorEmpleadoYRol(int idEmpleado, string rol)
        {
            var notificaciones = await _context.Notificaciones
                .Where(n =>
                    (n.idEmpleadoDestino == idEmpleado || n.rolDestino == rol)
                    && n.idEstadoNotificacion == 1)
                .OrderByDescending(n => n.fechaEnvio)
                .ToListAsync();

            return Ok(notificaciones);
        }


        // ✅ Marcar notificación como leída
        [HttpPost("marcar-leido/{id}")]
        public async Task<IActionResult> MarcarLeido(int id)
        {
            var noti = await _context.Notificaciones.FindAsync(id);
            if (noti == null) return NotFound();

            noti.idEstadoNotificacion = 2;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // ✅ Crear una nueva notificación
        [HttpPost("crear")]
        public async Task<IActionResult> Crear([FromBody] Notificacion noti)
        {
            noti.fechaEnvio = DateTime.Now;
            noti.idEstadoNotificacion = 1;
            _context.Notificaciones.Add(noti);
            await _context.SaveChangesAsync();

            return Ok(noti);
        }
    }

}
