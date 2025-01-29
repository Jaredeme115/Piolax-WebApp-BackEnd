using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface ICategoriaTicketService
    {
        Task<categoriaTicket> Consultar (int idCategoriaTicket);
    }
}
