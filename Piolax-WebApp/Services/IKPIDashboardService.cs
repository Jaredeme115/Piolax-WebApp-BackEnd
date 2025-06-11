using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using System.Diagnostics.Metrics;

namespace Piolax_WebApp.Services
{
    public interface IKPIDashboardService
    {
        Task<KPIResponseDTO> ObtenerMTTA(int? idArea = null, int? idMaquina = null, int? anio = null, int? mes = null);
        Task<KPIResponseDTO> ObtenerMTTR(int? idArea = null, int? idMaquina = null, int? idEmpleado = null, int? anio = null, int? mes = null);
   
        Task<KPIResponseDTO> ObtenerMTBF(int? idArea = null);
        Task<KPIResponseDTO> ObtenerTotalDowntime(int? idArea = null, int? idMaquina = null, int? anio = null, int? mes = null, int? semana = null, int? diaSemana = null);
        Task<IEnumerable<KPIResponseDTO>> ObtenerResumenKPIs(
            int? idAreaMTTA = null, int? idMaquinaMTTA = null, int? anioMTTA = null, int? mesMTTA = null, int? semanaMTTA = null, int? diaSemanaMTTA = null,
            int? idAreaMTTR = null, int? idMaquinaMTTR = null, int? idEmpleadoMTTR = null, int? anioMTTR = null, int? mesMTTR = null, int? semanaMTTR = null, int? diaSemanaMTTR = null,

            int? idAreaMTBF = null,
            int? idAreaDowntime = null, int? idMaquinaDowntime = null,
            int? añoDowntime = null, int? mesDowntime = null, int? semanaDowntime = null, int? diaSemanaDowntime = null);

        // Método para obtener MTTR segmentado
        Task<List<KpiSegmentadoDTO>> ObtenerMTTRSegmentado(
        int? idArea = null,
        int? idMaquina = null,
        int? idEmpleado = null,
        int? anio = null,
        int? mes = null
  );


        // Método para obtener MTTA segmentado
        Task<List<KpiSegmentadoDTO>> ObtenerMTTASegmentado(
        int? idArea = null,
        int? idMaquina = null,
        int? anio = null,
        int? mes = null);

        //Retorna para cada área la serie de 12 meses con el MTBF en horas.
        Task<List<KpiAreaMesSeriesDTO>> ObtenerMTBFPorAreaMes(int? anio = null);

        // Añade estos métodos a la interfaz IKPIDashboardService
        Task<List<KpiSegmentadoDTO>> ObtenerMTBFSegmentado(
            int? idArea = null,
            int? anio = null,
            int? objetivo = null);

        Task<List<KpiSegmentadoDTO>> ObtenerTotalDowntimeSegmentado(
        int? idArea = null,
        int? idMaquina = null,
        int? anio = null,
        int? mes = null,
        int? semana = null,
        int? diaSemana = null);

        // Métodos para los KPI Objetivos

        Task GuardarObjetivo(int idArea, int anio, int mes, float valorHoras);
        Task<List<KpiObjetivosDTO>> ObtenerObjetivosPorAnio(int anio);
        Task<List<KpiObjetivos>> ObtenerObjetivosPorAnioAsync(int anio);
        Task<List<KpiObjetivosSeriesDTO>> ObtenerObjetivosSeriesAsync(int anio);

    }
}
