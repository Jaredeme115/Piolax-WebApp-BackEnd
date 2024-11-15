using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Services;
using System.Text;
using System.Security.Cryptography;

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

  
        [HttpGet("Consultar Todos")]
        public async Task<ActionResult<IEnumerable<Maquinas>>> ConsultarTodos()
        {
            return Ok(await _service.ConsultarTodos());
        }

     
        [HttpPost("Registro")]
        public async Task<ActionResult<Maquinas>> Registro(MaquinaDTO maquina)
        {
            if (await _service.MaquinaExisteRegistro(maquina.descripcion))
            {
                return BadRequest("La maquina ya esta registrada");
            }

            // Generar texto de QR automáticamente
            maquina.codigoQR = GenerarCodigoQR(maquina.descripcion);

            // Asignar idArea predeterminado si es necesario
            maquina.idArea = maquina.idArea == 0 ? 2 : maquina.idArea;


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

        private string GenerarCodigoQR(string descripcion)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(descripcion + DateTime.Now.Ticks));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
