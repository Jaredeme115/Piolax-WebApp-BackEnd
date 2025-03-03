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

        public async Task<ActionResult<InventarioCategorias>> RegistrarInventarioCategoria([FromBody] InventarioCategoriasDTO inventarioCategoriaDTO)
        {
            // Verifica si el modelo es válido
            if (!ModelState.IsValid)
            {
                return BadRequest("Los datos de la categoría son inválidos.");
            }

            // Verifica si el nombre de la categoría ya existe
            if (await _service.CategoriaExistePorNombre(inventarioCategoriaDTO.nombreInventarioCategoria))
            {
                return Conflict("La categoría ya existe en el sistema.");
            }

            try
            {
                var nuevaCategoria = await _service.RegistrarInventarioCategoria(inventarioCategoriaDTO);
                return CreatedAtAction(nameof(RegistrarInventarioCategoria), new { id = nuevaCategoria.idInventarioCategoria }, nuevaCategoria);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("ModificarInventarioCategoria/{idInventarioCategoria}")]

        public async Task<ActionResult<InventarioCategorias>> Modificar(int idInventarioCategoria, [FromBody] InventarioCategoriasDTO inventarioCategoriaDTO)
        {
            if (inventarioCategoriaDTO == null)
            {
                return BadRequest("Los datos de la categoría son inválidos.");
            }

            if (await _service.CategoriaExistePorNombre(inventarioCategoriaDTO.nombreInventarioCategoria))
            {
                return Conflict("La categoría ya existe.");
            }

            try
            {
                var categoriaActualizada = await _service.Modificar(idInventarioCategoria, inventarioCategoriaDTO);
                return categoriaActualizada != null ? Ok(categoriaActualizada) : NotFound("No se encontró la categoría.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al modificar la categoría: " + ex.Message);
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("EliminarInventarioCategoria/{idInventarioCategoria}")]

        public async Task<ActionResult<InventarioCategorias>> Eliminar(int idInventarioCategoria)
        {
            var existeCategoria = await _service.CategoriaExistePorID(idInventarioCategoria);
            if (!existeCategoria)
            {
                return NotFound("La categoría no existe.");
            }

            try
            {
                var categoriaEliminada = await _service.Eliminar(idInventarioCategoria);
                return categoriaEliminada != null ? Ok(categoriaEliminada) : StatusCode(500, "Error al eliminar la categoría.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        //[Authorize(Policy = "AdminOnly")]
        /*[HttpGet("ConsultarTodasCategorias")]

        public async Task<IEnumerable<InventarioCategorias>> ConsultarTodasCategorias()
        {
            return await _service.ConsultarTodasCategorias();
        }*/

        [HttpGet("InventarioCategoriasNombres")]
        public async Task<ActionResult<IEnumerable<string>>> ObtenerNombresCategorias()
        {
            var categorias = await _service.ObtenerNombresCategorias();

            if (!categorias.Any())
            {
                return NotFound("No hay categorías registradas.");
            }

            return Ok(categorias);
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
