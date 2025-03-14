using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IInventarioRepository
    {
        Task<Inventario> RegistrarInventario(Inventario inventario);
        Task<Inventario> Modificar(Inventario inventario);
        Task<Inventario> Eliminar(int idRefaccion);
        Task<IEnumerable<Inventario>> ConsultarTodoInventario();

        //Metodo modificado para mostrar los detalles de la refaccion en base al nombre de la misma
        Task<InventarioDetalleDTO?> ConsultarRefaccionPorNombre(string nombreProducto);

        Task<Inventario> ConsultarInventarioPorCategoria(int idInventarioCategoria);
        Task<Inventario> ConsultarInventarioPorID(int idRefaccion);
        Task<IEnumerable<Inventario>> ConsultarTodosLosProductosPorCategoria(int idInventarioCategoria);
        Task<bool> ExisteProductoInventario(int idRefaccion);
        Task<bool> ExisteNumParte(string numParte);
        Task ActualizarCantidadInventario(int idRefaccion, int cantidadADescontar);
        Task<int> ConsultarCantidadDisponible(int idRefaccion);
        Task<IEnumerable<Inventario>> ConsultarRefaccionesPorFiltros(bool? piezaCritica, bool? inventarioActivoObsoleto, string? proceso);
        Task<IEnumerable<string>> ConsultarNombresRefaccionesPorCategoria(int idCategoria);

        //Consultar Inventario con detalles
        Task<InventarioDetalleDTO?> ConsultarRefaccionDetalle(int idInventario);

        //Carga masiva de Inventario
        Task AddRangeAsync(IEnumerable<Inventario> inventario);

    }
}
