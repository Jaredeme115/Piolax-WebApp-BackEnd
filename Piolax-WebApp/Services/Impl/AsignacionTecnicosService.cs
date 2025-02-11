using Microsoft.Extensions.FileProviders;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Repositories.Impl;
using System.Configuration;

namespace Piolax_WebApp.Services.Impl
{
    public class AsignacionTecnicosService(
        IAsignacionTecnicosRepository repository, 
        IAsignacionRepository asignacionRepository, 
        IAsignacionRefaccionesRepository asignacionRefaccionesRepository,
        IInventarioRepository inventarioRepository,
        IKPIRepository kpiRepository,
        IAsignacionService asignacionService) : IAsignacionTecnicosService
    {
        private readonly IAsignacionTecnicosRepository _repository = repository;
        private readonly IAsignacionRepository _asignacionRepository = asignacionRepository;
        private readonly IAsignacionRefaccionesRepository _asignacionRefaccionesRepository = asignacionRefaccionesRepository;
        private readonly IInventarioRepository _inventarioRepository = inventarioRepository;
        private readonly IKPIRepository _kpiRepository = kpiRepository;
        private readonly IAsignacionService _asignacionService = asignacionService;


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


        /*public async Task<Asignacion_TecnicoResponseDTO?> CrearAsignacionTecnico(Asignacion_TecnicoDTO asignacionTecnicoDTO)
        {
            try
            {
                // 1. Verifica que la asignación exista
                var asignacion = await _asignacionRepository.ConsultarAsignacionPorId(asignacionTecnicoDTO.idAsignacion);
                if (asignacion == null)
                    throw new Exception("La asignación no existe.");

                // 2. Busca si este técnico ya existe en la asignación (mismo idAsignacion + idEmpleado)
                var tecnicoExistente = await _repository.ConsultarTecnicoPorAsignacionYEmpleado(
                    asignacionTecnicoDTO.idAsignacion,
                    asignacionTecnicoDTO.idEmpleado
                );

                // 3. Si YA existe un técnico para esta asignación y empleado, entonces lo "reanudas"
                if (tecnicoExistente != null)
                {
                    // --- REUTILIZAR LA MISMA FILA ---

                    // (A) Revisar si ya hay otro técnico activo (que no sea él)
                    bool hayOtroTecnicoActivo = await _repository.ConsultarTecnicosActivosPorAsignacion(asignacionTecnicoDTO.idAsignacion);

                    // (B) Si no hay otro técnico activo, este será el activo.
                    //     Si sí hay otro, lo creamos como inactivo.
                    if (!hayOtroTecnicoActivo)
                    {
                        tecnicoExistente.esTecnicoActivo = true;
                        // Cambiar la asignación a "En Proceso Técnico" si no lo está
                        if (asignacion.idStatusAsignacion != 1) // 1 = "En Proceso Técnico"
                        {
                            asignacion.idStatusAsignacion = 1;
                            await _asignacionRepository.ActualizarAsignacion(asignacion.idAsignacion, asignacion);
                        }
                    }
                    else
                    {
                        tecnicoExistente.esTecnicoActivo = false;
                    }

                    // (C) Actualizar la hora de reanudación (cerrar el intervalo anterior si fuera necesario)
                    //     Como estamos reanudando, ponemos un nuevo "horaInicio".
                    //     Si antes lo habíamos pausado, su intervalo anterior ya debería haberse cerrado.
                    tecnicoExistente.horaInicio = DateTime.Now;
                    tecnicoExistente.comentarioPausa = "N/A"; // limpiar el comentario de pausa si deseas
                                                              // horaTermino queda como estaba hasta que se finalice o pause otra vez

                    // (D) Actualizar al técnico en la BD
                    await _repository.ActualizarTecnicoEnAsignacion(tecnicoExistente);

                    return tecnicoExistente;
                }
                else
                {
                    // --- CREAR UN NUEVO REGISTRO ---
                    // Validar si ya hay un técnico activo en la asignación
                    bool hayTecnicoActivo = await _repository.ConsultarTecnicosActivosPorAsignacion(asignacionTecnicoDTO.idAsignacion);
                    if (hayTecnicoActivo)
                    {
                        // Si ya hay un técnico activo, este nuevo será inactivo
                        asignacionTecnicoDTO.esTecnicoActivo = false;
                    }
                    else
                    {
                        // Si no hay técnico activo, este nuevo técnico será el activo
                        asignacionTecnicoDTO.esTecnicoActivo = true;

                        // Cambiamos la asignación a "En Proceso Técnico"
                        if (asignacion.idStatusAsignacion != 1) // 1 = "En Proceso Técnico"
                        {
                            asignacion.idStatusAsignacion = 1;
                            await _asignacionRepository.ActualizarAsignacion(asignacion.idAsignacion, asignacion);
                        }
                    }

                    // Crear la entidad Asignacion_Tecnico
                    var asignacionTecnico = new Asignacion_TecnicoResponseDTO
                    {
                        idAsignacion = asignacionTecnicoDTO.idAsignacion,
                        idEmpleado = asignacionTecnicoDTO.idEmpleado,
                        horaInicio = DateTime.Now,
                        horaTermino = asignacionTecnicoDTO.horaTermino,
                        solucion = "N/A", // Valor por defecto
                        idStatusAprobacionTecnico = 3, // Valor = Pendiente
                        comentarioPausa = "N/A",
                        esTecnicoActivo = asignacionTecnicoDTO.esTecnicoActivo
                    };

                    // Guardar la asignación del técnico en la base de datos
                    return await _repository.CrearAsignacionTecnico(asignacionTecnico);
                }
            }
            catch (Exception ex)
            {
                // Loggear el error
                Console.WriteLine($"Error al crear la asignación del técnico: {ex.Message}");
                throw;
            }
        }*/

        public async Task<Asignacion_TecnicoResponseDTO?> CrearAsignacionTecnico(Asignacion_TecnicoDTO asignacionTecnicoDTO)
        {
            try
            {
                // 1. Verifica que la asignación exista
                var asignacion = await _asignacionRepository.ConsultarAsignacionPorId(asignacionTecnicoDTO.idAsignacion);
                if (asignacion == null)
                    throw new Exception("La asignación no existe.");

                // 2. Busca si este técnico ya existe en la asignación (mismo idAsignacion + idEmpleado)
                var tecnicoExistente = await _repository.ConsultarTecnicoPorAsignacionYEmpleado(
                    asignacionTecnicoDTO.idAsignacion,
                    asignacionTecnicoDTO.idEmpleado
                );

                Asignacion_Tecnico entidadFinal; // guardaremos aquí la entidad resultante

                // 3. Si YA existe un técnico para esta asignación y empleado, entonces lo "reanudas"
                if (tecnicoExistente != null)
                {
                    // --- REUTILIZAR LA MISMA FILA ---

                    // (A) Revisar si ya hay otro técnico activo (que no sea él)
                    bool hayOtroTecnicoActivo = await _repository.ConsultarTecnicosActivosPorAsignacion(asignacionTecnicoDTO.idAsignacion);

                    // (B) Definir si este técnico será activo o no
                    if (!hayOtroTecnicoActivo)
                    {
                        tecnicoExistente.esTecnicoActivo = true;
                        // Cambiar la asignación a "En Proceso Técnico" si no lo está
                        if (asignacion.idStatusAsignacion != 1) // 1 = "En Proceso Técnico"
                        {
                            asignacion.idStatusAsignacion = 1;
                            await _asignacionRepository.ActualizarAsignacion(asignacion.idAsignacion, asignacion);
                        }
                    }
                    else
                    {
                        tecnicoExistente.esTecnicoActivo = false;
                    }

                    // (C) Actualizar la hora de reanudación
                    tecnicoExistente.horaInicio = DateTime.Now;
                    tecnicoExistente.comentarioPausa = "N/A"; // limpiar el comentario de pausa

                    // (D) Guardar cambios
                    await _repository.ActualizarTecnicoEnAsignacion(tecnicoExistente);

                    // Usaremos esta entidad para mapear el DTO final
                    entidadFinal = tecnicoExistente;
                }
                else
                {
                    // --- CREAR UN NUEVO REGISTRO ---
                    bool hayTecnicoActivo = await _repository.ConsultarTecnicosActivosPorAsignacion(asignacionTecnicoDTO.idAsignacion);
                    if (hayTecnicoActivo)
                    {
                        asignacionTecnicoDTO.esTecnicoActivo = false;
                    }
                    else
                    {
                        asignacionTecnicoDTO.esTecnicoActivo = true;

                        // Cambiamos la asignación a "En Proceso Técnico" si no lo está
                        if (asignacion.idStatusAsignacion != 1) // 1 = "En Proceso Técnico"
                        {
                            asignacion.idStatusAsignacion = 1;
                            await _asignacionRepository.ActualizarAsignacion(asignacion.idAsignacion, asignacion);
                        }
                    }

                    // Crear la entidad Asignacion_Tecnico
                    var nuevaEntidad = new Asignacion_Tecnico
                    {
                        idAsignacion = asignacionTecnicoDTO.idAsignacion,
                        idEmpleado = asignacionTecnicoDTO.idEmpleado,
                        horaInicio = DateTime.Now,
                        horaTermino = asignacionTecnicoDTO.horaTermino,
                        solucion = "N/A", // Valor por defecto
                        idStatusAprobacionTecnico = 3, // Valor = Pendiente
                        comentarioPausa = "N/A",
                        esTecnicoActivo = asignacionTecnicoDTO.esTecnicoActivo
                    };

                    // Guardar la asignación del técnico en la base de datos
                    var entidadCreada = await _repository.CrearAsignacionTecnico(nuevaEntidad);
                    entidadFinal = entidadCreada;
                }

                // 4. Mapear la entidad final a un DTO "Asignacion_TecnicoResponseDTO"
                var responseDto = new Asignacion_TecnicoResponseDTO
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

                return responseDto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear la asignación del técnico: {ex.Message}");
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
                asignacion.idStatusAsignacion = 3; // Estado "Finalizada"
                var asignacionActualizada = await _asignacionRepository.ActualizarAsignacion(asignacion.idAsignacion, asignacion);

                if (asignacionActualizada == null)
                {
                    throw new ArgumentException("No se pudo actualizar la asignación porque no existe.");
                }

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
                esTecnicoActivo = tecnico.esTecnicoActivo
            };

            return response;
        }

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
            tecnicoQuePausa.esTecnicoActivo = false;
            tecnicoQuePausa.comentarioPausa = comentarioPausa;
            await _repository.ActualizarTecnicoEnAsignacion(tecnicoQuePausa);

            // Cambiar la asignación a "pausa" y marcar el tiempo en que queda sin técnico
            asignacion.idStatusAsignacion = 2; // "Pausa"
            asignacion.ultimaVezSinTecnico = DateTime.Now; // Inicia espera
            await _asignacionRepository.ActualizarAsignacion(idAsignacion, asignacion);

            // Marcar al técnico que pausa como inactivo y agregar el comentario de pausa
            tecnicoQuePausa.esTecnicoActivo = false;
            tecnicoQuePausa.comentarioPausa = comentarioPausa;
            await _repository.ActualizarTecnicoEnAsignacion(tecnicoQuePausa);

            // Buscar otro técnico asignado a la misma asignación para convertirlo en activo
            var otrosTecnicos = await _repository.ConsultarTecnicosPorAsignacion(idAsignacion);
            var siguienteTecnicoActivo = otrosTecnicos.FirstOrDefault(t => t.idEmpleado != idTecnicoQuePausa && t.esTecnicoActivo == false);

            if (siguienteTecnicoActivo != null)
            {
                // Convertir al siguiente técnico en activo
                siguienteTecnicoActivo.esTecnicoActivo = true;
                await _repository.ActualizarTecnicoEnAsignacion(siguienteTecnicoActivo);

                // Transferir las refacciones seleccionadas al siguiente técnico activo
                var refacciones = await _asignacionRefaccionesRepository.ConsultarRefaccionesPorAsignacion(idAsignacion);
                foreach (var refaccion in refacciones)
                {
                    if (refaccion.idAsignacionTecnico == tecnicoQuePausa.idAsignacionTecnico)
                    {
                        refaccion.idAsignacionTecnico = siguienteTecnicoActivo.idAsignacionTecnico;
                        await _asignacionRefaccionesRepository.ActualizarRefaccionEnAsignacion(refaccion);
                    }
                }
            }
            else
            {
                // Si no hay otro técnico, la asignación queda en pausa y las refacciones se mantienen
                // (no se transfieren a otro técnico, pero siguen reservadas en el inventario)
            }

            return true;
        }

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

            // Agregar el comentario de retiro al técnico que se retira
            tecnicoQueSeRetira.comentarioPausa = comentarioRetiro;
            await _repository.ActualizarTecnicoEnAsignacion(tecnicoQueSeRetira);

            // Marcar al técnico que se retira como inactivo
            tecnicoQueSeRetira.esTecnicoActivo = false;
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
    }
}
