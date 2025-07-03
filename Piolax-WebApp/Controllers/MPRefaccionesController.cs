using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Services;

namespace Piolax_WebApp.Controllers
{
    public class MPRefaccionesController (IMantenimientoPreventivoRefaccionesService service) : BaseApiController
    {
        private readonly IMantenimientoPreventivoRefaccionesService _service = service;

        [HttpGet("ConsultarRefaccionesMP/{idHistoricoMP}")]
        public async Task<IActionResult> ConsultarRefaccionesMP(int idHistoricoMP)
        {
            var refacciones = await _service.ConsultarRefaccionesMP(idHistoricoMP);
            if (refacciones == null || !refacciones.Any())
            {
                return NotFound("No se encontraron refacciones para el mantenimiento preventivo especificado.");
            }
            return Ok(refacciones);
        }

        [HttpPost("CrearRefaccionMP")]
        public async Task<IActionResult> CrearRefaccionMP([FromBody] MantenimientoPreventivo_Refacciones mantenimientoPreventivoRefacciones)
        {
            try
            {
                var response = await _service.CrearRefaccionMP(mantenimientoPreventivoRefacciones);
                // Retorna CreatedAtAction para que el cliente tenga la URI del recurso creado.
                return CreatedAtAction(nameof(ConsultarRefaccionPorId), new { idMPRefaccion = response.idMPRefaccion }, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet("ConsultarTodasLasRefacciones")]
        public async Task<IActionResult> ConsultarTodasLasRefacciones()
        {
            var refacciones = await _service.ConsultarTodasLasRefacciones();
            return Ok(refacciones);
        }

        [HttpDelete("EliminarRefaccionMP/{idMPRefaccion}")]
        public async Task<IActionResult> EliminarRefaccionMP(int idMPRefaccion)
        {
            var resultado = await _service.EliminarRefaccionMP(idMPRefaccion);
            if (!resultado)
            {
                return NotFound("No se encontró la refacción con el identificador proporcionado.");
            }
            return Ok("Refacción eliminada correctamente.");
        }

        [HttpPut("ActualizarRefaccionMP")]
        public async Task<IActionResult> ActualizarRefaccionMP([FromBody] MantenimientoPreventivo_Refacciones mantenimientoPreventivoRefacciones)
        {
            var resultado = await _service.ActualizarRefaccionMP(mantenimientoPreventivoRefacciones);
            if (!resultado)
            {
                return NotFound("No se encontró la refacción a actualizar.");
            }
            return Ok("Refacción actualizada correctamente.");
        }

        [HttpGet("ConsultarRefaccionPorId/{idMPRefaccion}")]
        public async Task<IActionResult> ConsultarRefaccionPorId(int idMPRefaccion)
        {
            var refaccion = await _service.ConsultarRefaccionPorId(idMPRefaccion);
            if (refaccion == null)
            {
                return NotFound("No se encontró la refacción con el identificador especificado.");
            }
            return Ok(refaccion);
        }

        [HttpPost("ConfirmarUsoRefaccion/{idHistoricoMP}")]
        public async Task<IActionResult> ConfirmarUsoDeRefacciones(int idHistoricoMP)
        {
            try
            {
                var resultado = await _service.ConfirmarUsoDeRefacciones(idHistoricoMP);
                if (!resultado)
                {
                    return NotFound("No hay refacciones registradas para este mantenimiento preventivo.");
                }
                return Ok("Refacciones descontadas del inventario correctamente.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }


    }
}
