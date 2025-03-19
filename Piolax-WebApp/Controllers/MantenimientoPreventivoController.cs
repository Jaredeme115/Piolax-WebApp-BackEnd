using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Services;
using Piolax_WebApp.Services.Impl;

namespace Piolax_WebApp.Controllers
{
    public class MantenimientoPreventivoController(IMantenimientoPreventivoService service) : BaseApiController
    {
        private readonly IMantenimientoPreventivoService _service = service;

        // Método HTTP POST para crear un mantenimiento preventivo
        [HttpPost("CrearMantenimientoPreventivo")]
        public async Task<ActionResult<MantenimientoPreventivoDTO>> CrearMantenimientoPreventivo(MantenimientoPreventivoCreateDTO mantenimientoPreventivoCreateDTO)
        {
            // Verificar que los datos sean correctos (validación de entrada)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Llamar al servicio para crear el mantenimiento preventivo
            var mantenimientoPreventivoCreado = await _service.CrearMantenimientoPreventivo(mantenimientoPreventivoCreateDTO);

            // Si la creación es exitosa, devolver el DTO con los detalles del mantenimiento creado
            return CreatedAtAction(nameof(CrearMantenimientoPreventivo), new { id = mantenimientoPreventivoCreado.idMP }, mantenimientoPreventivoCreado);

        }

        // Método HTTP GET para consultar un mantenimiento preventivo con detalles
        [HttpGet("ConsultarMP/{idMP}")]
        public async Task<IActionResult> ConsultarMantenimientoPreventivo(int idMP)
        {
            var mantenimientoDetalles = await _service.ConsultarMPConDetalles(idMP);

            if (mantenimientoDetalles == null)
            {
                return NotFound(); // Si no se encuentra el mantenimiento
            }

            return Ok(mantenimientoDetalles); // Retorna el DTO con los detalles
        }

        // Método HTTP PUT para modificar un mantenimiento preventivo
        [HttpPut("ModificarMP/{idMP}")]
        public async Task<IActionResult> ModificarMantenimientoPreventivo(int idMP, MantenimientoPreventivoModificarDTO mantenimientoPreventivoModificarDTO)
        {
            var mantenimientoDetalles = await _service.ModificarMantenimientoPreventivo(idMP, mantenimientoPreventivoModificarDTO);

            if (mantenimientoDetalles == null)
            {
                return NotFound(); // Si no se encuentra el mantenimiento
            }

            return Ok(mantenimientoDetalles); // Retorna el DTO con los detalles actualizados
        }

        // Método HTTP DELETE para eliminar un mantenimiento preventivo
        [HttpDelete("EliminarMP/{idMP}")]
        public async Task<IActionResult> EliminarMantenimientoPreventivo(int idMP)
        {
            var resultado = await _service.EliminarMantenimientoPreventivo(idMP);

            if (!resultado)
            {
                return NotFound(); // Si no se encuentra el mantenimiento
            }

            return NoContent(); // El mantenimiento ha sido eliminado exitosamente
        }

        // Endpoint para marcar como realizado
        [HttpPut("MarcarRealizadoMP/{idMP}")]
        public async Task<IActionResult> MarcarComoRealizado(int idMP)
        {
            // Llamar al servicio para marcar como realizado
            var resultado = await _service.MarcarComoRealizado(idMP);

            if (!resultado)
            {
                return NotFound(new { success = false, message = "Mantenimiento no encontrado" });
            }

            return Ok(new { success = true, message = "Mantenimiento marcado como Realizado y KPIs actualizados." });
        }
    }
}
