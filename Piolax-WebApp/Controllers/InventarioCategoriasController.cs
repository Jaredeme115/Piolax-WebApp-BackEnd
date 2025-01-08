using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Services;

namespace Piolax_WebApp.Controllers
{
    public class InventarioCategoriasController(IInventarioCategoriasService service): BaseApiController
    {
        private readonly IInventarioCategoriasService _service = service;

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("RegistroInventarioCategoria")]

        public async Task<ActionResult<InventarioCategorias>> RegistrarInventarioCategoria(InventarioCategoriasDTO inventarioCategoriaDTO)
        {
            if(await _service.CategoriaExistePorNombre(inventarioCategoriaDTO.nombreInventarioCategoria))
            {
                return BadRequest("La categoria ya existe");
            }

            return await _service.RegistrarInventarioCategoria(inventarioCategoriaDTO);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("ModificarInventarioCategoria")]

        public async Task<ActionResult<InventarioCategorias>> Modificar(int idInventarioCategoria, InventarioCategoriasDTO inventarioCategoriaDTO)
        {
            if(await _service.CategoriaExistePorNombre(inventarioCategoriaDTO.nombreInventarioCategoria))
            {
                return BadRequest("La categoria ya existe");
            }
            return await _service.Modificar(idInventarioCategoria, inventarioCategoriaDTO);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("EliminarInventarioCategoria")]

        public async Task<ActionResult<InventarioCategorias>> Eliminar(int idInventarioCategoria)
        {
            if(await _service.CategoriaExistePorID(idInventarioCategoria))
            {
                return await _service.Eliminar(idInventarioCategoria);
            }

            return BadRequest("La categoria no existe");
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("ConsultarTodasCategorias")]

        public async Task<IEnumerable<InventarioCategorias>> ConsultarTodasCategorias()
        {
            return await _service.ConsultarTodasCategorias();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("ConsultarCategoriasPorID")]

        public async Task<InventarioCategorias> ConsultarCategoriaPorID(int idInventarioCategoria)
        {
            return await _service.ConsultarCategoriaPorID(idInventarioCategoria);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("ConsultarCategoriasPorNombre")]

        public async Task<InventarioCategorias> ConsultarCategoriaPorNombre(string nombreInventarioCategoria)
        {
            return await _service.ConsultarCategoriaPorNombre(nombreInventarioCategoria);
        }
    }
}
