using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Services;

namespace Piolax_WebApp.Controllers
{
    public class SolicitudesController(ISolicitudService service) : BaseApiController
    {
        private readonly ISolicitudService _service = service;

        [Authorize]
        [HttpGet("Consultar")]
        public ActionResult<Solicitudes?> Consultar(int idSolicitud)
        {
            return _service.Consultar(idSolicitud).Result;
        }

        [Authorize]
        [HttpGet("Consultar Todos")]
        public async Task<ActionResult<IEnumerable<Solicitudes>>> ConsultarTodos()
        {
            return Ok(await _service.ConsultarTodos());
        }

        [Authorize]
        [HttpPost("Registro")]
        public async Task<ActionResult<Solicitudes>> Registro(SolicitudesDTO solicitud)
        {

            // Asignar automáticamente los valores de idStatusOrden e idStatusAprobacionSolicitante
            solicitud.idStatusOrden = 3;
            solicitud.idStatusAprobacionSolicitante = 3;

            return Ok(await _service.Registro(solicitud));
        }

        [Authorize]
        [HttpPut("Modificar")]
        public async Task<ActionResult<Solicitudes>> Modificar(int idSolicitud, SolicitudesDTO solicitud)
        {
            if (!await _service.SolicitudExiste(idSolicitud))
            {
                return NotFound("La solicitud no existe");
            }

            var solicitudModificada = await _service.Modificar(idSolicitud, solicitud);
            return Ok(solicitudModificada);
        }

        [Authorize]
        [HttpDelete("Eliminar")]
        public async Task<ActionResult<Solicitudes>> Eliminar(int idSolicitud)
        {
            if (!await _service.SolicitudExiste(idSolicitud))
            {
                return NotFound("La solicitud no existe");
            }

            return Ok(await _service.Eliminar(idSolicitud));
        }
    }
}
