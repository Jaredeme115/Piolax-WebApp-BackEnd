﻿using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Repositories.Impl;
using Microsoft.AspNetCore.SignalR;
using Piolax_WebApp.Hubs;
using OfficeOpenXml;

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
        IAsignacionRepository asignacionRepository,
        IHubContext<SolicitudHub> hubContext
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
        private readonly IHubContext<SolicitudHub> _hubContext = hubContext;

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

            // 🔹 Notificar a todos los clientes conectados sobre la nueva solicitud
            await NotificarActualizacionSolicitudes(solicitudesDTO.idAreaSeleccionada);

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

            await NotificarActualizacionSolicitudes(solicitud.idAreaSeleccionada);
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

                // Obtener horas de inicio y término del técnico aprobado
                var tecnicoAprobado = solicitud.Asignaciones
                    .SelectMany(a => a.Asignacion_Tecnico)
                    .FirstOrDefault(t => t.idStatusAprobacionTecnico == 1);

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
                    Refacciones = refacciones,
                    horaInicio = tecnicoAprobado?.horaInicio,
                    horaTermino = tecnicoAprobado?.horaTermino
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
            await NotificarActualizacionSolicitudes(solicitud.idAreaSeleccionada);
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

                // 🆕 OBTENER EL TÉCNICO ASIGNADO (si existe)
                string nombreCompletoTecnico = "No asignado";

                // Verificamos si hay asignaciones para esta solicitud
                if (solicitud.Asignaciones != null && solicitud.Asignaciones.Any())
                {
                    // Buscamos un técnico activo entre todas las asignaciones
                    var tecnicoActivo = solicitud.Asignaciones
                        .SelectMany(a => a.Asignacion_Tecnico)
                        .FirstOrDefault(t => t.esTecnicoActivo);

                    // Si hay un técnico activo, obtenemos su nombre completo
                    if (tecnicoActivo != null && tecnicoActivo.Empleado != null)
                    {
                        nombreCompletoTecnico = $"{tecnicoActivo.Empleado.nombre} {tecnicoActivo.Empleado.apellidoPaterno} {tecnicoActivo.Empleado.apellidoMaterno}";
                    }
                    // Si no hay técnico activo pero hay técnicos asignados, tomamos el primero
                    else
                    {
                        var primerTecnico = solicitud.Asignaciones
                            .SelectMany(a => a.Asignacion_Tecnico)
                            .FirstOrDefault();

                        if (primerTecnico != null && primerTecnico.Empleado != null)
                        {
                            nombreCompletoTecnico = $"{primerTecnico.Empleado.nombre} {primerTecnico.Empleado.apellidoPaterno} {primerTecnico.Empleado.apellidoMaterno}";
                        }
                    }
                }

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
                    nombreCategoriaTicket = solicitud.categoriaTicket.descripcionCategoriaTicket,
                    nombreCompletoTecnico = nombreCompletoTecnico // 🆕 AGREGADO: NOMBRE DEL TÉCNICO
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


        public async Task<IEnumerable<SolicitudesDetalleDTO>> ObtenerSolicitudesPorAreaYRoles(int idArea, List<int> idRoles)
        {
            // Obtener solicitudes con filtro de área y roles, aplicando la prioridad según el status
            var solicitudes = await _repository.ObtenerSolicitudesPorAreaYRoles(idArea, idRoles);
            var solicitudesDetalleDTO = new List<SolicitudesDetalleDTO>();

            foreach (var solicitud in solicitudes)
            {
                var empleado = solicitud.Empleado;
                var areasRoles = empleado.EmpleadoAreaRol;

                string nombreArea = "N/A";
                string nombreRol = "N/A";

                // Gestión del área 19 (caso especial)
                if (solicitud.idAreaSeleccionada == 19)
                {
                    nombreArea = "Servicios Generales";
                    var rolPrincipal = areasRoles.FirstOrDefault(ar => ar.esAreaPrincipal);
                    nombreRol = rolPrincipal?.Rol?.nombreRol ?? "N/A";
                }
                else
                {
                    var areaSeleccionada = areasRoles.FirstOrDefault(ar => ar.idArea == solicitud.idAreaSeleccionada);
                    var rolSeleccionado = areasRoles.FirstOrDefault(ar => ar.idRol == solicitud.idRolSeleccionado && ar.idArea == solicitud.idAreaSeleccionada);

                    nombreArea = areaSeleccionada?.Area?.nombreArea ?? "N/A";
                    nombreRol = rolSeleccionado?.Rol?.nombreRol ?? "N/A";
                }

                // Obtener detalles adicionales
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
                    area = nombreArea,
                    rol = nombreRol,
                    idCategoriaTicket = solicitud.idCategoriaTicket,
                    nombreMaquina = maquina.nombreMaquina,
                    nombreTurno = turno.descripcion,
                    nombreStatusOrden = statusOrden.descripcionStatusOrden,
                    nombreStatusAprobacionSolicitante = statusAprobacionSolicitante.descripcionStatusAprobacionSolicitante,
                    nombreCategoriaTicket = categoriaTicket.descripcionCategoriaTicket
                };

                solicitudesDetalleDTO.Add(solicitudDetalleDTO);
            }

            return solicitudesDetalleDTO;
        }

        public async Task<IEnumerable<SolicitudesDetalleDTO>> ConsultarSolicitudesTerminadasPorArea(string numNomina)
        {
            // Obtener el empleado para verificar a qué áreas pertenece
            var empleado = await _empleadoService.Consultar(numNomina);
            if (empleado == null)
            {
                throw new Exception($"No se encontró el empleado con número de nómina: {numNomina}");
            }

            // Obtener las áreas asignadas al empleado
            var areasDelEmpleado = await _empleadoAreaRolService.ObtenerAreaPorEmpleado(numNomina);
            var idsAreas = areasDelEmpleado.Select(a => a.idArea).ToList();

            // Si no tiene áreas asignadas, regresamos una lista vacía
            if (!idsAreas.Any())
            {
                return new List<SolicitudesDetalleDTO>();
            }

            // Obtener todas las solicitudes terminadas
            var todasSolicitudesTerminadas = await _repository.ConsultarSolicitudesTerminadas();

            // Filtrar las solicitudes por las áreas del empleado
            var solicitudesFiltradas = todasSolicitudesTerminadas
                .Where(s => idsAreas.Contains(s.idAreaSeleccionada))
                .ToList();

            var solicitudesDetalleDTO = new List<SolicitudesDetalleDTO>();

            foreach (var solicitud in solicitudesFiltradas)
            {
                var solicitante = solicitud.Empleado;
                if (solicitante == null)
                {
                    continue;
                }

                var areasRoles = solicitante.EmpleadoAreaRol ?? new List<EmpleadoAreaRol>();

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

                // Obtener horas de inicio y término del técnico aprobado
                var tecnicoAprobado = solicitud.Asignaciones
                    .SelectMany(a => a.Asignacion_Tecnico)
                    .FirstOrDefault(t => t.idStatusAprobacionTecnico == 1);

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
                    nombreCompletoEmpleado = $"{solicitante.nombre} {solicitante.apellidoPaterno} {solicitante.apellidoMaterno}",
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
                    Refacciones = refacciones,
                    horaInicio = tecnicoAprobado?.horaInicio,
                    horaTermino = tecnicoAprobado?.horaTermino
                };

                solicitudesDetalleDTO.Add(solicitudDetalleDTO);
            }

            return solicitudesDetalleDTO;
        }

        public async Task<IEnumerable<SolicitudesDetalleDTO>> ConsultarSolicitudesTerminadasPorEmpleado(string numNomina)
        {
            // Obtener todas las solicitudes terminadas
            var todasSolicitudesTerminadas = await _repository.ConsultarSolicitudesTerminadas();

            // Filtrar las solicitudes que pertenecen al empleado específico
            var solicitudesFiltradas = todasSolicitudesTerminadas
                .Where(s => s.Empleado?.numNomina == numNomina)
                .ToList();

            var solicitudesDetalleDTO = new List<SolicitudesDetalleDTO>();

            foreach (var solicitud in solicitudesFiltradas)
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

                // Obtener horas de inicio y término del técnico aprobado
                var tecnicoAprobado = solicitud.Asignaciones
                    .SelectMany(a => a.Asignacion_Tecnico)
                    .FirstOrDefault(t => t.idStatusAprobacionTecnico == 1);

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
                    Refacciones = refacciones,
                    horaInicio = tecnicoAprobado?.horaInicio,
                    horaTermino = tecnicoAprobado?.horaTermino
                };

                solicitudesDetalleDTO.Add(solicitudDetalleDTO);
            }

            return solicitudesDetalleDTO;
        }

        public async Task<byte[]> ExportarSolicitudesTerminadasExcel()
        {
            // Obtener los datos de las solicitudes terminadas
            var solicitudes = await ConsultarSolicitudesTerminadas();

            // EPPlus requiere declarar el contexto de licencia
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Solicitudes Terminadas");

            // Encabezados
            worksheet.Cells[1, 1].Value = "Date";
            worksheet.Cells[1, 2].Value = "Area";
            worksheet.Cells[1, 3].Value = "Machine";
            worksheet.Cells[1, 4].Value = "Report Time";
            //worksheet.Cells[1, 5].Value = "Equipment stopped?";
            worksheet.Cells[1, 5].Value = "Shift";
            worksheet.Cells[1, 6].Value = "Tecnico/Operador";
            worksheet.Cells[1, 7].Value = "Order No.";
            worksheet.Cells[1, 8].Value = "Descripción de la falla";
            worksheet.Cells[1, 9].Value = "Solución de la falla";
            worksheet.Cells[1, 10].Value = "End Date";
            worksheet.Cells[1, 11].Value = "Start Time";
            worksheet.Cells[1, 12].Value = "End Time";
            worksheet.Cells[1, 13].Value = "Done by";

            // Estilo para encabezados
            using (var range = worksheet.Cells[1, 1, 1, 13])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Datos
            int row = 2;
            foreach (var s in solicitudes)
            {
                worksheet.Cells[row, 1].Value = s.fechaSolicitud.ToString("dd/MM/yyyy");
                worksheet.Cells[row, 2].Value = s.area;
                worksheet.Cells[row, 3].Value = s.nombreMaquina;
                worksheet.Cells[row, 4].Value = s.fechaSolicitud.ToString("HH:mm");
                //worksheet.Cells[row, 5].Value = s.nombreMaquina;
                worksheet.Cells[row, 5].Value = s.nombreTurno;
                worksheet.Cells[row, 6].Value = s.nombreCompletoEmpleado;
                worksheet.Cells[row, 7].Value = s.idSolicitud;
                worksheet.Cells[row, 8].Value = s.descripcion;
                worksheet.Cells[row, 9].Value = s.solucion;
                worksheet.Cells[row, 10].Value = s.horaTermino?.ToString("dd/MM/yyyy");
                worksheet.Cells[row, 11].Value = s.horaInicio?.ToString("HH:mm");
                worksheet.Cells[row, 12].Value = s.horaTermino?.ToString("HH:mm");
                worksheet.Cells[row, 13].Value = s.nombreCompletoTecnico;

                // Formatear refacciones como texto
                /*string refaccionesTexto = "";
                if (s.Refacciones != null && s.Refacciones.Any())
                {
                    refaccionesTexto = string.Join(", ", s.Refacciones.Select(r => $"{r.NombreRefaccion} ({r.Cantidad})"));
                }
                worksheet.Cells[row, 16].Value = refaccionesTexto;*/

                row++;
            }

            // Auto-ajustar columnas
            worksheet.Cells.AutoFitColumns();

            // Convertir a bytes y devolver
            return package.GetAsByteArray();
        }

        public async Task<byte[]> ExportarSolicitudesTerminadasPorAreaExcel(string numNomina)
        {
            // Obtener los datos de las solicitudes terminadas
            var solicitudes = await ConsultarSolicitudesTerminadasPorArea(numNomina);

            // EPPlus requiere declarar el contexto de licencia
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Solicitudes Terminadas");

            // Encabezados
            worksheet.Cells[1, 1].Value = "Date";
            worksheet.Cells[1, 2].Value = "Area";
            worksheet.Cells[1, 3].Value = "Machine";
            worksheet.Cells[1, 4].Value = "Report Time";
            //worksheet.Cells[1, 5].Value = "Equipment stopped?";
            worksheet.Cells[1, 5].Value = "Shift";
            worksheet.Cells[1, 6].Value = "Tecnico/Operador";
            worksheet.Cells[1, 7].Value = "Order No.";
            worksheet.Cells[1, 8].Value = "Descripción de la falla";
            worksheet.Cells[1, 9].Value = "Solución de la falla";
            worksheet.Cells[1, 10].Value = "End Date";
            worksheet.Cells[1, 11].Value = "Start Time";
            worksheet.Cells[1, 12].Value = "End Time";
            worksheet.Cells[1, 13].Value = "Done by";

            // Estilo para encabezados
            using (var range = worksheet.Cells[1, 1, 1, 13])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Datos
            int row = 2;
            foreach (var s in solicitudes)
            {
                worksheet.Cells[row, 1].Value = s.fechaSolicitud.ToString("dd/MM/yyyy");
                worksheet.Cells[row, 2].Value = s.area;
                worksheet.Cells[row, 3].Value = s.nombreMaquina;
                worksheet.Cells[row, 4].Value = s.fechaSolicitud.ToString("HH:mm");
                //worksheet.Cells[row, 5].Value = s.nombreMaquina;
                worksheet.Cells[row, 5].Value = s.nombreTurno;
                worksheet.Cells[row, 6].Value = s.nombreCompletoEmpleado;
                worksheet.Cells[row, 7].Value = s.idSolicitud;
                worksheet.Cells[row, 8].Value = s.descripcion;
                worksheet.Cells[row, 9].Value = s.solucion;
                worksheet.Cells[row, 10].Value = s.horaTermino?.ToString("dd/MM/yyyy");
                worksheet.Cells[row, 11].Value = s.horaInicio?.ToString("HH:mm");
                worksheet.Cells[row, 12].Value = s.horaTermino?.ToString("HH:mm");
                worksheet.Cells[row, 13].Value = s.nombreCompletoTecnico;

                // Formatear refacciones como texto
                /*string refaccionesTexto = "";
                if (s.Refacciones != null && s.Refacciones.Any())
                {
                    refaccionesTexto = string.Join(", ", s.Refacciones.Select(r => $"{r.NombreRefaccion} ({r.Cantidad})"));
                }
                worksheet.Cells[row, 16].Value = refaccionesTexto;*/

                row++;
            }

            // Auto-ajustar columnas
            worksheet.Cells.AutoFitColumns();

            // Convertir a bytes y devolver
            return package.GetAsByteArray();
        }



        // Método para notificar actualizaciones a través de SignalR
        private async Task NotificarActualizacionSolicitudes(int idArea)
        {
            try
            {
                // Notificar a todos los clientes
                await _hubContext.Clients.All.SendAsync("RecibirActualizacionSolicitudes", idArea);

                // Notificar específicamente al grupo de esta área
                await _hubContext.Clients.Group($"Area_{idArea}").SendAsync("RecibirActualizacionSolicitudes", idArea);
            }
            catch (Exception ex)
            {
                // Manejar el error si hay algún problema con SignalR
                Console.WriteLine($"Error al notificar actualización: {ex.Message}");
                // También puedes registrar el error en algún sistema de logs
            }
        }

    }
}
