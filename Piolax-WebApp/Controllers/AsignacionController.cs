using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Services;

namespace Piolax_WebApp.Controllers
{
    public class AsignacionController(IAsignacionService service) : BaseApiController
    {
        private readonly IAsignacionService _service = service;


    }
}
