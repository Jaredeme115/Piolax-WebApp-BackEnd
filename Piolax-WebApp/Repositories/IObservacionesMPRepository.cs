using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IObservacionesMPRepository
    {
        Task<ObservacionesMP> AgregarObservacion(ObservacionesMP observacion);
        Task<IEnumerable<ObservacionesMP>> ObtenerObservacionesPorMP(int idHistoricoMP);
        Task<ObservacionesMP?> EliminarObservacion(int idObservacionMP);
    }
}
