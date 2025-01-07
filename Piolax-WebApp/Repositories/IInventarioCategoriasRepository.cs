using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IInventarioCategoriasRepository
    {
        Task<InventarioCategorias> RegistrarInventarioCategoria(InventarioCategorias inventarioCategoria);
        Task<InventarioCategorias> Modificar(int idInventarioCategoria, InventarioCategorias inventarioCategoria);
        Task<InventarioCategorias> Eliminar(int idInventarioCategoria);
        Task<IEnumerable<InventarioCategorias>> ConsultarTodasCategorias();
        Task<InventarioCategorias> ConsultarCategoriaPorNombre(string nombreInventarioCategoria);
        Task<InventarioCategorias> ConsultarCategoriaPorID(int idInventarioCategoria);
        Task<bool> CategoriaExistePorNombre(string nombreInventarioCategoria);
        Task<bool> CategoriaExistePorID(int idInventarioCategoria);

    }
}
