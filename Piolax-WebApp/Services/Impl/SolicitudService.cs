using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Repositories.Impl;

namespace Piolax_WebApp.Services.Impl
{
    public class SolicitudService(ISolicitudesRepository repository, IEmpleadoService empleadoService, IEmpleadoAreaRolService empleadoAreaRolService) : ISolicitudService
    {
        private readonly ISolicitudesRepository _repository = repository;
        private readonly IEmpleadoService _empleadoService = empleadoService;
        private readonly IEmpleadoAreaRolService _empleadoAreaRolService = empleadoAreaRolService;

        public async Task<SolicitudesDetalleDTO> RegistrarSolicitud(SolicitudesDTO solicitudesDTO)
        {
            var empleado = await _empleadoService.Consultar(solicitudesDTO.numNomina);
            if (empleado == null)
            {
                throw new Exception($"No se encontró el empleado con número de nómina: {solicitudesDTO.numNomina}");
            }

            var areasRoles = await _empleadoAreaRolService.ObtenerAreasRolesPorEmpleado(empleado.numNomina);

            // Validar que el área y el rol seleccionados están entre los asignados al empleado
            var areaSeleccionada = areasRoles.FirstOrDefault(ar => ar.idArea == solicitudesDTO.idAreaSeleccionada);
            if (areaSeleccionada == null)
            {
                throw new Exception("El área seleccionada no está asignada al empleado.");
            }

            var rolSeleccionado = areasRoles.FirstOrDefault(ar => ar.idRol == solicitudesDTO.idRolSeleccionado && ar.idArea == solicitudesDTO.idAreaSeleccionada);
            if (rolSeleccionado == null)
            {
                throw new Exception("El rol seleccionado no está asignado al empleado en el área seleccionada.");
            }

            var solicitud = new Solicitudes
            {
                descripcion = solicitudesDTO.descripcion,
                fechaSolicitud = DateTime.Now,
                idEmpleado = empleado.idEmpleado,
                idMaquina = solicitudesDTO.idMaquina,
                idTurno = solicitudesDTO.idTurno,
                idStatusOrden = solicitudesDTO.idStatusOrden,
                idStatusAprobacionSolicitante = solicitudesDTO.idStatusAprobacionSolicitante,
                idAreaSeleccionada = solicitudesDTO.idAreaSeleccionada,
                idRolSeleccionado = solicitudesDTO.idRolSeleccionado
            };

            solicitud = await _repository.RegistrarSolicitud(solicitud);

            var solicitudDetalleDTO = new SolicitudesDetalleDTO
            {
                idSolicitud = solicitud.idSolicitud,
                descripcion = solicitud.descripcion,
                fechaSolicitud = solicitud.fechaSolicitud,
                nombreCompletoEmpleado = $"{empleado.nombre} {empleado.apellidoPaterno} {empleado.apellidoMaterno}",
                idMaquina = solicitud.idMaquina,
                idTurno = solicitud.idTurno,
                idStatusOrden = solicitud.idStatusOrden,
                idStatusAprobacionSolicitante = solicitud.idStatusAprobacionSolicitante,
                Areas = new List<string> { areaSeleccionada.Area.nombreArea },
                Roles = new List<string> { rolSeleccionado.Rol.nombreRol }
            };

            return solicitudDetalleDTO;
        }
        public async Task<SolicitudesDetalleDTO?> ObtenerSolicitudConDetalles(int idSolicitud)
        {
            var solicitud = await _repository.ObtenerSolicitudConDetalles(idSolicitud);
            if (solicitud == null)
            {
                return null;
            }

            var empleado = solicitud.Empleado;
            var areasRoles = empleado.EmpleadoAreaRol;

            // Filtrar el área y el rol específicos de la solicitud
            var areaSeleccionada = areasRoles.FirstOrDefault(ar => ar.idArea == solicitud.idAreaSeleccionada);
            var rolSeleccionado = areasRoles.FirstOrDefault(ar => ar.idRol == solicitud.idRolSeleccionado && ar.idArea == solicitud.idAreaSeleccionada);

            var solicitudesDetalleDTO = new SolicitudesDetalleDTO
            {
                idSolicitud = solicitud.idSolicitud,
                descripcion = solicitud.descripcion,
                fechaSolicitud = solicitud.fechaSolicitud,
                nombreCompletoEmpleado = $"{empleado.nombre} {empleado.apellidoPaterno} {empleado.apellidoMaterno}",
                idMaquina = solicitud.idMaquina,
                idTurno = solicitud.idTurno,
                idStatusOrden = solicitud.idStatusOrden,
                idStatusAprobacionSolicitante = solicitud.idStatusAprobacionSolicitante,
                Areas = new List<string> { areaSeleccionada.Area.nombreArea },
                Roles = new List<string> { rolSeleccionado.Rol.nombreRol }
            };

            return solicitudesDetalleDTO;
        }

        public async Task<IEnumerable<SolicitudesDetalleDTO>> ObtenerSolicitudes()
        {
            var solicitudes = await _repository.ObtenerSolicitudes();
            var solicitudesDetalleDTO = new List<SolicitudesDetalleDTO>();

            foreach (var solicitud in solicitudes)
            {
                var empleado = solicitud.Empleado;
                var areasRoles = empleado.EmpleadoAreaRol;

                // Filtrar el área y el rol específicos de la solicitud
                var areaSeleccionada = areasRoles.FirstOrDefault(ar => ar.idArea == solicitud.idAreaSeleccionada);
                var rolSeleccionado = areasRoles.FirstOrDefault(ar => ar.idRol == solicitud.idRolSeleccionado && ar.idArea == solicitud.idAreaSeleccionada);

                var solicitudDetalleDTO = new SolicitudesDetalleDTO
                {
                    idSolicitud = solicitud.idSolicitud,
                    descripcion = solicitud.descripcion,
                    fechaSolicitud = solicitud.fechaSolicitud,
                    nombreCompletoEmpleado = $"{empleado.nombre} {empleado.apellidoPaterno} {empleado.apellidoMaterno}",
                    idMaquina = solicitud.idMaquina,
                    idTurno = solicitud.idTurno,
                    idStatusOrden = solicitud.idStatusOrden,
                    idStatusAprobacionSolicitante = solicitud.idStatusAprobacionSolicitante,
                    Areas = new List<string> { areaSeleccionada?.Area?.nombreArea ?? "N/A" },
                    Roles = new List<string> { rolSeleccionado?.Rol?.nombreRol ?? "N/A" }
                };

                solicitudesDetalleDTO.Add(solicitudDetalleDTO);
            }

            return solicitudesDetalleDTO;
        }
    }
}
