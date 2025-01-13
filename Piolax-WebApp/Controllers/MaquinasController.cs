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

            // Generar el código QR visualmente
            string qrCodeImageBase64 = GenerateQRCode(maquina.descripcion); // Genera la imagen Base64 para mostrar

            // Asignar el texto del QR al campo codigoQR
            maquina.codigoQR = maquina.descripcion; // Guarda el contenido del QR como texto, no como la imagen

            // Puedes guardar la imagen Base64 en otro lugar, si lo necesitas (por ejemplo, para enviarlo al front-end).

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

        [HttpGet("DescargarQRCode/{idMaquina}")]
        public async Task<IActionResult> DescargarQRCode(int idMaquina)
        {
            // Consultar la máquina
            var maquina = await _service.Consultar(idMaquina);
            if (maquina == null)
            {
                return NotFound("La máquina no existe.");
            }

            // Validar el texto del QR
            string qrCodeText = maquina.codigoQR;
            if (string.IsNullOrEmpty(qrCodeText))
            {
                return BadRequest("La máquina no tiene un código QR.");
            }

            try
            {
                // Generar la imagen del QR a partir del texto
                byte[] qrCodeBytes = GenerateQRCodeBytes(qrCodeText);

                // Limpiar el nombre del archivo
                string sanitizedFileName = string.Join("_", maquina.nombreMaquina.Split(Path.GetInvalidFileNameChars()));
                string fileName = !string.IsNullOrWhiteSpace(sanitizedFileName)
                    ? $"QRCode_{sanitizedFileName}.png"
                    : "QRCode.png";

                // Devolver el archivo
                return File(qrCodeBytes, "image/png", fileName);
            }
            catch (Exception ex)
            {
                // Loguear errores inesperados
                Console.Error.WriteLine($"Error al generar el QR: {ex.Message}");
                return StatusCode(500, "Ocurrió un error inesperado al generar el código QR.");
            }
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
                            return Convert.ToBase64String(byteImage); // Esto genera la imagen en Base64
                        }
                    }
                }
            }
        }

        // Funcionalidad para generar la imagen QR
        private byte[] GenerateQRCodeBytes(string text)
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
                            return ms.ToArray();
                        }
                    }
                }
            }
        }



    }
}
