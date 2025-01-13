using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class AsignacionesService : IAsignacionesService
    {
        private readonly IAsignacionesRepository _repository;
        private readonly ISolicitudesRepository _solicitudesRepository;
        private readonly IInventarioService _inventarioService;
        private readonly IMaquinasService _maquinasService;

        public AsignacionesService(
            IAsignacionesRepository repository,
            ISolicitudesRepository solicitudesRepository,
            IInventarioService inventarioService,
            IMaquinasService maquinasService)
        {
            _repository = repository;
            _solicitudesRepository = solicitudesRepository;
            _inventarioService = inventarioService;
            _maquinasService = maquinasService;
        }

        public async Task<AsignacionesDetalleDTO> RegistrarAsignacion(AsignacionesDTO asignacionesDTO)
        {
            // Validar si la solicitud existe
            var solicitud = await _solicitudesRepository.ObtenerSolicitudConDetalles(asignacionesDTO.idSolicitud);
            if (solicitud == null)
            {
                throw new Exception($"No se encontró la solicitud con ID: {asignacionesDTO.idSolicitud}");
            }

            // Validar si la máquina asociada a la solicitud existe
            var maquina = await _maquinasService.Consultar(solicitud.idMaquina);
            if (maquina == null)
            {
                throw new Exception($"No se encontró la máquina asociada a la solicitud (ID Máquina: {solicitud.idMaquina}).");
            }

            // Validar el código QR escaneado
            if (string.IsNullOrEmpty(asignacionesDTO.codigoQR))
            {
                throw new Exception("El código QR escaneado no puede estar vacío.");
            }

            if (maquina.codigoQR != asignacionesDTO.codigoQR)
            {
                throw new Exception("El código QR escaneado no coincide con el de la máquina seleccionada en la solicitud.");
            }

            // Establecer la hora de inicio como TimeSpan
            var horaActual = DateTime.Now.TimeOfDay;

            // Crear la nueva asignación
            var asignacion = new Asignaciones
            {
                idSolicitud = asignacionesDTO.idSolicitud,
                idEmpleado = asignacionesDTO.idEmpleado,
                qrScaneado = true, // El QR fue validado
                horaInicio = horaActual, // Hora actual como TimeSpan
                horaTermino = asignacionesDTO.horaTermino, // Hora que el técnico termina
                solucion = asignacionesDTO.solucion,
                idRefaccion = asignacionesDTO.idRefaccion,
                cantidad = asignacionesDTO.cantidad,
                maquinaDetenida = asignacionesDTO.maquinaDetenida,
                idCategoriaAsignacion = asignacionesDTO.idCategoriaAsignacion,
                idStatusAprobacionTecnico = asignacionesDTO.idStatusAprobacionTecnico
            };

            // Guardar la asignación
            var asignacionGuardada = await _repository.RegistrarAsignacion(asignacion);

            // Descontar inventario si aplica
            if (asignacionesDTO.idRefaccion != 0 && asignacionesDTO.cantidad > 0)
            {
                await _inventarioService.ActualizarCantidadInventario(asignacionesDTO.idRefaccion, asignacionesDTO.cantidad);
            }

            // Retornar los detalles de la asignación
            return await ObtenerAsignacionConDetalles(asignacionGuardada.idAsignacion);
        }


        public async Task<AsignacionesDetalleDTO> ObtenerAsignacionConDetalles(int idAsignacion)
        {
            var asignacion = await _repository.ObtenerAsignacionConDetalles(idAsignacion);
            if (asignacion == null)
            {
                throw new Exception($"No se encontró la asignación con ID: {idAsignacion}");
            }

            return new AsignacionesDetalleDTO
            {
                idAsignacion = asignacion.idAsignacion,
                idSolicitud = asignacion.idSolicitud,
                nombreCompletoTecnico = $"{asignacion.Empleado.nombre} {asignacion.Empleado.apellidoPaterno} {asignacion.Empleado.apellidoMaterno}",
                qrScaneado = asignacion.qrScaneado,
                horaInicio = asignacion.horaInicio,
                horaTermino = asignacion.horaTermino,
                solucion = asignacion.solucion,
                idRefaccion = asignacion.idRefaccion,
                cantidad = asignacion.cantidad,
                maquinaDetenida = asignacion.maquinaDetenida,
                idCategoriaAsignacion = asignacion.idCategoriaAsignacion,
                idStatusAprobacionTecnico = asignacion.idStatusAprobacionTecnico,
                nombreRefaccion = asignacion.Inventario?.nombreProducto ?? "N/A",
                nombreCategoriaAsignacion = asignacion.CategoriaAsignacion?.descripcion ?? "N/A",
                nombreStatusAprobacionTecnico = asignacion.StatusAprobacionTecnico?.descripcionStatusAprobacionTecnico ?? "N/A"
            };
        }


        public async Task<IEnumerable<AsignacionesDetalleDTO>> ObtenerTodasLasAsignaciones()
        {
            var asignaciones = await _repository.ObtenerTodasLasAsignaciones();

            return asignaciones.Select(asignacion => new AsignacionesDetalleDTO
            {
                idAsignacion = asignacion.idAsignacion,
                idSolicitud = asignacion.idSolicitud,
                nombreCompletoTecnico = $"{asignacion.Empleado.nombre ?? "N/A"} {asignacion.Empleado.apellidoPaterno ?? ""} {asignacion.Empleado.apellidoMaterno ?? ""}".Trim(),
                qrScaneado = asignacion.qrScaneado,
                horaInicio = asignacion.horaInicio,
                horaTermino = asignacion.horaTermino,
                solucion = asignacion.solucion ?? "Sin solución",
                idRefaccion = asignacion.idRefaccion,
                cantidad = asignacion.cantidad,
                maquinaDetenida = asignacion.maquinaDetenida,
                idCategoriaAsignacion = asignacion.idCategoriaAsignacion,
                idStatusAprobacionTecnico = asignacion.idStatusAprobacionTecnico,
                nombreRefaccion = asignacion.Inventario?.nombreProducto ?? "N/A",
                nombreCategoriaAsignacion = asignacion.CategoriaAsignacion?.descripcion ?? "N/A",
                nombreStatusAprobacionTecnico = asignacion.StatusAprobacionTecnico?.descripcionStatusAprobacionTecnico ?? "N/A"
            });
        }


        public async Task<IEnumerable<AsignacionesDetalleDTO>> ObtenerAsignacionPorTecnico(string numNomina)
        {
            var asignaciones = await _repository.ObtenerAsignacionPorTecnico(numNomina);

            return asignaciones.Select(asignacion => new AsignacionesDetalleDTO
            {
                idAsignacion = asignacion.idAsignacion,
                idSolicitud = asignacion.idSolicitud,
                nombreCompletoTecnico = $"{asignacion.Empleado.nombre} {asignacion.Empleado.apellidoPaterno} {asignacion.Empleado.apellidoMaterno}",
                qrScaneado = asignacion.qrScaneado,
                horaInicio = asignacion.horaInicio,
                horaTermino = asignacion.horaTermino,
                solucion = asignacion.solucion,
                idRefaccion = asignacion.idRefaccion,
                cantidad = asignacion.cantidad,
                maquinaDetenida = asignacion.maquinaDetenida,
                idCategoriaAsignacion = asignacion.idCategoriaAsignacion,
                idStatusAprobacionTecnico = asignacion.idStatusAprobacionTecnico,
                nombreRefaccion = asignacion.Inventario.nombreProducto,
                nombreCategoriaAsignacion = asignacion.CategoriaAsignacion.descripcion,
                nombreStatusAprobacionTecnico = asignacion.StatusAprobacionTecnico.descripcionStatusAprobacionTecnico
            });
        }

        public async Task<AsignacionesDetalleDTO> ModificarEstatusAprobacionTecnico(int idAsignacion, int idStatusAprobacionTecnico)
        {
            // Verificar si la asignación existe
            var asignacion = await _repository.ObtenerAsignacionConDetalles(idAsignacion);
            if (asignacion == null)
            {
                throw new Exception($"No se encontró la asignación con ID: {idAsignacion}");
            }

            // Validar si el nuevo estado es válido
            var statusAprobacionTecnico = asignacion.StatusAprobacionTecnico;
            if (statusAprobacionTecnico == null || statusAprobacionTecnico.idStatusAprobacionTecnico != idStatusAprobacionTecnico)
            {
                throw new Exception($"El estado de aprobación técnico con ID: {idStatusAprobacionTecnico} no es válido.");
            }

            // Actualizar el estado de aprobación
            asignacion.idStatusAprobacionTecnico = idStatusAprobacionTecnico;
            await _repository.ModificarEstatusAprobacionTecnico(idAsignacion, idStatusAprobacionTecnico);

            // Mapear el resultado a DTO para la respuesta
            return new AsignacionesDetalleDTO
            {
                idAsignacion = asignacion.idAsignacion,
                idSolicitud = asignacion.idSolicitud,
                nombreCompletoTecnico = $"{asignacion.Empleado.nombre} {asignacion.Empleado.apellidoPaterno} {asignacion.Empleado.apellidoMaterno}",
                qrScaneado = asignacion.qrScaneado,
                horaInicio = asignacion.horaInicio,
                horaTermino = asignacion.horaTermino,
                solucion = asignacion.solucion,
                idRefaccion = asignacion.idRefaccion,
                cantidad = asignacion.cantidad,
                maquinaDetenida = asignacion.maquinaDetenida,
                idCategoriaAsignacion = asignacion.idCategoriaAsignacion,
                idStatusAprobacionTecnico = asignacion.idStatusAprobacionTecnico,
                nombreRefaccion = asignacion.Inventario?.nombreProducto ?? "N/A",
                nombreCategoriaAsignacion = asignacion.CategoriaAsignacion?.descripcion ?? "N/A",
                nombreStatusAprobacionTecnico = statusAprobacionTecnico.descripcionStatusAprobacionTecnico
            };
        }


    }
}
