using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class InventarioService(IInventarioRepository repository) : IInventarioService
    {
        private readonly IInventarioRepository _repository = repository;

        public async Task<Inventario> ConsultarInventarioConDetalles(int idRefaccion)
        {
            return await _repository.ConsultarInventarioConDetalles(idRefaccion);
        }

        public async Task<Inventario> ConsultarInventarioPorCategoria(int idInventarioCategoria)
        {
            return await _repository.ConsultarInventarioPorCategoria(idInventarioCategoria);
        }

        public async Task<Inventario> ConsultarInventarioPorNombre(string nombreProducto)
        {
            return await _repository.ConsultarInventarioPorNombre(nombreProducto);
        }

        public Task<IEnumerable<Inventario>> ConsultarTodoInventario()
        {
            return _repository.ConsultarTodoInventario();
        }

        public async Task<Inventario> Eliminar(int idRefaccion)
        {
            var productoExistente = await _repository.ConsultarInventarioConDetalles(idRefaccion);

            if (productoExistente == null)
                return null; // Devuelve null si el empleado no existe

            return await _repository.Eliminar(idRefaccion);
        }

        public async Task<Inventario> Modificar(int idRefaccion, InventarioDTO inventarioDTO)
        {
            var productoExistente = await _repository.ConsultarInventarioConDetalles(idRefaccion);

            if (productoExistente == null)
            {
                throw new Exception("El producto no existe");
            }

            // Actualizar los campos del producto existente
            productoExistente.descripcion = inventarioDTO.descripcion;
            productoExistente.ubicacion = inventarioDTO.ubicacion;
            productoExistente.idInventarioCategoria = inventarioDTO.idInventarioCategoria;
            productoExistente.cantidadActual = inventarioDTO.cantidadActual;
            productoExistente.cantidadMax = inventarioDTO.cantidadMax;
            productoExistente.cantidadMin = inventarioDTO.cantidadMin;
            productoExistente.piezaCritica = inventarioDTO.piezaCritica;
            productoExistente.nombreProducto = inventarioDTO.nombreProducto;
            productoExistente.numParte = inventarioDTO.numParte;
            productoExistente.proveedor = inventarioDTO.proveedor;
            productoExistente.precioUnitario = inventarioDTO.precioUnitario;
            productoExistente.precioInventarioTotal = inventarioDTO.precioInventarioTotal;
            productoExistente.codigoBarras = inventarioDTO.codigoBarras;
            productoExistente.codigoQR = inventarioDTO.codigoQR;
            productoExistente.proceso = inventarioDTO.proceso;
            productoExistente.idArea = inventarioDTO.idArea;
            productoExistente.idMaquina = inventarioDTO.idMaquina;
            productoExistente.fechaEntrega = inventarioDTO.fechaEntrega;
            productoExistente.inventarioActivoObsoleto = inventarioDTO.inventarioActivoObsoleto;

            return await _repository.Modificar(idRefaccion, productoExistente);

        }

        public async Task<Inventario> RegistrarInventario(InventarioDTO inventarioDTO)
        {
            // Transformar el DTO a la entidad Inventario
            var inventario = new Inventario
            {
                descripcion = inventarioDTO.descripcion,
                ubicacion = inventarioDTO.ubicacion,
                idInventarioCategoria = inventarioDTO.idInventarioCategoria,
                cantidadActual = inventarioDTO.cantidadActual,
                cantidadMax = inventarioDTO.cantidadMax,
                cantidadMin = inventarioDTO.cantidadMin,
                piezaCritica = inventarioDTO.piezaCritica,
                nombreProducto = inventarioDTO.nombreProducto,
                numParte = inventarioDTO.numParte,
                proveedor = inventarioDTO.proveedor,
                precioUnitario = inventarioDTO.precioUnitario,
                precioInventarioTotal = inventarioDTO.precioInventarioTotal,
                codigoBarras = inventarioDTO.codigoBarras,
                codigoQR = inventarioDTO.codigoQR,
                proceso = inventarioDTO.proceso,
                idArea = inventarioDTO.idArea,
                idMaquina = inventarioDTO.idMaquina,
                fechaEntrega = inventarioDTO.fechaEntrega,
                inventarioActivoObsoleto = inventarioDTO.inventarioActivoObsoleto
            };


            // Llamar al repositorio para registrar el inventario
            return await _repository.RegistrarInventario(inventario);
        }
    }
}
