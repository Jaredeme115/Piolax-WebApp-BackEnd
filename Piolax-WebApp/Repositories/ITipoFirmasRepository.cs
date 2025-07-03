using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface ITipoFirmasRepository
    {
        Task<TipoFirmas> RegistrarTipoFirma(TipoFirmas tipoFirma);
        Task<TipoFirmas> ModificarTipoFirma(int idTipoFirma);
        Task<bool> EliminarTipoFirma(int idTipoFirma);
    }
}
