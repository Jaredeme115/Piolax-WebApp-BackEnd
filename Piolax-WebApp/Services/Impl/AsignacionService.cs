using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Repositories.Impl;
using System.Globalization;
using System.Linq;

namespace Piolax_WebApp.Services.Impl
{
    public class AsignacionService( 
        IAsignacionRepository repository, 
        IAreasRepository areasRepository, 
        IRolesRepository rolesRepository,
        IAsignacionTecnicosRepository asignacionTecnicosRepository,
        ISolicitudesRepository solicitudRepository,
        IMaquinasRepository maquinasRepository,
        IKPIRepository kpiRepository) : IAsignacionService
    {
        private readonly IAsignacionRepository _repository = repository;
        private readonly IAreasRepository _areasRepository = areasRepository;
        private readonly IRolesRepository _rolesRepository = rolesRepository;
        private readonly IAsignacionTecnicosRepository _asignacionTecnicosRepository = asignacionTecnicosRepository;
        private readonly ISolicitudesRepository _solicitudRepository = solicitudRepository;
        private readonly IMaquinasRepository _maquinaRepository = maquinasRepository;
        private readonly IKPIRepository _kpiRepository = kpiRepository;

        public async Task<AsignacionResponseDTO> AgregarAsignacion(AsignacionesDTO asignacionesDTO)
        {
            // Verificar que la solicitud exista
            var solicitudExiste = await _solicitudRepository.ExisteSolicitud(asignacionesDTO.idSolicitud);
            if (!solicitudExiste)
            {
                throw new ArgumentException("La solicitud no existe.");
            }

            // Obtener detalles de la solicitud (para validaciones, etc.)
            var solicitudDetalle = await _solicitudRepository.ObtenerSolicitudConDetalles(asignacionesDTO.idSolicitud);
            if (solicitudDetalle == null)
            {
                throw new Exception("No se pudo obtener la información de la solicitud.");
            }

            // Validar el código QR
            string nombreMaquinaEsperado = solicitudDetalle.Maquina.nombreMaquina;
            if (!string.Equals(asignacionesDTO.codigoQR?.Trim(), nombreMaquinaEsperado?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("El código QR proporcionado no coincide con la máquina asignada.");
            }

            // Verificar si ya existe una asignación activa para la solicitud (estado 1 o 2)
            var asignacionActiva = await _repository.ObtenerAsignacionActivaPorSolicitud(asignacionesDTO.idSolicitud);
            if (asignacionActiva != null)
            {
                // Se reutiliza la asignación activa
                return new AsignacionResponseDTO
                {
                    idAsignacion = asignacionActiva.idAsignacion,
                    idSolicitud = asignacionActiva.idSolicitud,
                    idStatusAsignacion = asignacionActiva.idStatusAsignacion
                };
            }

            // Si no existe, crear una nueva asignación
            var asignacion = new Asignaciones
            {
                idSolicitud = asignacionesDTO.idSolicitud,
                idStatusAsignacion = 1 // En Proceso Técnico
            };

            var nuevaAsignacion = await _repository.AgregarAsignacion(asignacion);
            var response = new AsignacionResponseDTO
            {
                idAsignacion = nuevaAsignacion.idAsignacion,
                idSolicitud = nuevaAsignacion.idSolicitud,
                idStatusAsignacion = nuevaAsignacion.idStatusAsignacion
            };

            return response;
        }




        public async Task<IEnumerable<Asignaciones>> ConsultarTodasLasAsignaciones()
        {
            return await _repository.ConsultarTodasLasAsignaciones();
        }

        public async Task<Asignaciones> ConsultarAsignacionPorId(int idAsignacion)
        {
            return await _repository.ConsultarAsignacionPorId(idAsignacion);
        }

        public async Task<Asignaciones> ActualizarAsignacion(int idAsignacion, AsignacionesDTO asignacionesDTO)
        {
            var asignacionExiste = await _repository.AsignacionExiste(idAsignacion);
            if (!asignacionExiste)
            {
                throw new ArgumentException("La asignación no existe.");
            }

            var asignacion = new Asignaciones
            {
                idAsignacion = idAsignacion, 
                idSolicitud = asignacionesDTO.idSolicitud,
                idStatusAsignacion = asignacionesDTO.idStatusAsignacion
            };

            return await _repository.ActualizarAsignacion(idAsignacion, asignacion);
        }


        public async Task<bool> EliminarAsignacion(int idAsignacion)
        {
            return await _repository.EliminarAsignacion(idAsignacion);
        }

        public async Task<bool> AsignacionExiste(int idAsignacion)
        {
            return await _repository.AsignacionExiste(idAsignacion);
        }

        public async Task<AsignacionDetallesDTO?> ConsultarAsignacionConDetallesPorId(int idAsignacion)
        {
            // Consultar la asignación principal
            var asignacion = await _repository.ConsultarAsignacionPorId(idAsignacion);
            if (asignacion == null)
            {
                return null; // Devuelve null si no existe la asignación
            }

            // Consultar técnicos relacionados (ya que se van a mapear de forma plana)
            var tecnicos = await _asignacionTecnicosRepository.ConsultarTecnicosPorAsignacion(idAsignacion);

            // Consultar datos simples: nombre del área y del rol
            var areaSeleccionada = await _areasRepository.Consultar(asignacion.Solicitud.idAreaSeleccionada);
            var rolSeleccionado = await _rolesRepository.Consultar(asignacion.Solicitud.idRolSeleccionado);

            // Mapear los técnicos a DTOs sin incluir las entidades completas
            var tecnicoDetalles = tecnicos.Select(tecnico => new Asignacion_TecnicoDetallesDTO
            {
                idAsignacionTecnico = tecnico.idAsignacionTecnico,
                idAsignacion = tecnico.idAsignacion,
                idEmpleado = tecnico.idEmpleado,
                nombreCompletoTecnico = $"{tecnico.Empleado?.nombre} {tecnico.Empleado?.apellidoPaterno}",
                horaInicio = tecnico.horaInicio,
                horaTermino = tecnico.horaTermino,
                solucion = tecnico.solucion,
                idStatusAprobacionTecnico = tecnico.idStatusAprobacionTecnico,
                nombreStatusAprobacionTecnico = tecnico.StatusAprobacionTecnico?.descripcionStatusAprobacionTecnico,
                comentarioPausa = tecnico.comentarioPausa,
                esTecnicoActivo = tecnico.esTecnicoActivo,
                Refacciones = tecnico.Asignacion_Refacciones.Select(refaccion => new Asignacion_RefaccionesDetallesDTO
                {
                    idAsignacionRefaccion = refaccion.idAsignacionRefaccion,
                    idAsignacion = refaccion.idAsignacion,
                    idRefaccion = refaccion.idRefaccion,
                    nombreRefaccion = refaccion.Inventario?.nombreProducto,
                    idAsignacionTecnico = refaccion.idAsignacionTecnico,
                    cantidad = refaccion.cantidad
                }).ToList()
            }).ToList();

            // Mapear la solicitud a un DTO "plano"
            var solicitudDetalleDTO = new SolicitudesDetalleDTO
            {
                idSolicitud = asignacion.Solicitud?.idSolicitud ?? 0,
                descripcion = asignacion.Solicitud?.descripcion,
                fechaSolicitud = asignacion.Solicitud?.fechaSolicitud ?? DateTime.MinValue,
                nombreCompletoEmpleado = $"{asignacion.Solicitud?.Empleado?.nombre} {asignacion.Solicitud?.Empleado?.apellidoPaterno}",
                idMaquina = asignacion.Solicitud?.idMaquina ?? 0,
                idTurno = asignacion.Solicitud?.idTurno ?? 0,
                idStatusOrden = asignacion.Solicitud?.idStatusOrden ?? 0,
                idStatusAprobacionSolicitante = asignacion.Solicitud?.idStatusAprobacionSolicitante ?? 0,
                area = areaSeleccionada?.nombreArea,
                rol = rolSeleccionado?.nombreRol,
                idCategoriaTicket = asignacion.Solicitud?.idCategoriaTicket ?? 0,
                nombreMaquina = asignacion.Solicitud?.Maquina?.nombreMaquina,
                nombreTurno = asignacion.Solicitud?.Turno?.descripcion,
                nombreStatusOrden = asignacion.Solicitud?.StatusOrden?.descripcionStatusOrden,
                nombreStatusAprobacionSolicitante = asignacion.Solicitud?.StatusAprobacionSolicitante?.descripcionStatusAprobacionSolicitante,
                nombreCategoriaTicket = asignacion.Solicitud?.categoriaTicket?.descripcionCategoriaTicket
            };

            // Mapear la asignación principal a AsignacionDetallesDTO, usando el DTO plano de la solicitud y los técnicos
            var asignacionDetallesDTO = new AsignacionDetallesDTO
            {
                idAsignacion = asignacion.idAsignacion,
                idSolicitud = asignacion.idSolicitud,
                idStatusAsignacion = asignacion.idStatusAsignacion,
                nombreStatusAsignacion = asignacion.StatusAsignacion?.descripcionStatusAsignacion,
                Solicitud = solicitudDetalleDTO,
                Tecnicos = tecnicoDetalles
            };

            return asignacionDetallesDTO;
        }

        #region Métodos de Cálculo de KPIs

        /// <summary>
        /// Calcula el MTTR (Mean Time To Repair) en minutos. Recorre cada registro de técnico en cada asignación finalizada,
        /// sumando el tiempo de reparación (diferencia entre horaTermino y horaInicio) siempre que los valores sean válidos.
        /// </summary>
        /// <param name="idMaquina">Identificador de la máquina.</param>
        /// <param name="idArea">Identificador del área.</param>
        /// <param name="idEmpleado">Opcional: para filtrar por un técnico específico.</param>
        /// <returns>Promedio de tiempo de reparación en minutos.</returns>
        /*public async Task<double> CalcularMTTA(int idMaquina, int idArea)
        {
            if (idArea == 19) return 0;

            var solicitudes = await _solicitudRepository
                .ConsultarSolicitudesPorMaquinaYArea(idMaquina, idArea);

            double sumaEsperaTotal = 0;
            int count = 0;

            foreach (var s in solicitudes)
            {
                var asignacion = s.Asignaciones?
                    .FirstOrDefault(a => a.idStatusAsignacion >= 1);
                if (asignacion == null) continue;

                var techs = asignacion.Asignacion_Tecnico
                    .OrderBy(t => t.horaInicio)
                    .ToList();
                if (techs.Count < 1) continue;

                // 1) Espera inicial
                double espera = (techs[0].horaInicio - s.fechaSolicitud).TotalMinutes;

                // 2) Para cada re-toma, suma la pausa anterior
                for (int i = 1; i < techs.Count; i++)
                {
                    DateTime finAnterior = techs[i - 1].horaTermino != DateTime.MinValue
                        ? techs[i - 1].horaTermino
                        : techs[i - 1].horaInicio.AddMinutes(techs[i - 1].tiempoAcumuladoMinutos);

                    espera += (techs[i].horaInicio - finAnterior).TotalMinutes;
                }

                // 3) Restar TODO el tiempo de pausas (manuales + sistema)
                espera -= asignacion.tiempoEsperaAcumuladoMinutos;

                sumaEsperaTotal += espera;
                count++;
            }

            return (count > 0) ? (sumaEsperaTotal / count) / 60.0 : 0; //de minutos lo convierte a horas y se guarda como horas

        }*/

        //ovalado y piedra chiquita


        //

        public async Task<double> CalcularMTTA(int idMaquina, int idArea, DateTime? fechaHasta = null)
        {
            if (idArea == 19) return 0;

            var solicitudes = await _solicitudRepository
                .ConsultarSolicitudesPorMaquinaYArea2(idMaquina, idArea, fechaHasta);

            double sumaEsperaTotal = 0;
            int count = 0;

            foreach (var s in solicitudes)
            {
                var asignacion = s.Asignaciones?
                    .Where(a => a.idStatusAsignacion >= 1)
                    .OrderBy(a => a.Asignacion_Tecnico.Min(t => t.horaInicio))
                    .FirstOrDefault(a =>
                        !fechaHasta.HasValue ||
                        a.Asignacion_Tecnico.Min(t => t.horaInicio) <= fechaHasta.Value

                     );

                if (asignacion == null) continue;

                var techs = asignacion.Asignacion_Tecnico
                    .OrderBy(t => t.horaInicio)
                    .ToList();
                if (techs.Count < 1) continue;

                // 1) Espera inicial
                double espera = (techs[0].horaInicio - s.fechaSolicitud).TotalMinutes;

                // 2) Para cada re-toma, suma la pausa anterior
                for (int i = 1; i < techs.Count; i++)
                {
                    DateTime finAnterior = techs[i - 1].horaTermino != DateTime.MinValue
                        ? techs[i - 1].horaTermino
                        : techs[i - 1].horaInicio.AddMinutes(techs[i - 1].tiempoAcumuladoMinutos);

                    espera += (techs[i].horaInicio - finAnterior).TotalMinutes;
                }

                // 3) Restar TODO el tiempo de pausas (manuales + sistema)
                espera -= asignacion.tiempoEsperaAcumuladoMinutos;

                sumaEsperaTotal += espera;
                count++;
            }

            return (count > 0) ? (sumaEsperaTotal / count) / 60.0 : 0; //de minutos lo convierte a horas y se guarda como horas

        }

        /*public async Task<double> CalcularMTTR(int idMaquina, int idArea, int? idEmpleado = null)
        {
            if (idArea == 19) return 0;

            var asignaciones = await _repository.ConsultarAsignacionesCompletadas(idMaquina, idArea, null);
            if (!asignaciones.Any()) return 0;

            double sumaTotal = 0;
            int count = 0;

            foreach (var asignacion in asignaciones)
            {
                if (idEmpleado == null)
                {
                    double sumaGlobal = asignacion.Asignacion_Tecnico
                        .Where(t => t.tiempoAcumuladoMinutos > 0)
                        .Sum(t => t.tiempoAcumuladoMinutos);

                    sumaTotal += sumaGlobal;
                }
                else
                {
                    double tiempoTecnico = asignacion.Asignacion_Tecnico
                        .Where(t => t.idEmpleado == idEmpleado && t.tiempoAcumuladoMinutos > 0)
                        .Sum(t => t.tiempoAcumuladoMinutos);

                    if (tiempoTecnico > 0)
                    {
                        sumaTotal += tiempoTecnico;
                        count++;
                    }
                }
            }

            return (idEmpleado == null)
                ? (asignaciones.Count() > 0 ? (sumaTotal / asignaciones.Count()) / 60.0 : 0) // en horas
                : (count > 0 ? (sumaTotal / count) / 60.0 : 0); // en horas
        }*/

        public async Task<double> CalcularMTTR(int idMaquina, int idArea, int? idEmpleado = null, DateTime? fechaHasta = null)
        {
            if (idArea == 19) return 0;

            var asignaciones = await _repository.ConsultarAsignacionesCompletadas(idMaquina, idArea, null, fechaHasta);
            if (!asignaciones.Any()) return 0;

            double sumaTotal = 0;
            int count = 0;

            foreach (var asignacion in asignaciones)
            {
                if (idEmpleado == null)
                {
                    double sumaGlobal = asignacion.Asignacion_Tecnico
                        .Where(t => t.tiempoAcumuladoMinutos > 0 && (!fechaHasta.HasValue || t.horaInicio <= fechaHasta.Value))
                        .Sum(t => t.tiempoAcumuladoMinutos);

                    sumaTotal += sumaGlobal;
                }
                else
                {
                    double tiempoTecnico = asignacion.Asignacion_Tecnico
                        .Where(t => t.idEmpleado == idEmpleado && t.tiempoAcumuladoMinutos > 0 && (!fechaHasta.HasValue || t.horaInicio <= fechaHasta.Value))
                        .Sum(t => t.tiempoAcumuladoMinutos);

                    if (tiempoTecnico > 0)
                    {
                        sumaTotal += tiempoTecnico;
                        count++;
                    }
                }
            }

            return (idEmpleado == null)
                ? (asignaciones.Count() > 0 ? (sumaTotal / asignaciones.Count()) / 60.0 : 0) // en horas
                : (count > 0 ? (sumaTotal / count) / 60.0 : 0); // en horas
        }

        public async Task<double> CalcularMTBF(int idArea, int anio, int mes)
        {
            // 1) Obtener el día de la primera solicitud en ese mes:
            int? primerDiaNullable = await _solicitudRepository
                .ObtenerPrimerDiaSolicitudPorAreaEnMes(idArea, anio, mes);

            // Si no hay solicitudes, retorno 0
            if (primerDiaNullable == null)
                return 0;

            int primerDia = primerDiaNullable.Value;
            // Ejemplo: primerDia = 19 si la primera falla en mayo es 19/05/2025

            // 2) Contar cuántas fallas hay desde "primerDia" hasta fin de mes
            int numeroParadas = await _solicitudRepository
                .ContarFallasPorAreaEnMesDesdeDia(idArea, anio, mes, primerDia);

            // Si no hay ninguna falla en ese rango, retorno 0
            if (numeroParadas == 0)
                return 0;

            // 3) Cantidad de máquinas activas en el área
            int numMaquinasActivas = await _maquinaRepository
                .ContarMaquinasActivasPorArea(idArea);
            if (numMaquinasActivas == 0)
                return 0;

            // 4) Días del mes completo
            int diasDelMes = DateTime.DaysInMonth(anio, mes);

            // 5) Calcular “días ajustados”:
            var hoy = DateTime.Today;
            int diasAjustados;
            if (anio == hoy.Year && mes == hoy.Month)
            {
                // Mes actual: solo hasta el día de hoy
                int diaHoy = hoy.Day;
                diasAjustados = diaHoy - (primerDia - 1);
            }
            else
            {
                // Mes pasado o mes completo: hasta fin de mes
                diasAjustados = diasDelMes - (primerDia - 1);
            }
            diasAjustados = Math.Max(diasAjustados, 0);

            // 6) "Días efectivos" en el mes = (22 / díasDelMes) * díasAjustados
            double diasEfectivos = (22.0 / diasDelMes) * diasAjustados;

            // 7) Horas por máquina en esos días efectivos = díasEfectivos × 21.75
            double horasPorMaquinaEnMes = diasEfectivos * 21.75;

            // 8) Horas totales disponibles = horasPorMaquinaEnMes × numMaquinasActivas
            double horasTotalesDisponibles = horasPorMaquinaEnMes * numMaquinasActivas;

            // 9) MTBF = horasTotalesDisponibles / numeroParadas
            double mtbfHoras = horasTotalesDisponibles / numeroParadas;
            return mtbfHoras;
        }


        /// <summary>
        /// Recorre los meses del año y devuelve un DTO con el MTBF calculado
        /// para cada mes (usando la nueva fórmula). 
        /// </summary>
        public async Task<List<KpiSegmentadoDTO>> ObtenerMTBFPorAreaMes(int idArea, int anio)
        {
            var listaSegmentada = new List<KpiSegmentadoDTO>();

            // Para cada mes de 1 a 12
            for (int mes = 1; mes <= 12; mes++)
            {
                double valorHoras = await CalcularMTBF(idArea, anio, mes);

                // Nombre del mes en la cultura actual (p.ej. "enero", "febrero", …)
                string nombreMes = CultureInfo.CurrentCulture
                    .DateTimeFormat
                    .GetMonthName(mes);

                listaSegmentada.Add(new KpiSegmentadoDTO
                {
                    etiqueta = nombreMes,
                    valor = (float)valorHoras
                });
            }

            return listaSegmentada;
        }

        /// <summary>
        /// Si aún necesitas el MTBF “hasta hoy” (día a día) con la nueva lógica, 
        /// podrías adaptar CalcularMTBF_DiaAHoy de esta manera:
        ///
        ///   MTBF_DiaAHoy = 
        ///     { 
        ///       [((22 / DíasMes) × DíasAjustadosHastaHoy) × 21.75 × MáquinasActivas] 
        ///         / paradasHastaHoy 
        ///       , si paradasHastaHoy > 0 
        ///       ; 0 en otro caso
        ///     }
        ///
        ///   Aquí “DíasAjustadosHastaHoy” = (díaHoy – (primerDia – 1)), 
        ///   pero no supera (díasDelMes – (primerDia – 1)) si estamos después del mes.
        /// </summary>
        public async Task<double> CalcularMTBF_DiaAHoy(int idArea, int anio, int mes, int diaHoy)
        {
            // 1) Paradas acumuladas HASTA el día “diaHoy” del mes
            int paradasHastaHoy = await _solicitudRepository
                .ContarFallasPorAreaEnMesHastaDia(idArea, anio, mes, diaHoy);

            if (paradasHastaHoy == 0)
                return 0;

            // 2) Cantidad de máquinas activas en el área
            int numMaquinasActivas = await _maquinaRepository
                .ContarMaquinasActivasPorArea(idArea);

            if (numMaquinasActivas == 0)
                return 0;

            // 3) Obtener primer día de solicitud en ese mes
            int? primerDiaNullable = await _solicitudRepository
                .ObtenerPrimerDiaSolicitudPorAreaEnMes(idArea, anio, mes);

            if (primerDiaNullable == null)
                return 0;

            int primerDia = primerDiaNullable.Value;

            // 4) Días del mes
            int diasDelMes = DateTime.DaysInMonth(anio, mes);

            // 5) Calcular "días ajustados hasta hoy":
            //    No puede ser negativo, ni mayor que (díasDelMes - (primerDia - 1))
            int diasAjustadosHastaHoy = diaHoy - (primerDia - 1);
            diasAjustadosHastaHoy = Math.Max(diasAjustadosHastaHoy, 0);
            diasAjustadosHastaHoy = Math.Min(diasAjustadosHastaHoy, diasDelMes - (primerDia - 1));

            // 6) “Días efectivos hasta hoy” = (22 / díasDelMes) * diasAjustadosHastaHoy
            double diasEfectivosHastaHoy = (22.0 / diasDelMes) * diasAjustadosHastaHoy;

            // 7) Horas por máquina hasta hoy = diasEfectivosHastaHoy × 21.75
            double horasPorMaquinaHastaHoy = diasEfectivosHastaHoy * 21.75;

            // 8) Horas totales disponibles hasta hoy = horasPorMaquinaHastaHoy × numMaquinasActivas
            double horasDisponiblesHastaHoy = horasPorMaquinaHastaHoy * numMaquinasActivas;

            // 9) MTBF hasta hoy = horasDisponiblesHastaHoy / paradasHastaHoy
            double mtbfHoras = horasDisponiblesHastaHoy / paradasHastaHoy;

            return mtbfHoras;
        }


        #endregion

        public async Task GuardarKPIs(int idMaquina, int idArea, int? idEmpleado = null)
        {
            if (idArea == 19) return;

            var asignaciones = await _repository.ConsultarAsignacionesCompletadas(idMaquina, idArea, null);
            if (!asignaciones.Any()) return;

            var anioActual = DateTime.Now.Year;
            var mesActual = DateTime.Now.Month;

            var mtta = await CalcularMTTA(idMaquina, idArea); // este también en horas
            var mtbf = await CalcularMTBF(idArea, anioActual, mesActual);
            var mttrGlobal = await CalcularMTTR(idMaquina, idArea, null); // ahora en horas

            // Técnicos involucrados
            var tecnicos = asignaciones
                .SelectMany(a => a.Asignacion_Tecnico)
                .Where(t => t.tiempoAcumuladoMinutos > 0)
                .Select(t => t.idEmpleado)
                .Distinct()
                .ToList();

            bool esPrimero = true;

            foreach (var tecnicoId in tecnicos)
            {
                var mttrIndividual = await CalcularMTTR(idMaquina, idArea, tecnicoId); // en horas

                var kpiMantenimiento = new KpisMantenimiento
                {
                    idMaquina = idMaquina,
                    idArea = idArea,
                    idEmpleado = tecnicoId,
                    fechaCalculo = DateTime.Now
                };

                await _kpiRepository.GuardarKPIMantenimiento(kpiMantenimiento);

                var detalles = new List<KpisDetalle>
        {
            new KpisDetalle { kpiNombre = "MTTR", kpiValor = (float)mttrIndividual }
        };

                if (esPrimero)
                {
                    detalles.Add(new KpisDetalle { kpiNombre = "MTTA", kpiValor = (float)mtta });
                    detalles.Add(new KpisDetalle { kpiNombre = "MTTR_Global", kpiValor = (float)mttrGlobal });
                    detalles.Add(new KpisDetalle { kpiNombre = "MTBF", MTBF_HorasNueva = mtbf });
                    esPrimero = false;
                }

                await _kpiRepository.GuardarKPIDetalles(kpiMantenimiento.idKPIMantenimiento, detalles);
            }
        }


    }
}
