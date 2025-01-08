using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IInventarioRepository
    {
        Task<Inventario> RegistrarInventario(Inventario inventario);
        Task<Inventario> Modificar(int idRefaccion, Inventario inventario);
        Task<Inventario> Eliminar(int idRefaccion);
        Task<IEnumerable<Inventario>> ConsultarTodoInventario();
        //Task<Inventario> ConsultarInventarioConDetalles(int idRefaccion);
        Task<Inventario> ConsultarInventarioPorNombre(string nombreProducto);
        Task<Inventario> ConsultarInventarioPorCategoria(int idInventarioCategoria);
        Task<Inventario> ConsultarInventarioPorID(int idRefaccion);
        Task<IEnumerable<Inventario>> ConsultarProductosPorCategoria(int idInventarioCategoria);
        Task<bool> ExisteProductoInventario(int idRefaccion);
        Task<bool> ExisteNumParte(string numParte);

    }
}
