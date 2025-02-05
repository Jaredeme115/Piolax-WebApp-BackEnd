using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IKPIRepository
    {
        Task GuardarKPIMantenimiento(KpisMantenimiento kpiMantenimiento);
        Task GuardarKPIDetalles(int idKPIMantenimiento, List<KpisDetalle> kpiDetalles);
    }
}
