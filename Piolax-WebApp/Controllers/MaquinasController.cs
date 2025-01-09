using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Services;
using System.Text;
using System.Security.Cryptography;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Piolax_WebApp.Controllers
{
    public class MaquinasController(IMaquinasService service) : BaseApiController
    {
        private readonly IMaquinasService _service = service;

      
        [HttpGet("Consultar")]
        public ActionResult<Maquinas?> Consultar(int idMaquina)
        {
            return _service.Consultar(idMaquina).Result;
        }

        [Authorize]
        [HttpGet("ConsultarTodos")]
        public async Task<ActionResult<IEnumerable<Maquinas>>> ConsultarTodos()
        {
            return Ok(await _service.ConsultarTodos());
        }

        [HttpGet("ConsultarPorArea")]
        public async Task<ActionResult<IEnumerable<Maquinas>>> ConsultarPorArea(int idArea)
        {
            return Ok(await _service.ConsultarPorArea(idArea));
        }


        [HttpPost("Registro")]
        public async Task<ActionResult<Maquinas>> Registro(MaquinaDTO maquina)
        {
            if (await _service.MaquinaExisteRegistro(maquina.descripcion))
            {
                return BadRequest("La maquina ya esta registrada");
            }

            // Generar el código QR
            string qrCodeText = maquina.descripcion; // Puedes personalizar el contenido del código QR
            string qrCodeBase64 = GenerateQRCode(qrCodeText);
            maquina.codigoQR = qrCodeBase64;

            return Ok(await _service.Registro(maquina));
        }

       
        [HttpPut("Modificar")]
        public async Task<ActionResult<Maquinas>> Modificar(int idMaquina, MaquinaDTO maquina)
        {
            if (!await _service.MaquinaExiste(idMaquina))
            {
                return NotFound("La maquina no existe");
            }

            var maquinaModificada = await _service.Modificar(idMaquina, maquina);
            return Ok(maquinaModificada);
        }

   
        [HttpDelete("Eliminar")]
        public async Task<ActionResult<Maquinas>> Eliminar(int idMaquina)
        {
            if (!await _service.MaquinaExiste(idMaquina))
            {
                return NotFound("La maquina no existe");
            }

            return Ok(await _service.Eliminar(idMaquina));
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpGet("DescargarQRCode/{idMaquina}")]

        public async Task<IActionResult> DescargarQRCode(int idMaquina)
        {
            // Consultar la maquina en la base de datos
            var maquina = await _service.Consultar(idMaquina);
            if (maquina == null)
            {
                return NotFound("La maquina no existe.");
            }

            // Obtener el código QR en formato Base64 desde la maquina
            string codigoQRBase64 = maquina.codigoQR;
            if (string.IsNullOrEmpty(codigoQRBase64))
            {
                return BadRequest("La maquina no tiene un código QR.");
            }

            // Decodificar el string Base64 a una imagen en bytes
            byte[] qrCodeBytes = Convert.FromBase64String(codigoQRBase64);

            // Devolver la imagen como archivo descargable
            return File(qrCodeBytes, "image/png", $"QRCode_{maquina.nombreMaquina}.png");
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
