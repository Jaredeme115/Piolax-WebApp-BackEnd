using Piolax_WebApp.DTOs;

namespace Piolax_WebApp.Services
{
    public interface IEmpleadoAreaRolService
    {
        Task RegistrarEmpleadoConAreaYRol(RegistroDTO registroDTO);
    }
}
