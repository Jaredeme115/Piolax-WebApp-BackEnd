using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.Models;
using Piolax_WebApp.Services;
using Piolax_WebApp.Services.Impl;

namespace Piolax_WebApp.Controllers
{
    public class FrecuenciaMPController(IFrecuenciaMPService service): BaseApiController
    {
        private readonly IFrecuenciaMPService _service = service;

        [HttpGet("ObtenerFrecuenciaPorID/{id}")]
        public async Task<ActionResult<FrecuenciaMP>> GetFrecuenciaMPById(int id)
        {
            var frecuenciaMP = await _service.ConsultarFrecuenciaPreventivoPorID(id);
            if (frecuenciaMP == null)
            {
                return NotFound();
            }
            return Ok(frecuenciaMP);
        }

        [HttpGet("ObtenerTodasLasFrecuencias")]
        public async Task<ActionResult<IEnumerable<FrecuenciaMP>>> GetAllFrecuenciasMP()
        {
            var frecuenciasMP = await _service.ConsultarTodasFrecuenciasPreventivo();
            return Ok(frecuenciasMP);
        }

    }
}
