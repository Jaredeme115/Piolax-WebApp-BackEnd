using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Services;
using Piolax_WebApp.Services.Impl;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using BarcodeStandard;
using OfficeOpenXml;
using System.Web;

namespace Piolax_WebApp.Controllers
{
    public class InventarioController(IInventarioService service): BaseApiController
    {
        private readonly IInventarioService _service = service;

        //[Authorize(Policy = "AdminOnly")]
        [HttpPost("Registro")]

        public async Task<ActionResult<Inventario>> RegistrarInventario([FromBody] InventarioDTO inventarioDTO)
        {
            if (inventarioDTO == null)
            {
                return BadRequest("Los datos del inventario son inválidos.");
            }

            if (await _service.ExisteNumParte(inventarioDTO.numParte))
            {
                return Conflict("El producto ya existe en el inventario.");
            }

            try
            {
                var inventario = await _service.RegistrarInventario(inventarioDTO);
                return CreatedAtAction(nameof(RegistrarInventario), new { id = inventario.idRefaccion }, inventario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al registrar el inventario: " + ex.Message);
            }
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpPut("Modificar/{idRefaccion}")]
        public async Task<ActionResult<InventarioDetalleDTO>> ModificarRefaccion(int idRefaccion, [FromBody] InventarioModificarDTO inventarioDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var refaccionModificada = await _service.Modificar(idRefaccion, inventarioDTO);
                return Ok(refaccionModificada);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpDelete("Eliminar/{idRefaccion}")]
        public async Task<ActionResult<bool>> Eliminar(int idRefaccion)
        {
            try
            {
                var resultado = await _service.Eliminar(idRefaccion);
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // ID inválido
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message); // No existe el ID
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpGet("ConsultarTodoInventario")]
        public async Task<ActionResult<IEnumerable<Inventario>>> ConsultarTodoInventario()
        {
            return Ok(await _service.ConsultarTodoInventario());
        }

        //Metodo modificado para mostrar los detalles de la refaccion en base al nombre de la misma
        //[Authorize(Policy = "AdminOnly")]
        [HttpGet("detalle/nombre/{nombreProducto}")]
        public async Task<ActionResult<InventarioDetalleDTO>> ConsultarRefaccionPorNombre(string nombreProducto)
        {
            string decodedNombreProducto = HttpUtility.UrlDecode(nombreProducto);
            var refaccion = await _service.ConsultarRefaccionPorNombre(decodedNombreProducto); //Antes tenia nombreProducto

            if (refaccion == null)
            {
                return NotFound($"No se encontró la refacción con nombre: {nombreProducto}");
            }

            return Ok(refaccion);
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpGet("ConsultarInventarioPorCategoria")]
        public async Task<ActionResult<Inventario>> ConsultarInventarioPorCategoria(int idInventarioCategoria)
        {
            return await _service.ConsultarInventarioPorCategoria(idInventarioCategoria);
        }


        //[Authorize(Policy = "AdminOnly")]
        [HttpGet("ConsultarInventarioPorID")]

        public async Task<ActionResult<Inventario>> ConsultarInventarioPorID(int idRefaccion)
        {
            return await _service.ConsultarInventarioPorID(idRefaccion);
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpGet("ConsultarProductosPorCategoria")]

        public async Task<ActionResult<IEnumerable<Inventario>>> ConsultarTodosLosProductosPorCategoria(int idInventarioCategoria)
        {
            return Ok(await _service.ConsultarTodosLosProductosPorCategoria(idInventarioCategoria));
        }

        
        [HttpGet("filtrar-refacciones")]
        public async Task<IActionResult> FiltrarRefacciones([FromQuery] bool? piezaCritica, [FromQuery] bool? inventarioObsoleto, [FromQuery] string? proceso)
        {
            var result = await _service.ConsultarRefaccionesPorFiltros(piezaCritica, inventarioObsoleto, proceso);
            return Ok(result);
        }


        [HttpGet("detalle/{idInventario}")]
        public async Task<ActionResult<InventarioDetalleDTO>> ObtenerRefaccionDetalle(int idInventario)
        {
            var refaccion = await _service.ObtenerRefaccionDetalle(idInventario);

            if (refaccion == null)
            {
                return NotFound($"No se encontró la refacción con ID: {idInventario}");
            }

            return Ok(refaccion);
        }

        [HttpGet("ConsultarNombresRefaccionesPorCategoria/{idCategoria}")]
        public async Task<ActionResult<IEnumerable<string>>> ConsultarNombresRefaccionesPorCategoria(int idCategoria)
        {
            var nombresRefacciones = await _service.ConsultarNombresRefaccionesPorCategoria(idCategoria);
            if (nombresRefacciones == null || !nombresRefacciones.Any())
            {
                return Ok(new List<string>()); // Devuelve una lista vacía en lugar de un error
            }
            return Ok(nombresRefacciones);
        }

        [HttpPost("DescontarInventario")]
        public async Task<IActionResult> DescontarInventario([FromBody] DescontarInventarioDTO descontarInventarioDTO)
        {
            if (descontarInventarioDTO == null)
            {
                return BadRequest(new { error = "Los datos de la refacción son inválidos." });
            }

            // Llamar al servicio para descontar inventario
            try
            {
                await _service.DescontarInventario(descontarInventarioDTO.idRefaccion, descontarInventarioDTO.cantidadADescontar);
                return Ok(new { mensaje = "Cantidad descontada correctamente del inventario." });
            }

            catch (Exception ex)
            {
                // Si ocurre algún error, devolver una respuesta adecuada
                return BadRequest(new { error = $"Error al descontar el inventario: {ex.Message}" });
            }
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpGet("DescargarQRCode/{idRefaccion}")]

        public async Task<IActionResult> DescargarQRCode(int idRefaccion)
        {
            // Consultar el producto en el inventario
            // Consultar el producto en el inventario
            var producto = await _service.ConsultarInventarioPorID(idRefaccion);
            if (producto == null)
            {
                return NotFound("El producto no existe.");
            }

            // ✅ Generar el texto del código QR con nombreProducto, idArea y idMaquina
            string qrCodeText = $"{producto.nombreProducto} | {producto.descripcion} | Area: {producto.idArea} | Maquina: {producto.idMaquina}";

            try
            {
                // ✅ Generar el QR en bytes en tiempo real
                byte[] qrCodeBytes = _service.GenerateQRCodeBytes(qrCodeText);

                // ✅ Formatear el nombre del archivo de manera segura
                string sanitizedFileName = string.Join("_", producto.nombreProducto.Split(Path.GetInvalidFileNameChars()));
                string fileName = !string.IsNullOrWhiteSpace(sanitizedFileName)
                    ? $"QRCode_{sanitizedFileName}.png"
                    : "QRCode.png";

                return File(qrCodeBytes, "image/png", fileName);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al generar el QR: {ex.Message}");
                return StatusCode(500, "Ocurrió un error inesperado al generar el código QR.");
            }
        }


        // Metodo para cargar refacciones desde un archivo Excel
        [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
        [DisableRequestSizeLimit]
        [HttpPost("CargarRefaccionesDesdeExcel")]
        public async Task<IActionResult> SubirInventarioDesdeExcel(IFormFile file)
        {
            try
            {
                // Validar que se haya recibido un archivo
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { error = "Debe proporcionar un archivo Excel válido." });
                }

                string resultado = await _service.RegistrarInventarioDesdeExcel(file);
                return Ok(new { mensaje = resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ocurrió un error al procesar el archivo.", detalle = ex.Message });
            }
        }

        /*[HttpGet("ExportarInventario")]
        public async Task<IActionResult> ExportarInventario()
        {
            var excelBytes = await _service.ExportarInventarioExcel();

            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Inventario.xlsx");
        }*/



    }
}
