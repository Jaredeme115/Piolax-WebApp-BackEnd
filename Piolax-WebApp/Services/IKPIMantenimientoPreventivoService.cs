using Piolax_WebApp.DTOs;

namespace Piolax_WebApp.Services
{
    public interface IKPIMantenimientoPreventivoService 
    {
        Task CalcularYGuardarKPIs(DateTime inicio, DateTime fin);

        // Métodos nuevos para consulta
        Task<KPIResponseDTO> ObtenerCumplimiento(int? año = null, int? mes = null);
        Task<KPIResponseDTO> ObtenerEfectividad(int? año = null, int? mes = null);
        Task<IEnumerable<KPIResponseDTO>> ObtenerResumenKPIsPreventivo(int? año = null, int? mes = null);
        Task<ContadoresMPDTO> ObtenerContadoresMP(int? anio = null, int? mes = null);
    }
}
