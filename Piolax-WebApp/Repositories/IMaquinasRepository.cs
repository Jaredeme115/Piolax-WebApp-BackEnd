using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IMaquinasRepository
    {
        Task<Maquinas> Consultar(int idMaquina);
        Task<IEnumerable<Maquinas>> ConsultarTodos();
        Task<Maquinas> Registro(Maquinas maquina);
        Task<Maquinas> Modificar(int idMaquina, Maquinas maquina);
        Task<Maquinas> Eliminar(int idMaquina);
        Task<bool> MaquinaExiste(int idMaquina);
        Task<bool> MaquinaExisteRegistro(string nombreMaquina);
        Task<IEnumerable<Maquinas>> ConsultarPorArea(int idArea);

        Task<Maquinas> ConsultarPorId(int idMaquina);
    }
}
