using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface IStatusAprobacionSolicitanteService
    {
        Task<StatusAprobacionSolicitante> Consultar(int idStatusAprobacionSolicitante);
        Task<IEnumerable<StatusAprobacionSolicitante>> ConsultarTodos();
        Task<StatusAprobacionSolicitante> Registro(StatusAprobacionSolicitanteDTO statusAprobacionSolicitante);
        Task<StatusAprobacionSolicitante> Modificar(int idStatusAprobacionSolicitante, StatusAprobacionSolicitanteDTO statusAprobacionSolicitante);
        Task<StatusAprobacionSolicitante> Eliminar(int idStatusAprobacionSolicitante);
        Task<bool> StatusAprobacionSolicitanteExiste(int idStatusAprobacionSolicitante);
        Task<bool> StatusAprobacionSolicitanteExisteRegistro(string descripcionStatusAprobacionSolicitante);


    }
}
