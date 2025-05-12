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
        IEstatusPreventivoRepository estatusPreventivoRepository,
        IKPIMantenimientoPreventivoService kPIMantenimientoPreventivoService): IMantenimientoPreventivoService
    {
        private readonly IMantenimientoPreventivoRepository _repository = repository;
        private readonly IAreasRepository _areasRepository = areasRepository;
        private readonly IMaquinasRepository _maquinasRepository = maquinasRepository;
        private readonly IEmpleadoRepository _empleadoRepository = empleadoRepository;
        private readonly IFrecuenciaMPRepository _frecuenciaRepository = frecuenciaMPRepository;
        private readonly IEstatusPreventivoRepository _estatusRepository = estatusPreventivoRepository;
        private readonly IKPIMantenimientoPreventivoService _kpiMantenimientoPreventivoService = kPIMantenimientoPreventivoService;

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
                semanaOriginalMP = mantenimientoPreventivoCreateDTO.semanaPreventivo,
                activo = mantenimientoPreventivoCreateDTO.activo,
                idEmpleado = mantenimientoPreventivoCreateDTO.idEmpleado,
                anioPreventivo = DateTime.Now.Year,


                // No asignamos fecha de ejecución / pró para un mantenimiento nuevo
                ultimaEjecucion = null, // La última ejecución es null para un nuevo mantenimiento
                fechaEjecucion = null,  // No asignamos fecha de ejecución para un mantenimiento nuevo
                proximaEjecucion = null  // No asignamos próxima ejecución para un mantenimiento nuevo

            };

            if (mantenimientoPreventivoCreateDTO.idEstatusPreventivo == 3)
            {
                // Si el usuario marca "Realizado" desde el comienzo
                mantenimientoPreventivo.idEstatusPreventivo = 3;
                mantenimientoPreventivo.fechaEjecucion = DateTime.Now;

                DateTime fechaActual = GetStartOfWeek(DateTime.Now.Year, mantenimientoPreventivo.semanaPreventivo);
                // La "ejecución" se hace en la semana ingresada, sumamos la frecuencia para la próxima
                switch (mantenimientoPreventivo.idFrecuenciaPreventivo)
                {
                    case 1: // Mensual
                        fechaActual = fechaActual.AddMonths(1);
                        break;
                    case 2: // Bimestral
                        fechaActual = fechaActual.AddMonths(2);
                        break;
                    case 3: // Trimestral
                        fechaActual = fechaActual.AddMonths(3);
                        break;
                    case 4: // Anual
                        fechaActual = fechaActual.AddYears(1);
                        break;
                }
                // Actualizar la nueva semana y proximaEjecucion
                mantenimientoPreventivo.proximaEjecucion = fechaActual;
                mantenimientoPreventivo.semanaPreventivo = GetWeekOfYear(fechaActual);
                mantenimientoPreventivo.anioPreventivo = fechaActual.Year;

                // Asignar la última ejecución a la fecha actual
                mantenimientoPreventivo.ultimaEjecucion = DateTime.Now;
            }
            else
            {
                // Mantener la lógica actual para Pendiente o No realizado
                if (mantenimientoPreventivo.semanaPreventivo >= semanaActual && mantenimientoPreventivo.anioPreventivo >= DateTime.Now.Year)
                    mantenimientoPreventivo.idEstatusPreventivo = 1; // Pendiente
                else
                    mantenimientoPreventivo.idEstatusPreventivo = 2; // No realizado
            }

            // Llamar al repositorio para crear el mantenimiento preventivo
            var mantenimientoCreado = await _repository.CrearMantenimientoPreventivo(mantenimientoPreventivo);

            // Convertir la entidad MantenimientoPreventivo a DTO para devolverlo
            var mantenimientoPreventivoDTOResult = new MantenimientoPreventivoDTO
            {
                idMP = mantenimientoCreado.idMP,
                idArea = mantenimientoCreado.idArea,
                idMaquina = mantenimientoCreado.idMaquina,
                idFrecuenciaPreventivo = mantenimientoCreado.idFrecuenciaPreventivo,
                semanaPreventivo = mantenimientoCreado.semanaPreventivo,
                semanaOriginalMP = mantenimientoCreado.semanaOriginalMP,
                anioPreventivo = mantenimientoCreado.anioPreventivo,
                activo = mantenimientoCreado.activo,
                idEmpleado = mantenimientoCreado.idEmpleado,
                ultimaEjecucion = mantenimientoCreado.ultimaEjecucion,
                proximaEjecucion = mantenimientoCreado.proximaEjecucion,
                fechaEjecucion = mantenimientoCreado.fechaEjecucion,
                idEstatusPreventivo = mantenimientoCreado.idEstatusPreventivo 
            };

            return mantenimientoPreventivoDTOResult;
        }

        // Método para marcar como realizado y recalcular la próxima semana
        public async Task<bool> MarcarComoRealizado(int idMP)
        {
            var mantenimiento = await _repository.ConsultarMP(idMP);
            if (mantenimiento == null)
                return false;

            // Cambiar estatus y asignar fecha de ejecución
            mantenimiento.idEstatusPreventivo = 3; // "Realizado"
            mantenimiento.fechaEjecucion = DateTime.Now;

            // Calcular la próxima fecha de ejecución según la frecuencia
            DateTime fechaActualEjecucion = GetStartOfWeek(DateTime.Now.Year, mantenimiento.semanaPreventivo);
            DateTime proximaFecha = fechaActualEjecucion;
            switch (mantenimiento.idFrecuenciaPreventivo)
            {
                case 1: proximaFecha = fechaActualEjecucion.AddMonths(1); break;
                case 2: proximaFecha = fechaActualEjecucion.AddMonths(2); break;
                case 3: proximaFecha = fechaActualEjecucion.AddMonths(3); break;
                case 4: proximaFecha = fechaActualEjecucion.AddYears(1); break;
            }

            mantenimiento.semanaPreventivo = GetWeekOfYear(proximaFecha);
            mantenimiento.proximaEjecucion = proximaFecha;
            mantenimiento.anioPreventivo = proximaFecha.Year;

            // Actualizar el mantenimiento en la BD
            await _repository.Modificar(idMP, mantenimiento);

            // Disparar el cálculo de KPIs en tiempo real
            // Por ejemplo, calculamos los KPIs para el mes actual
            DateTime inicioPeriodo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime finPeriodo = inicioPeriodo.AddMonths(1).AddDays(-1);
            await _kpiMantenimientoPreventivoService.CalcularYGuardarKPIs(inicioPeriodo, finPeriodo);

            return true;
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
                semanaOriginalMP = mantenimiento.semanaOriginalMP,
                idFrecuenciaPreventivo = mantenimiento.idFrecuenciaPreventivo,
                nombreFrecuenciaPreventivo = frecuencia?.nombreFrecuenciaMP, // Descripción de la frecuencia
                idEstatusPreventivo = mantenimiento.idEstatusPreventivo,
                nombreEstatusPreventivo = estatus?.nombreEstatusPreventivo, // Descripción del estatus
                idEmpleado = mantenimiento.idEmpleado,
                nombreCompletoTecnicoMP = $"{empleado.nombre} {empleado.apellidoPaterno} {empleado.apellidoMaterno}", // Nombre completo del técnico
                activo = mantenimiento.activo,
                ultimaEjecucion = mantenimiento.ultimaEjecucion,
                proximaEjecucion = mantenimiento.proximaEjecucion,
                fechaEjecucion = mantenimiento.fechaEjecucion
            };

            // Retornar el DTO con todos los detalles
            return mantenimientoPreventivoDetallesDTO;
        }

        // Método para modificar un mantenimiento preventivo
        /*public async Task<MantenimientoPreventivoDTO> ModificarMantenimientoPreventivo(int idMP, MantenimientoPreventivoModificarDTO mantenimientoPreventivoModificarDTO)
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
            mantenimientoExistente.activo = mantenimientoPreventivoModificarDTO.activo;

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
                activo = mantenimientoExistente.activo,
                idEmpleado = mantenimientoExistente.idEmpleado,
                ultimaEjecucion = mantenimientoExistente.ultimaEjecucion,
                proximaEjecucion = mantenimientoExistente.proximaEjecucion,
                fechaEjecucion = mantenimientoExistente.fechaEjecucion,
                idEstatusPreventivo = mantenimientoExistente.idEstatusPreventivo
            };

            return mantenimientoPreventivoDTOResult;
        }*/

        public async Task<MantenimientoPreventivoDTO> ModificarMantenimientoPreventivo(int idMP, MantenimientoPreventivoModificarDTO mantenimientoPreventivoModificarDTO)
        {
            // Buscar el mantenimiento preventivo por ID en el repositorio
            var mantenimientoExistente = await _repository.ConsultarMP(idMP);

            // Si no se encuentra el mantenimiento, devolver null o lanzar una excepción
            if (mantenimientoExistente == null)
            {
                return null; // O puedes lanzar una excepción si prefieres
            }

            // Si es una reprogramación (cambio de semana)
            /*if (mantenimientoExistente.semanaPreventivo != mantenimientoPreventivoModificarDTO.semanaPreventivo)
            {
                // CORRECCIÓN: Asegurar que semanaOriginalMP siempre tenga un valor válido
                // Si es la primera reprogramación O si semanaOriginalMP no es válida
                if (mantenimientoExistente.idEstatusPreventivo != 4 ||
                    mantenimientoExistente.semanaOriginalMP == 0 ||
                    mantenimientoExistente.semanaOriginalMP == mantenimientoExistente.semanaPreventivo)
                {
                    // Guardamos la semana actual como la original antes de cambiarla
                    mantenimientoExistente.semanaOriginalMP = mantenimientoExistente.semanaPreventivo;

                    // Si la semana original sigue siendo igual a la nueva semana (caso extraño)
                    if (mantenimientoExistente.semanaOriginalMP == mantenimientoPreventivoModificarDTO.semanaPreventivo)
                    {
                        // Calcular una semana original diferente basada en el intervalo
                        int intervalo = ObtenerIntervaloDeFrecuencia(mantenimientoExistente.idFrecuenciaPreventivo);
                        mantenimientoExistente.semanaOriginalMP = mantenimientoPreventivoModificarDTO.semanaPreventivo - intervalo;

                        // Asegurar que esté en rango válido (1-52)
                        if (mantenimientoExistente.semanaOriginalMP <= 0)
                        {
                            mantenimientoExistente.semanaOriginalMP += 52;
                        }
                    }
                }

                

                // Cambio a estado reprogramado
                mantenimientoExistente.idEstatusPreventivo = 4; // "Reprogramado"

                // Actualizamos a la nueva semana
                mantenimientoExistente.semanaPreventivo = mantenimientoPreventivoModificarDTO.semanaPreventivo;

                // Actualizamos el año si es que la semana pasa del mes de diciembre
                var fechaSemana = GetStartOfWeek(DateTime.Now.Year, mantenimientoExistente.semanaPreventivo);
                mantenimientoExistente.anioPreventivo = fechaSemana.Year;
            }*/

            // Lógica de reprogramación
            if (mantenimientoExistente.semanaPreventivo != mantenimientoPreventivoModificarDTO.semanaPreventivo)
            {
                // Preservar semana original SOLO en la primera reprogramación
                if (mantenimientoExistente.idEstatusPreventivo != 4)
                {
                    mantenimientoExistente.semanaOriginalMP = mantenimientoExistente.semanaPreventivo;
                }

                mantenimientoExistente.idEstatusPreventivo = 4;
                mantenimientoExistente.semanaPreventivo = mantenimientoPreventivoModificarDTO.semanaPreventivo;

                // Calcular año correcto para semanas de transición
                var fechaSemana = GetStartOfWeek(DateTime.Now.Year, mantenimientoExistente.semanaPreventivo);
                mantenimientoExistente.anioPreventivo = fechaSemana.Year;
            }

            // Actualizar la frecuencia solo si se modificó
            if (mantenimientoExistente.idFrecuenciaPreventivo != mantenimientoPreventivoModificarDTO.idFrecuenciaPreventivo)
            {
                mantenimientoExistente.idFrecuenciaPreventivo = mantenimientoPreventivoModificarDTO.idFrecuenciaPreventivo;
            }

            // Actualizamos otros campos modificables
            mantenimientoExistente.activo = mantenimientoPreventivoModificarDTO.activo;

            // Verificamos si el técnico ha cambiado, y si es así, actualizamos
            if (mantenimientoPreventivoModificarDTO.idEmpleado != 0)
            {
                mantenimientoExistente.idEmpleado = mantenimientoPreventivoModificarDTO.idEmpleado;
            }

            // Actualizar la proximaEjecucion según el estado y la información disponible
            if (mantenimientoExistente.fechaEjecucion.HasValue)
            {
                // Si tiene fechaEjecucion, calculamos la próxima según la frecuencia
                switch (mantenimientoExistente.idFrecuenciaPreventivo)
                {
                    case 1: // Frecuencia mensual
                        mantenimientoExistente.proximaEjecucion = mantenimientoExistente.fechaEjecucion.Value.AddMonths(1);
                        break;
                    case 2: // Frecuencia bimestral
                        mantenimientoExistente.proximaEjecucion = mantenimientoExistente.fechaEjecucion.Value.AddMonths(2);
                        break;
                    case 3: // Frecuencia trimestral
                        mantenimientoExistente.proximaEjecucion = mantenimientoExistente.fechaEjecucion.Value.AddMonths(3);
                        break;
                    case 4: // Frecuencia Anual
                        mantenimientoExistente.proximaEjecucion = mantenimientoExistente.fechaEjecucion.Value.AddYears(1);
                        break;
                    default:
                        mantenimientoExistente.proximaEjecucion = mantenimientoExistente.fechaEjecucion.Value;
                        break;
                }
            }
            else if (mantenimientoExistente.idEstatusPreventivo == 4) // Si es reprogramado
            {
                // Para mantenimientos reprogramados, la proximaEjecucion es la fecha de la semana reprogramada
                // Usamos el año actual o el año de anioPreventivo si es diferente
                int yearToUse = mantenimientoExistente.anioPreventivo;
                mantenimientoExistente.proximaEjecucion = GetStartOfWeek(yearToUse, mantenimientoExistente.semanaPreventivo);
            }
            else if (!mantenimientoExistente.proximaEjecucion.HasValue)
            {
                // Si no hay proximaEjecucion, usamos la fecha de la semana programada
                int yearToUse = mantenimientoExistente.anioPreventivo;
                mantenimientoExistente.proximaEjecucion = GetStartOfWeek(yearToUse, mantenimientoExistente.semanaPreventivo);
            }

            // Guardar los cambios en la base de datos
            await _repository.Modificar(idMP, mantenimientoExistente);

            // Convertir el objeto MantenimientoPreventivo modificado a DTO para devolverlo
            var mantenimientoPreventivoDTOResult = new MantenimientoPreventivoDTO
            {
                idMP = mantenimientoExistente.idMP,
                idArea = mantenimientoExistente.idArea,
                idMaquina = mantenimientoExistente.idMaquina,
                idFrecuenciaPreventivo = mantenimientoExistente.idFrecuenciaPreventivo,
                semanaPreventivo = mantenimientoExistente.semanaPreventivo,
                semanaOriginalMP = mantenimientoExistente.semanaOriginalMP, // Incluir semana original
                anioPreventivo = mantenimientoExistente.anioPreventivo,
                activo = mantenimientoExistente.activo,
                idEmpleado = mantenimientoExistente.idEmpleado,
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

        public async Task<IEnumerable<MantenimientoPreventivoDetallesDTO>> ConsultarTodosMPsDTO()
        {
            var lista = await _repository.ConsultarTodosMPs();

            var listaDto = lista.Select(mp => new MantenimientoPreventivoDetallesDTO
            {
                idMP = mp.idMP,
                idArea = mp.idArea,
                nombreArea = mp.Area?.nombreArea ?? "",
                idMaquina = mp.idMaquina,
                nombreMaquina = mp.Maquina?.nombreMaquina ?? "",
                semanaPreventivo = mp.semanaPreventivo,
                semanaOriginalMP = mp.semanaOriginalMP,
                anioPreventivo = mp.anioPreventivo,
                idFrecuenciaPreventivo = mp.idFrecuenciaPreventivo,
                nombreFrecuenciaPreventivo = mp.FrecuenciaMP?.nombreFrecuenciaMP ?? "",
                idEstatusPreventivo = mp.idEstatusPreventivo,
                nombreEstatusPreventivo = mp.EstatusPreventivo?.nombreEstatusPreventivo ?? "",
                idEmpleado = mp.idEmpleado,
                nombreCompletoTecnicoMP = mp.Empleado == null
                    ? ""
                    : $"{mp.Empleado.nombre} {mp.Empleado.apellidoPaterno} {mp.Empleado.apellidoMaterno}",
                activo = mp.activo,
                ultimaEjecucion = mp.ultimaEjecucion,
                proximaEjecucion = mp.proximaEjecucion,
                fechaEjecucion = mp.fechaEjecucion
            });

            return listaDto;
        }

        public async Task<IEnumerable<MantenimientoPreventivoDetallesDTO>> MostrarMPsAsignados(int idEmpleado)
        {
            // 1. Llamar al repositorio para obtener la lista de mantenimientos asignados al empleado
            var mantenimientos = await _repository.MostrarMPsAsignados(idEmpleado);

            // 2. Convertir la lista de entidades a DTOs (MantenimientoPreventivoDetallesDTO)
            var resultado = mantenimientos.Select(mp => new MantenimientoPreventivoDetallesDTO
            {
                idMP = mp.idMP,
                idArea = mp.idArea,
                nombreArea = mp.Area?.nombreArea,      // Relación con área
                idMaquina = mp.idMaquina,
                nombreMaquina = mp.Maquina?.nombreMaquina,
                idFrecuenciaPreventivo = mp.idFrecuenciaPreventivo,
                nombreFrecuenciaPreventivo = mp.FrecuenciaMP?.nombreFrecuenciaMP,
                idEstatusPreventivo = mp.idEstatusPreventivo,
                nombreEstatusPreventivo = mp.EstatusPreventivo?.nombreEstatusPreventivo,
                idEmpleado = mp.idEmpleado,
                nombreCompletoTecnicoMP = mp.Empleado != null
                    ? mp.Empleado.nombre + " " + mp.Empleado.apellidoPaterno
                    : string.Empty,
                ultimaEjecucion = mp.ultimaEjecucion,
                proximaEjecucion = mp.proximaEjecucion,
                fechaEjecucion = mp.fechaEjecucion,
                semanaPreventivo = mp.semanaPreventivo,
                anioPreventivo = mp.anioPreventivo
            });

            return resultado;
        }

        public async Task<MantenimientoPreventivoDetallesDTO> ActivarMantenimientoPreventivo(int idMP)
        {
            // 1) Llamamos al repositorio para activar el MP
            var mp = await _repository.ActivarMP(idMP);
            if (mp == null)
                return null; // o lanzar excepción si prefieres

            // 2) Cargar la relación si lo requieres (Empleado, Área, etc.) 
            //    o usar un Include en 'ActivarMP' si no dispones de mp con toda la información.
            //    Aquí asumo que mp viene con la información necesaria,
            //    o la consultas aparte con un Include. Ejemplo:
            //mp = await _context.MantenimientoPreventivo
            //       .Include(m => m.Area)
            //       .Include(m => m.Maquina)
            //       .Include(m => m.Empleado)
            //       .FirstOrDefaultAsync(m => m.idMP == idMP);

            // 3) Mapear la entidad a DTO
            var dto = new MantenimientoPreventivoDetallesDTO
            {
                idMP = mp.idMP,
                idArea = mp.idArea,
                nombreArea = mp.Area?.nombreArea ?? "",
                idMaquina = mp.idMaquina,
                nombreMaquina = mp.Maquina?.nombreMaquina ?? "",
                semanaPreventivo = mp.semanaPreventivo,
                idFrecuenciaPreventivo = mp.idFrecuenciaPreventivo,
                // FrecuenciaMP?: mp.FrecuenciaMP?.nombreFrecuenciaMP
                idEstatusPreventivo = mp.idEstatusPreventivo,
                // nombreEstatusPreventivo = mp.EstatusPreventivo?.nombreEstatusPreventivo
                idEmpleado = mp.idEmpleado,
                nombreCompletoTecnicoMP = mp.Empleado == null
                    ? ""
                    : $"{mp.Empleado.nombre} {mp.Empleado.apellidoPaterno} {mp.Empleado.apellidoMaterno}",
                activo = mp.activo,
                ultimaEjecucion = mp.ultimaEjecucion,
                proximaEjecucion = mp.proximaEjecucion,
                fechaEjecucion = mp.fechaEjecucion,
                anioPreventivo = mp.anioPreventivo
            };

            // 4) Retornar el DTO
            return dto;
        }

        public async Task<MantenimientoPreventivoDetallesDTO> DesactivarMantenimientoPreventivo(int idMP)
        {
            var mp = await _repository.DesactivarMP(idMP);
            if (mp == null) return null;

            // Opcionalmente, incluir .Include(...) si quieres mapear Empleado, Área, etc.
            return new MantenimientoPreventivoDetallesDTO
            {
                idMP = mp.idMP,
                idArea = mp.idArea,
                nombreArea = mp.Area?.nombreArea ?? "",
                idMaquina = mp.idMaquina,
                nombreMaquina = mp.Maquina?.nombreMaquina ?? "",
                semanaPreventivo = mp.semanaPreventivo,
                idFrecuenciaPreventivo = mp.idFrecuenciaPreventivo,
                // FrecuenciaMP?: mp.FrecuenciaMP?.nombreFrecuenciaMP
                idEstatusPreventivo = mp.idEstatusPreventivo,
                // nombreEstatusPreventivo = mp.EstatusPreventivo?.nombreEstatusPreventivo
                idEmpleado = mp.idEmpleado,
                nombreCompletoTecnicoMP = mp.Empleado == null
                    ? ""
                    : $"{mp.Empleado.nombre} {mp.Empleado.apellidoPaterno} {mp.Empleado.apellidoMaterno}",
                activo = mp.activo,
                ultimaEjecucion = mp.ultimaEjecucion,
                proximaEjecucion = mp.proximaEjecucion,
                fechaEjecucion = mp.fechaEjecucion,
                anioPreventivo = mp.anioPreventivo
            };
        }

        public async Task<MantenimientoPreventivoDetallesDTO> CambiarEstatusEnProceso(int idMP)
        {
            var mp = await _repository.CambiarEstatusEnProceso(idMP);

            if (mp == null) return null;

            return new MantenimientoPreventivoDetallesDTO
            {
                idMP = mp.idMP,
                idArea = mp.idArea,
                nombreArea = mp.Area?.nombreArea ?? "",
                idMaquina = mp.idMaquina,
                nombreMaquina = mp.Maquina?.nombreMaquina ?? "",
                semanaPreventivo = mp.semanaPreventivo,
                idFrecuenciaPreventivo = mp.idFrecuenciaPreventivo,
                idEstatusPreventivo = mp.idEstatusPreventivo,
                idEmpleado = mp.idEmpleado,
                nombreCompletoTecnicoMP = mp.Empleado == null
                    ? ""
                    : $"{mp.Empleado.nombre} {mp.Empleado.apellidoPaterno} {mp.Empleado.apellidoMaterno}",
                activo = mp.activo,
                ultimaEjecucion = mp.ultimaEjecucion,
                proximaEjecucion = mp.proximaEjecucion,
                fechaEjecucion = mp.fechaEjecucion,
                anioPreventivo = mp.anioPreventivo
            };
        }

        public async Task<MantenimientoPreventivoDetallesDTO> CancelarMantenimientoEnProceso(int idMP)
        {
            var mp = await _repository.CancelarMantenimientoEnProceso(idMP);
            if (mp == null) return null;

            return new MantenimientoPreventivoDetallesDTO
            {
                idMP = mp.idMP,
                idArea = mp.idArea,
                nombreArea = mp.Area?.nombreArea ?? "",
                idMaquina = mp.idMaquina,
                nombreMaquina = mp.Maquina?.nombreMaquina ?? "",
                semanaPreventivo = mp.semanaPreventivo,
                idFrecuenciaPreventivo = mp.idFrecuenciaPreventivo,
                idEstatusPreventivo = mp.idEstatusPreventivo,
                idEmpleado = mp.idEmpleado,
                nombreCompletoTecnicoMP = mp.Empleado == null
                    ? ""
                    : $"{mp.Empleado.nombre} {mp.Empleado.apellidoPaterno} {mp.Empleado.apellidoMaterno}",
                activo = mp.activo,
                ultimaEjecucion = mp.ultimaEjecucion,
                proximaEjecucion = mp.proximaEjecucion,
                fechaEjecucion = mp.fechaEjecucion,
                anioPreventivo = mp.anioPreventivo
            };
        }

        // Agregar a MantenimientoPreventivoService.cs
        public async Task<bool> CorregirMantenimientosReprogramados()
        {
            try
            {
                // Consultar todos los mantenimientos preventivos
                var mantenimientos = await _repository.ConsultarTodosMPs();

                // Filtrar solo los reprogramados con inconsistencias
                var reprogramadosInconsistentes = mantenimientos
                    .Where(mp => mp.idEstatusPreventivo == 4 && (mp.semanaOriginalMP == 0 || mp.semanaOriginalMP == mp.semanaPreventivo))
                    .ToList();

                if (reprogramadosInconsistentes.Count == 0)
                {
                    return true; // No hay inconsistencias para corregir
                }

                // Corregir cada uno de los registros inconsistentes
                foreach (var mp in reprogramadosInconsistentes)
                {
                    // Si la semana original no es válida, estimarla
                    if (mp.semanaOriginalMP == 0 || mp.semanaOriginalMP == mp.semanaPreventivo)
                    {
                        // Estimar una semana original basada en el intervalo de frecuencia
                        var intervalo = ObtenerIntervaloDeFrecuencia(mp.idFrecuenciaPreventivo);

                        // Calcular una semana original probable (1 intervalo antes)
                        mp.semanaOriginalMP = mp.semanaPreventivo - intervalo;

                        // Asegurarse de que la semana sea válida (entre 1 y 53)
                        if (mp.semanaOriginalMP <= 0)
                        {
                            mp.semanaOriginalMP += 52; // Ajustar al año anterior
                        }

                        // Actualizar en la base de datos
                        await _repository.Modificar(mp.idMP, mp);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                // Registrar el error
                Console.Error.WriteLine($"Error al corregir mantenimientos reprogramados: {ex.Message}");
                return false;
            }
        }

        // Método auxiliar para obtener el intervalo en semanas según la frecuencia
        private int ObtenerIntervaloDeFrecuencia(int idFrecuenciaPreventivo)
        {
            return idFrecuenciaPreventivo switch
            {
                1 => 4,  // Mensual: 4 semanas
                2 => 8,  // Bimestral: 8 semanas
                3 => 12, // Trimestral: 12 semanas
                4 => 52, // Anual: 52 semanas
                _ => 4   // Valor por defecto: mensual
            };
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
