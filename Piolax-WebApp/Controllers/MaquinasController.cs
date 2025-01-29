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
using OfficeOpenXml;
using Piolax_WebApp.Services.Impl;

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
                return BadRequest("La máquina ya está registrada.");
            }

            // Generar el código QR utilizando el servicio
            maquina.codigoQR = maquina.descripcion;

            // Registrar la máquina
            return Ok(await _service.Registro(maquina));
        }

        [HttpPut("Modificar")]
        public async Task<ActionResult<Maquinas>> Modificar(int idMaquina, MaquinaDTO maquina)
        {
            if (!await _service.MaquinaExiste(idMaquina))
            {
                return NotFound("La máquina no existe.");
            }

            // Regenerar el código QR si se modifica la descripción
            maquina.codigoQR = maquina.descripcion;

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

        [HttpPost("RegistrarMaquinasDesdeExcel")]
        public async Task<IActionResult> RegistrarMaquinasDesdeExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("El archivo proporcionado es inválido o está vacío.");
            }

            try
            {
                var resultado = await _service.RegistrarMaquinasDesdeExcel(file);
                return Ok(new { Message = resultado });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Ocurrió un error inesperado.", Details = ex.Message });
            }
        }


        [HttpGet("DescargarQRCode/{idMaquina}")]
        public async Task<IActionResult> DescargarQRCode(int idMaquina)
        {
            var maquina = await _service.Consultar(idMaquina);
            if (maquina == null)
            {
                return NotFound("La máquina no existe.");
            }

            string qrCodeText = maquina.codigoQR;
            if (string.IsNullOrEmpty(qrCodeText))
            {
                return BadRequest("La máquina no tiene un código QR.");
            }

            try
            {
                byte[] qrCodeBytes = _service.GenerateQRCodeBytes(qrCodeText); // Usa el método del servicio

                string sanitizedFileName = string.Join("_", maquina.nombreMaquina.Split(Path.GetInvalidFileNameChars()));
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


        private byte[] GenerateQRCodeBytes(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("El texto para generar el código QR no puede estar vacío.");

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            qrCodeImage.Save(ms, ImageFormat.Png); // Guarda la imagen como PNG en el stream
                            return ms.ToArray(); // Devuelve los bytes de la imagen
                        }
                    }
                }
            }
        }



    }
}
