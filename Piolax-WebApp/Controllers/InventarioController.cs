using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Services;
using Piolax_WebApp.Services.Impl;

namespace Piolax_WebApp.Controllers
{
    public class InventarioController(IInventarioService service): BaseApiController
    {
        private readonly IInventarioService _service = service;

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("Registro")]

        public async Task<ActionResult<Inventario>> RegistrarInventario(InventarioDTO inventarioDTO)
        {
            return await _service.RegistrarInventario(inventarioDTO);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("Modificar")]

        public async Task<ActionResult<Inventario>> Modificar(int idRefaccion, InventarioDTO inventarioDTO)
        {
            return await _service.Modificar(idRefaccion, inventarioDTO);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("Eliminar")]
        public async Task<ActionResult<Inventario>> Eliminar(int idRefaccion)
        {
            return await _service.Eliminar(idRefaccion);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("ConsultarTodoInventario")]
        public async Task<ActionResult<IEnumerable<Inventario>>> ConsultarTodoInventario()
        {
            return Ok(await _service.ConsultarTodoInventario());
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("ConsultarInventarioPorNombre")]
        public async Task<ActionResult<Inventario>> ConsultarInventarioPorNombre(string nombreProducto)
        {
            return await _service.ConsultarInventarioPorNombre(nombreProducto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("ConsultarInventarioPorCategoria")]
        public async Task<ActionResult<Inventario>> ConsultarInventarioPorCategoria(int idInventarioCategoria)
        {
            return await _service.ConsultarInventarioPorCategoria(idInventarioCategoria);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("ConsultarInventarioConDetalles")]
        public async Task<ActionResult<Inventario>> ConsultarInventarioConDetalles(int idRefaccion)
        {
            return await _service.ConsultarInventarioConDetalles(idRefaccion);
        }

    }
}
