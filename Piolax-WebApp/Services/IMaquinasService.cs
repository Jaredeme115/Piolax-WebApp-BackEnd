using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface IMaquinasService
    {
        Task<Maquinas> Consultar(int idMaquina);
        Task<IEnumerable<Maquinas>> ConsultarTodos();
        Task<Maquinas> Registro(MaquinaDTO maquina);
        Task<Maquinas> Modificar(int idMaquina, MaquinaDTO maquina);
        Task<Maquinas> Eliminar(int idMaquina);
        Task<bool> MaquinaExiste(int idMaquina);
        Task<bool> MaquinaExisteRegistro(string nombreMaquina);
        Task<IEnumerable<Maquinas>> ConsultarPorArea(int idArea);
    }
}
