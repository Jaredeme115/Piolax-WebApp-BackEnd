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

            if (inventarioCategoriaDTO == null || string.IsNullOrWhiteSpace(inventarioCategoriaDTO.nombreInventarioCategoria))
            {
                throw new ArgumentException("El objeto InventarioCategoriasDTO no puede ser nulo o vacío.");
            }

            var nuevaCategoria = new InventarioCategorias
            {
                nombreInventarioCategoria = inventarioCategoriaDTO.nombreInventarioCategoria
            };

            var categoriaGuardada = await _repository.RegistrarInventarioCategoria(nuevaCategoria);

            if (categoriaGuardada == null)
            {
                throw new Exception("Error al registrar la categoría en la base de datos.");
            }

            return categoriaGuardada;
        }

        public async Task<InventarioCategorias?> Modificar(int idInventarioCategoria, InventarioCategoriasDTO inventarioCategoriaDTO)
        {
            var categoriaExistente = await _repository.ConsultarCategoriaPorID(idInventarioCategoria);

            if (categoriaExistente == null)
            {
                return null; // Retornar null para manejarlo en el controlador
            }

            // Actualizamos los datos de la Categoría
            categoriaExistente.nombreInventarioCategoria = inventarioCategoriaDTO.nombreInventarioCategoria;

            var resultado = await _repository.Modificar(idInventarioCategoria, categoriaExistente);
            return resultado ?? throw new Exception("Error inesperado al modificar la categoría.");
        }

        public async Task<InventarioCategorias?> Eliminar(int idInventarioCategoria)
        {
            var categoriaExistente = await _repository.ConsultarCategoriaPorID(idInventarioCategoria);

            if (categoriaExistente == null)
            {
                return null; // Para manejarlo en el controlador
            }

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
