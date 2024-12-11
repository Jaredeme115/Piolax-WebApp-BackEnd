using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Repositories.Impl;

namespace Piolax_WebApp.Services.Impl
{
    public class SolicitudService(
        ISolicitudesRepository repository,
        IEmpleadoService empleadoService,
        IEmpleadoAreaRolService empleadoAreaRolService,
        IMaquinasService maquinasService,
        ITurnosService turnosService,
        IStatusOrdenService statusOrdenService,
        IStatusAprobacionSolicitanteService statusAprobacionSolicitanteService
        ) : ISolicitudService
    {
        private readonly ISolicitudesRepository _repository = repository;
        private readonly IEmpleadoService _empleadoService = empleadoService;
        private readonly IEmpleadoAreaRolService _empleadoAreaRolService = empleadoAreaRolService;
        private readonly IMaquinasService _maquinasService = maquinasService;
        private readonly ITurnosService _turnoService = turnosService;
        private readonly IStatusOrdenService _statusOrdenService = statusOrdenService;
        private readonly IStatusAprobacionSolicitanteService _statusAprobacionSolicitanteService = statusAprobacionSolicitanteService;

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
                idRolSeleccionado = solicitudesDTO.idRolSeleccionado,
                paroMaquina = solicitudesDTO.paroMaquina
            };

            solicitud = await _repository.RegistrarSolicitud(solicitud);

            //Se recurre a llamar a Maquina Service para obtener el nombre de la maquina en base al id de la maquina

            var maquina = await _maquinasService.Consultar(solicitud.idMaquina); // Obtener la máquina por ID

            //Se recurre a llamar a Turno Service para obtener el nombre del turno en base al id del turno

            var turno = await _turnoService.Consultar(solicitud.idTurno); // Obtener el turno por ID

            //Se recurre a llamar a StatusOrden Service para obtener el nombre del status de la orden en base al id del status de la orden
            var statusOrden = await _statusOrdenService.Consultar(solicitud.idStatusOrden); // Obtener el status de la orden por ID

            //Se recurre a llamar a StatusAprobacionSolicitante Service para obtener el nombre del status de aprobación del solicitante en base al id del status de aprobación del solicitante
            var statusAprobacionSolicitante = await _statusAprobacionSolicitanteService.Consultar(solicitud.idStatusAprobacionSolicitante); // Obtener el status de aprobación del solicitante por ID

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
                area = areaSeleccionada.Area.nombreArea,
                rol = rolSeleccionado.Rol.nombreRol,
                paroMaquina = solicitud.paroMaquina,
                nombreMaquina = maquina.nombreMaquina,
                nombreTurno = turno.descripcion,
                nombreStatusOrden = statusOrden.descripcionStatusOrden,
                nombreStatusAprobacionSolicitante = statusAprobacionSolicitante.descripcionStatusAprobacionSolicitante
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

            //Se recurre a llamar a Maquina Service para obtener el nombre de la maquina en base al id de la maquina

            var maquina = await _maquinasService.Consultar(solicitud.idMaquina); // Obtener la máquina por ID

            //Se recurre a llamar a Turno Service para obtener el nombre del turno en base al id del turno

            var turno = await _turnoService.Consultar(solicitud.idTurno); // Obtener el turno por ID

            //Se recurre a llamar a StatusOrden Service para obtener el nombre del status de la orden en base al id del status de la orden
            var statusOrden = await _statusOrdenService.Consultar(solicitud.idStatusOrden); // Obtener el status de la orden por ID

            //Se recurre a llamar a StatusAprobacionSolicitante Service para obtener el nombre del status de aprobación del solicitante en base al id del status de aprobación del solicitante
            var statusAprobacionSolicitante = await _statusAprobacionSolicitanteService.Consultar(solicitud.idStatusAprobacionSolicitante); // Obtener el status de aprobación del solicitante por ID

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
                area = areaSeleccionada.Area.nombreArea,
                rol = rolSeleccionado.Rol.nombreRol,
                paroMaquina = solicitud.paroMaquina,
                nombreMaquina = maquina.nombreMaquina,
                nombreTurno = turno.descripcion,
                nombreStatusOrden = statusOrden.descripcionStatusOrden,
                nombreStatusAprobacionSolicitante = statusAprobacionSolicitante.descripcionStatusAprobacionSolicitante
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

                //Se recurre a llamar a Maquina Service para obtener el nombre de la maquina en base al id de la maquina

                var maquina = await _maquinasService.Consultar(solicitud.idMaquina); // Obtener la máquina por ID

                //Se recurre a llamar a Turno Service para obtener el nombre del turno en base al id del turno

                var turno = await _turnoService.Consultar(solicitud.idTurno); // Obtener el turno por ID

                //Se recurre a llamar a StatusOrden Service para obtener el nombre del status de la orden en base al id del status de la orden
                var statusOrden = await _statusOrdenService.Consultar(solicitud.idStatusOrden); // Obtener el status de la orden por ID

                //Se recurre a llamar a StatusAprobacionSolicitante Service para obtener el nombre del status de aprobación del solicitante en base al id del status de aprobación del solicitante
                var statusAprobacionSolicitante = await _statusAprobacionSolicitanteService.Consultar(solicitud.idStatusAprobacionSolicitante); // Obtener el status de aprobación del solicitante por ID

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
                    area = areaSeleccionada?.Area?.nombreArea ?? "N/A",
                    rol = rolSeleccionado?.Rol?.nombreRol ?? "N/A",
                    paroMaquina = solicitud.paroMaquina,
                    nombreMaquina = maquina.nombreMaquina,
                    nombreTurno = turno.descripcion,
                    nombreStatusOrden = statusOrden.descripcionStatusOrden,
                    nombreStatusAprobacionSolicitante = statusAprobacionSolicitante.descripcionStatusAprobacionSolicitante
                };

                solicitudesDetalleDTO.Add(solicitudDetalleDTO);
            }

            return solicitudesDetalleDTO;
        }

        public async Task<IEnumerable<SolicitudesDetalleDTO>> ObtenerSolicitudesEmpleado(string numNomina)
        {
            var solicitudes = await _repository.ObtenerSolicitudesEmpleado(numNomina);
            var solicitudesDetalleDTO = new List<SolicitudesDetalleDTO>();

            foreach (var solicitud in solicitudes)
            {
                var empleado = solicitud.Empleado;
                var areasRoles = empleado.EmpleadoAreaRol;

                // Filtrar el área y el rol específicos de la solicitud
                var areaSeleccionada = areasRoles.FirstOrDefault(ar => ar.idArea == solicitud.idAreaSeleccionada);
                var rolSeleccionado = areasRoles.FirstOrDefault(ar => ar.idRol == solicitud.idRolSeleccionado && ar.idArea == solicitud.idAreaSeleccionada);

                //Se recurre a llamar a Maquina Service para obtener el nombre de la maquina en base al id de la maquina

                var maquina = await _maquinasService.Consultar(solicitud.idMaquina); // Obtener la máquina por ID

                //Se recurre a llamar a Turno Service para obtener el nombre del turno en base al id del turno

                var turno = await _turnoService.Consultar(solicitud.idTurno); // Obtener el turno por ID

                //Se recurre a llamar a StatusOrden Service para obtener el nombre del status de la orden en base al id del status de la orden
                var statusOrden = await _statusOrdenService.Consultar(solicitud.idStatusOrden); // Obtener el status de la orden por ID

                //Se recurre a llamar a StatusAprobacionSolicitante Service para obtener el nombre del status de aprobación del solicitante en base al id del status de aprobación del solicitante
                var statusAprobacionSolicitante = await _statusAprobacionSolicitanteService.Consultar(solicitud.idStatusAprobacionSolicitante); // Obtener el status de aprobación del solicitante por ID

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
                    area = areaSeleccionada?.Area?.nombreArea ?? "N/A",
                    rol = rolSeleccionado?.Rol?.nombreRol ?? "N/A",
                    paroMaquina = solicitud.paroMaquina,
                    nombreMaquina = maquina.nombreMaquina,
                    nombreTurno = turno.descripcion,
                    nombreStatusOrden = statusOrden.descripcionStatusOrden,
                    nombreStatusAprobacionSolicitante = statusAprobacionSolicitante.descripcionStatusAprobacionSolicitante
                };

                solicitudesDetalleDTO.Add(solicitudDetalleDTO);
            }

            return solicitudesDetalleDTO;
        }

        public async Task<SolicitudesDetalleDTO> ModificarEstatusAprobacionSolicitante(int idSolicitud, int idStatusAprobacionSolicitante)
        {
            var solicitud = await _repository.ObtenerSolicitudConDetalles(idSolicitud);
            if (solicitud == null)
            {
                throw new Exception($"No se encontró la solicitud con ID: {idSolicitud}");
            }

            var statusAprobacionSolicitante = await _statusAprobacionSolicitanteService.Consultar(idStatusAprobacionSolicitante);
            if (statusAprobacionSolicitante == null)
            {
                throw new Exception($"No se encontró el status de aprobación del solicitante con ID: {idStatusAprobacionSolicitante}");
            }

            solicitud.idStatusAprobacionSolicitante = idStatusAprobacionSolicitante;
            await _repository.ModificarEstatusAprobacionSolicitante(idSolicitud, idStatusAprobacionSolicitante);

            var solicitudDetalleDTO = new SolicitudesDetalleDTO
            {
                idSolicitud = solicitud.idSolicitud,
                descripcion = solicitud.descripcion,
                fechaSolicitud = solicitud.fechaSolicitud,
                nombreCompletoEmpleado = $"{solicitud.Empleado.nombre} {solicitud.Empleado.apellidoPaterno} {solicitud.Empleado.apellidoMaterno}",
                idMaquina = solicitud.idMaquina,
                idTurno = solicitud.idTurno,
                idStatusOrden = solicitud.idStatusOrden,
                idStatusAprobacionSolicitante = solicitud.idStatusAprobacionSolicitante,
                area = solicitud.Empleado.EmpleadoAreaRol.FirstOrDefault(ar => ar.idArea == solicitud.idAreaSeleccionada)?.Area?.nombreArea ?? "N/A",
                rol = solicitud.Empleado.EmpleadoAreaRol.FirstOrDefault(ar => ar.idRol == solicitud.idRolSeleccionado && ar.idArea == solicitud.idAreaSeleccionada)?.Rol?.nombreRol ?? "N/A",
                paroMaquina = solicitud.paroMaquina,
                nombreMaquina = solicitud.Maquina.nombreMaquina,
                nombreTurno = solicitud.Turno.descripcion,
                nombreStatusOrden = solicitud.StatusOrden.descripcionStatusOrden,
                nombreStatusAprobacionSolicitante = statusAprobacionSolicitante.descripcionStatusAprobacionSolicitante
            };

            return solicitudDetalleDTO;
        }

    }
}
