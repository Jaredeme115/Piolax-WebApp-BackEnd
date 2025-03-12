using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Repositories.Impl;

namespace Piolax_WebApp.Services.Impl
{
    public class MantenimientoPreventivoService(
        IMantenimientoPreventivoRepository repository,
        IAreasRepository areasRepository,
        IMaquinasRepository maquinasRepository,
        IEmpleadoRepository empleadoRepository,
        IFrecuenciaMPRepository frecuenciaMPRepository,
        IEstatusPreventivoRepository estatusPreventivoRepository): IMantenimientoPreventivoService
    {
        private readonly IMantenimientoPreventivoRepository _repository = repository;
        private readonly IAreasRepository _areasRepository = areasRepository;
        private readonly IMaquinasRepository _maquinasRepository = maquinasRepository;
        private readonly IEmpleadoRepository _empleadoRepository = empleadoRepository;
        private readonly IFrecuenciaMPRepository _frecuenciaRepository = frecuenciaMPRepository;
        private readonly IEstatusPreventivoRepository _estatusRepository = estatusPreventivoRepository;

        // Método para crear un mantenimiento preventivo
        public async Task<MantenimientoPreventivoDTO> CrearMantenimientoPreventivo(MantenimientoPreventivoCreateDTO mantenimientoPreventivoCreateDTO)
        {
            // Obtener la semana actual del año
            int semanaActual = GetWeekOfYear(DateTime.Now); // Semana actual del año

            // Crear el objeto de MantenimientoPreventivo
            var mantenimientoPreventivo = new MantenimientoPreventivo
            {
                idArea = mantenimientoPreventivoCreateDTO.idArea,
                idMaquina = mantenimientoPreventivoCreateDTO.idMaquina,
                idFrecuenciaPreventivo = mantenimientoPreventivoCreateDTO.idFrecuenciaPreventivo,
                semanaPreventivo = mantenimientoPreventivoCreateDTO.semanaPreventivo,
                Activo = mantenimientoPreventivoCreateDTO.Activo,
                idEmpleado = mantenimientoPreventivoCreateDTO.idEmpleado,
                rutaPDF = mantenimientoPreventivoCreateDTO.rutaPDF,
                ultimaEjecucion = null, // La última ejecución es null para un nuevo mantenimiento
                fechaEjecucion = null   // No asignamos fecha de ejecución para un mantenimiento nuevo
            };

            // Asignar el idEstatusPreventivo dependiendo de la semana
            if (mantenimientoPreventivo.semanaPreventivo >= semanaActual)
            {
                mantenimientoPreventivo.idEstatusPreventivo = 1; // "Pendiente"
            }
            else
            {
                mantenimientoPreventivo.idEstatusPreventivo = 2; // "No realizado"
            }

            // Calcular la fecha de la ejecución (lunes de la semana)
            DateTime fechaEjecucion = GetStartOfWeek(DateTime.Now.Year, mantenimientoPreventivo.semanaPreventivo);
            mantenimientoPreventivo.fechaEjecucion = fechaEjecucion; // Asignamos la fecha de ejecución

            // Calcular la proximaEjecucion en base a la frecuencia
            DateTime proximaEjecucion = fechaEjecucion; // Inicializamos proximaEjecucion con la fecha de ejecución
            int nuevaSemanaPreventivo = mantenimientoPreventivo.semanaPreventivo; // Inicializamos nueva semana con la actual

            switch (mantenimientoPreventivo.idFrecuenciaPreventivo)
            {
                case 1: // Frecuencia Mensual
                    proximaEjecucion = fechaEjecucion.AddMonths(1);
                    break;

                case 2: // Frecuencia Bimestral
                    proximaEjecucion = fechaEjecucion.AddMonths(2);
                    break;

                case 3: // Frecuencia Trimestral
                    proximaEjecucion = fechaEjecucion.AddMonths(3);
                    break;

                case 4: // Frecuencia Anual
                    proximaEjecucion = fechaEjecucion.AddYears(1);
                    break;

                default:
                    proximaEjecucion = DateTime.Now;
                    break;
            }

            // Recalcular semana para la próxima ejecución
            nuevaSemanaPreventivo = GetWeekOfYear(proximaEjecucion);

            // Asignamos la próxima ejecución
            mantenimientoPreventivo.proximaEjecucion = proximaEjecucion;
            mantenimientoPreventivo.semanaPreventivo = nuevaSemanaPreventivo; // Asignamos la nueva semana

            // Asignar la fecha de última ejecución, si aplica, de manera similar a la fecha de ejecución.
            mantenimientoPreventivo.ultimaEjecucion = fechaEjecucion;

            // Llamar al repositorio para crear el mantenimiento preventivo
            var mantenimientoCreado = await _repository.CrearMantenimientoPreventico(mantenimientoPreventivo);

            // Convertir la entidad MantenimientoPreventivo a DTO para devolverlo
            var mantenimientoPreventivoDTOResult = new MantenimientoPreventivoDTO
            {
                idArea = mantenimientoCreado.idArea,
                idMaquina = mantenimientoCreado.idMaquina,
                idFrecuenciaPreventivo = mantenimientoCreado.idFrecuenciaPreventivo,
                semanaPreventivo = mantenimientoCreado.semanaPreventivo,
                Activo = mantenimientoCreado.Activo,
                idEmpleado = mantenimientoCreado.idEmpleado,
                rutaPDF = mantenimientoCreado.rutaPDF,
                ultimaEjecucion = mantenimientoCreado.ultimaEjecucion,
                proximaEjecucion = mantenimientoCreado.proximaEjecucion,
                fechaEjecucion = mantenimientoCreado.fechaEjecucion,
                idEstatusPreventivo = mantenimientoCreado.idEstatusPreventivo // Incluir el estatus en el DTO
            };

            return mantenimientoPreventivoDTOResult;
        }




        //Consultar un mantenimiento preventivo con detalles
        public async Task<MantenimientoPreventivoDetallesDTO> ConsultarMPConDetalles(int idMP)
        {
            // Consultar el mantenimiento preventivo por ID
            var mantenimiento = await _repository.ConsultarMP(idMP);

            // Si no se encuentra el mantenimiento, retornar null o lanzar una excepción
            if (mantenimiento == null)
            {
                return null; // O puedes lanzar una excepción si prefieres
            }

            // Obtener detalles adicionales relacionados (Área, Maquina, Frecuencia, Estatus, Empleado)
            var area = await _areasRepository.Consultar(mantenimiento.idArea);
            var maquina = await _maquinasRepository.Consultar(mantenimiento.idMaquina);
            var frecuencia = await _frecuenciaRepository.ConsultarFrecuenciaPreventivoPorID(mantenimiento.idFrecuenciaPreventivo);
            var estatus = await _estatusRepository.ConsultarEstatusPreventivoPorID(mantenimiento.idEstatusPreventivo);
            var empleado = await _empleadoRepository.ConsultarPorId(mantenimiento.idEmpleado);

            // Crear el DTO y asignar los valores de la consulta
            var mantenimientoPreventivoDetallesDTO = new MantenimientoPreventivoDetallesDTO
            {
                idMP = mantenimiento.idMP,
                idArea = mantenimiento.idArea,
                nombreArea = area?.nombreArea, // Obtener el nombre del área
                idMaquina = mantenimiento.idMaquina,
                nombreMaquina = maquina?.nombreMaquina, // Obtener el nombre de la máquina
                semanaPreventivo = mantenimiento.semanaPreventivo,
                idFrecuenciaPreventivo = mantenimiento.idFrecuenciaPreventivo,
                nombreFrecuenciaPreventivo = frecuencia?.nombreFrecuenciaMP, // Descripción de la frecuencia
                idEstatusPreventivo = mantenimiento.idEstatusPreventivo,
                nombreEstatusPreventivo = estatus?.nombreEstatusPreventivo, // Descripción del estatus
                idEmpleado = mantenimiento.idEmpleado,
                nombreCompletoTecnicoMP = $"{empleado.nombre} {empleado.apellidoPaterno} {empleado.apellidoMaterno}", // Nombre completo del técnico
                Activo = mantenimiento.Activo,
                rutPDF = mantenimiento.rutaPDF,
                ultimaEjecucion = mantenimiento.ultimaEjecucion,
                proximaEjecucion = mantenimiento.proximaEjecucion,
                fechaEjecucion = mantenimiento.fechaEjecucion
            };

            // Retornar el DTO con todos los detalles
            return mantenimientoPreventivoDetallesDTO;
        }

        // Método para modificar un mantenimiento preventivo
        public async Task<MantenimientoPreventivoDTO> ModificarMantenimientoPreventivo(int idMP, MantenimientoPreventivoModificarDTO mantenimientoPreventivoModificarDTO)
        {
            // Buscar el mantenimiento preventivo por ID en el repositorio
            var mantenimientoExistente = await _repository.ConsultarMP(idMP);

            // Si no se encuentra el mantenimiento, devolver null o lanzar una excepción
            if (mantenimientoExistente == null)
            {
                return null; // O puedes lanzar una excepción si prefieres
            }

            // Si la semana se modifica, cambiar el estatus a "Reprogramado"
            if (mantenimientoExistente.semanaPreventivo != mantenimientoPreventivoModificarDTO.semanaPreventivo)
            {
                mantenimientoExistente.idEstatusPreventivo = 4; // "Reprogramado"
            }

            // Actualizar la frecuencia solo si se modificó, y ajustar la semana si la frecuencia cambia
            if (mantenimientoExistente.idFrecuenciaPreventivo != mantenimientoPreventivoModificarDTO.idFrecuenciaPreventivo)
            {
                // Actualizar la frecuencia con el nuevo valor
                mantenimientoExistente.idFrecuenciaPreventivo = mantenimientoPreventivoModificarDTO.idFrecuenciaPreventivo;

                // Recalcular la semana en función de la nueva frecuencia
                DateTime nuevaFechaEjecucion = GetStartOfWeek(DateTime.Now.Year, mantenimientoExistente.semanaPreventivo);

                switch (mantenimientoExistente.idFrecuenciaPreventivo)
                {
                    case 1: // Frecuencia Mensual
                        nuevaFechaEjecucion = nuevaFechaEjecucion.AddMonths(1);
                        break;

                    case 2: // Frecuencia Bimestral
                        nuevaFechaEjecucion = nuevaFechaEjecucion.AddMonths(2);
                        break;

                    case 3: // Frecuencia Trimestral
                        nuevaFechaEjecucion = nuevaFechaEjecucion.AddMonths(3);
                        break;

                    case 4: // Frecuencia Anual
                        nuevaFechaEjecucion = nuevaFechaEjecucion.AddYears(1);
                        break;

                    default:
                        break;
                }

                // Recalcular la semana de la nueva fecha de ejecución
                mantenimientoExistente.semanaPreventivo = GetWeekOfYear(nuevaFechaEjecucion);
            }

            // Asignamos los demás campos que pueden haberse actualizado
            mantenimientoExistente.semanaPreventivo = mantenimientoPreventivoModificarDTO.semanaPreventivo;
            mantenimientoExistente.Activo = mantenimientoPreventivoModificarDTO.Activo;

            // Verificamos si el técnico ha cambiado, y si es así, actualizamos
            if (mantenimientoPreventivoModificarDTO.idEmpleado != 0)
            {
                mantenimientoExistente.idEmpleado = mantenimientoPreventivoModificarDTO.idEmpleado;
            }

            // Actualizar la `proximaEjecucion` según la frecuencia
            DateTime nuevaProximaEjecucion;

            if (mantenimientoExistente.fechaEjecucion.HasValue)
            {
                // Dependiendo de la frecuencia, sumamos el número de meses o años a la fecha de ejecución
                switch (mantenimientoExistente.idFrecuenciaPreventivo)
                {
                    case 1: // Frecuencia mensual
                        nuevaProximaEjecucion = mantenimientoExistente.fechaEjecucion.Value.AddMonths(1); // Sumar 1 mes
                        break;

                    case 2: // Frecuencia bimestral
                        nuevaProximaEjecucion = mantenimientoExistente.fechaEjecucion.Value.AddMonths(2); // Sumar 2 meses
                        break;

                    case 3: // Frecuencia trimestral
                        nuevaProximaEjecucion = mantenimientoExistente.fechaEjecucion.Value.AddMonths(3); // Sumar 3 meses
                        break;

                    case 4: // Frecuencia Anual
                        nuevaProximaEjecucion = mantenimientoExistente.fechaEjecucion.Value.AddYears(1); // Sumar 1 año
                        break;

                    default:
                        nuevaProximaEjecucion = mantenimientoExistente.fechaEjecucion.Value; // Si no se encuentra la frecuencia, no hacer ningún ajuste
                        break;
                }
            }
            else
            {
                // Si fechaEjecucion es null, asignamos una fecha predeterminada para proximaEjecucion
                nuevaProximaEjecucion = DateTime.Now; // O una fecha de tu elección
            }

            // Asignar la nueva fecha de la próxima ejecución
            mantenimientoExistente.proximaEjecucion = nuevaProximaEjecucion;

            // Guardar los cambios en la base de datos
            await _repository.Modificar(idMP, mantenimientoExistente);

            // Convertir el objeto MantenimientoPreventivo modificado a DTO para devolverlo
            var mantenimientoPreventivoDTOResult = new MantenimientoPreventivoDTO
            {
                idFrecuenciaPreventivo = mantenimientoExistente.idFrecuenciaPreventivo,
                semanaPreventivo = mantenimientoExistente.semanaPreventivo,
                Activo = mantenimientoExistente.Activo,
                idEmpleado = mantenimientoExistente.idEmpleado,
                rutaPDF = mantenimientoExistente.rutaPDF,
                ultimaEjecucion = mantenimientoExistente.ultimaEjecucion,
                proximaEjecucion = mantenimientoExistente.proximaEjecucion,
                fechaEjecucion = mantenimientoExistente.fechaEjecucion,
                idEstatusPreventivo = mantenimientoExistente.idEstatusPreventivo
            };

            return mantenimientoPreventivoDTOResult;
        }



        // Método para Eliminar un mantenimiento preventivo
        public async Task<bool> EliminarMantenimientoPreventivo(int idMP)
        {
            // Llamar al repositorio para eliminar el mantenimiento preventivo
            var mantenimientoPreventivoEliminado = await _repository.Eliminar(idMP);

            // Si el mantenimiento no existe, devolver false
            if (mantenimientoPreventivoEliminado == null)
            {
                return false; // No se encontró el mantenimiento para eliminar
            }

            // Si se elimina con éxito, devolver true
            return true;
        }

        // Método para marcar como realizado
        public async Task<bool> MarcarComoRealizado(int idMP)
        {
            // Buscar el mantenimiento preventivo por ID
            var mantenimiento = await _repository.ConsultarMP(idMP);

            // Si no se encuentra el mantenimiento, retornar false
            if (mantenimiento == null)
            {
                return false; // Mantenimiento no encontrado
            }

            // Cambiar el estatus a "Realizado"
            mantenimiento.idEstatusPreventivo = 3; // "Realizado"

            // Marcar la fecha de ejecución como la fecha actual
            mantenimiento.fechaEjecucion = DateTime.Now;

            // Guardar los cambios en la base de datos
            await _repository.Modificar(idMP, mantenimiento);

            return true; // Mantenimiento marcado como Realizado exitosamente
        }




        //Obtener la semana actual
        private int GetWeekOfYear(DateTime date)
        {
            var calendar = System.Globalization.CultureInfo.CurrentCulture.Calendar;
            // Usamos `CalendarWeekRule.FirstDay` para que la semana comience el lunes
            // Se puede usar `FirstFullWeek` si prefieres que la primera semana sea la primera semana completa del año.
            return calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }

        // Método para calcular el inicio de una semana del año (lunes)
        private DateTime GetStartOfWeek(int year, int weekNumber)
        {
            var jan1 = new DateTime(year, 1, 1);
            var daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;
            var startOfWeek = jan1.AddDays(daysOffset).AddDays((weekNumber - 1) * 7);
            return startOfWeek;
        }

        // Método para calcular el final de la semana (domingo)
        private DateTime GetEndOfWeek(DateTime startOfWeek)
        {
            return startOfWeek.AddDays(6); // Domingo de la misma semana
        }


    }
}
