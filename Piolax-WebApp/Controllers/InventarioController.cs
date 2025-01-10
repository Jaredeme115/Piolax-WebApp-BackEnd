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

namespace Piolax_WebApp.Controllers
{
    public class InventarioController(IInventarioService service): BaseApiController
    {
        private readonly IInventarioService _service = service;

        //[Authorize(Policy = "AdminOnly")]
        [HttpPost("Registro")]

        public async Task<ActionResult<Inventario>> RegistrarInventario(InventarioDTO inventarioDTO)
        {
            if (await _service.ExisteNumParte(inventarioDTO.numParte))
            {
                return BadRequest("El producto dentro del inventario ya existe");
            }

            // Generar el código QR
            string qrCodeText = inventarioDTO.numParte; // Puedes personalizar el contenido del código QR
            string qrCodeBase64 = GenerateQRCode(qrCodeText);
            inventarioDTO.codigoQR = qrCodeBase64;

            // Asignar valor a la propiedad precioInventarioTotal
            inventarioDTO.precioInventarioTotal = inventarioDTO.precioUnitario * inventarioDTO.cantidadActual;

            // Registrar el inventario sin el valor de item
            var inventario = await _service.RegistrarInventario(inventarioDTO);

            // Asignar valor a la propiedad item basado en el idRefaccion generado
            switch (inventarioDTO.idArea)
            {
                case 2:
                    inventario.item = "Met0" + inventario.idRefaccion;
                    break;
                case 3:
                    inventario.item = "Inyec0" + inventario.idRefaccion;
                    break;
                case 6:
                    inventario.item = "Ens0" + inventario.idRefaccion;
                    break;
                case 7:
                    inventario.item = "Mtto0" + inventario.idRefaccion;
                    break;
            }

            // Actualizar el inventario con el valor de item
            await _service.Modificar(inventario.idRefaccion, inventarioDTO);

            return Ok(inventario);
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpPut("Modificar")]

        public async Task<ActionResult<Inventario>> Modificar(int idRefaccion, InventarioDTO inventarioDTO)
        {
            if(!await _service.ExisteProductoInventario(idRefaccion))
            {
                return BadRequest("El producto no existe dentro del Inventario");
            }

            // Generar el código QR
            string qrCodeText = inventarioDTO.numParte; ; // Puedes personalizar el contenido del código QR
            string qrCodeBase64 = GenerateQRCode(qrCodeText);
            inventarioDTO.codigoQR = qrCodeBase64;


            // Asignar valor a la propiedad precioInventarioTotal
            inventarioDTO.precioInventarioTotal = inventarioDTO.precioUnitario * inventarioDTO.cantidadActual;


            return await _service.Modificar(idRefaccion, inventarioDTO);
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpDelete("Eliminar")]
        public async Task<ActionResult<Inventario>> Eliminar(int idRefaccion)
        {
            if (await _service.ExisteProductoInventario(idRefaccion))
            {
                return BadRequest("El producto no existe");
            }

            return await _service.Eliminar(idRefaccion);
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

        public async Task<ActionResult<IEnumerable<Inventario>>> ConsultarProductosPorCategoria(int idInventarioCategoria)
        {
            return Ok(await _service.ConsultarProductosPorCategoria(idInventarioCategoria));
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

        // Funcionalidad para generar el código QR
        private string GenerateQRCode(string text)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            qrCodeImage.Save(ms, ImageFormat.Png);
                            byte[] byteImage = ms.ToArray();
                            return Convert.ToBase64String(byteImage);
                        }
                    }
                }
            }
        }


    }
}
