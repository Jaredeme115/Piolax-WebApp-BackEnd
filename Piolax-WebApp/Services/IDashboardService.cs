namespace Piolax_WebApp.Services;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.DTOs.Piolax_WebApp.DTOs;

public interface IDashboardService
    {
    Task<FiltrosDashboardDTO> ObtenerFiltrosDashboardAsync();
    Task<List<KpiDashboardDTO>> ObtenerKpisDashboardAsync(int? anio);

    Task<List<KpiDashboardDTO>> ObtenerIndicadoresDashboardAsync(FiltroDashboardDTO filtro);


}

