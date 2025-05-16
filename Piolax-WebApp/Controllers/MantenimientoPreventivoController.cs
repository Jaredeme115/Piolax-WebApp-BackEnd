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


        [HttpGet("ConsultarTodos")]
        public async Task<ActionResult<IEnumerable<MantenimientoPreventivoDetallesDTO>>> ConsultarTodos()
        {
            try
            {
                var listaDto = await _service.ConsultarTodosMPsDTO();
                return Ok(listaDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("MostrarMPsAsignados/{idEmpleado}")]
        public async Task<ActionResult<IEnumerable<MantenimientoPreventivoDetallesDTO>>> MostrarMPsAsignados(int idEmpleado)
        {
            try
            {
                var listaDto = await _service.MostrarMPsAsignados(idEmpleado);
                return Ok(listaDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }


        [HttpPut("ActivarMP/{idMP}")]
        public async Task<IActionResult> ActivarMP(int idMP)
        {
            try
            {
                var dto = await _service.ActivarMantenimientoPreventivo(idMP);
                if (dto == null)
                    return NotFound($"No se encontró MP con id {idMP}.");

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("DesactivarMP/{idMP}")]
        public async Task<IActionResult> DesactivarMP(int idMP)
        {
            try
            {
                var dto = await _service.DesactivarMantenimientoPreventivo(idMP);
                if (dto == null) return NotFound($"No se encontró MP con id {idMP}");
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("CambiarEstatusEnProceso/{idMP}")]
        public async Task<IActionResult> CambiarEstatusEnProceso(int idMP)
        {
            var mpActualizado = await _service.CambiarEstatusEnProceso(idMP);

            if (mpActualizado == null)
                return NotFound();

            return Ok(mpActualizado);
        }

        [HttpPut("CancelarMPEnProceso/{idMP}")]
        public async Task<IActionResult> CancelarMantenimientoEnProceso(int idMP)
        {
            var mp = await _service.CancelarMantenimientoEnProceso(idMP);
            if (mp == null) return NotFound();
            return Ok(mp);
        }

        [HttpPost("corregir-inconsistencias")]
        public async Task<IActionResult> CorregirInconsistencias()
        {
            var resultado = await _service.CorregirMantenimientosReprogramados();
            if (resultado)
            {
                return Ok(new { mensaje = "Inconsistencias corregidas con éxito" });
            }
            return BadRequest(new { mensaje = "Error al corregir inconsistencias" });
        }

        [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
        [DisableRequestSizeLimit]
        [HttpPost("RegistrarPreventivosDesdeExcel")]

        public async Task<IActionResult> RegistrarPreventivosDesdeExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { Error = "Por favor, sube un archivo Excel válido." });

            try
            {
                var resultado = await _service.RegistrarPreventivosDesdeExcel(file);
                return Ok(new { Message = resultado });
            }
            catch (ArgumentException ex)
            {
                // errores de validación (archivo inválido, extensión, etc.)
                return BadRequest(new { Error = ex.Message });
            }
            catch (ApplicationException ex)
            {
                // errores durante el procesamiento (detalles de EPPlus, formato de celdas, etc.)
                return StatusCode(500, new
                {
                    Error = ex.Message,
                    Details = ex.InnerException?.Message ?? "Sin detalles adicionales"
                });
            }
            catch (Exception ex)
            {
                // cualquier otro error inesperado
                return StatusCode(500, new
                {
                    Error = "Error inesperado procesando el Excel.",
                    Details = ex.Message
                });
            }
        }



    }
}
