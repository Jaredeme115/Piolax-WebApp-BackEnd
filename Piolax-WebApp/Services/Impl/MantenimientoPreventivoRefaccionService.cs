using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class MantenimientoPreventivoRefaccionService(
        IMantenimientoPreventivoRefaccionesRepository repository,
        IMantenimientoPreventivoRepository mantenimientoPreventivoRepository,
        IInventarioRepository inventarioRepository
        ) : IMantenimientoPreventivoRefaccionesService
    {
        private readonly IMantenimientoPreventivoRefaccionesRepository _repository = repository;
        private readonly IMantenimientoPreventivoRepository _mantenimientoPreventivoRepository = mantenimientoPreventivoRepository;
        private readonly IInventarioRepository _inventarioRepository = inventarioRepository;

        public async Task<IEnumerable<MantenimientoPreventivo_Refacciones>> ConsultarRefaccionesMP(int idMP)
        {
            return await _repository.ConsultarRefaccionesMP(idMP);
        }

        public async Task<MantenimientoPreventivoRefaccionesDTO> CrearRefaccionMP(MantenimientoPreventivo_Refacciones mantenimientoPreventivoRefacciones)
        {
            // Validar si el mantenimiento preventivo existe
            if (await _mantenimientoPreventivoRepository.ConsultarMP(mantenimientoPreventivoRefacciones.idMP) == null)
            {
                throw new ArgumentException("El mantenimiento preventivo no existe.");
            }

            // Validar si la refacción existe en el inventario
            var inventario = await _inventarioRepository.ConsultarInventarioPorID(mantenimientoPreventivoRefacciones.idRefaccion);
            if (inventario == null)
            {
                throw new ArgumentException("La refacción no existe en el inventario.");
            }

            // Validar si hay suficiente cantidad en el inventario
            if (inventario.cantidadActual < mantenimientoPreventivoRefacciones.cantidad)
            {
                throw new ArgumentException("No hay suficiente cantidad disponible en el inventario.");
            }

            // Restar la cantidad usada del inventario
            inventario.cantidadActual -= mantenimientoPreventivoRefacciones.cantidad;
            await _inventarioRepository.ActualizarInventario(inventario);

            // Crear el registro de refacción utilizada en el mantenimiento preventivo
            var refaccionMP = new MantenimientoPreventivo_Refacciones
            {
                idMP = mantenimientoPreventivoRefacciones.idMP,
                idRefaccion = mantenimientoPreventivoRefacciones.idRefaccion,
                cantidad = mantenimientoPreventivoRefacciones.cantidad
            };

            // Guardar en la base de datos
            var nuevaRefaccion = await _repository.CrearRefaccionMP(refaccionMP);

            // Mapear la entidad a DTO de respuesta
            return new MantenimientoPreventivoRefaccionesDTO
            {
                idMPRefaccion = nuevaRefaccion.idMPRefaccion,
                idMP = nuevaRefaccion.idMP,
                idRefaccion = nuevaRefaccion.idRefaccion,
                cantidad = nuevaRefaccion.cantidad
            };
        }

        public async Task<IEnumerable<MantenimientoPreventivo_Refacciones>> ConsultarTodasLasRefacciones()
        {
            return await _repository.ConsultarTodasLasRefacciones();
        }

        public async Task<bool> EliminarRefaccionMP(int idMPRefaccion)
        {
            var refaccion = await _repository.ConsultarRefaccionPorId(idMPRefaccion);
            if (refaccion == null) return false;

            // Devolver la cantidad eliminada al inventario
            var inventario = await _inventarioRepository.ConsultarInventarioPorID(refaccion.idRefaccion);
            if (inventario != null)
            {
                inventario.cantidadActual += refaccion.cantidad;
                await _inventarioRepository.ActualizarInventario(inventario);
            }

            return await _repository.EliminarRefaccionMP(idMPRefaccion);
        }

        public async Task<bool> ActualizarRefaccionMP(MantenimientoPreventivo_Refacciones mantenimientoPreventivoRefacciones)
        {
            var existente = await _repository.ConsultarRefaccionPorId(mantenimientoPreventivoRefacciones.idMPRefaccion);
            if (existente == null) return false;

            // Validar cambios en la cantidad
            if (existente.cantidad != mantenimientoPreventivoRefacciones.cantidad)
            {
                var inventario = await _inventarioRepository.ConsultarInventarioPorID(existente.idRefaccion);
                if (inventario == null)
                {
                    throw new ArgumentException("La refacción no existe en el inventario.");
                }

                int diferencia = mantenimientoPreventivoRefacciones.cantidad - existente.cantidad;

                // Verificar si hay suficiente stock para aumentar la cantidad
                if (diferencia > 0 && inventario.cantidadActual < diferencia)
                {
                    throw new ArgumentException("No hay suficiente cantidad en el inventario.");
                }

                // Actualizar el stock en el inventario
                inventario.cantidadActual -= diferencia;
                await _inventarioRepository.ActualizarInventario(inventario);
            }

            return await _repository.ActualizarRefaccionMP(mantenimientoPreventivoRefacciones);
        }

        public async Task<MantenimientoPreventivoRefaccionesDetalleDTO> ConsultarRefaccionPorId(int idMPRefaccion)
        {
            var refaccion = await _repository.ConsultarRefaccionPorId(idMPRefaccion);
            if (refaccion == null) return null;

            return new MantenimientoPreventivoRefaccionesDetalleDTO
            {
                idMPRefaccion = refaccion.idMPRefaccion,
                idMP = refaccion.idMP,
                idRefaccion = refaccion.idRefaccion,
                nombreRefaccion = refaccion.Inventario?.nombreProducto ?? "N/A",
                cantidad = refaccion.cantidad
            };
        }
    }
}

