using Microsoft.Extensions.FileProviders;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Repositories.Impl;
using System.Configuration;
using static Piolax_WebApp.Services.Impl.AsignacionTecnicosService;
using Piolax_WebApp.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Piolax_WebApp.Services.Impl
{
    public class AsignacionTecnicosService(
        IAsignacionTecnicosRepository repository, 
        IAsignacionRepository asignacionRepository, 
        IAsignacionRefaccionesRepository asignacionRefaccionesRepository,
        IInventarioRepository inventarioRepository,
        IKPIRepository kpiRepository,
        IAsignacionService asignacionService,
        ISolicitudService solicitudService,
        IHubContext<AsignacionHub> hubContext) : IAsignacionTecnicosService
    {
        private readonly IAsignacionTecnicosRepository _repository = repository;
        private readonly IAsignacionRepository _asignacionRepository = asignacionRepository;
        private readonly IAsignacionRefaccionesRepository _asignacionRefaccionesRepository = asignacionRefaccionesRepository;
        private readonly IInventarioRepository _inventarioRepository = inventarioRepository;
        private readonly IKPIRepository _kpiRepository = kpiRepository;
        private readonly IAsignacionService _asignacionService = asignacionService;
        private readonly ISolicitudService _solicitudService = solicitudService;
        private readonly IHubContext<AsignacionHub> _hubContext = hubContext;



        public async Task<IEnumerable<Asignacion_TecnicoDetallesDTO>> ConsultarTecnicosPorAsignacion(int idAsignacion)
        {
            var tecnicos = await _repository.ConsultarTecnicosPorAsignacion(idAsignacion);
            return tecnicos.Select(tecnico => new Asignacion_TecnicoDetallesDTO
            {
                idAsignacionTecnico = tecnico.idAsignacionTecnico,
                idAsignacion = tecnico.idAsignacion,
                idEmpleado = tecnico.idEmpleado,
                nombreCompletoTecnico = $"{tecnico.Empleado.nombre} {tecnico.Empleado.apellidoPaterno} {tecnico.Empleado.apellidoMaterno}",
                horaInicio = tecnico.horaInicio,
                horaTermino = tecnico.horaTermino,
                solucion = tecnico.solucion,
                idStatusAprobacionTecnico = tecnico.idStatusAprobacionTecnico,
                nombreStatusAprobacionTecnico = tecnico.StatusAprobacionTecnico.descripcionStatusAprobacionTecnico,
                comentarioPausa = tecnico.comentarioPausa,
                esTecnicoActivo = tecnico.esTecnicoActivo
            });
        }

        public async Task<Asignacion_Tecnico?> ConsultarTecnicoPorAsignacionYEmpleado(int idAsignacion, int idEmpleado)
        {
            return await _repository.ConsultarTecnicoPorAsignacionYEmpleado(idAsignacion, idEmpleado);
        }

        /*public async Task<Asignacion_TecnicoResponseDTO?> CrearAsignacionTecnico(Asignacion_TecnicoDTO asignacionTecnicoDTO)
        {
            try
            {
                // 0️⃣ Reusar o crear la asignación principal
                var asignacionExistente = await _asignacionRepository.ObtenerAsignacionActivaPorSolicitud(asignacionTecnicoDTO.idSolicitud);
                if (asignacionExistente != null)
                    asignacionTecnicoDTO.idAsignacion = asignacionExistente.idAsignacion;
                // (opcional: crear nueva asignación si no existiera…)

                // 1️⃣ Verificar que la asignación existe
                var asignacion = await _asignacionRepository.ConsultarAsignacionPorId(asignacionTecnicoDTO.idAsignacion);
                if (asignacion == null)
                    throw new Exception("La asignación no existe.");

                // 2️⃣ Cargar **todos** los técnicos de la asignación
                var todosTecnicos = (await _repository.ConsultarTecnicosPorAsignacion(asignacionTecnicoDTO.idAsignacion)).ToList();

                // 3️⃣ ¿Este técnico ya está registrado?
                var tecnicoExistente = todosTecnicos
                    .FirstOrDefault(t => t.idEmpleado == asignacionTecnicoDTO.idEmpleado);

                Asignacion_Tecnico entidadFinal;

                if (tecnicoExistente != null)
                {
                    // ─── Reactivar técnico existente ───

                    // 3.a) Desactivar a **todos** los demás
                    foreach (var otro in todosTecnicos.Where(t => t.idEmpleado != tecnicoExistente.idEmpleado && t.esTecnicoActivo))
                    {
                        otro.esTecnicoActivo = false;
                        await _repository.ActualizarTecnicoEnAsignacion(otro);
                    }

                    // 3.b) Activar al que reingresa
                    bool estabaInactivo = !tecnicoExistente.esTecnicoActivo;
                    tecnicoExistente.esTecnicoActivo = true;
                    tecnicoExistente.comentarioPausa = "N/A";
                    if (estabaInactivo)
                        tecnicoExistente.horaInicio = DateTime.Now;

                    // 3.c) Reactivar el estado de la orden si estaba pausada
                    if (asignacion.idStatusAsignacion != 1)
                    {
                        asignacion.idStatusAsignacion = 1;
                        await _asignacionRepository.ActualizarAsignacion(asignacion.idAsignacion, asignacion);
                        await _solicitudService.ActualizarStatusOrden(asignacion.Solicitud.idSolicitud, 2);
                    }

                    await _repository.ActualizarTecnicoEnAsignacion(tecnicoExistente);
                    entidadFinal = tecnicoExistente;
                }
                else
                {
                    // ─── Crear un técnico nuevo ───

                    // 4.a) Validar límite de dos técnicos **solo** para nuevos
                    if (todosTecnicos.Count(t => !t.retirado) >= 2)
                        throw new InvalidOperationException("Ya se han asignado los dos técnicos permitidos para esta solicitud.");

                    // 4.b) Decidir quién va activo
                    bool hayActivo = todosTecnicos.Any(t => t.esTecnicoActivo);
                    asignacionTecnicoDTO.esTecnicoActivo = !hayActivo;

                    // 4.c) Si va activo, desactivar a cualquiera que quedara activo por cierre inesperado
                    if (asignacionTecnicoDTO.esTecnicoActivo)
                    {
                        foreach (var otro in todosTecnicos.Where(t => t.esTecnicoActivo))
                        {
                            otro.esTecnicoActivo = false;
                            await _repository.ActualizarTecnicoEnAsignacion(otro);
                        }

                        // Actualizar estado de la orden
                        await _solicitudService.ActualizarStatusOrden(asignacion.Solicitud.idSolicitud, 2);
                        if (asignacion.idStatusAsignacion != 1)
                        {
                            asignacion.idStatusAsignacion = 1;
                            await _asignacionRepository.ActualizarAsignacion(asignacion.idAsignacion, asignacion);
                        }
                    }

                    var nuevaEntidad = new Asignacion_Tecnico
                    {
                        idAsignacion = asignacionTecnicoDTO.idAsignacion,
                        idEmpleado = asignacionTecnicoDTO.idEmpleado,
                        horaInicio = DateTime.Now,
                        horaTermino = asignacionTecnicoDTO.horaTermino,
                        solucion = "N/A",
                        idStatusAprobacionTecnico = 3,
                        comentarioPausa = "N/A",
                        esTecnicoActivo = asignacionTecnicoDTO.esTecnicoActivo,
                        retirado = false
                    };

                    entidadFinal = await _repository.CrearAsignacionTecnico(nuevaEntidad);
                }

                // 5️⃣ Mapear al DTO de respuesta
                return new Asignacion_TecnicoResponseDTO
                {
                    idAsignacionTecnico = entidadFinal.idAsignacionTecnico,
                    idAsignacion = entidadFinal.idAsignacion,
                    idEmpleado = entidadFinal.idEmpleado,
                    idStatusAprobacionTecnico = entidadFinal.idStatusAprobacionTecnico,
                    horaInicio = entidadFinal.horaInicio,
                    horaTermino = entidadFinal.horaTermino,
                    solucion = entidadFinal.solucion,
                    comentarioPausa = entidadFinal.comentarioPausa,
                    esTecnicoActivo = entidadFinal.esTecnicoActivo,
                    tiempoAcumuladoMinutos = entidadFinal.tiempoAcumuladoMinutos
                };
            }
            catch
            {
                // Déjalo que el controlador capture y devuelva el código HTTP apropiado
                throw;
            }
        }*/

        public async Task<Asignacion_TecnicoResponseDTO?> CrearAsignacionTecnico(Asignacion_TecnicoDTO asignacionTecnicoDTO)
        {
            try
            {
                // 0️⃣ Reusar o crear la asignación principal
                var asignacionExistente = await _asignacionRepository.ObtenerAsignacionActivaPorSolicitud(asignacionTecnicoDTO.idSolicitud);
                if (asignacionExistente != null)
                    asignacionTecnicoDTO.idAsignacion = asignacionExistente.idAsignacion;
                // (opcional: crear nueva asignación si no existiera…)

                // 1️⃣ Verificar que la asignación existe
                var asignacion = await _asignacionRepository.ConsultarAsignacionPorId(asignacionTecnicoDTO.idAsignacion);
                if (asignacion == null)
                    throw new Exception("La asignación no existe.");

                // 2️⃣ Cargar **todos** los técnicos de la asignación
                var todosTecnicos = (await _repository.ConsultarTecnicosPorAsignacion(asignacionTecnicoDTO.idAsignacion)).ToList();

                // 3️⃣ ¿Este técnico ya está registrado?
                var tecnicoExistente = todosTecnicos
                    .FirstOrDefault(t => t.idEmpleado == asignacionTecnicoDTO.idEmpleado);

                Asignacion_Tecnico entidadFinal;


                if (tecnicoExistente != null)
                {
                    // Si ya existe el registro del técnico, se actualiza (reanuda)
                    // (A) Se verifica si ya hay otro técnico activo
                    bool hayOtroTecnicoActivo = await _repository.ConsultarTecnicosActivosPorAsignacion(asignacionTecnicoDTO.idAsignacion);
                    if (!hayOtroTecnicoActivo)
                    {
                        tecnicoExistente.esTecnicoActivo = true;
                        // Si la asignación no está en "En Proceso Técnico", se actualiza
                        if (asignacion.idStatusAsignacion != 1)
                        {

                            // ← Acumular pausa sistema --> Agregado para Pausa Por Sistema
                            //AcumularPausaSistema(asignacion);

                            asignacion.idStatusAsignacion = 1;
                            await _asignacionRepository.ActualizarAsignacion(asignacion.idAsignacion, asignacion);
                            await _solicitudService.ActualizarStatusOrden(asignacion.Solicitud.idSolicitud, 2);
                        }
                    }
                    else
                    {
                        tecnicoExistente.esTecnicoActivo = false;
                    }

                    // Actualizar la hora de reanudación
                    tecnicoExistente.horaInicio = DateTime.Now;
                    tecnicoExistente.comentarioPausa = "N/A";

                    await _repository.ActualizarTecnicoEnAsignacion(tecnicoExistente);
                    entidadFinal = tecnicoExistente;
                }
                else
                {
                    // ─── Crear un técnico nuevo ───

                    // 4.a) Validar límite de dos técnicos **solo** para nuevos
                    if (todosTecnicos.Count(t => !t.retirado) >= 2)
                        throw new InvalidOperationException("Ya se han asignado los dos técnicos permitidos para esta solicitud.");

                    // 4.b) Decidir quién va activo
                    bool hayActivo = todosTecnicos.Any(t => t.esTecnicoActivo);
                    asignacionTecnicoDTO.esTecnicoActivo = !hayActivo;

                    // 4.c) Si va activo, desactivar a cualquiera que quedara activo por cierre inesperado
                    if (asignacionTecnicoDTO.esTecnicoActivo)
                    {
                        foreach (var otro in todosTecnicos.Where(t => t.esTecnicoActivo))
                        {
                            otro.esTecnicoActivo = false;
                            await _repository.ActualizarTecnicoEnAsignacion(otro);
                        }

                        // Actualizar estado de la orden
                        await _solicitudService.ActualizarStatusOrden(asignacion.Solicitud.idSolicitud, 2);
                        if (asignacion.idStatusAsignacion != 1)
                        {
                            asignacion.idStatusAsignacion = 1;
                            await _asignacionRepository.ActualizarAsignacion(asignacion.idAsignacion, asignacion);
                        }
                    }

                    var nuevaEntidad = new Asignacion_Tecnico
                    {
                        idAsignacion = asignacionTecnicoDTO.idAsignacion,
                        idEmpleado = asignacionTecnicoDTO.idEmpleado,
                        horaInicio = DateTime.Now,
                        horaTermino = asignacionTecnicoDTO.horaTermino,
                        solucion = "N/A",
                        idStatusAprobacionTecnico = 3,
                        comentarioPausa = "N/A",
                        esTecnicoActivo = asignacionTecnicoDTO.esTecnicoActivo,
                        retirado = false
                    };

                    entidadFinal = await _repository.CrearAsignacionTecnico(nuevaEntidad);
                }

                // 5️⃣ Mapear al DTO de respuesta
                return new Asignacion_TecnicoResponseDTO
                {
                    idAsignacionTecnico = entidadFinal.idAsignacionTecnico,
                    idAsignacion = entidadFinal.idAsignacion,
                    idEmpleado = entidadFinal.idEmpleado,
                    idStatusAprobacionTecnico = entidadFinal.idStatusAprobacionTecnico,
                    horaInicio = entidadFinal.horaInicio,
                    horaTermino = entidadFinal.horaTermino,
                    solucion = entidadFinal.solucion,
                    comentarioPausa = entidadFinal.comentarioPausa,
                    esTecnicoActivo = entidadFinal.esTecnicoActivo,
                    tiempoAcumuladoMinutos = entidadFinal.tiempoAcumuladoMinutos
                };
            }
            catch
            {
                // Déjalo que el controlador capture y devuelva el código HTTP apropiado
                throw;
            }
        }



        public async Task<Asignacion_TecnicoFinalizacionResponseDTO> FinalizarAsignacionTecnico(Asignacion_TecnicoFinalizacionDTO asignacionTecnicoFinalizacionDTO)
        {
            // Validar si la asignación existe
            var asignacion = await _asignacionRepository.ConsultarAsignacionPorId(asignacionTecnicoFinalizacionDTO.idAsignacion);
            if (asignacion == null)
            {
                throw new ArgumentException("La asignación no existe.");
            }

            // Validar si la solicitud asociada a la asignación existe
            if (asignacion.Solicitud == null)
            {
                throw new ArgumentException("La solicitud asociada a la asignación no está disponible.");
            }

            // Validar si idMaquina e idAreaSeleccionada tienen valores válidos
            if (asignacion.Solicitud.idMaquina == 0 || asignacion.Solicitud.idAreaSeleccionada == 0)
            {
                throw new ArgumentException("La solicitud no tiene un idMaquina o idAreaSeleccionada válido.");
            }

            // Validar si el técnico existe en la asignación (usando idAsignacion + idEmpleado)
            var tecnico = await _repository.ConsultarTecnicoPorAsignacionYEmpleado(
                asignacionTecnicoFinalizacionDTO.idAsignacion,
                asignacionTecnicoFinalizacionDTO.idEmpleado
            );

            if (tecnico == null)
            {
                throw new ArgumentException("El técnico no está asignado a esta tarea.");
            }

            // Validar si el empleado asociado al técnico existe
            if (tecnico.Empleado == null)
            {
                throw new ArgumentException("El empleado asociado al técnico no está disponible.");
            }

            // 1. Cerrar su intervalo de trabajo
            double minutosTrabajo = (DateTime.Now - tecnico.horaInicio).TotalMinutes;
            tecnico.tiempoAcumuladoMinutos += minutosTrabajo;
            tecnico.horaTermino = DateTime.Now;
            tecnico.solucion = asignacionTecnicoFinalizacionDTO.solucion;
            tecnico.idStatusAprobacionTecnico = 1;
            tecnico.esTecnicoActivo = false;

            //Agregado para el paro de máquina por parte del Tecnico
            tecnico.paroMaquinaTecnico = asignacionTecnicoFinalizacionDTO.paroMaquinaTecnico;

            await _repository.ActualizarTecnicoEnAsignacion(tecnico);

            // Descontar las refacciones utilizadas en la asignación
            var refaccionesUtilizadas = await _asignacionRefaccionesRepository.ConsultarRefaccionesPorAsignacion(asignacionTecnicoFinalizacionDTO.idAsignacion);
            foreach (var refaccion in refaccionesUtilizadas)
            {
                await _inventarioRepository.ActualizarCantidadInventario(refaccion.idRefaccion, refaccion.cantidad);
            }

            // Verificar si quedan técnicos activos en la asignación
            var hayTecnicosActivos = await _repository.ConsultarTecnicosActivosPorAsignacion(tecnico.idAsignacion);
            if (!hayTecnicosActivos)
            {
                // Si no hay técnicos activos, marcar la asignación como finalizada
                //asignacion.idStatusAsignacion = 3; // Estado "Finalizada"
                var asignacionActualizada = await _asignacionRepository.ActualizarAsignacion(asignacion.idAsignacion, asignacion);

                if (asignacionActualizada == null)
                {
                    throw new ArgumentException("No se pudo actualizar la asignación porque no existe.");
                }

                await _solicitudService.ActualizarStatusOrden(asignacion.Solicitud.idSolicitud, 4); // "Esperando Validacion"

                //return tecnico; // Devuelve el técnico actualizado en lugar de la asignación
            }

            // Aquí es el momento adecuado para calcular y guardar los KPIs
            // Se asume que GuardarKPIs es un método disponible (por ejemplo, en un servicio de KPIs o en AsignacionService)
            await asignacionService.GuardarKPIs(asignacion.Solicitud.idMaquina,
                              asignacion.Solicitud.idAreaSeleccionada,
                              asignacionTecnicoFinalizacionDTO.idEmpleado);

            // Mapear la entidad a un DTO de respuesta
            var response = new Asignacion_TecnicoFinalizacionResponseDTO
            {
                idAsignacionTecnico = tecnico.idAsignacionTecnico,
                idAsignacion = tecnico.idAsignacion,
                idEmpleado = tecnico.idEmpleado,
                nombreCompletoTecnico = $"{tecnico.Empleado.nombre} {tecnico.Empleado.apellidoPaterno} {tecnico.Empleado.apellidoMaterno}",
                horaInicio = tecnico.horaInicio,
                horaTermino = tecnico.horaTermino,
                solucion = tecnico.solucion,
                idStatusAprobacionTecnico = tecnico.idStatusAprobacionTecnico,
                nombreStatusAprobacionTecnico = tecnico.StatusAprobacionTecnico?.descripcionStatusAprobacionTecnico,
                esTecnicoActivo = tecnico.esTecnicoActivo,

                // ← Devuelve el valor guardado para que el frontend lo confirme si es necesario
                paroMaquinaTecnico = tecnico.paroMaquinaTecnico
            };

            return response;
        }

        /*public async Task<bool> PausarAsignacion(int idAsignacion, int idTecnicoQuePausa, string comentarioPausa)
        {
            // Validar que la asignación exista
            var asignacion = await _asignacionRepository.ConsultarAsignacionPorId(idAsignacion);
            if (asignacion == null)
            {
                throw new ArgumentException("La asignación no existe.");
            }

            // Validar que el técnico que pausa esté asignado a esta asignación
            var tecnicoQuePausa = await _repository.ConsultarTecnicoPorAsignacionYEmpleado(idAsignacion, idTecnicoQuePausa);
            if (tecnicoQuePausa == null)
            {
                throw new ArgumentException("El técnico no está asignado a esta asignación.");
            }

            // Validar que el técnico que pausa sea el técnico activo
            if (!tecnicoQuePausa.esTecnicoActivo)
            {
                throw new InvalidOperationException("Solo el técnico activo puede pausar la asignación.");
            }

            // Acumular tiempo efectivo del técnico
            double minutosTrabajo = (DateTime.Now - tecnicoQuePausa.horaInicio).TotalMinutes;
            tecnicoQuePausa.tiempoAcumuladoMinutos += minutosTrabajo;
            tecnicoQuePausa.horaTermino = DateTime.Now;  // Indica pausa
                                                         // Se marca como inactivo (pausa), pero NO se marca como retirado (queda en la asignación)
            tecnicoQuePausa.esTecnicoActivo = false;
            tecnicoQuePausa.comentarioPausa = comentarioPausa;
            await _repository.ActualizarTecnicoEnAsignacion(tecnicoQuePausa);

            // Cambiar la asignación a "Pausa" y marcar el tiempo en que queda sin técnico
            asignacion.idStatusAsignacion = 2; // "Pausa"
            asignacion.ultimaVezSinTecnico = DateTime.Now; // Inicia espera
            await _asignacionRepository.ActualizarAsignacion(idAsignacion, asignacion);

            // Buscar otro técnico asignado a la misma asignación para convertirlo en activo,
            // excluyendo al que está pausando y a aquellos marcados como retirados.
            var todosTecnicos = await _repository.ConsultarTecnicosPorAsignacion(idAsignacion);
            var candidatos = todosTecnicos
                                .Where(t => t.idEmpleado != idTecnicoQuePausa && !t.retirado && t.esTecnicoActivo == false)
                                .ToList();

            // Ordenar los candidatos para seleccionar al que se unió más recientemente
            var siguienteTecnicoActivo = candidatos
                                            .OrderByDescending(t => t.horaInicio)
                                            .FirstOrDefault();

            if (siguienteTecnicoActivo != null)
            {
                // Convertir al siguiente técnico en activo
                siguienteTecnicoActivo.esTecnicoActivo = true;
                await _repository.ActualizarTecnicoEnAsignacion(siguienteTecnicoActivo);

                // Transferir las refacciones asignadas al técnico que pausó
                var refacciones = await _asignacionRefaccionesRepository.ConsultarRefaccionesPorAsignacion(idAsignacion);
                foreach (var refaccion in refacciones)
                {
                    if (refaccion.idAsignacionTecnico == tecnicoQuePausa.idAsignacionTecnico)
                    {
                        refaccion.idAsignacionTecnico = siguienteTecnicoActivo.idAsignacionTecnico;
                        await _asignacionRefaccionesRepository.ActualizarRefaccionEnAsignacion(refaccion);
                    }
                }

                // Notificar al grupo de la asignación con el nuevo técnico activo
                await _hubContext.Clients.Group(idAsignacion.ToString())
                    .SendAsync("TecnicoActivoChanged", siguienteTecnicoActivo.idEmpleado);
            }
            else
            {
                // Si no hay otro técnico, la asignación queda en pausa y se notifica que no hay técnico activo
                await _solicitudService.ActualizarStatusOrden(asignacion.Solicitud.idSolicitud, 5); // "Pausa" o el estado correspondiente
                await _hubContext.Clients.Group(idAsignacion.ToString())
                    .SendAsync("TecnicoActivoChanged", 0);
            }

            return true;
        }*/

        public async Task<bool> PausarAsignacion(int idAsignacion, int idTecnicoQuePausa, string comentarioPausa)
        {
            // Validar que la asignación exista
            var asignacion = await _asignacionRepository.ConsultarAsignacionPorId(idAsignacion);
            if (asignacion == null)
            {
                throw new ArgumentException("La asignación no existe.");
            }

            // Validar que el técnico que pausa esté asignado a esta asignación
            var tecnicoQuePausa = await _repository.ConsultarTecnicoPorAsignacionYEmpleado(idAsignacion, idTecnicoQuePausa);
            if (tecnicoQuePausa == null)
            {
                throw new ArgumentException("El técnico no está asignado a esta asignación.");
            }

            // Validar que el técnico que pausa sea el técnico activo
            if (!tecnicoQuePausa.esTecnicoActivo)
            {
                throw new InvalidOperationException("Solo el técnico activo puede pausar la asignación.");
            }

            // Acumular tiempo efectivo del técnico
            double minutosTrabajo = (DateTime.Now - tecnicoQuePausa.horaInicio).TotalMinutes;
            tecnicoQuePausa.tiempoAcumuladoMinutos += minutosTrabajo;
            tecnicoQuePausa.horaTermino = DateTime.Now;  // Indica pausa

            // Marcar como inactivo y retirado ya que al pausar realmente se retira para tomar otra asignación
            tecnicoQuePausa.esTecnicoActivo = false;
            tecnicoQuePausa.retirado = true; // <-- AQUÍ se marca como retirado
            tecnicoQuePausa.comentarioPausa = comentarioPausa;
            await _repository.ActualizarTecnicoEnAsignacion(tecnicoQuePausa);

            // Cambiar la asignación a "Pausa" y marcar el tiempo en que queda sin técnico
            asignacion.idStatusAsignacion = 2; // "Pausa"
            asignacion.ultimaVezSinTecnico = DateTime.Now; // Inicia espera
            await _asignacionRepository.ActualizarAsignacion(idAsignacion, asignacion);

            // Buscar otro técnico asignado a la misma asignación para convertirlo en activo,
            // excluyendo al que está pausando y a aquellos marcados como retirados.
            var todosTecnicos = await _repository.ConsultarTecnicosPorAsignacion(idAsignacion);
            var candidatos = todosTecnicos
                                .Where(t => t.idEmpleado != idTecnicoQuePausa && !t.retirado && t.esTecnicoActivo == false)
                                .ToList();

            // Ordenar los candidatos para seleccionar al que se unió más recientemente
            var siguienteTecnicoActivo = candidatos
                                            .OrderByDescending(t => t.horaInicio)
                                            .FirstOrDefault();

            if (siguienteTecnicoActivo != null)
            {
                // Convertir al siguiente técnico en activo
                siguienteTecnicoActivo.esTecnicoActivo = true;
                await _repository.ActualizarTecnicoEnAsignacion(siguienteTecnicoActivo);

                // Transferir las refacciones asignadas al técnico que pausó
                var refacciones = await _asignacionRefaccionesRepository.ConsultarRefaccionesPorAsignacion(idAsignacion);
                foreach (var refaccion in refacciones)
                {
                    if (refaccion.idAsignacionTecnico == tecnicoQuePausa.idAsignacionTecnico)
                    {
                        refaccion.idAsignacionTecnico = siguienteTecnicoActivo.idAsignacionTecnico;
                        await _asignacionRefaccionesRepository.ActualizarRefaccionEnAsignacion(refaccion);
                    }
                }

                // Notificar al grupo de la asignación con el nuevo técnico activo
                await _hubContext.Clients.Group(idAsignacion.ToString())
                    .SendAsync("TecnicoActivoChanged", siguienteTecnicoActivo.idEmpleado);
            }
            else
            {
                // Si no hay otro técnico, la asignación queda en pausa y se notifica que no hay técnico activo
                await _solicitudService.ActualizarStatusOrden(asignacion.Solicitud.idSolicitud, 5); // "Pausa" o el estado correspondiente
                await _hubContext.Clients.Group(idAsignacion.ToString())
                    .SendAsync("TecnicoActivoChanged", 0);
            }

            return true;
        }



        /*public async Task<bool> RetirarTecnicoDeApoyo(int idAsignacion, int idTecnicoQueSeRetira, string comentarioRetiro)
        {
            // Validar que la asignación exista
            var asignacion = await _asignacionRepository.ConsultarAsignacionPorId(idAsignacion);
            if (asignacion == null)
            {
                throw new ArgumentException("La asignación no existe.");
            }

            // Validar que el técnico que se retira esté asignado a esta asignación
            var tecnicoQueSeRetira = await _repository.ConsultarTecnicoPorAsignacionYEmpleado(idAsignacion, idTecnicoQueSeRetira);
            if (tecnicoQueSeRetira == null)
            {
                throw new ArgumentException("El técnico no está asignado a esta asignación.");
            }

            // Validar que el técnico que se retira NO sea el técnico activo
            if (tecnicoQueSeRetira.esTecnicoActivo)
            {
                throw new InvalidOperationException("El técnico activo no puede retirarse usando este método. Use el método de pausar asignación.");
            }

            // Cerrar su intervalo de trabajo
            double minutosTrabajo = (DateTime.Now - tecnicoQueSeRetira.horaInicio).TotalMinutes;
            tecnicoQueSeRetira.tiempoAcumuladoMinutos += minutosTrabajo;
            tecnicoQueSeRetira.horaTermino = DateTime.Now;

            // Agregar el comentario de retiro al técnico que se retira
            tecnicoQueSeRetira.comentarioPausa = comentarioRetiro;
            await _repository.ActualizarTecnicoEnAsignacion(tecnicoQueSeRetira);

            // Marcar al técnico que se retira como inactivo
            tecnicoQueSeRetira.esTecnicoActivo = false;
            await _repository.ActualizarTecnicoEnAsignacion(tecnicoQueSeRetira);

            // No se transfieren refacciones, ya que el técnico no activo no puede registrar refacciones
            // (Las refacciones asignadas al técnico que se retira permanecen asociadas a él, pero no se transfieren)

            return true;
        }*/

        public async Task<bool> RetirarTecnicoDeApoyo(int idAsignacion, int idTecnicoQueSeRetira, string comentarioRetiro)
        {
            // Validar que la asignación exista
            var asignacion = await _asignacionRepository.ConsultarAsignacionPorId(idAsignacion);
            if (asignacion == null)
            {
                throw new ArgumentException("La asignación no existe.");
            }

            // Validar que el técnico que se retira esté asignado a esta asignación
            var tecnicoQueSeRetira = await _repository.ConsultarTecnicoPorAsignacionYEmpleado(idAsignacion, idTecnicoQueSeRetira);
            if (tecnicoQueSeRetira == null)
            {
                throw new ArgumentException("El técnico no está asignado a esta asignación.");
            }

            // Validar que el técnico que se retira NO sea el técnico activo
            if (tecnicoQueSeRetira.esTecnicoActivo)
            {
                throw new InvalidOperationException("El técnico activo no puede retirarse usando este método. Use el método de pausar asignación.");
            }

            // Cerrar su intervalo de trabajo
            double minutosTrabajo = (DateTime.Now - tecnicoQueSeRetira.horaInicio).TotalMinutes;
            tecnicoQueSeRetira.tiempoAcumuladoMinutos += minutosTrabajo;
            tecnicoQueSeRetira.horaTermino = DateTime.Now;

            // Marcar al técnico como retirado e inactivo
            tecnicoQueSeRetira.retirado = true;
            tecnicoQueSeRetira.esTecnicoActivo = false;
            tecnicoQueSeRetira.comentarioPausa = comentarioRetiro;

            // Una sola actualización para todos los cambios
            await _repository.ActualizarTecnicoEnAsignacion(tecnicoQueSeRetira);

            // No se transfieren refacciones, ya que el técnico no activo no puede registrar refacciones
            // (Las refacciones asignadas al técnico que se retira permanecen asociadas a él, pero no se transfieren)

            return true;
        }


        public async Task<IEnumerable<Asignacion_Tecnico>> ConsultarTodosLosTecnicos()
        {
            return await _repository.ConsultarTodosLosTecnicos();
        }

        public async Task<bool> EliminarTecnicoDeAsignacion(int idAsignacionTecnico)
        {
            return await _repository.EliminarTecnicoDeAsignacion(idAsignacionTecnico);
        }

        public async Task<bool> ActualizarTecnicoEnAsignacion(Asignacion_TecnicoDTO asignacionTecnicoDTO)
        {
            // 1️⃣ Validar si la asignación existe
            var asignacion = await _asignacionRepository.ConsultarAsignacionPorId(asignacionTecnicoDTO.idAsignacion);
            if (asignacion == null)
            {
                throw new ArgumentException("La asignación no existe.");
            }
            var asignacionTecnico = new Asignacion_Tecnico
            {
                idAsignacion = asignacionTecnicoDTO.idAsignacion,
                idEmpleado = asignacionTecnicoDTO.idEmpleado,
                horaInicio = asignacionTecnicoDTO.horaInicio,
                horaTermino = asignacionTecnicoDTO.horaTermino,
                solucion = asignacionTecnicoDTO.solucion,
                idStatusAprobacionTecnico = asignacionTecnicoDTO.idStatusAprobacionTecnico,
                comentarioPausa = asignacionTecnicoDTO.comentarioPausa,
                esTecnicoActivo = asignacionTecnicoDTO.esTecnicoActivo
            };
            return await _repository.ActualizarTecnicoEnAsignacion(asignacionTecnico);
        }

        public async Task<IEnumerable<Asignacion_TecnicoDetallesDTO>> ConsultarTecnicosConDetallesPorAsignacion(int idAsignacion)
        {

            // Validar existencia de la asignación
            if (!(await _asignacionRepository.ConsultarAsignacionPorId(idAsignacion) != null))
            {
                throw new ArgumentException("La asignación no existe.");
            }

            // Consultar técnicos relacionados
            var tecnicos = await _repository.ConsultarTecnicosPorAsignacion(idAsignacion);
            if (!tecnicos.Any())
            {
                return Enumerable.Empty<Asignacion_TecnicoDetallesDTO>();
            }

            // Mapear a DTO's
            var tecnicoDetalles = tecnicos.Select(tecnico => new Asignacion_TecnicoDetallesDTO
            {
                idAsignacionTecnico = tecnico.idAsignacionTecnico,
                idAsignacion = tecnico.idAsignacion,
                idEmpleado = tecnico.idEmpleado,
                nombreCompletoTecnico = $"{tecnico.Empleado?.nombre ?? "N/A"} {tecnico.Empleado?.apellidoPaterno ?? ""} {tecnico.Empleado?.apellidoMaterno ?? ""}",
                horaInicio = tecnico.horaInicio,
                horaTermino = tecnico.horaTermino,
                solucion = tecnico.solucion,
                idStatusAprobacionTecnico = tecnico.idStatusAprobacionTecnico,
                nombreStatusAprobacionTecnico = tecnico.StatusAprobacionTecnico?.descripcionStatusAprobacionTecnico ?? "N/A",
                comentarioPausa = tecnico.comentarioPausa,
                esTecnicoActivo = tecnico.esTecnicoActivo,
                Refacciones = tecnico.Asignacion_Refacciones.Select(refaccion => new Asignacion_RefaccionesDetallesDTO
                {
                    idAsignacionRefaccion = refaccion.idAsignacionRefaccion,
                    idAsignacionTecnico = refaccion.idAsignacionTecnico,
                    idRefaccion = refaccion.idRefaccion,
                    nombreRefaccion = refaccion.Inventario?.nombreProducto ?? "N/A",
                    cantidad = refaccion.cantidad
                }).ToList() ?? new List<Asignacion_RefaccionesDetallesDTO>()
            });
            return tecnicoDetalles;
        }


        public async Task<IEnumerable<SolicitudesDetalleDTO>> ConsultarSolicitudesPausadasPorTecnico(int idTecnico)
        {
            var asignacionesPausadas = await _repository.ObtenerAsignacionesPausadasPorTecnico(idTecnico);
            var solicitudesDetalleDTO = new List<SolicitudesDetalleDTO>();

            foreach (var asignacion in asignacionesPausadas)
            {
                var solicitud = asignacion.Solicitud;
                var empleado = solicitud.Empleado;

                var solicitudDetalleDTO = new SolicitudesDetalleDTO
                {
                    idSolicitud = solicitud.idSolicitud,
                    idAsignacion = asignacion.idAsignacion, // 🔥 Asignamos el ID de la asignación aquí
                    descripcion = solicitud.descripcion,
                    fechaSolicitud = solicitud.fechaSolicitud,
                    nombreCompletoEmpleado = $"{empleado.nombre} {empleado.apellidoPaterno} {empleado.apellidoMaterno}",
                    idMaquina = solicitud.idMaquina,
                    idTurno = solicitud.idTurno,
                    idStatusOrden = solicitud.idStatusOrden,
                    idStatusAprobacionSolicitante = solicitud.idStatusAprobacionSolicitante,
                    area = solicitud.idAreaSeleccionada.ToString(), // Ajustar según sea necesario
                    rol = solicitud.idRolSeleccionado.ToString(), // Ajustar según sea necesario
                    idCategoriaTicket = solicitud.idCategoriaTicket,
                    nombreMaquina = solicitud.Maquina?.nombreMaquina ?? "No disponible",
                    nombreTurno = solicitud.Turno?.descripcion ?? "No disponible",
                    nombreStatusOrden = solicitud.StatusOrden?.descripcionStatusOrden ?? "No disponible",
                    nombreStatusAprobacionSolicitante = solicitud.StatusAprobacionSolicitante?.descripcionStatusAprobacionSolicitante ?? "No disponible",
                    nombreCategoriaTicket = solicitud.categoriaTicket?.descripcionCategoriaTicket ?? "No disponible"
                };

                solicitudesDetalleDTO.Add(solicitudDetalleDTO);
            }

            return solicitudesDetalleDTO;
        }


        public async Task<bool> RetomarAsignacion(int idAsignacion, int idEmpleado)
        {
            return await _repository.RetomarAsignacion(idAsignacion, idEmpleado);
        }

        private async Task ActualizarTiempoEsperaAcumulado(Asignaciones asignacion)
        {
            if (asignacion.ultimaVezSinTecnico.HasValue)
            {
                // Calcular el tiempo que ha estado sin técnico (en pausa)
                var tiempoEspera = (DateTime.Now - asignacion.ultimaVezSinTecnico.Value).TotalMinutes;

                // Acumular este tiempo en la asignación
                asignacion.tiempoEsperaAcumuladoMinutos += tiempoEspera;

                // Resetear la marca de última vez sin técnico
                asignacion.ultimaVezSinTecnico = null;

                // Actualizar la asignación en la base de datos
                await _asignacionRepository.ActualizarAsignacion(asignacion.idAsignacion, asignacion);
            }
        }

        /*void AcumularPausaSistema(Asignaciones asignacion)
        {
            if (asignacion.idStatusAsignacion == 6
                && asignacion.ultimaVezSinTecnico.HasValue)
            {
                asignacion.tiempoEsperaAcumuladoMinutos +=
                  (DateTime.Now - asignacion.ultimaVezSinTecnico.Value).TotalMinutes;
                asignacion.ultimaVezSinTecnico = null;
            }
        }*/

    }
}
