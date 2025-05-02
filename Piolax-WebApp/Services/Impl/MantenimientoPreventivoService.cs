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
        IKPIMantenimientoPreventivoService kPIMantenimientoPreventivoService) : IMantenimientoPreventivoService
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

            int semanaActual = GetWeekOfYear(DateTime.Now);

            var mantenimientoPreventivo = new MantenimientoPreventivo
            {
                idArea = mantenimientoPreventivoCreateDTO.idArea,
                idMaquina = mantenimientoPreventivoCreateDTO.idMaquina,
                idFrecuenciaPreventivo = mantenimientoPreventivoCreateDTO.idFrecuenciaPreventivo,
                anioPreventivo = mantenimientoPreventivoCreateDTO.anioPreventivo,
                semanaPreventivo = mantenimientoPreventivoCreateDTO.semanaPreventivo,
                semanaOriginalMP = mantenimientoPreventivoCreateDTO.semanaOriginalMP,
                activo = mantenimientoPreventivoCreateDTO.activo,
                idEmpleado = mantenimientoPreventivoCreateDTO.idEmpleado,
                ultimaEjecucion = null,
                fechaEjecucion = null,
                proximaEjecucion = null
            };

            if (mantenimientoPreventivoCreateDTO.idEstatusPreventivo == 3)
            {
                mantenimientoPreventivo.idEstatusPreventivo = 3;
                mantenimientoPreventivo.fechaEjecucion = DateTime.Now;

                DateTime fechaActual = GetStartOfWeek(mantenimientoPreventivo.anioPreventivo, mantenimientoPreventivo.semanaPreventivo);

                switch (mantenimientoPreventivo.idFrecuenciaPreventivo)
                {
                    case 1: fechaActual = fechaActual.AddMonths(1); break;
                    case 2: fechaActual = fechaActual.AddMonths(2); break;
                    case 3: fechaActual = fechaActual.AddMonths(3); break;
                    case 4: fechaActual = fechaActual.AddYears(1); break;
                }

                mantenimientoPreventivo.proximaEjecucion = fechaActual;
                mantenimientoPreventivo.semanaPreventivo = GetWeekOfYear(fechaActual);
                mantenimientoPreventivo.anioPreventivo = fechaActual.Year;
                mantenimientoPreventivo.ultimaEjecucion = DateTime.Now;
            }
            else
            {
                mantenimientoPreventivo.idEstatusPreventivo = (mantenimientoPreventivo.semanaPreventivo >= semanaActual) ? 1 : 2;
            }

            var mantenimientoCreado = await _repository.CrearMantenimientoPreventivo(mantenimientoPreventivo);

            var mantenimientoPreventivoDTOResult = new MantenimientoPreventivoDTO
            {
                idMP = mantenimientoCreado.idMP,
                idArea = mantenimientoCreado.idArea,
                idMaquina = mantenimientoCreado.idMaquina,
                idFrecuenciaPreventivo = mantenimientoCreado.idFrecuenciaPreventivo,
                anioPreventivo = mantenimientoCreado.anioPreventivo,
                semanaPreventivo = mantenimientoCreado.semanaPreventivo,
                semanaOriginalMP = mantenimientoCreado.semanaOriginalMP,
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


            // Si la semana se modifica, cambiar el estatus a "Reprogramado"
            bool semanaCambio = mantenimientoExistente.semanaPreventivo != mantenimientoPreventivoModificarDTO.semanaPreventivo;

            if (semanaCambio)
            {
                // ✅ Solo guarda la original si NO estaba ya reprogramado
                if (mantenimientoExistente.semanaOriginalMP == null)
                {
                    mantenimientoExistente.semanaOriginalMP = mantenimientoExistente.semanaPreventivo;
                }

                // ✅ Actualiza la nueva semana
                mantenimientoExistente.semanaPreventivo = mantenimientoPreventivoModificarDTO.semanaPreventivo;
                mantenimientoExistente.idEstatusPreventivo = 4; // Reprogramado
            }
            else
            {
                // ✅ Si ya estaba reprogramado y vuelves a su semana original, límpialo
                if (mantenimientoExistente.semanaOriginalMP == mantenimientoPreventivoModificarDTO.semanaPreventivo)
                {
                    mantenimientoExistente.semanaOriginalMP = null;
                    mantenimientoExistente.idEstatusPreventivo = 1; // Pendiente
                }
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
                // Recalcular semanaPreventivo SOLO si no fue reprogramado manualmente
                if (!semanaCambio)
                {
                    mantenimientoExistente.semanaPreventivo = GetWeekOfYear(nuevaFechaEjecucion);
                    mantenimientoExistente.anioPreventivo = nuevaFechaEjecucion.Year; // ✅ agrega esto

                }
            }

            // Asignamos los demás campos que pueden haberse actualizado
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

            // Paso 1: Detectar todas las semanas originales que fueron reprogramadas por máquina
            var semanasReprogramadas = lista
                .Where(mp => mp.idEstatusPreventivo == 4 && mp.semanaOriginalMP.HasValue)
                .Select(mp => $"{mp.idMaquina}|{mp.semanaOriginalMP.Value}")
                .ToHashSet();

            // Paso 2: Filtrar la lista
            var filtrados = lista.Where(mp =>
                mp.idEstatusPreventivo == 4 || // mostrar reprogramados
                !semanasReprogramadas.Contains($"{mp.idMaquina}|{mp.semanaPreventivo}") // ocultar originales reprogramados
            );

            // Paso 3: Mapear a DTO
            var listaDto = filtrados.Select(mp => new MantenimientoPreventivoDetallesDTO
            {
                idMP = mp.idMP,
                idArea = mp.idArea,
                nombreArea = mp.Area?.nombreArea ?? "",
                idMaquina = mp.idMaquina,
                nombreMaquina = mp.Maquina?.nombreMaquina ?? "",
                semanaPreventivo = mp.semanaPreventivo,
                semanaOriginalMP = mp.semanaOriginalMP,
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
            var mantenimientos = await _repository.MostrarMPsAsignados(idEmpleado);

            // 1. Detectar semanas originales reprogramadas
            var semanasReprogramadas = mantenimientos
                .Where(mp => mp.idEstatusPreventivo == 4 && mp.semanaOriginalMP.HasValue)
                .Select(mp => $"{mp.idMaquina}|{mp.semanaOriginalMP.Value}")
                .ToHashSet();

            // 2. Filtrar los originales que ya fueron reprogramados
            var filtrados = mantenimientos.Where(mp =>
                mp.idEstatusPreventivo == 4 || // incluir los reprogramados
                !semanasReprogramadas.Contains($"{mp.idMaquina}|{mp.semanaPreventivo}") // excluir los originales reemplazados
            );

            // 3. Mapear a DTO
            var resultado = filtrados.Select(mp => new MantenimientoPreventivoDetallesDTO
            {
                idMP = mp.idMP,
                idArea = mp.idArea,
                nombreArea = mp.Area?.nombreArea ?? "",
                idMaquina = mp.idMaquina,
                nombreMaquina = mp.Maquina?.nombreMaquina ?? "",
                semanaPreventivo = mp.semanaPreventivo,
                semanaOriginalMP = mp.semanaOriginalMP,
                idFrecuenciaPreventivo = mp.idFrecuenciaPreventivo,
                nombreFrecuenciaPreventivo = mp.FrecuenciaMP?.nombreFrecuenciaMP ?? "",
                idEstatusPreventivo = mp.idEstatusPreventivo,
                nombreEstatusPreventivo = mp.EstatusPreventivo?.nombreEstatusPreventivo ?? "",
                idEmpleado = mp.idEmpleado,
                nombreCompletoTecnicoMP = mp.Empleado != null
                    ? mp.Empleado.nombre + " " + mp.Empleado.apellidoPaterno
                    : string.Empty,
                ultimaEjecucion = mp.ultimaEjecucion,
                proximaEjecucion = mp.proximaEjecucion,
                fechaEjecucion = mp.fechaEjecucion
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
                semanaOriginalMP = mp.semanaOriginalMP,

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
                fechaEjecucion = mp.fechaEjecucion
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
                semanaOriginalMP = mp.semanaOriginalMP,

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
                fechaEjecucion = mp.fechaEjecucion
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
                semanaOriginalMP = mp.semanaOriginalMP,

                idFrecuenciaPreventivo = mp.idFrecuenciaPreventivo,
                idEstatusPreventivo = mp.idEstatusPreventivo,
                idEmpleado = mp.idEmpleado,
                nombreCompletoTecnicoMP = mp.Empleado == null
                    ? ""
                    : $"{mp.Empleado.nombre} {mp.Empleado.apellidoPaterno} {mp.Empleado.apellidoMaterno}",
                activo = mp.activo,
                ultimaEjecucion = mp.ultimaEjecucion,
                proximaEjecucion = mp.proximaEjecucion,
                fechaEjecucion = mp.fechaEjecucion
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
                semanaOriginalMP = mp.semanaOriginalMP,

                idFrecuenciaPreventivo = mp.idFrecuenciaPreventivo,
                idEstatusPreventivo = mp.idEstatusPreventivo,
                idEmpleado = mp.idEmpleado,
                nombreCompletoTecnicoMP = mp.Empleado == null
                    ? ""
                    : $"{mp.Empleado.nombre} {mp.Empleado.apellidoPaterno} {mp.Empleado.apellidoMaterno}",
                activo = mp.activo,
                ultimaEjecucion = mp.ultimaEjecucion,
                proximaEjecucion = mp.proximaEjecucion,
                fechaEjecucion = mp.fechaEjecucion
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


        public async Task<ContadoresMPDTO> ObtenerContadoresMP(int? anio = null, int? mes = null)
        {
            List<MantenimientoPreventivo> mps;

            if (anio.HasValue && mes.HasValue)
            {
                mps = (await _repository.ConsultarPorAnioYMes(anio.Value, mes.Value)).ToList();
            }
            else
            {
                mps = (await _repository.ConsultarTodosMPs()).ToList();

                if (anio.HasValue)
                {
                    mps = mps.Where(mp => mp.anioPreventivo == anio.Value).ToList();
                }
            }

            return new ContadoresMPDTO
            {
                TotalMP = mps.Count(),
                Pendientes = mps.Count(mp => mp.idEstatusPreventivo == 1),
                NoRealizados = mps.Count(mp => mp.idEstatusPreventivo == 2),
                Realizados = mps.Count(mp => mp.idEstatusPreventivo == 3),
                Reprogramados = mps.Count(mp => mp.idEstatusPreventivo == 4),
                EnProceso = mps.Count(mp => mp.idEstatusPreventivo == 5)
            };
        }

    }
}