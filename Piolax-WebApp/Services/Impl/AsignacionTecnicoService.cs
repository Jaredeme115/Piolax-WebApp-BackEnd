using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories.Impl;

namespace Piolax_WebApp.Services.Impl
{
    public class AsignacionTecnicoService : IAsignacionTecnicosService
    {
        private readonly AsignacionTecnicoRepository _repository;
        private readonly AsignacionRepository _asignacionRepository;

        public AsignacionTecnicoService(AsignacionTecnicoRepository repository, AsignacionRepository asignacionRepository)
        {
            _repository = repository;
            _asignacionRepository = asignacionRepository;
        }

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
                var asignacionTecnico = new Asignacion_Tecnico
                {
                    idAsignacion = asignacionTecnicoDTO.idAsignacion,
                    idEmpleado = asignacionTecnicoDTO.idEmpleado,
                    horaInicio = DateTime.UtcNow,
                    horaTermino = asignacionTecnicoDTO.horaTermino,
                    solucion = asignacionTecnicoDTO.solucion,
                    idStatusAprobacionTecnico = asignacionTecnicoDTO.idStatusAprobacionTecnico,
                    comentarioPausa = asignacionTecnicoDTO.comentarioPausa,
                    esTecnicoActivo = asignacionTecnicoDTO.esTecnicoActivo
                };

                return await _repository.CrearAsignacionTecnico(asignacionTecnico);
            }
            catch (Exception ex)
            {
                // Loggear el error (usar una herramienta de logging como Serilog)
                Console.WriteLine($"Error al crear la asignación del técnico: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> FinalizarAsignacionTecnico(Asignacion_TecnicoDTO asignacionTecnicoDTO)
        {
            // 1️⃣ Validar si la asignación y el técnico existen
            var asignacion = await _asignacionRepository.ConsultarAsignacionPorId(asignacionTecnicoDTO.idAsignacion);
            if (asignacion == null)
            {
                throw new ArgumentException("La asignación no existe.");
            }

            var tecnico = await _repository.ConsultarTecnicoPorID(asignacionTecnicoDTO.idAsignacion);
            if (tecnico == null)
            {
                throw new ArgumentException("El técnico no está asignado a esta tarea.");
            }

            // 2️⃣ Marcar la hora de finalización y la solución
            tecnico.horaTermino = DateTime.UtcNow;
            tecnico.solucion = asignacionTecnicoDTO.solucion;
            tecnico.idStatusAprobacionTecnico = asignacionTecnicoDTO.idStatusAprobacionTecnico;
            tecnico.esTecnicoActivo = false;

            await _repository.ActualizarTecnicoEnAsignacion(tecnico);

            // 3️⃣ Verificar si quedan técnicos activos en la asignación
            var hayTecnicosActivos = await _repository.ConsultarTecnicosActivosPorAsignacion(tecnico.idAsignacion);
            if (!hayTecnicosActivos)
            {
                // Si no hay técnicos activos, marcar la asignación como finalizada
                asignacion.idStatusAsignacion = 4; // Estado "Finalizada"
                await _asignacionRepository.ActualizarAsignacion(asignacion);
            }

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
