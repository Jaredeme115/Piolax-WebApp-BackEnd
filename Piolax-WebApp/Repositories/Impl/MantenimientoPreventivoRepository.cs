using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class MantenimientoPreventivoRepository (AppDbContext context) : IMantenimientoPreventivoRepository
    {
        private readonly AppDbContext _context = context;

        // Método para crear un nuevo mantenimiento preventivo
        public async Task<MantenimientoPreventivo> CrearMantenimientoPreventico(MantenimientoPreventivo mantenimientoPreventivo)
        {
            _context.MantenimientoPreventivo.Add(mantenimientoPreventivo);
            await _context.SaveChangesAsync();
            return mantenimientoPreventivo;
        }

        // Método para consultar un mantenimiento preventivo con detalles
        public async Task<MantenimientoPreventivo?> ConsultarMP(int idMP)
        {
            return await _context.MantenimientoPreventivo
                .Include(mp => mp.Area)
                .Include(mp => mp.Maquina)
                .Include(mp => mp.FrecuenciaMP)
                .Include(mp => mp.EstatusPreventivo)
                .FirstOrDefaultAsync(mp => mp.idMP == idMP);
        }

        // Método para modificar un mantenimiento preventivo
        // Método para modificar un mantenimiento preventivo
        public async Task<MantenimientoPreventivo> Modificar(int idMP, MantenimientoPreventivo mantenimientoPreventivo)
        {
            var mantenimientoExistente = await _context.MantenimientoPreventivo.FindAsync(idMP);

            if (mantenimientoExistente == null)
            {
                return null; // Si no existe el mantenimiento, devolvemos null
            }

            // Actualizamos las propiedades del mantenimiento preventivo existente
            mantenimientoExistente.semanaPreventivo = mantenimientoPreventivo.semanaPreventivo;
            mantenimientoExistente.idFrecuenciaPreventivo = mantenimientoPreventivo.idFrecuenciaPreventivo;
            mantenimientoExistente.idEstatusPreventivo = mantenimientoPreventivo.idEstatusPreventivo;
            mantenimientoExistente.Activo = mantenimientoPreventivo.Activo;
            mantenimientoExistente.ultimaEjecucion = mantenimientoPreventivo.ultimaEjecucion;
            mantenimientoExistente.proximaEjecucion = mantenimientoPreventivo.proximaEjecucion;

            // Guardamos los cambios en la base de datos
            await _context.SaveChangesAsync();

            return mantenimientoExistente; // Retornamos el mantenimiento modificado
        }



        // Método para eliminar un mantenimiento preventivo
        public async Task<MantenimientoPreventivo> Eliminar(int idMP)
        {
            var mantenimientoPreventivo = await _context.MantenimientoPreventivo.FindAsync(idMP);
            if (mantenimientoPreventivo != null)
            {
                _context.MantenimientoPreventivo.Remove(mantenimientoPreventivo); // Eliminar el mantenimiento
                await _context.SaveChangesAsync(); // Guardar cambios
            }

            return mantenimientoPreventivo; // Devolvemos el mantenimiento eliminado (o null si no existía)
        }

    }
}
