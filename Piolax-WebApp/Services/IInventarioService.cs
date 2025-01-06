using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface IInventarioService
    {
        Task<Inventario> RegistrarInventario(Inventario inventario);
        Task<Inventario> Modificar(int idRefaccion, Inventario inventario);
        Task<Inventario> Eliminar(int idRefaccion);
        Task<Inventario> ConsultarInventarioConDetalles(int idRefaccion);
        Task<IEnumerable<Inventario>> ConsultarTodoInventario();
        Task<Inventario> ConsultarInventarioPorNombre(string nombreProducto);
        Task<Inventario> ConsultarInventarioPorCategoria(string categoria);
    }
}
