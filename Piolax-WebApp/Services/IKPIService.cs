using Piolax_WebApp.DTOs;

namespace Piolax_WebApp.Services
{
    public interface IKPIService
    {
        Task<List<KpiDashboardDTO>> ObtenerKPIs(KPIFiltroDTO filtros);

    }
}
