using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class SolicitudService(ISolicitudesRepository repository) : ISolicitudService
    {
        private readonly ISolicitudesRepository _repository = repository;

        public Task<IEnumerable<Solicitudes>> ConsultarTodos()
        {
            return _repository.ConsultarTodos();
        }

        public Task<Solicitudes> Consultar(int idSolicitud)
        {
            return _repository.Consultar(idSolicitud);
        }

        public async Task<Solicitudes> Registro(SolicitudesDTO solicitud)
        {

            var solicitudes = new Solicitudes
            {
                descripcion = solicitud.descripcion,
                fechaSolicitud = solicitud.fechaSolicitud,
                idEmpleado = solicitud.idEmpleado,
                idMaquina = solicitud.idMaquina,
                idTurno = solicitud.idTurno,
                idStatusOrden = solicitud.idStatusOrden,
                idStatusAprobacionSolicitante = solicitud.idStatusAprobacionSolicitante

            };

            return await _repository.Registro(solicitudes);
        }

        public async Task<Solicitudes> Modificar(int idSolicitud, SolicitudesDTO solicitud)
        {
            var solicitudes = new Solicitudes
            {
                descripcion = solicitud.descripcion,
                fechaSolicitud = solicitud.fechaSolicitud,
                idEmpleado = solicitud.idEmpleado,
                idMaquina = solicitud.idMaquina,
                idTurno = solicitud.idTurno,
                idStatusOrden = solicitud.idStatusOrden,
                idStatusAprobacionSolicitante = solicitud.idStatusAprobacionSolicitante
            };

            return await _repository.Modificar(idSolicitud, solicitudes);
        }

        public async Task<Solicitudes> Eliminar(int idSolicitud)
        {
            return await _repository.Eliminar(idSolicitud);
        }

        public Task<bool> SolicitudExiste(int idSolicitud)
        {
            return _repository.SolicitudExiste(idSolicitud);
        }

    }
}
