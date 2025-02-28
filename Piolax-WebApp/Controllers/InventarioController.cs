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

        public async Task<ActionResult<Inventario>> Modificar(int idRefaccion, [FromBody] InventarioDTO inventarioDTO)
        {

            if (idRefaccion <= 0)
            {
                return BadRequest("ID de refacción inválido.");
            }

            var existeProducto = await _service.ExisteProductoInventario(idRefaccion);
            if (!existeProducto)
            {
                return NotFound("El producto no existe en el inventario.");
            }

            try
            {
                var resultado = await _service.Modificar(idRefaccion, inventarioDTO);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
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

        //[Authorize(Policy = "AdminOnly")]
        [HttpGet("ConsultarInventarioPorNombre")]
        public async Task<ActionResult<Inventario>> ConsultarInventarioPorNombre(string nombreProducto)
        {
            return await _service.ConsultarInventarioPorNombre(nombreProducto);
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpGet("ConsultarInventarioPorCategoria")]
        public async Task<ActionResult<Inventario>> ConsultarInventarioPorCategoria(int idInventarioCategoria)
        {
            return await _service.ConsultarInventarioPorCategoria(idInventarioCategoria);
        }

        /*[Authorize(Policy = "AdminOnly")]
        [HttpGet("ConsultarInventarioConDetalles")]
        public async Task<ActionResult<Inventario>> ConsultarInventarioConDetalles(int idRefaccion)
        {
            return await _service.ConsultarInventarioConDetalles(idRefaccion);
        }*/

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
        public async Task<IActionResult> FiltrarRefacciones([FromQuery] bool? piezaCritica, [FromQuery] bool? inventarioObsoleto)
        {
            var result = await _service.ConsultarRefaccionesPorFiltros(piezaCritica, inventarioObsoleto);
            return Ok(result);
        }


        //[Authorize(Policy = "AdminOnly")]
        [HttpGet("DescargarQRCode/{idRefaccion}")]

        public async Task<IActionResult> DescargarQRCode(int idRefaccion)
        {
            // Consultar el producto en el inventario
            var producto = await _service.ConsultarInventarioPorID(idRefaccion);
            if (producto == null)
            {
                return NotFound("El producto no existe.");
            }

            // Obtener el código QR en formato Base64 desde el producto
            string codigoQRBase64 = producto.codigoQR;
            if (string.IsNullOrEmpty(codigoQRBase64))
            {
                return BadRequest("El producto no tiene un código QR.");
            }

            // Decodificar el string Base64 a una imagen en bytes
            byte[] qrCodeBytes = Convert.FromBase64String(codigoQRBase64);

            // Devolver la imagen como archivo descargable
            return File(qrCodeBytes, "image/png", $"QRCode_{producto.numParte}.png");
        }

        // Metodo para cargar refacciones desde un archivo Excel
       [HttpPost("CargarRefaccionesDesdeExcel")]
       [Consumes("multipart/form-data")] // Indica que acepta archivos
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



    }
}
