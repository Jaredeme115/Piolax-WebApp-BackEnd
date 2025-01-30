using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Repositories.Impl;

namespace Piolax_WebApp.Services.Impl
{
    public class AsignacionService: IAsignacionService
    {
        private readonly AsignacionRepository _repository;
        private readonly AreasRepository _areasRepository;
        private readonly RolesRepository _rolesRepository;
        private readonly AsignacionTecnicosRepository _asignacionTecnicosRepository;
        private readonly SolicitudRepository _solicitudRepository;

        public AsignacionService(
            AsignacionRepository repository, 
            AreasRepository areasRepository, 
            RolesRepository rolesRepository, 
            AsignacionTecnicosRepository asignacionTecnicosRepository,
            SolicitudRepository solicitudRepository)
        {
            _repository = repository;
            _areasRepository = areasRepository;
            _rolesRepository = rolesRepository;
            _asignacionTecnicosRepository = asignacionTecnicosRepository;
            _solicitudRepository = solicitudRepository;
        }

        public async Task<Asignaciones> AgregarAsignacion(AsignacionesDTO asignacionesDTO)
        {
            var solicitiud = await _solicitudRepository.ExisteSolicitud(asignacionesDTO.idSolicitud);
            if(solicitiud == false)
            {
                throw new ArgumentException("La solicitud no existe.");
            }
            var asignacion = new Asignaciones
            {
                idSolicitud = asignacionesDTO.idSolicitud,
                idStatusAsignacion = asignacionesDTO.idStatusAsignacion
            };

            return await _repository.AgregarAsignacion(asignacion);
        }

        public async Task<IEnumerable<Asignaciones>> ConsultarTodasLasAsignaciones()
        {
            return await _repository.ConsultarTodasLasAsignaciones();
        }

        public async Task<Asignaciones> ConsultarAsignacionPorId(int idAsignacion)
        {
            return await _repository.ConsultarAsignacionPorId(idAsignacion);
        }

        public async Task<Asignaciones> ActualizarAsignacion(int idAsignacion, AsignacionesDTO asignacionesDTO)
        {
            var asignacionExiste = await _repository.AsignacionExiste(idAsignacion);
            if (!asignacionExiste)
            {
                throw new ArgumentException("La asignación no existe.");
            }

            var asignacion = new Asignaciones
            {
                idAsignacion = idAsignacion, 
                idSolicitud = asignacionesDTO.idSolicitud,
                idStatusAsignacion = asignacionesDTO.idStatusAsignacion
            };

            return await _repository.ActualizarAsignacion(idAsignacion, asignacion);
        }


        public async Task<bool> EliminarAsignacion(int idAsignacion)
        {
            return await _repository.EliminarAsignacion(idAsignacion);
        }

        public async Task<bool> AsignacionExiste(int idAsignacion)
        {
            return await _repository.AsignacionExiste(idAsignacion);
        }

        public async Task<AsignacionDetallesDTO?> ConsultarAsignacionConDetallesPorId(int idAsignacion)
        {
            // Consultar la asignación principal
            var asignacion = await _repository.ConsultarAsignacionPorId(idAsignacion);
            if (asignacion == null)
            {
                return null; // Devuelve null si no existe la asignación
            }

            // Consultar técnicos relacionados
            var tecnicos = await _asignacionTecnicosRepository.ConsultarTecnicosPorAsignacion(idAsignacion);

            // Consultar nombre del area seleccionada
            var areas = await _areasRepository.Consultar(asignacion.Solicitud.idAreaSeleccionada);

            // Consultar nombre del rol seleccionado
            var roles = await _rolesRepository.Consultar(asignacion.Solicitud.idRolSeleccionado);

            // Mapear a DTOs
            var tecnicoDetalles = tecnicos.Select(tecnico => new Asignacion_TecnicoDetallesDTO
            {
                idAsignacionTecnico = tecnico.idAsignacionTecnico,
                idAsignacion = tecnico.idAsignacion,
                idEmpleado = tecnico.idEmpleado,
                nombreCompletoTecnico = $"{tecnico.Empleado.nombre} {tecnico.Empleado.apellidoPaterno} {tecnico.Empleado.apellidoMaterno}",
                horaInicio = tecnico.horaInicio,
                horaTermino = tecnico.horaTermino,
                solucion = tecnico.solucion,
                idStatusAprobacionTecnico = tecnico.idStatusAprobacionTecnico,
                nombreStatusAprobacionTecnico = tecnico.StatusAprobacionTecnico.descripcionStatusAprobacionTecnico,
                comentarioPausa = tecnico.comentarioPausa,
                esTecnicoActivo = tecnico.esTecnicoActivo,
                Refacciones = tecnico.Asignacion_Refacciones.Select(refaccion => new Asignacion_RefaccionesDetallesDTO
                {
                    idAsignacionRefaccion = refaccion.idAsignacionRefaccion,
                    idAsignacion = refaccion.idAsignacion,
                    idRefaccion = refaccion.idRefaccion,
                    nombreRefaccion = refaccion.Inventario.nombreProducto,
                    idAsignacionTecnico = refaccion.idAsignacionTecnico,
                    cantidad = refaccion.cantidad
                }).ToList()
            }).ToList();

            // Mapear la asignación principal a AsignacionDetallesDTO
            var asignacionDetallesDTO = new AsignacionDetallesDTO
            {
                idAsignacion = asignacion.idAsignacion,
                idSolicitud = asignacion.idSolicitud,
                idStatusAsignacion = asignacion.idStatusAsignacion,
                nombreStatusAsignacion = asignacion.StatusAsignacion.descripcionStatusAsignacion,
                Solicitud = new SolicitudesDetalleDTO
                {
                    idSolicitud = asignacion.Solicitud.idSolicitud,
                    descripcion = asignacion.Solicitud.descripcion,
                    fechaSolicitud = asignacion.Solicitud.fechaSolicitud,
                    nombreCompletoEmpleado = $"{asignacion.Solicitud.Empleado.nombre} {asignacion.Solicitud.Empleado.apellidoPaterno} {asignacion.Solicitud.Empleado.apellidoMaterno}",
                    idMaquina = asignacion.Solicitud.idMaquina,
                    idTurno = asignacion.Solicitud.idTurno,
                    idStatusOrden = asignacion.Solicitud.idStatusOrden,
                    idStatusAprobacionSolicitante = asignacion.Solicitud.idStatusAprobacionSolicitante,
                    area = areas.nombreArea,
                    rol = roles.nombreRol,
                    idCategoriaTicket = asignacion.Solicitud.idCategoriaTicket,
                    nombreMaquina = asignacion.Solicitud.Maquina.nombreMaquina,
                    nombreTurno = asignacion.Solicitud.Turno.descripcion,
                    nombreStatusOrden = asignacion.Solicitud.StatusOrden.descripcionStatusOrden,
                    nombreStatusAprobacionSolicitante = asignacion.Solicitud.StatusAprobacionSolicitante.descripcionStatusAprobacionSolicitante,
                    nombreCategoriaTicket = asignacion.Solicitud.categoriaTicket.descripcionCategoriaTicket
                },
                Tecnicos = tecnicoDetalles
            };

            return asignacionDetallesDTO;

        }

    }
}
