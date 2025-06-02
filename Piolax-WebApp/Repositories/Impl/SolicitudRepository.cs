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
                // Incluir las asignaciones y sus técnicos para poder evaluar el estado de aprobación del técnico.
                .Include(s => s.Asignaciones)
                    .ThenInclude(a => a.Asignacion_Tecnico)
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

        /*public async Task<IEnumerable<Solicitudes>> ObtenerSolicitudesEmpleado(string numNomina)
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
        }*/

        public async Task<IEnumerable<Solicitudes>> ObtenerSolicitudesEmpleado(string numNomina)
        {
            return await _context.Solicitudes
                .Where(s => s.Empleado.numNomina == numNomina &&
                            (s.idStatusOrden == 3 || s.idStatusOrden == 6 || s.idStatusOrden == 5 || s.idStatusOrden == 4 || s.idStatusOrden == 2))
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
                .OrderByDescending(s => s.idStatusOrden == 3 ? s.fechaSolicitud : DateTime.MinValue) // Prioridad a "No tomada"
                .ThenByDescending(s => s.idStatusOrden == 6 ? s.fechaSolicitud : DateTime.MinValue) // Luego "Rechazada"
                .ThenByDescending(s => s.idStatusOrden == 5 ? s.fechaSolicitud : DateTime.MinValue) // Luego "Pausada"
                .ThenByDescending(s => s.idStatusOrden == 4 ? s.fechaSolicitud : DateTime.MinValue) // Luego "Esperando validación"
                .ThenByDescending(s => s.idStatusOrden == 2 ? s.fechaSolicitud : DateTime.MinValue) // Luego "En proceso"
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

        public async Task<IEnumerable<Solicitudes>> ConsultarSolicitudesPorMaquinaYArea(int idMaquina, int idArea)
        {
            return await _context.Solicitudes
                .Include(s => s.Asignaciones) // Suponiendo que cada solicitud tiene una asignación asociada
                .Where(s => s.idMaquina == idMaquina && s.idAreaSeleccionada == idArea)
                .ToListAsync();
        }

        public async Task<IEnumerable<Solicitudes>> ConsultarSolicitudesNoTomadas()
        {
            return await _context.Solicitudes
                .Where(s => (s.idStatusOrden == 6 || s.idStatusOrden == 3)) // Filtra solicitudes "No tomadas"
                .OrderBy(s => s.idStatusOrden == 6 ? 0 : s.idStatusOrden == 3 ? 1 : 2)
                .ThenBy(s => s.fechaSolicitud)
                .ToListAsync();
        }

        public async Task<IEnumerable<Solicitudes>> ConsultarSolicitudesTerminadas()
        {
            return await _context.Solicitudes
                .Where(s => s.idStatusOrden == 1) // Filtra solicitudes "Terminadas"
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
                .Include(s => s.Asignaciones)
                    .ThenInclude(a => a.Asignacion_Tecnico)
                        .ThenInclude(at => at.Empleado) // Incluir detalles del técnico
                .Include(s => s.Asignaciones)
                    .ThenInclude(a => a.Asignacion_Tecnico)
                        .ThenInclude(at => at.Asignacion_Refacciones) // Incluir refacciones utilizadas
                            .ThenInclude(ar => ar.Inventario) // Incluir detalles del inventario
                .OrderBy(s => s.fechaSolicitud)
                .ToListAsync();
        }


        public async Task ActualizarStatusOrden(int idSolicitud, int idStatusOrden)
        {
            var solicitud = await _context.Solicitudes.FindAsync(idSolicitud);
            if (solicitud != null)
            {
                solicitud.idStatusOrden = idStatusOrden;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Solicitudes>> ObtenerSolicitudesConPrioridadAsync()
        {
            return await _context.Solicitudes
                .Where(s => s.idStatusOrden == 3 || s.idStatusOrden == 6 || s.idStatusOrden == 5 || s.idStatusOrden == 4 || s.idStatusOrden == 2)
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
                // Incluir las asignaciones y los técnicos
                .Include(s => s.Asignaciones)
                    .ThenInclude(a => a.Asignacion_Tecnico)
                        .ThenInclude(at => at.Empleado)
                .OrderByDescending(s => s.idStatusOrden == 3 ? s.fechaSolicitud : DateTime.MinValue) // Prioridad a "No tomada"
                .ThenByDescending(s => s.idStatusOrden == 6 ? s.fechaSolicitud : DateTime.MinValue) // Luego "Rechazada"
                .ThenByDescending(s => s.idStatusOrden == 5 ? s.fechaSolicitud : DateTime.MinValue) // Luego "Pausada"
                .ThenByDescending(s => s.idStatusOrden == 4 ? s.fechaSolicitud : DateTime.MinValue) // Luego "Esperando validación"
                .ThenByDescending(s => s.idStatusOrden == 2 ? s.fechaSolicitud : DateTime.MinValue) // Luego "En proceso"
                .ToListAsync();
        }




        public async Task<bool> EliminarSolicitud(int idSolicitud)
        {
            var solicitud = await _context.Solicitudes
                .Include(s => s.Asignaciones) // 🔹 Incluir asignaciones si existen
                .FirstOrDefaultAsync(s => s.idSolicitud == idSolicitud);

            if (solicitud == null)
            {
                return false; // No existe la solicitud
            }

            // 🔹 Eliminar asignaciones asociadas antes de eliminar la solicitud (si aplica)
            if (solicitud.Asignaciones != null && solicitud.Asignaciones.Any())
            {
                _context.Asignaciones.RemoveRange(solicitud.Asignaciones);
            }

            _context.Solicitudes.Remove(solicitud);
            await _context.SaveChangesAsync();
            return true; // Eliminación exitosa
        }

        public async Task<IEnumerable<Solicitudes>> ObtenerSolicitudesPorAreaYRoles(int idArea, List<int> idRoles)
        {
            return await _context.Solicitudes
                .Where(s => s.idAreaSeleccionada == idArea &&
                            (s.idStatusOrden == 3 || s.idStatusOrden == 6 || s.idStatusOrden == 5 || s.idStatusOrden == 4 || s.idStatusOrden == 2))
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
                .OrderByDescending(s => s.idStatusOrden == 3 ? s.fechaSolicitud : DateTime.MinValue) // Prioridad a "No tomada"
                .ThenByDescending(s => s.idStatusOrden == 6 ? s.fechaSolicitud : DateTime.MinValue) // Luego "Rechazada"
                .ThenByDescending(s => s.idStatusOrden == 5 ? s.fechaSolicitud : DateTime.MinValue) // Luego "Pausada"
                .ThenByDescending(s => s.idStatusOrden == 4 ? s.fechaSolicitud : DateTime.MinValue) // Luego "Esperando validación"
                .ThenByDescending(s => s.idStatusOrden == 2 ? s.fechaSolicitud : DateTime.MinValue) // Luego "En proceso"
                .ToListAsync();
        }

        public async Task<List<Solicitudes>> ObtenerSolicitudesEnStatus(int idStatusOrden)
        {
            // Traemos solo lo que necesitamos: idSolicitud, fechaSolicitud y status
            return await _context.Solicitudes
                .Where(s => s.idStatusOrden == idStatusOrden)
                .Select(s => new Solicitudes
                {
                    idSolicitud = s.idSolicitud,
                    fechaSolicitud = s.fechaSolicitud,
                    idStatusOrden = s.idStatusOrden
                })
                .ToListAsync();
        }

    }
}
