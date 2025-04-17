using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IKPIRepository
    {
        // Métodos existentes
        Task GuardarKPIMantenimiento(KpisMantenimiento kpiMantenimiento);
        Task GuardarKPIDetalles(int idKPIMantenimiento, List<KpisDetalle> kpiDetalles);
        Task GuardarKPIPreventivo(KpisMP kpisMP);
        Task GuardarKPIDetallesMP(int idKPIMP, List<KpisMPDetalle> kpiDetalles);

        // Nuevos métodos para consulta de KPIs filtrados
        Task<IEnumerable<KpisDetalle>> ConsultarMTTA(int? idArea = null, int? idMaquina = null, int? anio = null, int? mes = null);
        Task<IEnumerable<KpisDetalle>> ConsultarMTTR(int? idArea = null, int? idMaquina = null, int? idEmpleado = null, int? anio = null, int? mes = null);
        Task<IEnumerable<KpisDetalle>> ConsultarMTBF(int? idArea = null);
        Task<IEnumerable<KpisMantenimiento>> ConsultarTotalDowntime(int? idArea = null, int? idMaquina = null, int? año = null, int? mes = null, int? semana = null, int? diaSemana = null);
    }
}
