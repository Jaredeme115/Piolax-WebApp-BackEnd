using Microsoft.AspNetCore.Mvc;

namespace Piolax_WebApp.Controllers
{
    public class PDFsController(IWebHostEnvironment env) : BaseApiController
    {
        private readonly IWebHostEnvironment _env = env;

        [HttpPost("SubirPDF")]
        public async Task<IActionResult> SubirPDF(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("No se proporcionó ningún archivo.");

            var carpetaDestino = Path.Combine(_env.WebRootPath, "PDFs", "Ejecutados");

            if (!Directory.Exists(carpetaDestino))
                Directory.CreateDirectory(carpetaDestino);

            var rutaArchivo = Path.Combine(carpetaDestino, archivo.FileName);

            using (var stream = new FileStream(rutaArchivo, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            return Ok(new { mensaje = "Archivo guardado correctamente." });
        }
    }
}
