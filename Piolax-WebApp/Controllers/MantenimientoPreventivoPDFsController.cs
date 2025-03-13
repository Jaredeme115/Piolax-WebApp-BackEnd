using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Services;

namespace Piolax_WebApp.Controllers
{
    public class MantenimientoPreventivoPDFsController(IMantenimientoPreventivoPDFsService service): BaseApiController
    {
        private readonly IMantenimientoPreventivoPDFsService _service = service;

        [HttpPost("AgregarMantenimientoPreventivoPDFs")]
        public async Task<ActionResult<MantenimientoPreventivoPDFsDTO>> AgregarMantenimientoPreventivoPDFs(MantenimientoPreventivoPDFCrearDTO mantenimientoPreventivoPDFCrearDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var mantenimientoPreventivoPDFsAgregado = await _service.AgregarMantenimientoPreventivoPDFs(mantenimientoPreventivoPDFCrearDTO);
            return CreatedAtAction(nameof(AgregarMantenimientoPreventivoPDFs), new { id = mantenimientoPreventivoPDFsAgregado.idMP }, mantenimientoPreventivoPDFsAgregado);
        }
    }
}
