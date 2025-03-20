using Piolax_WebApp.DTOs;

namespace Piolax_WebApp.Services
{
    public interface IObservacionesMPService
    {
        Task<ObservacionesMPDTO> AgregarObservacion(ObservacionesMPCrearDTO observacionDTO);
        Task<IEnumerable<ObservacionesMPDTO>> ObtenerObservacionesPorMP(int idMP);
        Task<ObservacionesMPDTO?> EliminarObservacion(int idObservacionMP);
    }
}
