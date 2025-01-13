using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class AsignacionesRepository(AppDbContext context) : IAsignacionesRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Asignaciones> RegistrarAsignacion(Asignaciones asignaciones)
        {
            _context.Asignaciones.Add(asignaciones);
            await _context.SaveChangesAsync();
            return asignaciones;
        }

        public async Task<Asignaciones> ObtenerAsignacionConDetalles(int idAsignacion)
        {
            return await _context.Asignaciones
                .Include(a => a.Solicitud) // Relación con la tabla de solicitudes
                .Include(a => a.Empleado) // Relación con el técnico asignado
                    .ThenInclude(e => e.EmpleadoAreaRol)
                    .ThenInclude(ar => ar.Area) // Relación con el área
                .Include(a => a.Empleado)
                    .ThenInclude(e => e.EmpleadoAreaRol)
                    .ThenInclude(ar => ar.Rol) // Relación con el rol
                .Include(a => a.Inventario) // Relación con la tabla de inventario (idRefaccion)
                .Include(a => a.CategoriaAsignacion) // Relación con la categoría de asignación
                .Include(a => a.StatusAprobacionTecnico) // Relación con el estado de aprobación técnico
                .FirstOrDefaultAsync(a => a.idAsignacion == idAsignacion);
        }



        public async Task<IEnumerable<Asignaciones>> ObtenerTodasLasAsignaciones()
        {
            return await _context.Asignaciones
                .Include(a => a.Solicitud) // Relación con la tabla de solicitudes
                .Include(a => a.Empleado) // Relación con el técnico asignado
                    .ThenInclude(e => e.EmpleadoAreaRol)
                    .ThenInclude(ar => ar.Area) // Relación con el área
                .Include(a => a.Empleado)
                    .ThenInclude(e => e.EmpleadoAreaRol)
                    .ThenInclude(ar => ar.Rol) // Relación con el rol
                .Include(a => a.Inventario) // Relación con la tabla de inventario (idRefaccion)
                .Include(a => a.CategoriaAsignacion) // Relación con la categoría de asignación
                .Include(a => a.StatusAprobacionTecnico) // Relación con el estado de aprobación técnico
                .ToListAsync();
        }


        public async Task<IEnumerable<Asignaciones>> ObtenerAsignacionPorTecnico(string numNomina)
        {
            return await _context.Asignaciones
                .Include(a => a.Solicitud) // Relación con la tabla de solicitudes
                .Include(a => a.Empleado) // Relación con el técnico asignado
                    .ThenInclude(e => e.EmpleadoAreaRol)
                    .ThenInclude(ar => ar.Area) // Relación con el área
                .Include(a => a.Empleado)
                    .ThenInclude(e => e.EmpleadoAreaRol)
                    .ThenInclude(ar => ar.Rol) // Relación con el rol
                .Include(a => a.Inventario) // Relación con la tabla de inventario (idRefaccion)
                .Include(a => a.CategoriaAsignacion) // Relación con la categoría de asignación
                .Include(a => a.StatusAprobacionTecnico) // Relación con el estado de aprobación técnico
                .Where(a => a.Empleado.numNomina == numNomina)
                .ToListAsync();
        }


        public async Task<AsignacionesDetalleDTO> ModificarEstatusAprobacionTecnico(int idAsignacion, int idStatusAprobacionTecnico)
        {
            var asignacion = await _context.Asignaciones
                .Include(s => s.StatusAprobacionTecnico)
                .FirstOrDefaultAsync(s => s.idAsignacion == idAsignacion);

            if (asignacion == null)
            {
                throw new Exception($"No se encontró la asignacion con ID: {idAsignacion}");
            }

            asignacion.idStatusAprobacionTecnico = idStatusAprobacionTecnico;
            await _context.SaveChangesAsync();

            var asignacionDetalleDTO = new AsignacionesDetalleDTO
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
            };

            return asignacionDetalleDTO;
        }

    }
}
