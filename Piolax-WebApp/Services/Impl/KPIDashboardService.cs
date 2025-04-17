using Piolax_WebApp.DTOs;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Repositories.Impl;

namespace Piolax_WebApp.Services.Impl
{
    public class KPIDashboardService(IKPIRepository repository, IAsignacionService asignacionService) : IKPIDashboardService
    {
        private readonly IKPIRepository _repository = repository;
        private readonly IAsignacionService _asignacionService = asignacionService;

        /// <summary>
        /// Obtiene el MTTA filtrado por 
        /// área y/o máquina
        /// </summary>
        public async Task<KPIResponseDTO> ObtenerMTTA(int? idArea = null, int? idMaquina = null, int? anio = null, int? mes = null)
        {
            var kpiDetalles = await _repository.ConsultarMTTA(idArea, idMaquina, anio, mes);
            if (!kpiDetalles.Any())
                return new KPIResponseDTO { Nombre = "MTTA", Valor = 0, UnidadMedida = "minutos" };

            // Calcular el promedio de los valores de MTTA
            float valorPromedio = kpiDetalles.Average(k => k.kpiValor);

            return new KPIResponseDTO
            {
                Nombre = "MTTA",
                Valor = valorPromedio,
                UnidadMedida = "minutos"
            };
        }

        /// <summary>
        /// Obtiene el MTTR filtrado por área, máquina y/o técnico
        /// </summary>
        public async Task<KPIResponseDTO> ObtenerMTTR(int? idArea = null, int? idMaquina = null, int? idEmpleado = null, int? anio = null, int? mes = null)
        {
            var kpiDetalles = await _repository.ConsultarMTTR(idArea, idMaquina, idEmpleado, anio, mes);
            if (!kpiDetalles.Any())
                return new KPIResponseDTO { Nombre = "MTTR", Valor = 0, UnidadMedida = "minutos" };

            // Calcular el promedio de los valores de MTTR
            float valorPromedio = kpiDetalles.Average(k => k.kpiValor);

            return new KPIResponseDTO
            {
                Nombre = "MTTR",
                Valor = valorPromedio,
                UnidadMedida = "minutos"
            };
        }

        /// Obtiene el MTBF filtrado por área
        /// </summary>
        public async Task<KPIResponseDTO> ObtenerMTBF(int? idArea = null)
        {
            var kpiDetalles = await _repository.ConsultarMTBF(idArea);
            if (!kpiDetalles.Any())
                return new KPIResponseDTO { Nombre = "MTBF", Valor = 0, UnidadMedida = "minutos" };

            // Calcular el promedio de los valores de MTBF
            float valorPromedio = kpiDetalles.Average(k => k.kpiValor);

            return new KPIResponseDTO
            {
                Nombre = "MTBF",
                Valor = valorPromedio,
                UnidadMedida = "minutos"
            };
        }

        /// <summary>
        /// Calcula el tiempo total de inactividad (TotalDowntime) filtrado por área, máquina y período de tiempo
        /// </summary>
        public async Task<KPIResponseDTO> ObtenerTotalDowntime(int? idArea = null, int? idMaquina = null, int? año = null, int? mes = null, int? semana = null, int? diaSemana = null)
        {
            // Obtenemos los KPIs de mantenimiento filtrados
            var kpisMantenimiento = await _repository.ConsultarTotalDowntime(idArea, idMaquina, año, mes, semana, diaSemana);

            if (!kpisMantenimiento.Any())
                return new KPIResponseDTO { Nombre = "TotalDowntime", Valor = 0, UnidadMedida = "minutos" };

            float totalDowntime = 0;

            // Para cada KPI de mantenimiento, obtenemos sus detalles y sumamos los tiempos de MTTR
            foreach (var kpi in kpisMantenimiento)
            {
                // Asumimos que MTTR está en los detalles de cada KPI
                var mttrDetalle = kpi.KpisDetalle.FirstOrDefault(d => d.kpiNombre == "MTTR");
                if (mttrDetalle != null)
                {
                    totalDowntime += mttrDetalle.kpiValor;
                }
            }

            return new KPIResponseDTO
            {
                Nombre = "TotalDowntime",
                Valor = totalDowntime,
                UnidadMedida = "minutos"
            };
        }

        /// <summary>
        /// Obtiene un resumen de todos los KPIs aplicando los filtros correspondientes a cada uno
        /// </summary>
        public async Task<IEnumerable<KPIResponseDTO>> ObtenerResumenKPIs(
            int? idAreaMTTA = null, int? idMaquinaMTTA = null,
            int? idAreaMTTR = null, int? idMaquinaMTTR = null, int? idEmpleadoMTTR = null,
            int? idAreaMTBF = null,
            int? idAreaDowntime = null, int? idMaquinaDowntime = null,
            int? anioDowntime = null, int? mesDowntime = null, int? semanaDowntime = null, int? diaSemanaDowntime = null)
        {
            var kpis = new List<KPIResponseDTO>();

            // Obtener MTTA
            var mtta = await ObtenerMTTA(idAreaMTTA, idMaquinaMTTA, anioDowntime, mesDowntime);
            kpis.Add(mtta);

            // Obtener MTTR
            var mttr = await ObtenerMTTR(idAreaMTTR, idMaquinaMTTR, idEmpleadoMTTR);
            kpis.Add(mttr);

            // Obtener MTBF
            var mtbf = await ObtenerMTBF(idAreaMTBF);
            kpis.Add(mtbf);

            // Obtener TotalDowntime
            var totalDowntime = await ObtenerTotalDowntime(idAreaDowntime, idMaquinaDowntime,
                anioDowntime, mesDowntime, semanaDowntime, diaSemanaDowntime);
            kpis.Add(totalDowntime);

            return kpis;

        }

    }
}
