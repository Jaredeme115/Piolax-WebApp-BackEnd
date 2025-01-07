using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface IInventarioService
    {
        Task<Inventario> RegistrarInventario(InventarioDTO inventarioDTO);
        Task<Inventario> Modificar(int idRefaccion, InventarioDTO inventarioDTO);
        Task<Inventario> Eliminar(int idRefaccion);
        Task<Inventario> ConsultarInventarioConDetalles(int idRefaccion);
        Task<IEnumerable<Inventario>> ConsultarTodoInventario();
        Task<Inventario> ConsultarInventarioPorNombre(string nombreProducto);
        Task<Inventario> ConsultarInventarioPorCategoria(int idInventarioCategoria);
    }
}
