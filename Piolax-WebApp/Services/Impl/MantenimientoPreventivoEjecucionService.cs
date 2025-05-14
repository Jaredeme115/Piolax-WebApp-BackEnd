using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class MantenimientoPreventivoEjecucionService(IMantenimientoPreventivoEjecucionRepository repository): IMantenimientoPreventivoEjecucionService
    {
        private readonly IMantenimientoPreventivoEjecucionRepository _repository = repository;

        public async Task<MantenimientoPreventivoEjecucionDTO> CrearEjecucionAsync(MantenimientoPreventivoEjecucionCrearDTO mantenimientoPreventivoEjecucionCrearDTO)
        {
            var entity = new MantenimientoPreventivoEjecuciones
            {
                idMP = mantenimientoPreventivoEjecucionCrearDTO.idMP,
                nombrePDF = mantenimientoPreventivoEjecucionCrearDTO.nombrePDF,
                rutaPDF = mantenimientoPreventivoEjecucionCrearDTO.rutaPDF,
                semanaEjecucion = mantenimientoPreventivoEjecucionCrearDTO.semanaEjecucion,
                anioEjecucion = mantenimientoPreventivoEjecucionCrearDTO.anioEjecucion,
                fechaEjecucion = DateTime.UtcNow
            };

            var saved = await _repository.CrearAsync(entity);

            return new MantenimientoPreventivoEjecucionDTO
            {
                idMPEjecucion = saved.idMPEjecucion,
                idMP = saved.idMP,
                nombrePDF = saved.nombrePDF,
                rutaPDF = saved.rutaPDF,
                semanaEjecucion = saved.semanaEjecucion,
                anioEjecucion = saved.anioEjecucion,
                fechaEjecucion = saved.fechaEjecucion
            };
        }

        public async Task<IEnumerable<MantenimientoPreventivoEjecucionDTO>> ObtenerEjecucionesPorMPAsync(int idMP)
        {
            var list = await _repository.ObtenerPorMPAsync(idMP);
            return list.Select(e => new MantenimientoPreventivoEjecucionDTO
            {
                idMPEjecucion = e.idMPEjecucion,
                idMP = e.idMP,
                nombrePDF = e.nombrePDF,
                rutaPDF = e.rutaPDF,
                semanaEjecucion = e.semanaEjecucion,
                anioEjecucion = e.anioEjecucion,
                fechaEjecucion = e.fechaEjecucion
            });
        }
    }
}
