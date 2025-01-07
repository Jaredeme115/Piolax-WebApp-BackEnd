using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class InventarioCategoriasService(IInventarioCategoriasRepository repository): IInventarioCategoriasService
    {
        private readonly IInventarioCategoriasRepository _repository = repository;

        public async Task<InventarioCategorias> RegistrarInventarioCategoria(InventarioCategoriasDTO inventarioCategoriaDTO)
        {

            var inventarioCategoria = new InventarioCategorias
            {
                nombreInventarioCategoria = inventarioCategoriaDTO.nombreInventarioCategoria
            };
            return await _repository.RegistrarInventarioCategoria(inventarioCategoria);
        }

        public async Task<InventarioCategorias> Modificar(int idInventarioCategoria, InventarioCategoriasDTO inventarioCategoriaDTO)
        {
            var categoriaExiste = await _repository.ConsultarCategoriaPorID(idInventarioCategoria);
            if (categoriaExiste == null)
            {
                throw new Exception("La categoria no existe");
            }

            // Actualizamos los datos de la Categoria
            categoriaExiste.nombreInventarioCategoria = inventarioCategoriaDTO.nombreInventarioCategoria;

            return await _repository.Modificar(idInventarioCategoria, categoriaExiste);
        }

        public async Task<InventarioCategorias> Eliminar(int idInventarioCategoria)
        {
            return await _repository.Eliminar(idInventarioCategoria);
        }

        public async Task<IEnumerable<InventarioCategorias>> ConsultarTodasCategorias()
        {
            return await _repository.ConsultarTodasCategorias();
        }

        public async Task<InventarioCategorias> ConsultarCategoriaPorID(int idInventarioCategoria) 
        {
            var categoriaExiste = await _repository.ConsultarCategoriaPorID(idInventarioCategoria);
            if (categoriaExiste == null)
            {
                throw new Exception("La categoria no existe");
            }

            return await _repository.ConsultarCategoriaPorID(idInventarioCategoria);
        }

        public async Task<InventarioCategorias> ConsultarCategoriaPorNombre(string nombreInventarioCategoria)
        {
            var categoriaExiste = await _repository.ConsultarCategoriaPorNombre(nombreInventarioCategoria);
            if (categoriaExiste == null)
            {
                throw new Exception("La categoria no existe");
            }

            return await _repository.ConsultarCategoriaPorNombre(nombreInventarioCategoria);
        }

        public async Task<bool> CategoriaExistePorNombre(string nombreInventarioCategoria)
        {
            return await _repository.CategoriaExistePorNombre(nombreInventarioCategoria);
        }

        public async Task<bool> CategoriaExistePorID(int idInventarioCategoria)
        {
            return await _repository.CategoriaExistePorID(idInventarioCategoria);
        }
    }
}
