using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface IInventarioCategoriasService
    {
        Task<InventarioCategorias> RegistrarInventarioCategoria(InventarioCategoriasDTO inventarioCategoriaDTO);
        Task<InventarioCategorias> Modificar(int idInventarioCategoria, InventarioCategoriasDTO inventarioCategoriaDTO);
        Task<InventarioCategorias> Eliminar(int idInventarioCategoria);

        Task<IEnumerable<string>> ObtenerNombresCategorias();
        //Task<IEnumerable<string>> ConsultarTodasCategorias();
        Task<InventarioCategorias> ConsultarCategoriaPorNombre(string nombreInventarioCategoria);
        Task<InventarioCategorias> ConsultarCategoriaPorID(int idInventarioCategoria);
        Task<bool> CategoriaExistePorNombre(string nombreInventarioCategoria);
        Task<bool> CategoriaExistePorID(int idInventarioCategoria);



    }
}
