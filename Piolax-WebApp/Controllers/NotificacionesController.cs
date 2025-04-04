using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Services;
using Piolax_WebApp.Services.Impl;

namespace Piolax_WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificacionesController : ControllerBase
    {
        private readonly INotificacionService _service;

        public NotificacionesController(INotificacionService service)
        {
            _service = service;
        }

        [HttpPost("crear")]
        public async Task<IActionResult> Crear([FromBody] NotificacionDTO dto)
        {
            await _service.CrearNotificacion(dto);
            return Ok();
        }

        [HttpGet("{idEmpleado}")]
        public async Task<ActionResult<List<Notificacion>>> ObtenerPorUsuario(int idEmpleado)
        {

                var notificaciones = await _service.ObtenerNotificacionesPorUsuario(idEmpleado);
                return Ok(notificaciones);
        }

        [HttpPut("leida/{id}")]
        public async Task<IActionResult> MarcarLeida(int id)
        {
            await _service.MarcarComoLeida(id);
            return Ok();
        }
    }

}
