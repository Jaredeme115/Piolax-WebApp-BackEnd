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
        IInventarioRepository inventarioRepository) : IAsignacionTecnicosService
    {
        private readonly IAsignacionTecnicosRepository _repository = repository;
        private readonly IAsignacionRepository _asignacionRepository = asignacionRepository;
        private readonly IAsignacionRefaccionesRepository _asignacionRefaccionesRepository = asignacionRefaccionesRepository;
        private readonly IInventarioRepository _inventarioRepository = inventarioRepository;


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


        public async Task<Asignacion_Tecnico> CrearAsignacionTecnico(Asignacion_TecnicoDTO asignacionTecnicoDTO)
        {
           
            try
            {
                // Validar si ya hay un técnico activo en la asignación
                bool hayTecnicoActivo = await _repository.ConsultarTecnicosActivosPorAsignacion(asignacionTecnicoDTO.idAsignacion);
                if (hayTecnicoActivo)
                {
                    // Si ya hay un técnico activo, el nuevo técnico debe ser inactivo
                    asignacionTecnicoDTO.esTecnicoActivo = false;
                }
                else
                {
                    // Si no hay un técnico activo, el nuevo técnico será el activo
                    asignacionTecnicoDTO.esTecnicoActivo = true;
                }

                // Crear la entidad Asignacion_Tecnico
                var asignacionTecnico = new Asignacion_Tecnico
                {
                    idAsignacion = asignacionTecnicoDTO.idAsignacion,
                    idEmpleado = asignacionTecnicoDTO.idEmpleado,
                    horaInicio = DateTime.UtcNow,
                    horaTermino = asignacionTecnicoDTO.horaTermino,
                    solucion = "N/A", // Valor por defecto
                    idStatusAprobacionTecnico = 3, // Valor por defecto (por ejemplo, "Pendiente")
                    comentarioPausa = "N/A", // Valor por defecto
                    esTecnicoActivo = asignacionTecnicoDTO.esTecnicoActivo // Definido por la validación anterior
                };

                // Guardar la asignación del técnico en la base de datos
                return await _repository.CrearAsignacionTecnico(asignacionTecnico);
            }
            catch (Exception ex)
            {
                // Loggear el error (usar una herramienta de logging como Serilog)
                Console.WriteLine($"Error al crear la asignación del técnico: {ex.Message}");
                throw;
            }
        }

        public async Task<Asignacion_Tecnico> FinalizarAsignacionTecnico(Asignacion_TecnicoFinalizacionDTO asignacionTecnicoFinalizacionDTO)
        {
            // Validar si la asignación existe
            var asignacion = await _asignacionRepository.ConsultarAsignacionPorId(asignacionTecnicoFinalizacionDTO.idAsignacion);
            if (asignacion == null)
            {
                throw new ArgumentException("La asignación no existe.");
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

            // Marcar la hora de finalización y la solución
            tecnico.horaTermino = DateTime.UtcNow;
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
                asignacion.idStatusAsignacion = 4; // Estado "Finalizada"
                var asignacionActualizada = await _asignacionRepository.ActualizarAsignacion(asignacion.idAsignacion, asignacion);

                if (asignacionActualizada == null)
                {
                    throw new ArgumentException("No se pudo actualizar la asignación porque no existe.");
                }

                return tecnico; // Devuelve el técnico actualizado en lugar de la asignación
            }

            return tecnico; // Devuelve el técnico actualizado
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

            // Cambiar el estado de la asignación a "Pausa"
            asignacion.idStatusAsignacion = 3; // Suponiendo que 3 es el ID del estado "Pausa"
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
                nombreCompletoTecnico = $"{tecnico.Empleado.nombre} {tecnico.Empleado.apellidoPaterno} {tecnico.Empleado.apellidoMaterno}",
                horaInicio = tecnico.horaInicio,
                horaTermino = tecnico.horaTermino,
                solucion = tecnico.solucion,
                idStatusAprobacionTecnico = tecnico.idStatusAprobacionTecnico,
                nombreStatusAprobacionTecnico = tecnico.StatusAprobacionTecnico.descripcionStatusAprobacionTecnico,
                comentarioPausa = tecnico.comentarioPausa,
                esTecnicoActivo = tecnico.esTecnicoActivo,
                Refacciones = tecnico.Asignacion_Refacciones.Select(refaccion => new Asignacion_RefaccionesDetallesDTO
                {
                    idAsignacionRefaccion = refaccion.idAsignacionRefaccion,
                    idAsignacionTecnico = refaccion.idAsignacionTecnico,
                    idRefaccion = refaccion.idRefaccion,
                    nombreRefaccion = refaccion.Inventario.nombreProducto,
                    cantidad = refaccion.cantidad
                }).ToList()
            });

            return tecnicoDetalles;

           
        }
    }
}
