using Piolax_WebApp.DTOs;

namespace Piolax_WebApp.Services
{
    public interface IKPIDashboardService
    {
        Task<KPIResponseDTO> ObtenerMTTA(int? idArea = null, int? idMaquina = null, int? anio = null, int? mes = null);
        Task<KPIResponseDTO> ObtenerMTTR(int? idArea = null, int? idMaquina = null, int? idEmpleado = null, int? anio = null, int? mes = null);
        Task<KPIResponseDTO> ObtenerMTBF(int? idArea = null);
        Task<KPIResponseDTO> ObtenerTotalDowntime(int? idArea = null, int? idMaquina = null, int? anio = null, int? mes = null, int? semana = null, int? diaSemana = null);
        Task<IEnumerable<KPIResponseDTO>> ObtenerResumenKPIs(
            int? idAreaMTTA = null, int? idMaquinaMTTA = null,
            int? idAreaMTTR = null, int? idMaquinaMTTR = null, int? idEmpleadoMTTR = null,
            int? idAreaMTBF = null,
            int? idAreaDowntime = null, int? idMaquinaDowntime = null,
            int? añoDowntime = null, int? mesDowntime = null, int? semanaDowntime = null, int? diaSemanaDowntime = null);
    }
}
