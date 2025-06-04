using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Repositories.Impl;
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
                nombreCompletoTecnico = $"{tecnico.Empleado?.nombre} {tecnico.Empleado?.apellidoPaterno} {tecnico.Empleado?.apellidoMaterno}",
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
                nombreCompletoEmpleado = $"{asignacion.Solicitud?.Empleado?.nombre} {asignacion.Solicitud?.Empleado?.apellidoPaterno} {asignacion.Solicitud?.Empleado?.apellidoMaterno}",
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
        public async Task<double> CalcularMTTR(int idMaquina, int idArea, int? idEmpleado = null)
        {
            var asignaciones = await _repository.ConsultarAsignacionesCompletadas(idMaquina, idArea, idEmpleado);
            if (!asignaciones.Any())
                return 0;

            double tiempoTotalReparacion = 0;
            int count = 0;

            foreach (var asignacion in asignaciones)
            {
                foreach (var tecnico in asignacion.Asignacion_Tecnico)
                {
                    // En lugar de restar (horaTermino - horaInicio),
                    // usamos tecnico.tiempoAcumuladoMinutos:
                    if (tecnico.tiempoAcumuladoMinutos > 0)
                    {
                        tiempoTotalReparacion += tecnico.tiempoAcumuladoMinutos;
                        count++;
                    }
                }
            }
            return count > 0 ? tiempoTotalReparacion / count : 0;
        }

        /*public async Task<double> CalcularMTTR(int idMaquina, int idArea, int? idEmpleado = null)
        {
            var asignaciones = await _repository.ConsultarAsignacionesCompletadas(idMaquina, idArea, idEmpleado);
            if (!asignaciones.Any())
                return 0;

            double sumaTotalPorOrdenes = 0;
            foreach (var asignacion in asignaciones)
            {
                double sumaSegmentos = asignacion.Asignacion_Tecnico
                    .Where(t => t.tiempoAcumuladoMinutos > 0)
                    .Sum(t => t.tiempoAcumuladoMinutos);

                sumaTotalPorOrdenes += sumaSegmentos;
            }

            // Aquí llamamos correctamente a Count()
            return sumaTotalPorOrdenes / asignaciones.Count();
        }*/


        /// <summary>
        /// Calcula el MTTA (Mean Time To Acknowledge) en minutos. Se toma el tiempo transcurrido entre la creación
        /// de la solicitud y el inicio del primer técnico asignado para cada solicitud.
        /// </summary>
        /// <param name="idMaquina">Identificador de la máquina.</param>
        /// <param name="idArea">Identificador del área.</param>
        /// <returns>Promedio de tiempo de asignación (MTTA) en minutos.</returns>
        public async Task<double> CalcularMTTA(int idMaquina, int idArea)
        {
            // Obtiene solicitudes
            var solicitudes = await _solicitudRepository.ConsultarSolicitudesPorMaquinaYArea(idMaquina, idArea);
            if (!solicitudes.Any())
                return 0;

            double tiempoTotal = 0;
            int count = 0;

            foreach (var s in solicitudes)
            {
                // Podrías buscar la asignación principal o la primera
                var asignacion = s.Asignaciones?.FirstOrDefault(a => a.idStatusAsignacion >= 1);
                if (asignacion == null) continue;

                // Encuentra primer técnico
                var primerTecnico = asignacion.Asignacion_Tecnico.OrderBy(t => t.horaInicio).FirstOrDefault();
                if (primerTecnico == null) continue;

                // Espera inicial: primerTecnico.horaInicio - s.fechaSolicitud
                double esperaInicial = (primerTecnico.horaInicio - s.fechaSolicitud).TotalMinutes;

                // Espera por pausas: asignacion.tiempoEsperaAcumuladoMinutos
                // (Asumiendo que has ido sumando en cada pausa)
                double esperaPausas = asignacion.tiempoEsperaAcumuladoMinutos;

                tiempoTotal += (esperaInicial + esperaPausas);
                count++;
            }

            return (count > 0) ? (tiempoTotal / count) : 0;
        }

        /*public async Task<double> CalcularMTTA(int idMaquina, int idArea)
        {
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
                    // Calcula finAnterior sin usar '??'
                    DateTime finAnterior;
                    if (techs[i - 1].horaTermino != DateTime.MinValue)
                        finAnterior = techs[i - 1].horaTermino;
                    else
                        finAnterior = techs[i - 1].horaInicio.AddMinutes(techs[i - 1].tiempoAcumuladoMinutos);

                    // Espera desde finAnterior hasta el nuevo inicio
                    espera += (techs[i].horaInicio - finAnterior).TotalMinutes;
                }

                sumaEsperaTotal += espera;
                count++;
            }

            return (count > 0) ? sumaEsperaTotal / count : 0;
        }*/


        /// <summary>
        /// Calcula el MTBF (Mean Time Between Failures) en minutos. Dado que no se cuenta con la fecha de instalación de la máquina,
        /// se utiliza la fecha mínima de solicitud registrada para la máquina y área como proxy del inicio de operación.
        /// </summary>
        /// <param name="idMaquina">Identificador de la máquina.</param>
        /// <param name="idArea">Identificador del área.</param>
        /// <returns>Promedio de tiempo entre fallas en minutos.</returns>
        /*public async Task<double> CalcularMTBF(int idMaquina, int idArea)
        {
            var solicitudes = await _solicitudRepository.ConsultarSolicitudesPorMaquinaYArea(idMaquina, idArea);
            if (!solicitudes.Any())
                return 0;

            // Se utiliza la fecha mínima de solicitud como proxy para el inicio de operación
            DateTime fechaInicioOperacion = solicitudes.Min(s => s.fechaSolicitud);
            double tiempoTotalOperacion = (DateTime.Now - fechaInicioOperacion).TotalMinutes;
            int cantidadFallas = solicitudes.Count(); // Se asume cada solicitud es una "falla"
            return cantidadFallas > 0 ? tiempoTotalOperacion / cantidadFallas : 0;
        }*/

        /// <summary>
        /// Calcula el MTBF (Mean Time Between Failures) en minutos.
        /// Se resta el tiempo perdido en reparaciones (MTTR) del tiempo total 
        /// disponible desde la primera falla hasta ahora.
        /// </summary>
        public async Task<double> CalcularMTBF(int idMaquina, int idArea)
        {
            // 1) Todas las solicitudes (cada una es una falla)
            var solicitudes = await _solicitudRepository
                .ConsultarSolicitudesPorMaquinaYArea(idMaquina, idArea);
            if (!solicitudes.Any())
                return 0;

            // 2) Fecha de inicio de operación = primera solicitud (proxy de instalación)
            DateTime fechaInicioOperacion = solicitudes.Min(s => s.fechaSolicitud);

            // 3) Tiempo total disponible en minutos
            double tiempoTotalDisponible = (DateTime.Now - fechaInicioOperacion).TotalMinutes;

            // 4) Sumar todo el tiempo perdido (MTTR) en minutos
            double tiempoPerdido = 0;
            foreach (var sol in solicitudes)
            {
                // Obtenemos la asignación principal (fase de reparación)
                var asign = sol.Asignaciones?
                    .FirstOrDefault(a => a.idStatusAsignacion >= 1);
                if (asign == null)
                    continue;

                // Sumamos el tiempo acumulado de cada técnico en esa falla
                tiempoPerdido += asign.Asignacion_Tecnico
                    .Sum(t => t.tiempoAcumuladoMinutos);
            }

            // 5) Número de paradas = número de solicitudes (fallas)
            int numeroParadas = solicitudes.Count();

            // 6) Aplicar fórmula MTBF = (Disponible – Perdido) / Paradas
            var mtbfMinutos = numeroParadas > 0
                ? (tiempoTotalDisponible - tiempoPerdido) / numeroParadas
                : 0;

            return mtbfMinutos;
        }


        #endregion

        public async Task GuardarKPIs(int idMaquina, int idArea, int? idEmpleado = null)
        {
            var mttr = await CalcularMTTR(idMaquina, idArea, idEmpleado);
            var mtta = await CalcularMTTA(idMaquina, idArea);
            var mtbf = await CalcularMTBF(idMaquina, idArea);

            // Si la propiedad idEmpleado en el modelo es no nullable, se asigna un valor por defecto (por ejemplo, 0)
            var kpiMantenimiento = new KpisMantenimiento
            {
                idMaquina = idMaquina,
                idArea = idArea,
                idEmpleado = idEmpleado ?? 0,
                fechaCalculo = DateTime.Now
            };

            await _kpiRepository.GuardarKPIMantenimiento(kpiMantenimiento);

            var kpiDetalles = new List<KpisDetalle>
            {
                new KpisDetalle { kpiNombre = "MTTR", kpiValor = (float)mttr },
                new KpisDetalle { kpiNombre = "MTTA", kpiValor = (float)mtta },
                new KpisDetalle { kpiNombre = "MTBF", kpiValor = (float)mtbf }
            };

            await _kpiRepository.GuardarKPIDetalles(kpiMantenimiento.idKPIMantenimiento, kpiDetalles);
        }



    }
}
