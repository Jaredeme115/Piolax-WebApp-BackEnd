using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IStatusAprobacionSolicitanteRepository
    {
        Task<StatusAprobacionSolicitante> Consultar(int idStatusAprobacionSolicitante);
        Task<IEnumerable<StatusAprobacionSolicitante>> ConsultarTodos();
        Task<StatusAprobacionSolicitante> Registro(StatusAprobacionSolicitante statusAprobacionSolicitante);
        Task<StatusAprobacionSolicitante> Modificar(int idStatusAprobacionSolicitante, StatusAprobacionSolicitante statusAprobacionSolicitante);
        Task<StatusAprobacionSolicitante> Eliminar(int idStatusAprobacionSolicitante);
        Task<bool> StatusAprobacionSolicitanteExiste(int idStatusAprobacionSolicitante);
        Task<bool> StatusAprobacionSolicitanteExisteRegistro(string descripcionStatusAprobacionSolicitante);
    }
}
