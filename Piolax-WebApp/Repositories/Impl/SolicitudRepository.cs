using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class SolicitudRepository(AppDbContext context) : ISolicitudesRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Solicitudes> RegistrarSolicitud(Solicitudes solicitudes)
        {
            _context.Solicitudes.Add(solicitudes);
            await _context.SaveChangesAsync();
            return solicitudes;
        }

        public async Task<Solicitudes?> ObtenerSolicitudConDetalles(int idSolicitud)
        {
            return await _context.Solicitudes
                .Include(s => s.Empleado)
                .ThenInclude(e => e.EmpleadoAreaRol)
                .ThenInclude(ar => ar.Area)
                .Include(s => s.Empleado)
                .ThenInclude(e => e.EmpleadoAreaRol)
                .ThenInclude(ar => ar.Rol)
                .Include(s => s.Maquina)
                .Include(s => s.Turno)
                .Include(s => s.StatusOrden)
                .Include(s => s.StatusAprobacionSolicitante)
                .Include(s => s.categoriaTicket)
                .FirstOrDefaultAsync(s => s.idSolicitud == idSolicitud);

        }

        public async Task<IEnumerable<Solicitudes>> ObtenerSolicitudes()
        {
            return await _context.Solicitudes
                .Include(s => s.Empleado)
                .ThenInclude(e => e.EmpleadoAreaRol)
                .ThenInclude(ar => ar.Area)
                .Include(s => s.Empleado)
                .ThenInclude(e => e.EmpleadoAreaRol)
                .ThenInclude(ar => ar.Rol)
                .Include(s => s.Maquina)
                .Include(s => s.Turno)
                .Include(s => s.StatusOrden)
                .Include(s => s.StatusAprobacionSolicitante)
                .Include(s => s.categoriaTicket)
                .ToListAsync();
        }

        public async Task<IEnumerable<Solicitudes>> ObtenerSolicitudesEmpleado(string numNomina)
        {
            return await _context.Solicitudes
           .Include(s => s.Empleado)
           .ThenInclude(e => e.EmpleadoAreaRol)
           .ThenInclude(ar => ar.Area)
           .Include(s => s.Empleado)
           .ThenInclude(e => e.EmpleadoAreaRol)
           .ThenInclude(ar => ar.Rol)
           .Include(s => s.Maquina)
           .Include(s => s.Turno)
           .Include(s => s.StatusOrden)
           .Include(s => s.StatusAprobacionSolicitante)
           .Include(s => s.categoriaTicket)
           .Where(s => s.Empleado.numNomina == numNomina)
           .ToListAsync();
        }

        public async Task<SolicitudesDetalleDTO> ModificarEstatusAprobacionSolicitante(int idSolicitud, int idStatusAprobacionSolicitante)
        {
            var solicitud = await _context.Solicitudes
                .Include(s => s.StatusAprobacionSolicitante)
                .FirstOrDefaultAsync(s => s.idSolicitud == idSolicitud);

            if (solicitud == null)
            {
                throw new Exception($"No se encontró la solicitud con ID: {idSolicitud}");
            }

            solicitud.idStatusAprobacionSolicitante = idStatusAprobacionSolicitante;
            await _context.SaveChangesAsync();

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
                idCategoriaTicket = solicitud.idCategoriaTicket,
                nombreMaquina = solicitud.Maquina.nombreMaquina,
                nombreTurno = solicitud.Turno.descripcion,
                nombreStatusOrden = solicitud.StatusOrden.descripcionStatusOrden,
                nombreStatusAprobacionSolicitante = solicitud.StatusAprobacionSolicitante.descripcionStatusAprobacionSolicitante,
                nombreCategoriaTicket = solicitud.categoriaTicket.descripcionCategoriaTicket
            };

            return solicitudDetalleDTO;
        }

        public async Task<bool> ExisteSolicitud(int idSolicitud)
        {
            return await _context.Solicitudes.AnyAsync(s => s.idSolicitud == idSolicitud);
        }


    }
}
