﻿using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using System.Globalization;

namespace Piolax_WebApp.Repositories
{
    public interface ISolicitudesRepository
    {
        Task<Solicitudes> RegistrarSolicitud(Solicitudes solicitudes);
        Task<Solicitudes> ObtenerSolicitudConDetalles(int idSolicitud);
        Task<IEnumerable<Solicitudes>> ObtenerSolicitudes();
        Task<IEnumerable<Solicitudes>> ObtenerSolicitudesEmpleado(string numNomina);
        Task<bool> ExisteSolicitud(int idSolicitud);

        //Método para obtener modificar el estatus de aprobación de la solicitud
        Task<SolicitudesDetalleDTO> ModificarEstatusAprobacionSolicitante(int idSolicitud, int idStatusAprobacionSolicitante);
        Task<IEnumerable<Solicitudes>> ConsultarSolicitudesPorMaquinaYArea(int idMaquina, int idArea);

    }
}
