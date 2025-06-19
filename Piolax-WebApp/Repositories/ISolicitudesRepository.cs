using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using System.Globalization;
using System.Threading.Tasks;

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
        Task<IEnumerable<Solicitudes>> ConsultarSolicitudesPorMaquinaYArea2(int idMaquina, int idArea, DateTime? fechaHasta = null);
        Task<IEnumerable<Solicitudes>> ConsultarSolicitudesNoTomadas();

        Task<IEnumerable<Solicitudes>> ConsultarSolicitudesTerminadas();

        Task<IEnumerable<Solicitudes>> ConsultarSolicitudesTerminadasPorMesYAnio(int mes, int anio);

        Task ActualizarStatusOrden(int idSolicitud, int idStatusOrden);
        Task<IEnumerable<Solicitudes>> ObtenerSolicitudesConPrioridadAsync();
        Task<bool> EliminarSolicitud(int idSolicitud);

        //Método para obtener solicitudes por area
        Task<IEnumerable<Solicitudes>> ObtenerSolicitudesPorAreaYRoles(int idArea, List<int> idRoles);
        Task<List<Solicitudes>> ObtenerSolicitudesEnStatus(int idStatusOrden);

        //Método de apoyo para calculo de MTBF
        Task<int> ContarFallasPorAreaEnMes(int idArea, int anio, int mes);

        // <summary>
        /// Devuelve el día del mes (1–31) de la primera solicitud hecha en el área indicada,
        /// para el año y mes especificados. Retorna null si no existen solicitudes en ese período.
        /// </summary>
        Task<int?> ObtenerPrimerDiaSolicitudPorAreaEnMes(int idArea, int anio, int mes);

        /// <summary>
        /// Cuenta cuántas solicitudes (fallas) hubo en el área indicada durante el mes y año especificados,
        /// considerando solo aquellas cuya fecha de solicitud tenga día <= diaHoy.
        /// </summary>
        /// <param name="idArea">ID del área</param>
        /// <param name="anio">Año de búsqueda (por ejemplo, 2025)</param>
        /// <param name="mes">Mes de búsqueda (1–12)</param>
        /// <param name="diaHoy">Día del mes hasta el cual contar (1–31)</param>
        /// <returns>El número total de solicitudes que cumplen con los criterios</returns>
        ///
        Task<int> ContarFallasPorAreaEnMesHastaDia(int idArea, int anio, int mes, int diaHoy);



        /// <summary>
        /// Cuenta cuántas solicitudes (fallas) hubo en el área indicada durante el mes y año especificados,
        /// pero solo aquellas cuya fecha de solicitud tenga día >= diaInicio.
        /// Es decir, cuántas fallas hay desde “diaInicio” hasta el fin de mes.
        /// </summary>
        /// <param name="idArea">ID del área (1, 2 ó 3, por ejemplo)</param>
        /// <param name="anio">Año de búsqueda (por ejemplo, 2025)</param>
        /// <param name="mes">Mes de búsqueda (1–12)</param>
        /// <param name="diaInicio">Día del mes desde el cual contar (1–31)</param>
        /// <returns>El número total de solicitudes que cumplen con los criterios</returns>
        Task<int> ContarFallasPorAreaEnMesDesdeDia(int idArea, int anio, int mes, int diaInicio);

    }
}
