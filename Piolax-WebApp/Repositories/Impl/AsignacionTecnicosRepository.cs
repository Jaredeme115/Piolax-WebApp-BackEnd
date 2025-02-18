using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class AsignacionTecnicosRepository(AppDbContext context) : IAsignacionTecnicosRepository
    {
        private readonly AppDbContext _context = context;

        /*public async Task<IEnumerable<Asignacion_Tecnico>> ConsultarTecnicosPorAsignacion(int idAsignacion)
        {
            return await _context.Asignacion_Tecnico
            .Where(x => x.idAsignacion == idAsignacion)
            .Include(x => x.Empleado) // Cargar datos del técnico
            .ToListAsync();
        }*/

        public async Task<IEnumerable<Asignacion_Tecnico>> ConsultarTecnicosPorAsignacion(int idAsignacion)
        {
            return await _context.Asignacion_Tecnico
                .Include(t => t.Empleado)
                .Include(t => t.StatusAprobacionTecnico) // Incluir la propiedad de navegación
                .Include(t => t.Asignacion_Refacciones)
                    .ThenInclude(r => r.Inventario)
                .Where(t => t.idAsignacion == idAsignacion)
                .ToListAsync();
        }

        public async Task<Asignacion_Tecnico?> ConsultarTecnicoPorID(int idAsignacion)
        {
            var tecnico = await _context.Asignacion_Tecnico.FindAsync(idAsignacion);
            if (tecnico != null)
            {
                await _context.Entry(tecnico).Reference(t => t.Empleado).LoadAsync();
            }
            return tecnico;
        }

        public async Task<Asignacion_Tecnico?> ConsultarTecnicoPorAsignacionYEmpleado(int idAsignacion, int idEmpleado)
        {
            return await _context.Asignacion_Tecnico
                .Include(t => t.Empleado) // Cargar la propiedad Empleado
                .Where(t => t.idAsignacion == idAsignacion && t.idEmpleado == idEmpleado)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ConsultarTecnicosActivosPorAsignacion(int idAsignacion)
        {
            // Consulta a la base de datos para verificar si hay técnicos activos en la asignación
            return await _context.Asignacion_Tecnico
                .AnyAsync(t => t.idAsignacion == idAsignacion && t.esTecnicoActivo == true);
        }

        public async Task<Asignacion_Tecnico> CrearAsignacionTecnico(Asignacion_Tecnico asignacionTecnico)
        {
            await _context.Asignacion_Tecnico.AddAsync(asignacionTecnico);
            await _context.SaveChangesAsync();
            return asignacionTecnico;
        }

        public async Task<IEnumerable<Asignacion_Tecnico>> ConsultarTodosLosTecnicos()
        {
            return await _context.Asignacion_Tecnico.ToListAsync();
        }

        public async Task<bool> EliminarTecnicoDeAsignacion(int idAsignacionTecnico)
        {
            // Buscar la asignación del técnico por su ID
            var asignacionTecnico = await _context.Asignacion_Tecnico.FindAsync(idAsignacionTecnico);

            // Si no se encuentra la asignación del técnico, devolver false
            if (asignacionTecnico == null)
            {
                return false;
            }

            // Eliminar la asignación del técnico del contexto
            _context.Asignacion_Tecnico.Remove(asignacionTecnico);

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            // Devolver true para indicar que la eliminación fue exitosa
            return true;
        }

        public async Task<bool> ActualizarTecnicoEnAsignacion(Asignacion_Tecnico asignacionTecnico)
        {
            var existente = await _context.Asignacion_Tecnico.FindAsync(asignacionTecnico.idAsignacionTecnico);
            if (existente == null) return false;

            _context.Entry(existente).CurrentValues.SetValues(asignacionTecnico);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Asignacion_Tecnico>> ConsultarOrdenesEnPausaDelTecnico(int idEmpleado)
        {
            // Ajusta los valores 3 y 5 según tu base de datos
            return await _context.Asignacion_Tecnico
                .Include(at => at.Asignacion)  // Para poder mapear después a un DTO que incluya datos de la asignación
                .Where(at =>
                    at.idEmpleado == idEmpleado
                    && !string.IsNullOrEmpty(at.comentarioPausa) // El técnico pausó
                    && at.idStatusAprobacionTecnico != 3         // No está “Completada”
                    && at.idStatusAprobacionTecnico != 5         // No está “Esperando validación”
                )
                .ToListAsync();
        }

    }
}
