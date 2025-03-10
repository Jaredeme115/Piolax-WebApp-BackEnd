using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Repositories.Impl;

namespace Piolax_WebApp.Services.Impl
{
    public class SolicitudService(
        ISolicitudesRepository repository,
        IEmpleadoService empleadoService,
        IEmpleadoAreaRolService empleadoAreaRolService,
        IMaquinasService maquinasService,
        ITurnosService turnosService,
        IStatusOrdenService statusOrdenService,
        IStatusAprobacionSolicitanteService statusAprobacionSolicitanteService,
        ICategoriaTicketService categoriaTicketService,
        IAsignacionRepository asignacionRepository
        ) : ISolicitudService
    {
        private readonly ISolicitudesRepository _repository = repository;
        private readonly IEmpleadoService _empleadoService = empleadoService;
        private readonly IEmpleadoAreaRolService _empleadoAreaRolService = empleadoAreaRolService;
        private readonly IMaquinasService _maquinasService = maquinasService;
        private readonly ITurnosService _turnoService = turnosService;
        private readonly IStatusOrdenService _statusOrdenService = statusOrdenService;
        private readonly IStatusAprobacionSolicitanteService _statusAprobacionSolicitanteService = statusAprobacionSolicitanteService;
        private readonly ICategoriaTicketService _categoriaTicketService = categoriaTicketService;
        private readonly IAsignacionRepository _asignacionRepository = asignacionRepository;

        public async Task<SolicitudesDetalleDTO> RegistrarSolicitud(SolicitudesDTO solicitudesDTO)
        {
            var empleado = await _empleadoService.Consultar(solicitudesDTO.numNomina);
            if (empleado == null)
            {
                throw new Exception($"No se encontró el empleado con número de nómina: {solicitudesDTO.numNomina}");
            }

            var areasRoles = await _empleadoAreaRolService.ObtenerAreasRolesPorEmpleado(empleado.numNomina);

            // Permitir si el idAreaSeleccionada es 19, aunque el usuario no esté asignado a ella
            if (solicitudesDTO.idAreaSeleccionada != 19)
            {
                var areaSeleccionada = areasRoles.FirstOrDefault(ar => ar.idArea == solicitudesDTO.idAreaSeleccionada);
                if (areaSeleccionada == null)
                {
                    throw new Exception("El área seleccionada no está asignada al empleado.");
                }
            }

            // Validar el rol SOLO si el área no es 19
            var rolSeleccionado = areasRoles.FirstOrDefault(ar => ar.idRol == solicitudesDTO.idRolSeleccionado && ar.idArea == solicitudesDTO.idAreaSeleccionada);
            if (rolSeleccionado == null && solicitudesDTO.idAreaSeleccionada != 19)
            {
                throw new Exception("El rol seleccionado no está asignado al empleado en el área seleccionada.");
            }

            var solicitud = new Solicitudes
            {
                descripcion = solicitudesDTO.descripcion,
                fechaSolicitud = DateTime.Now,
                idEmpleado = empleado.idEmpleado,
                idMaquina = solicitudesDTO.idMaquina,
                idTurno = solicitudesDTO.idTurno,
                idStatusOrden = solicitudesDTO.idStatusOrden,
                idStatusAprobacionSolicitante = solicitudesDTO.idStatusAprobacionSolicitante,
                idAreaSeleccionada = solicitudesDTO.idAreaSeleccionada,
                idRolSeleccionado = solicitudesDTO.idRolSeleccionado,
                idCategoriaTicket = solicitudesDTO.idCategoriaTicket
            };

            solicitud = await _repository.RegistrarSolicitud(solicitud);

            //Se recurre a llamar a Maquina Service para obtener el nombre de la maquina en base al id de la maquina

            var maquina = await _maquinasService.Consultar(solicitud.idMaquina); // Obtener la máquina por ID

            //Se recurre a llamar a Turno Service para obtener el nombre del turno en base al id del turno

            var turno = await _turnoService.Consultar(solicitud.idTurno); // Obtener el turno por ID

            //Se recurre a llamar a StatusOrden Service para obtener el nombre del status de la orden en base al id del status de la orden
            var statusOrden = await _statusOrdenService.Consultar(solicitud.idStatusOrden); // Obtener el status de la orden por ID

            //Se recurre a llamar a StatusAprobacionSolicitante Service para obtener el nombre del status de aprobación del solicitante en base al id del status de aprobación del solicitante
            var statusAprobacionSolicitante = await _statusAprobacionSolicitanteService.Consultar(solicitud.idStatusAprobacionSolicitante); // Obtener el status de aprobación del solicitante por ID

            var categoriaTicket = await _categoriaTicketService.Consultar(solicitud.idCategoriaTicket); // Obtener la categoría del ticket por ID

            var solicitudDetalleDTO = new SolicitudesDetalleDTO
            {
                idSolicitud = solicitud.idSolicitud,
                descripcion = solicitud.descripcion,
                fechaSolicitud = solicitud.fechaSolicitud,
                nombreCompletoEmpleado = $"{empleado.nombre} {empleado.apellidoPaterno} {empleado.apellidoMaterno}",
                idMaquina = solicitud.idMaquina,
                idTurno = solicitud.idTurno,
                idStatusOrden = solicitud.idStatusOrden,
                idStatusAprobacionSolicitante = solicitud.idStatusAprobacionSolicitante,
                area = solicitudesDTO.idAreaSeleccionada == 19 ? "Área Permitida" : areasRoles.FirstOrDefault(ar => ar.idArea == solicitudesDTO.idAreaSeleccionada)?.Area.nombreArea,
                rol = solicitudesDTO.idAreaSeleccionada == 19 ? "Rol Asignado Automáticamente" : areasRoles.FirstOrDefault(ar => ar.idRol == solicitudesDTO.idRolSeleccionado)?.Rol.nombreRol,
                idCategoriaTicket = solicitud.idCategoriaTicket,
                nombreMaquina = maquina.nombreMaquina,
                nombreTurno = turno.descripcion,
                nombreStatusOrden = statusOrden.descripcionStatusOrden,
                nombreStatusAprobacionSolicitante = statusAprobacionSolicitante.descripcionStatusAprobacionSolicitante,
                nombreCategoriaTicket = solicitud.categoriaTicket.descripcionCategoriaTicket
            };

            return solicitudDetalleDTO;
        }
        public async Task<SolicitudesDetalleDTO?> ObtenerSolicitudConDetalles(int idSolicitud)
        {
            var solicitud = await _repository.ObtenerSolicitudConDetalles(idSolicitud);
            if (solicitud == null)
            {
                return null;
            }

            var empleado = solicitud.Empleado;
            var areasRoles = empleado.EmpleadoAreaRol;

            string nombreArea = "N/A";
            string nombreRol = "N/A";

            // 🟢 EXCEPCIÓN PARA idArea = 19 (Área especial)
            if (solicitud.idAreaSeleccionada == 19)
            {
                nombreArea = "Servicios Generales";  // 🔹 Puedes cambiarlo si tiene un nombre específico
                var rolPrincipal = areasRoles.FirstOrDefault(ar => ar.esAreaPrincipal);
                nombreRol = rolPrincipal?.Rol?.nombreRol ?? "N/A";
            }
            else
            {
                // 🔹 CASO NORMAL: SE OBTIENE EL ÁREA Y EL ROL ASIGNADO EN LA SOLICITUD
                var areaSeleccionada = areasRoles.FirstOrDefault(ar => ar.idArea == solicitud.idAreaSeleccionada);
                var rolSeleccionado = areasRoles.FirstOrDefault(ar => ar.idRol == solicitud.idRolSeleccionado && ar.idArea == solicitud.idAreaSeleccionada);

                nombreArea = areaSeleccionada?.Area?.nombreArea ?? "N/A";
                nombreRol = rolSeleccionado?.Rol?.nombreRol ?? "N/A";
            }

            // 📌 OBTENER DETALLES ADICIONALES
            var maquina = await _maquinasService.Consultar(solicitud.idMaquina);
            var turno = await _turnoService.Consultar(solicitud.idTurno);
            var statusOrden = await _statusOrdenService.Consultar(solicitud.idStatusOrden);
            var statusAprobacionSolicitante = await _statusAprobacionSolicitanteService.Consultar(solicitud.idStatusAprobacionSolicitante);
            var categoriaTicket = await _categoriaTicketService.Consultar(solicitud.idCategoriaTicket);

            var solicitudesDetalleDTO = new SolicitudesDetalleDTO
            {
                idSolicitud = solicitud.idSolicitud,
                descripcion = solicitud.descripcion,
                fechaSolicitud = solicitud.fechaSolicitud,
                nombreCompletoEmpleado = $"{empleado.nombre} {empleado.apellidoPaterno} {empleado.apellidoMaterno}",
                idMaquina = solicitud.idMaquina,
                idTurno = solicitud.idTurno,
                idStatusOrden = solicitud.idStatusOrden,
                idStatusAprobacionSolicitante = solicitud.idStatusAprobacionSolicitante,
                area = nombreArea, // 🔥 SE USA EL NOMBRE DEL ÁREA CORRECTO
                rol = nombreRol,   // 🔥 SE USA EL NOMBRE DEL ROL CORRECTO
                idCategoriaTicket = solicitud.idCategoriaTicket,
                nombreMaquina = maquina.nombreMaquina,
                nombreTurno = turno.descripcion,
                nombreStatusOrden = statusOrden.descripcionStatusOrden,
                nombreStatusAprobacionSolicitante = statusAprobacionSolicitante.descripcionStatusAprobacionSolicitante,
                nombreCategoriaTicket = solicitud.categoriaTicket.descripcionCategoriaTicket
            };

            return solicitudesDetalleDTO;
        }


        public async Task<IEnumerable<SolicitudesDetalleDTO>> ObtenerSolicitudes()
        {
            var solicitudes = await _repository.ObtenerSolicitudes();
            var solicitudesDetalleDTO = new List<SolicitudesDetalleDTO>();

            foreach (var solicitud in solicitudes)
            {
                var empleado = solicitud.Empleado;
                var areasRoles = empleado.EmpleadoAreaRol;

                // Filtrar el área y el rol específicos de la solicitud
                var areaSeleccionada = areasRoles.FirstOrDefault(ar => ar.idArea == solicitud.idAreaSeleccionada);
                var rolSeleccionado = areasRoles.FirstOrDefault(ar => ar.idRol == solicitud.idRolSeleccionado && ar.idArea == solicitud.idAreaSeleccionada);

                //Se recurre a llamar a Maquina Service para obtener el nombre de la maquina en base al id de la maquina

                var maquina = await _maquinasService.Consultar(solicitud.idMaquina); // Obtener la máquina por ID

                //Se recurre a llamar a Turno Service para obtener el nombre del turno en base al id del turno

                var turno = await _turnoService.Consultar(solicitud.idTurno); // Obtener el turno por ID

                //Se recurre a llamar a StatusOrden Service para obtener el nombre del status de la orden en base al id del status de la orden
                var statusOrden = await _statusOrdenService.Consultar(solicitud.idStatusOrden); // Obtener el status de la orden por ID

                //Se recurre a llamar a StatusAprobacionSolicitante Service para obtener el nombre del status de aprobación del solicitante en base al id del status de aprobación del solicitante
                var statusAprobacionSolicitante = await _statusAprobacionSolicitanteService.Consultar(solicitud.idStatusAprobacionSolicitante); // Obtener el status de aprobación del solicitante por ID

                var categoriaTicket = await _categoriaTicketService.Consultar(solicitud.idCategoriaTicket); // Obtener la categoría del ticket por ID

                var solicitudDetalleDTO = new SolicitudesDetalleDTO
                {
                    idSolicitud = solicitud.idSolicitud,
                    descripcion = solicitud.descripcion,
                    fechaSolicitud = solicitud.fechaSolicitud,
                    nombreCompletoEmpleado = $"{empleado.nombre} {empleado.apellidoPaterno} {empleado.apellidoMaterno}",
                    idMaquina = solicitud.idMaquina,
                    idTurno = solicitud.idTurno,
                    idStatusOrden = solicitud.idStatusOrden,
                    idStatusAprobacionSolicitante = solicitud.idStatusAprobacionSolicitante,
                    area = areaSeleccionada?.Area?.nombreArea ?? "N/A",
                    rol = rolSeleccionado?.Rol?.nombreRol ?? "N/A",
                    idCategoriaTicket = solicitud.idCategoriaTicket,
                    nombreMaquina = maquina.nombreMaquina,
                    nombreTurno = turno.descripcion,
                    nombreStatusOrden = statusOrden.descripcionStatusOrden,
                    nombreStatusAprobacionSolicitante = statusAprobacionSolicitante.descripcionStatusAprobacionSolicitante,
                    nombreCategoriaTicket = solicitud.categoriaTicket.descripcionCategoriaTicket
                };

                solicitudesDetalleDTO.Add(solicitudDetalleDTO);
            }

            return solicitudesDetalleDTO;
        }

        public async Task<IEnumerable<SolicitudesDetalleDTO>> ObtenerSolicitudesEmpleado(string numNomina)
        {
            var solicitudes = await _repository.ObtenerSolicitudesEmpleado(numNomina);
            var solicitudesDetalleDTO = new List<SolicitudesDetalleDTO>();

            foreach (var solicitud in solicitudes)
            {
                var empleado = solicitud.Empleado;
                var areasRoles = empleado.EmpleadoAreaRol;

                // Filtrar el área y el rol específicos de la solicitud
                var areaSeleccionada = areasRoles.FirstOrDefault(ar => ar.idArea == solicitud.idAreaSeleccionada);
                var rolSeleccionado = areasRoles.FirstOrDefault(ar => ar.idRol == solicitud.idRolSeleccionado && ar.idArea == solicitud.idAreaSeleccionada);

                //Se recurre a llamar a Maquina Service para obtener el nombre de la maquina en base al id de la maquina

                var maquina = await _maquinasService.Consultar(solicitud.idMaquina); // Obtener la máquina por ID

                //Se recurre a llamar a Turno Service para obtener el nombre del turno en base al id del turno

                var turno = await _turnoService.Consultar(solicitud.idTurno); // Obtener el turno por ID

                //Se recurre a llamar a StatusOrden Service para obtener el nombre del status de la orden en base al id del status de la orden
                var statusOrden = await _statusOrdenService.Consultar(solicitud.idStatusOrden); // Obtener el status de la orden por ID

                //Se recurre a llamar a StatusAprobacionSolicitante Service para obtener el nombre del status de aprobación del solicitante en base al id del status de aprobación del solicitante
                var statusAprobacionSolicitante = await _statusAprobacionSolicitanteService.Consultar(solicitud.idStatusAprobacionSolicitante); // Obtener el status de aprobación del solicitante por ID

                var categoriaTicket = await _categoriaTicketService.Consultar(solicitud.idCategoriaTicket); // Obtener la categoría del ticket por ID

                var solicitudDetalleDTO = new SolicitudesDetalleDTO
                {
                    idSolicitud = solicitud.idSolicitud,
                    descripcion = solicitud.descripcion,
                    fechaSolicitud = solicitud.fechaSolicitud,
                    nombreCompletoEmpleado = $"{empleado.nombre} {empleado.apellidoPaterno} {empleado.apellidoMaterno}",
                    idMaquina = solicitud.idMaquina,
                    idTurno = solicitud.idTurno,
                    idStatusOrden = solicitud.idStatusOrden,
                    idStatusAprobacionSolicitante = solicitud.idStatusAprobacionSolicitante,
                    area = areaSeleccionada?.Area?.nombreArea ?? "N/A",
                    rol = rolSeleccionado?.Rol?.nombreRol ?? "N/A",
                    idCategoriaTicket = solicitud.idCategoriaTicket,
                    nombreMaquina = maquina.nombreMaquina,
                    nombreTurno = turno.descripcion,
                    nombreStatusOrden = statusOrden.descripcionStatusOrden,
                    nombreStatusAprobacionSolicitante = statusAprobacionSolicitante.descripcionStatusAprobacionSolicitante,
                    nombreCategoriaTicket = solicitud.categoriaTicket.descripcionCategoriaTicket
                };

                solicitudesDetalleDTO.Add(solicitudDetalleDTO);
            }

            return solicitudesDetalleDTO;
        }

        public async Task<SolicitudesDetalleDTO> ModificarEstatusAprobacionSolicitante(int idSolicitud, int idStatusAprobacionSolicitante)
        {
            var solicitud = await _repository.ObtenerSolicitudConDetalles(idSolicitud);
            if (solicitud == null)
            {
                throw new Exception($"No se encontró la solicitud con ID: {idSolicitud}");
            }

            // ✅ Verificar que al menos un técnico haya aprobado antes de que el usuario pueda aprobar/rechazar
            bool tecnicoAprobado = solicitud.Asignaciones != null &&
                                   solicitud.Asignaciones.Any(a => a.Asignacion_Tecnico != null &&
                                                                    a.Asignacion_Tecnico.Any(t => t.idStatusAprobacionTecnico == 1));

            if (!tecnicoAprobado)
            {
                throw new Exception("No se puede aprobar/rechazar porque ningún técnico ha aprobado el mantenimiento.");
            }

            // ✅ Verificar que el status de aprobación del solicitante sea válido
            var statusAprobacionSolicitante = await _statusAprobacionSolicitanteService.Consultar(idStatusAprobacionSolicitante);
            if (statusAprobacionSolicitante == null)
            {
                throw new Exception($"No se encontró el status de aprobación del solicitante con ID: {idStatusAprobacionSolicitante}");
            }

            // 🟢 Actualizamos el status de aprobación del solicitante
            solicitud.idStatusAprobacionSolicitante = idStatusAprobacionSolicitante;
            await _repository.ModificarEstatusAprobacionSolicitante(idSolicitud, idStatusAprobacionSolicitante);

            if (idStatusAprobacionSolicitante == 1) // ✅ Aprobado por el usuario
            {
                solicitud.idStatusOrden = 1; // "Realizado"
                await _repository.ActualizarStatusOrden(idSolicitud, 1);

                foreach (var asignacion in solicitud.Asignaciones)
                {
                    asignacion.idStatusAsignacion = 3; // Se asume que 3 es el estado "Cerrada/Finalizada"
                    await _asignacionRepository.ActualizarAsignacion(asignacion.idAsignacion, asignacion);
                }
            }
            else if (idStatusAprobacionSolicitante == 2) // ❌ Rechazado por el usuario
            {
                solicitud.idStatusOrden = 6; // "Rechazada"
                await _repository.ActualizarStatusOrden(idSolicitud, 6);

                foreach (var asignacion in solicitud.Asignaciones)
                {
                    asignacion.idStatusAsignacion = 4; // Se asume que 4 es "No tomada/Rechazada"
                    await _asignacionRepository.ActualizarAsignacion(asignacion.idAsignacion, asignacion);
                }
            }

            // ✅ Retornar el DTO con los datos actualizados
            var solicitudDetalleDTO = new SolicitudesDetalleDTO
            {
                idSolicitud = solicitud.idSolicitud,
                descripcion = solicitud.descripcion,
                fechaSolicitud = solicitud.fechaSolicitud,
                idStatusOrden = solicitud.idStatusOrden,
                idStatusAprobacionSolicitante = solicitud.idStatusAprobacionSolicitante
            };

            return solicitudDetalleDTO;
        }



        public async Task<IEnumerable<Solicitudes>> ConsultarSolicitudesNoTomadas()
        {
            return await _repository.ConsultarSolicitudesNoTomadas();
        }

        public async Task<IEnumerable<SolicitudesDetalleDTO>> ConsultarSolicitudesTerminadas()
        {
            var solicitudes = await _repository.ConsultarSolicitudesTerminadas();
            var solicitudesDetalleDTO = new List<SolicitudesDetalleDTO>();

            foreach (var solicitud in solicitudes)
            {
                var empleado = solicitud.Empleado;
                if (empleado == null)
                {
                    continue;
                }

                var areasRoles = empleado.EmpleadoAreaRol ?? new List<EmpleadoAreaRol>();

                var areaSeleccionada = areasRoles.FirstOrDefault(ar => ar.idArea == solicitud.idAreaSeleccionada);
                var rolSeleccionado = areasRoles.FirstOrDefault(ar => ar.idRol == solicitud.idRolSeleccionado && ar.idArea == solicitud.idAreaSeleccionada);

                // Obtener el técnico aprobado
                var tecnico = solicitud.Asignaciones
                    .SelectMany(a => a.Asignacion_Tecnico)
                    .Where(t => t.idStatusAprobacionTecnico == 1)
                    .Select(t => t.Empleado)
                    .FirstOrDefault();

                var nombreCompletoTecnico = tecnico != null
                    ? $"{tecnico.nombre} {tecnico.apellidoPaterno} {tecnico.apellidoMaterno}"
                    : "No asignado";

                // Obtener la solución aplicada
                var solucion = solicitud.Asignaciones
                    .SelectMany(a => a.Asignacion_Tecnico)
                    .Where(s => !string.IsNullOrEmpty(s.solucion))
                    .Select(s => s.solucion)
                    .FirstOrDefault();

                // Obtener las refacciones utilizadas
                var refacciones = solicitud.Asignaciones
                    .SelectMany(a => a.Asignacion_Tecnico)
                    .SelectMany(at => at.Asignacion_Refacciones)
                    .Select(ar => new RefaccionesDTO
                    {
                        NombreRefaccion = ar.Inventario.nombreProducto,
                        Cantidad = ar.cantidad
                    }).ToList();

                var solicitudDetalleDTO = new SolicitudesDetalleDTO
                {
                    idSolicitud = solicitud.idSolicitud,
                    descripcion = solicitud.descripcion,
                    fechaSolicitud = solicitud.fechaSolicitud,
                    nombreCompletoEmpleado = $"{empleado.nombre} {empleado.apellidoPaterno} {empleado.apellidoMaterno}",
                    idMaquina = solicitud.idMaquina,
                    idTurno = solicitud.idTurno,
                    idStatusOrden = solicitud.idStatusOrden,
                    idStatusAprobacionSolicitante = solicitud.idStatusAprobacionSolicitante,
                    area = areaSeleccionada?.Area?.nombreArea ?? "N/A",
                    rol = rolSeleccionado?.Rol?.nombreRol ?? "N/A",
                    idCategoriaTicket = solicitud.idCategoriaTicket,
                    nombreMaquina = solicitud.Maquina?.nombreMaquina ?? "No disponible",
                    nombreTurno = solicitud.Turno?.descripcion ?? "No disponible",
                    nombreStatusOrden = solicitud.StatusOrden?.descripcionStatusOrden ?? "No disponible",
                    nombreStatusAprobacionSolicitante = solicitud.StatusAprobacionSolicitante?.descripcionStatusAprobacionSolicitante ?? "No disponible",
                    nombreCategoriaTicket = solicitud.categoriaTicket?.descripcionCategoriaTicket ?? "No disponible",
                    nombreCompletoTecnico = nombreCompletoTecnico,
                    solucion = solucion,
                    Refacciones = refacciones
                };

                solicitudesDetalleDTO.Add(solicitudDetalleDTO);
            }

            return solicitudesDetalleDTO;
        }


        public async Task ActualizarStatusOrden(int idSolicitud, int idStatusOrden)
        {
            var solicitud = await _repository.ObtenerSolicitudConDetalles(idSolicitud);
            if (solicitud == null)
            {
                throw new Exception($"No se encontró la solicitud con ID: {idSolicitud}");
            }
            solicitud.idStatusOrden = idStatusOrden;
            
            await _repository.ActualizarStatusOrden(idSolicitud, idStatusOrden);
        }

        public async Task<IEnumerable<SolicitudesDetalleDTO>> ObtenerSolicitudesConPrioridadAsync()
        {
            var solicitudes = await _repository.ObtenerSolicitudesConPrioridadAsync();
            var solicitudesDetalleDTO = new List<SolicitudesDetalleDTO>();

            foreach (var solicitud in solicitudes)
            {
                var empleado = solicitud.Empleado;
                var areasRoles = empleado.EmpleadoAreaRol;

                string nombreArea = "N/A";
                string nombreRol = "N/A";

                // 🟢 CASO EXCEPCIÓN: SI EL ÁREA ES 19, SE SELECCIONA EL ÁREA 19 Y EL ROL PRINCIPAL
                if (solicitud.idAreaSeleccionada == 19)
                {
                    nombreArea = "Servicios Generales";  
                    var rolPrincipal = areasRoles.FirstOrDefault(ar => ar.esAreaPrincipal);
                    nombreRol = rolPrincipal?.Rol?.nombreRol ?? "N/A";
                }
                else
                {
                    // 🔹 CASO NORMAL: SE OBTIENE EL ÁREA Y EL ROL ASIGNADO EN LA SOLICITUD
                    var areaSeleccionada = areasRoles.FirstOrDefault(ar => ar.idArea == solicitud.idAreaSeleccionada);
                    var rolSeleccionado = areasRoles.FirstOrDefault(ar => ar.idRol == solicitud.idRolSeleccionado && ar.idArea == solicitud.idAreaSeleccionada);

                    nombreArea = areaSeleccionada?.Area?.nombreArea ?? "N/A";
                    nombreRol = rolSeleccionado?.Rol?.nombreRol ?? "N/A";
                }

                // 📌 OBTENER DETALLES ADICIONALES
                var maquina = await _maquinasService.Consultar(solicitud.idMaquina);
                var turno = await _turnoService.Consultar(solicitud.idTurno);
                var statusOrden = await _statusOrdenService.Consultar(solicitud.idStatusOrden);
                var statusAprobacionSolicitante = await _statusAprobacionSolicitanteService.Consultar(solicitud.idStatusAprobacionSolicitante);
                var categoriaTicket = await _categoriaTicketService.Consultar(solicitud.idCategoriaTicket);

                var solicitudDetalleDTO = new SolicitudesDetalleDTO
                {
                    idSolicitud = solicitud.idSolicitud,
                    descripcion = solicitud.descripcion,
                    fechaSolicitud = solicitud.fechaSolicitud,
                    nombreCompletoEmpleado = $"{empleado.nombre} {empleado.apellidoPaterno} {empleado.apellidoMaterno}",
                    idMaquina = solicitud.idMaquina,
                    idTurno = solicitud.idTurno,
                    idStatusOrden = solicitud.idStatusOrden,
                    idStatusAprobacionSolicitante = solicitud.idStatusAprobacionSolicitante,
                    area = nombreArea, // 🔥 AHORA ASIGNA EL ÁREA CORRECTA
                    rol = nombreRol,   // 🔥 AHORA ASIGNA EL ROL CORRECTO
                    idCategoriaTicket = solicitud.idCategoriaTicket,
                    nombreMaquina = maquina.nombreMaquina,
                    nombreTurno = turno.descripcion,
                    nombreStatusOrden = statusOrden.descripcionStatusOrden,
                    nombreStatusAprobacionSolicitante = statusAprobacionSolicitante.descripcionStatusAprobacionSolicitante,
                    nombreCategoriaTicket = solicitud.categoriaTicket.descripcionCategoriaTicket
                };

                solicitudesDetalleDTO.Add(solicitudDetalleDTO);
            }

            return solicitudesDetalleDTO;
        }

        public async Task<bool> EliminarSolicitud(int idSolicitud)
        {
            // 🔹 Verificar si la solicitud existe antes de eliminar
            var existe = await _repository.ExisteSolicitud(idSolicitud);
            if (!existe)
            {
                throw new Exception($"No se encontró la solicitud con ID: {idSolicitud}");
            }

            // 🔹 Intentar eliminar la solicitud
            return await _repository.EliminarSolicitud(idSolicitud);
        }

    }
}
