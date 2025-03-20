using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Services;

namespace Piolax_WebApp.Controllers
{
    public class ObservacionesMPController(IObservacionesMPService service) : BaseApiController
    {
        private readonly IObservacionesMPService _service = service;

        [HttpPost("AgregarObservacion")]
        public async Task<ActionResult<ObservacionesMPDTO>> AgregarObservacion(ObservacionesMPCrearDTO observacionDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var resultado = await _service.AgregarObservacion(observacionDTO);

                if (resultado == null)
                {
                    return StatusCode(500, "Error al guardar la observación en la base de datos.");
                }

                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("ObtenerObservacionesPorMP/{idMP}")]
        public async Task<ActionResult<IEnumerable<ObservacionesMPDTO>>> ObtenerObservacionesPorMP(int idMP)
        {
            var resultado = await _service.ObtenerObservacionesPorMP(idMP);
            return Ok(resultado);
        }

        [HttpDelete("EliminarObservacion/{idObservacionMP}")]
        public async Task<ActionResult<ObservacionesMPDTO>> EliminarObservacion(int idObservacionMP)
        {
            var resultado = await _service.EliminarObservacion(idObservacionMP);
            if (resultado == null)
            {
                return NotFound($"No se encontró la observación con ID {idObservacionMP}.");
            }
            return Ok(resultado);
        }
    }
}
