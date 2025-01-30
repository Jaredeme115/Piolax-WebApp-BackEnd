using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Repositories.Impl;
using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Services.Impl
{
    public class AsignacionRefaccionesService(
        IAsignacionRefaccionesRepository repository,
        IAsignacionRepository asignacionRepository,
        IInventarioRepository inventarioRepository
        ) : IAsignacionRefaccionesService
    {
        private readonly IAsignacionRefaccionesRepository _repository = repository;
        private readonly IAsignacionRepository _asignacionRepository = asignacionRepository;
        private readonly IInventarioRepository _inventarioRepository = inventarioRepository;

        public async Task<IEnumerable<asignacion_refacciones>> ConsultarRefaccionesPorAsignacion(int idAsignacion)
        {
            return await _repository.ConsultarRefaccionesPorAsignacion(idAsignacion);
        }

        public async Task<asignacion_refacciones> CrearAsignacionRefacciones(Asignacion_RefaccionesDTO asignacionRefaccionesDTO)
        {
            // Validar si la asignación existe
            if (await _asignacionRepository.ConsultarAsignacionPorId(asignacionRefaccionesDTO.idAsignacion) == null)
            {
                throw new ArgumentException("La asignación no existe.");
            }

            // Validar si la refacción existe en el inventario
            var inventario = await _inventarioRepository.ConsultarInventarioPorID(asignacionRefaccionesDTO.idRefaccion);
            if (inventario == null)
            {
                throw new ArgumentException("La refacción no existe en el inventario.");
            }

            // Validar si hay suficiente cantidad en el inventario
            if (inventario.cantidadActual < asignacionRefaccionesDTO.cantidad)
            {
                throw new ArgumentException("No hay suficiente cantidad disponible en el inventario.");
            }

            var refacciones = new asignacion_refacciones
            {
                idAsignacion = asignacionRefaccionesDTO.idAsignacion,
                idRefaccion = asignacionRefaccionesDTO.idRefaccion,
                cantidad = asignacionRefaccionesDTO.cantidad,
                idAsignacionTecnico = asignacionRefaccionesDTO.idAsignacionTecnico
            };

            return await _repository.CrearAsignacionRefacciones(refacciones);
        }
        

        public async Task<IEnumerable<Asignacion_RefaccionesDetallesDTO>> ConsultarTodasLasRefacciones()
        {
            var refacciones = await _repository.ConsultarTodasLasRefacciones();
            return refacciones.Select(refaccion => new Asignacion_RefaccionesDetallesDTO
            {
                
                idAsignacion = refaccion.idAsignacion,
                idRefaccion = refaccion.idRefaccion,
                nombreRefaccion = refaccion.Inventario.nombreProducto,
                cantidad = refaccion.cantidad
            });
        }

        public async Task<bool> EliminarRefaccionDeAsignacion(int idAsignacionRefaccion)
        {
            var refaccion = await _repository.ConsultarRefaccionesPorId(idAsignacionRefaccion);
            if (refaccion == null) return false;

            // Validar que la asignación no esté en un estado bloqueado
            var asignacion = await _asignacionRepository.ConsultarAsignacionPorId(refaccion.idAsignacion);
            if (asignacion == null)
            {
                throw new InvalidOperationException("La asignación no existe.");
            }

            if (asignacion.StatusAsignacion == null)
            {
                throw new InvalidOperationException("El estado de la asignación no está definido.");
            }

            if (asignacion.StatusAsignacion.descripcionStatusAsignacion == "Pausa Por Tecnico")
            {
                throw new InvalidOperationException("No se puede eliminar una refacción de una asignación Pausada.");
            }

            return await _repository.EliminarRefaccionDeAsignacion(idAsignacionRefaccion);
        }

        public async Task<bool> ActualizarRefaccionEnAsignacion(Asignacion_RefaccionesDTO asignacionRefaccionesDTO)
        {
            var refacciones = new asignacion_refacciones
            {
                idAsignacion = asignacionRefaccionesDTO.idAsignacion,
                idRefaccion = asignacionRefaccionesDTO.idRefaccion,
                cantidad = asignacionRefaccionesDTO.cantidad,
                idAsignacionTecnico = asignacionRefaccionesDTO.idAsignacionTecnico
            };
            return await _repository.ActualizarRefaccionEnAsignacion(refacciones);
        }

        public async Task<IEnumerable<Asignacion_RefaccionesDetallesDTO>> ConsultarRefaccionesConDetallesPorAsignacion(int idAsignacion)
        {

            // Consulta directamente las refacciones relacionadas con la asignación
            var refacciones = await _repository.ConsultarRefaccionesPorAsignacion(idAsignacion);

            // Si no hay refacciones, devuelve una colección vacía
            if (!refacciones.Any())
            {
                return Enumerable.Empty<Asignacion_RefaccionesDetallesDTO>();
            }

            // Mapear a DTOs
            return refacciones.Select(refaccion => new Asignacion_RefaccionesDetallesDTO
            {
                idAsignacionRefaccion = refaccion.idAsignacionRefaccion,
                idAsignacion = refaccion.idAsignacion,
                idRefaccion = refaccion.idRefaccion,
                nombreRefaccion = refaccion.Inventario.nombreProducto,
                cantidad = refaccion.cantidad
            });
        }
    }

}
