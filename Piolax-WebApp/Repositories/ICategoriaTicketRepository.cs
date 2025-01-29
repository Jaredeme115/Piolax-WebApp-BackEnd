using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface ICategoriaTicketRepository
    {
        Task<categoriaTicket> Consultar(int idCategoriaTicket);
    }
}
