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

            // 1) Carpeta base “Ejecutados”
            var basePath = Path.Combine(_env.WebRootPath, "PDFs", "Ejecutados");

            // 2) Subcarpeta con el año-mes actual (por ejemplo: "2025-05")
            var mesCarpeta = DateTime.Now.ToString("yyyy-MM");
            var carpetaMensual = Path.Combine(basePath, mesCarpeta);

            // 3) Asegurarte de que exista
            if (!Directory.Exists(carpetaMensual))
                Directory.CreateDirectory(carpetaMensual);

            // 4) Nombre único (puedes reutilizar el timestamp o GUID que ya vimos)
            var nombreSinExt = Path.GetFileNameWithoutExtension(archivo.FileName);
            var extension = Path.GetExtension(archivo.FileName);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
            var nuevoNombre = $"{nombreSinExt}_{timestamp}{extension}";

            // 5) Ruta final
            var rutaArchivo = Path.Combine(carpetaMensual, nuevoNombre);

            // 6) Guardado
            using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                await archivo.CopyToAsync(stream);

            return Ok(new
            {
                mensaje = "Archivo guardado correctamente.",
                fileName = nuevoNombre,
                carpeta = mesCarpeta
            });
        }


    }
}
