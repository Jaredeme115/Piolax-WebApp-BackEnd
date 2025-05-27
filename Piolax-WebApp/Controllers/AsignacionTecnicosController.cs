using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Services;
using Piolax_WebApp.Services.Impl;
using Microsoft.AspNetCore.SignalR;
using Piolax_WebApp.Hubs;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Controllers
{
    public class AsignacionTecnicosController(IAsignacionTecnicosService service, IHubContext<NotificationHub> hubContext, AppDbContext dbContext) : BaseApiController
    {
        private readonly IAsignacionTecnicosService _service = service;
        private readonly IHubContext<NotificationHub> _hubContext = hubContext;
        private readonly AppDbContext _dbContext = dbContext;

        [HttpGet("ConsultarTecnicosPorAsignacion")]
        public async Task<ActionResult<IEnumerable<Asignacion_TecnicoDetallesDTO>>> ConsultarTecnicosPorAsignacion(int idAsignacion)
        {
            return Ok(await _service.ConsultarTecnicosPorAsignacion(idAsignacion));
        }

        [HttpPost("CrearAsignacionTecnico")]
        public async Task<ActionResult<Asignacion_TecnicoResponseDTO?>> CrearAsignacionTecnico([FromBody] Asignacion_TecnicoDTO asignacionTecnicoDTO)
        {
            return await _service.CrearAsignacionTecnico(asignacionTecnicoDTO);
        }

        [HttpPost("FinalizarAsignacionTecnico")]
        public async Task<ActionResult<Asignacion_TecnicoFinalizacionResponseDTO>> FinalizarAsignacionTecnico([FromBody] Asignacion_TecnicoFinalizacionDTO asignacionTecnicoFinalizacionDTO)
        {
            //return await _service.FinalizarAsignacionTecnico(asignacionTecnicoFinalizacionDTO);

            // 1) Finaliza la asignación en tu servicio
            var resultado = await _service.FinalizarAsignacionTecnico(asignacionTecnicoFinalizacionDTO);

            // 2) A partir del idAsignacionTecnico, obtén la fila de asignacion_tecnico
            var at = await _dbContext.Asignacion_Tecnico
                .FirstOrDefaultAsync(x => x.idAsignacionTecnico == resultado.idAsignacionTecnico);

            if (at != null)
            {
                // 3) Con el idAsignacion de ahí, obtén la orden (asignaciones)
                var orden = await _dbContext.Asignaciones
                    .FirstOrDefaultAsync(a => a.idAsignacion == at.idAsignacion);

                if (orden != null)
                {
                    // 4) Con el idSolicitud de la orden, obtén la solicitud
                    var solicitud = await _dbContext.Solicitudes
                        .FirstOrDefaultAsync(s => s.idSolicitud == orden.idSolicitud);

                    if (solicitud != null)
                    {
                        // Extrae quién creó la solicitud y de qué área es
                        var idSolicitante = solicitud.idEmpleado;
                        var idArea = solicitud.idAreaSeleccionada;

                        // 5) Notificar al solicitante
                        await _hubContext.Clients
                            .Group($"User_{idSolicitante}")
                            .SendAsync("RequestReadyForApproval", new
                            {
                                idAsignacionTecnico = resultado.idAsignacionTecnico,
                                idSolicitud = solicitud.idSolicitud,
                                solucion = asignacionTecnicoFinalizacionDTO.solucion
                            });

                        // 6) Notificar a los assistance managers de esa área
                        await _hubContext.Clients
                            .Group($"Area_{idArea}_Assistant")
                            .SendAsync("RequestAwaitingValidation", new
                            {
                                idAsignacionTecnico = resultado.idAsignacionTecnico,
                                idSolicitud = solicitud.idSolicitud,
                                Area = idArea
                            });

                        // 7) Notificar a los supervisores de esa área
                        await _hubContext.Clients
                            .Group($"Area_{idArea}_Supervisor")
                            .SendAsync("RequestAwaitingValidation", new
                            {
                                idAsignacionTecnico = resultado.idAsignacionTecnico,
                                idSolicitud = solicitud.idSolicitud,
                                Area = idArea
                            });
                    }
                }
            }

            return Ok(resultado);
        }

        [HttpPost("PausarAsignacion")]
        public async Task<ActionResult<bool>> PausarAsignacion([FromBody] PausarAsignacionDTO pausarAsignacionDTO)
        {
            return await _service.PausarAsignacion(pausarAsignacionDTO.idAsignacion, pausarAsignacionDTO.idTecnicoQuePausa, pausarAsignacionDTO.comentarioPausa);
        }

        [HttpPost("RetirarTecnicoDeApoyo")]
        public async Task<ActionResult<bool>> RetirarTecnicoDeApoyo([FromBody] RetirarTecnicoDTO retirarTecnicoDTO)
        {
            return await _service.RetirarTecnicoDeApoyo(retirarTecnicoDTO.idAsignacion, retirarTecnicoDTO.idTecnicoQueSeRetira, retirarTecnicoDTO.comentarioRetiro);
        }

        [HttpDelete("EliminarTecnicoDeAsignacion")]
        public async Task<ActionResult<bool>> EliminarTecnicoDeAsignacion([FromBody] EliminarTecnicoDTO eliminarTecnicoDTO)
        {
            return await _service.EliminarTecnicoDeAsignacion(eliminarTecnicoDTO.idAsignacionTecnico);
        }

        [HttpGet("ConsultarTodosLosTecnicos")]
        public async Task<ActionResult<IEnumerable<Asignacion_Tecnico>>> ConsultarTodosLosTecnicos()
        {
            return Ok(await _service.ConsultarTodosLosTecnicos());
        }

        [HttpPut("ActualizarTecnicoEnAsignacion")]
        public async Task<ActionResult<bool>> ActualizarTecnicoEnAsignacion([FromBody] Asignacion_TecnicoDTO asignacionTecnicoDTO)
        {
            return await _service.ActualizarTecnicoEnAsignacion(asignacionTecnicoDTO);
        }

        [HttpGet("ConsultarTecnicosConDetallesPorAsignacion")]
        public async Task<ActionResult<IEnumerable<Asignacion_TecnicoDetallesDTO>>> ConsultarTecnicosConDetallesPorAsignacion(int idAsignacion)
        {
            return Ok(await _service.ConsultarTecnicosConDetallesPorAsignacion(idAsignacion));
        }


        [HttpGet("OrdenesPausadasPorTecnico/{idEmpleado}")]
        public async Task<ActionResult<IEnumerable<SolicitudesDetalleDTO>>> ConsultarSolicitudesPausadasPorTecnico(int idEmpleado)
        {
            var ordenes = await _service.ConsultarSolicitudesPausadasPorTecnico(idEmpleado);

            if (ordenes == null || !ordenes.Any())
                return NotFound("No se encontraron órdenes pausadas para este técnico.");

            return Ok(ordenes);
        }


        [HttpPut("RetomarAsignacion/{idAsignacion}/{idEmpleado}")]
        public async Task<IActionResult> RetomarAsignacion(int idAsignacion, int idEmpleado)
        {
            var resultado = await _service.RetomarAsignacion(idAsignacion, idEmpleado);
            if (!resultado)
            {
                return BadRequest(new { error = "No se pudo retomar la asignación." });
            }

            return Ok(true);
        }

        [HttpGet("ObtenerAsignacionTecnico/{idAsignacion}/{idEmpleado}")]
        public async Task<IActionResult> ObtenerAsignacionTecnico(int idAsignacion, int idEmpleado)
        {
            var asignacionTecnico = await _service.ConsultarTecnicoPorAsignacionYEmpleado(idAsignacion, idEmpleado);

            if (asignacionTecnico == null)
            {
                return NotFound(new { error = "No se encontró la asignación del técnico." });
            }

            return Ok(new { idAsignacionTecnico = asignacionTecnico.idAsignacionTecnico });
        }



    }
}
