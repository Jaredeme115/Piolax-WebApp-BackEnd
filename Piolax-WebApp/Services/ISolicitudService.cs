﻿using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface ISolicitudService
    {
        Task<SolicitudesDetalleDTO> RegistrarSolicitud(SolicitudesDTO solicitudesDTO);
        Task<SolicitudesDetalleDTO> ObtenerSolicitudConDetalles(int idSolicitud);
        Task<IEnumerable<SolicitudesDetalleDTO>> ObtenerSolicitudes();
    }
}
