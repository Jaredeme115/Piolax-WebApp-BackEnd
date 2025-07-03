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

        public async Task<IEnumerable<MantenimientoPreventivo_Refacciones>> ConsultarRefaccionesMP(int idHistoricoMP)
        {
            return await _repository.ConsultarRefaccionesMP(idHistoricoMP);
        }

        public async Task<MPRefaccionesResponseDTO> CrearRefaccionMP(MantenimientoPreventivo_Refacciones mantenimientoPreventivoRefacciones)
        {
            // Validar si el mantenimiento preventivo existe
            if (await _mantenimientoPreventivoRepository.ConsultarMP(mantenimientoPreventivoRefacciones.idHistoricoMP) == null)
            {
                throw new ArgumentException("El mantenimiento preventivo no existe.");
            }

            // Validar si la refacción existe en el inventario
            var inventario = await _inventarioRepository.ConsultarInventarioPorID(mantenimientoPreventivoRefacciones.idRefaccion);
            if (inventario == null)
            {
                throw new ArgumentException("La refacción no existe en el inventario.");
            }

            // Crear el registro de refacción utilizada en el mantenimiento preventivo
            var refaccionMP = new MantenimientoPreventivo_Refacciones
            {
                idHistoricoMP = mantenimientoPreventivoRefacciones.idHistoricoMP,
                idRefaccion = mantenimientoPreventivoRefacciones.idRefaccion,
                cantidad = mantenimientoPreventivoRefacciones.cantidad
            };

            // Guardar en la base de datos
            var nuevaRefaccion = await _repository.CrearRefaccionMP(refaccionMP);

            // Mapear la entidad a DTO de respuesta
            return new MPRefaccionesResponseDTO
            {
                idMPRefaccion = nuevaRefaccion.idMPRefaccion,
                idHistoricoMP = nuevaRefaccion.idHistoricoMP,
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

            return await _repository.EliminarRefaccionMP(idMPRefaccion);
        }

        public async Task<bool> ActualizarRefaccionMP(MantenimientoPreventivo_Refacciones mantenimientoPreventivoRefacciones)
        {
            var existente = await _repository.ConsultarRefaccionPorId(mantenimientoPreventivoRefacciones.idMPRefaccion);
            if (existente == null) return false;

            return await _repository.ActualizarRefaccionMP(mantenimientoPreventivoRefacciones);
        }

        public async Task<MantenimientoPreventivoRefaccionesDetalleDTO> ConsultarRefaccionPorId(int idMPRefaccion)
        {
            var refaccion = await _repository.ConsultarRefaccionPorId(idMPRefaccion);
            if (refaccion == null) return null;

            return new MantenimientoPreventivoRefaccionesDetalleDTO
            {
                idMPRefaccion = refaccion.idMPRefaccion,
                idHistoricoMP = refaccion.idHistoricoMP,
                idRefaccion = refaccion.idRefaccion,
                nombreRefaccion = refaccion.Inventario?.nombreProducto ?? "N/A",
                cantidad = refaccion.cantidad
            };
        }

        public async Task<bool> ConfirmarUsoDeRefacciones(int idHistoricoMP)
        {
            var refacciones = await _repository.ConsultarRefaccionesMP(idHistoricoMP);
            if (!refacciones.Any()) return false;

            foreach (var refaccion in refacciones)
            {
                var inventario = await _inventarioRepository.ConsultarInventarioPorID(refaccion.idRefaccion);
                if (inventario == null)
                {
                    throw new ArgumentException($"La refacción con ID {refaccion.idRefaccion} no existe en el inventario.");
                }

                // Verificar si hay suficiente stock antes de descontar
                if (inventario.cantidadActual < refaccion.cantidad)
                {
                    throw new ArgumentException($"Stock insuficiente para la refacción con ID {refaccion.idRefaccion}.");
                }

                // Descontar del inventario
                await _inventarioRepository.ActualizarCantidadInventario(refaccion.idRefaccion, refaccion.cantidad);
            }

            return true;
        }

    }
}

