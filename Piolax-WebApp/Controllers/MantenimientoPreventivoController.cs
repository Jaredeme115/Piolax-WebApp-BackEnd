using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Services;

namespace Piolax_WebApp.Controllers
{
    public class MantenimientoPreventivoController(IMantenimientoPreventivoService service) : BaseApiController
    {
        private readonly IMantenimientoPreventivoService _service = service;

        // Método HTTP POST para crear un mantenimiento preventivo
        [HttpPost]
        public async Task<IActionResult> CrearMantenimientoPreventivo(MantenimientoPreventivoDTO mantenimientoPreventivoDTO)
        {
            var mantenimientoDetalles = await _service.CrearMantenimientoPreventivo(mantenimientoPreventivoDTO);

            if (mantenimientoDetalles == null)
            {
                return BadRequest("No se pudo crear el mantenimiento preventivo."); // Si algo sale mal
            }

            return Ok(new { mensaje = "Mantenimiento Preventivo creado con éxito" });
        }

        // Método HTTP GET para consultar un mantenimiento preventivo con detalles
        [HttpGet("{idMP}")]
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
        [HttpPut("{idMP}")]
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
        [HttpDelete("{idMP}")]
        public async Task<IActionResult> EliminarMantenimientoPreventivo(int idMP)
        {
            var resultado = await _service.EliminarMantenimientoPreventivo(idMP);

            if (!resultado)
            {
                return NotFound(); // Si no se encuentra el mantenimiento
            }

            return NoContent(); // El mantenimiento ha sido eliminado exitosamente
        }
    }
}
