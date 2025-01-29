using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class CategoriaTicketService (ICategoriaTicketRepository repository): ICategoriaTicketService
    {

        private readonly ICategoriaTicketRepository _repository = repository;

        public Task<categoriaTicket> Consultar(int idCategoriaTicket)
        {
            return _repository.Consultar(idCategoriaTicket);
        }
    }
}
