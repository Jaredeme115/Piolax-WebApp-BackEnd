using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class InventarioService(IInventarioRepository repository) : IInventarioService
    {
        private readonly IInventarioRepository _repository = repository;

        public Task<Inventario> ConsultarInventarioConDetalles(int idRefaccion)
        {
            return _repository.ConsultarInventarioConDetalles(idRefaccion);
        }

        public Task<Inventario> ConsultarInventarioPorCategoria(string categoria)
        {
            return _repository.ConsultarInventarioPorCategoria(categoria);
        }

        public Task<Inventario> ConsultarInventarioPorNombre(string nombreProducto)
        {
            return _repository.ConsultarInventarioPorNombre(nombreProducto);
        }

        public Task<IEnumerable<Inventario>> ConsultarTodoInventario()
        {
            return _repository.ConsultarTodoInventario();
        }

        public Task<Inventario> Eliminar(int idRefaccion)
        {
            return _repository.Eliminar(idRefaccion);
        }

        public Task<Inventario> Modificar(int idRefaccion, Inventario inventario)
        {
            return _repository.Modificar(idRefaccion, inventario);
        }

        public Task<Inventario> RegistrarInventario(Inventario inventario)
        {
            return _repository.RegistrarInventario(inventario);
        }
    }
}
