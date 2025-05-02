using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Repositories.Impl;
using System.Globalization;

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
        public async Task<KPIResponseDTO> ObtenerMTTA(int? idArea = null, int? idMaquina = null, int? anio = null, int? mes = null, int? semana = null, int? diaSemana = null)
        {
            var kpiDetalles = await _repository.ConsultarMTTA(idArea, idMaquina, anio, mes, semana, diaSemana);
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
        ////////// FILTROS MTT  //////////////////
        public async Task<List<KpiSegmentadoDTO>> ObtenerMTTASegmentado(
        int? idArea = null, int? idMaquina = null, int? anio = null, int? mes = null, int? semana = null, int? diaSemana = null){
            var kpiDetalles = await _repository.ConsultarMTTA(idArea, idMaquina, anio, mes, semana, diaSemana);

            if (!kpiDetalles.Any())
                return new List<KpiSegmentadoDTO>();

            if (semana.HasValue && diaSemana.HasValue)
            {
                // ✅ Filtrado por SEMANA + DÍA: mostrar solo ese día
                return kpiDetalles
                    .Where(k => (int)k.KpisMantenimiento.fechaCalculo.DayOfWeek == diaSemana.Value)
                    .GroupBy(k => k.KpisMantenimiento.fechaCalculo.Date)
                    .Select(g => new KpiSegmentadoDTO
                    {
                        Etiqueta = g.Key.ToString("dd/MM/yyyy"),
                        Valor = g.Average(x => x.kpiValor)
                    })
                    .ToList();
            }
            else if (semana.HasValue)
            {
                // ✅ Filtrado por SEMANA: agrupar por días
                return kpiDetalles
                    .GroupBy(k => k.KpisMantenimiento.fechaCalculo.DayOfWeek)
                    .Select(g => new KpiSegmentadoDTO
                    {
                        Etiqueta = g.Key.ToString(), // Lunes, Martes...
                        Valor = g.Average(x => x.kpiValor)
                    })
                    .OrderBy(g => g.Etiqueta)
                    .ToList();
            }
            else if (mes.HasValue)
            {
                // ✅ Filtrado por MES: agrupar por semanas
                return kpiDetalles
                    .GroupBy(k => System.Globalization.ISOWeek.GetWeekOfYear(k.KpisMantenimiento.fechaCalculo))
                    .Select(g => new KpiSegmentadoDTO
                    {
                        Etiqueta = $"Semana {g.Key}",
                        Valor = g.Average(x => x.kpiValor)
                    })
                    .OrderBy(g => g.Etiqueta)
                    .ToList();
            }
            else if (anio.HasValue)
            {
                // ✅ Filtrado por AÑO: agrupar por meses
                return kpiDetalles
                    .GroupBy(k => k.KpisMantenimiento.fechaCalculo.Month)
                    .Select(g => new KpiSegmentadoDTO
                    {
                        Etiqueta = $"Mes {g.Key}",
                        Valor = g.Average(x => x.kpiValor)
                    })
                    .OrderBy(g => g.Etiqueta)
                    .ToList();
            }
            else
            {
                float promedio = kpiDetalles.Average(x => x.kpiValor);
                return new List<KpiSegmentadoDTO>
        {
            new KpiSegmentadoDTO { Etiqueta = "Promedio General", Valor = promedio }
        };
            }
        }

        /// Obtiene el MTTR filtrado por área, máquina y/o técnico
        public async Task<KPIResponseDTO> ObtenerMTTR(int? idArea = null, int? idMaquina = null, int? idEmpleado = null, int? anio = null, int? mes = null, int? semana = null, int? diaSemana = null)
        {
            var kpiDetalles = await _repository.ConsultarMTTR(idArea, idMaquina, idEmpleado, anio, mes, semana, diaSemana);
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
        ////////segmento para que funcionen los  filtros por promedio////
        public async Task<List<KpiSegmentadoDTO>> ObtenerMTTRSegmentado(
        int? idArea = null, int? idMaquina = null, int? idEmpleado = null, int? anio = null, int? mes = null, int? semana = null, int? diaSemana = null){

            var kpiDetalles = await _repository.ConsultarMTTR(idArea, idMaquina, idEmpleado, anio, mes, semana, diaSemana);

            if (!kpiDetalles.Any())
                return new List<KpiSegmentadoDTO>();

            if (semana.HasValue && diaSemana.HasValue)
            {
                // ✅ Si filtras SEMANA y DÍA: buscar solo ese día
                return kpiDetalles
                    .Where(k => (int)k.KpisMantenimiento.fechaCalculo.DayOfWeek == diaSemana.Value)
                    .GroupBy(k => k.KpisMantenimiento.fechaCalculo.Date)
                    .Select(g => new KpiSegmentadoDTO
                    {
                        Etiqueta = g.Key.ToString("dd/MM/yyyy"),
                        Valor = g.Average(x => x.kpiValor)
                    })
                    .ToList();
            }
            else if (semana.HasValue)
            {
                // ✅ Si filtras solo SEMANA: agrupar por día de la semana
                return kpiDetalles
                    .GroupBy(k => k.KpisMantenimiento.fechaCalculo.DayOfWeek)
                    .Select(g => new KpiSegmentadoDTO
                    {
                        Etiqueta = g.Key.ToString(), // "Monday", "Tuesday", etc.
                        Valor = g.Average(x => x.kpiValor)
                    })
                    .OrderBy(g => g.Etiqueta)
                    .ToList();
            }
            else if (mes.HasValue)
            {
                // ✅ Si filtras solo MES: agrupar por semana
                return kpiDetalles
                    .GroupBy(k => System.Globalization.ISOWeek.GetWeekOfYear(k.KpisMantenimiento.fechaCalculo))
                    .Select(g => new KpiSegmentadoDTO
                    {
                        Etiqueta = $"Semana {g.Key}",
                        Valor = g.Average(x => x.kpiValor)
                    })
                    .OrderBy(g => g.Etiqueta)
                    .ToList();
            }
            else if (anio.HasValue)
            {
                // ✅ Si filtras solo AÑO: agrupar por mes
                return kpiDetalles
                    .GroupBy(k => k.KpisMantenimiento.fechaCalculo.Month)
                    .Select(g => new KpiSegmentadoDTO
                    {
                        Etiqueta = $"Mes {g.Key}",
                        Valor = g.Average(x => x.kpiValor)
                    })
                    .OrderBy(g => g.Etiqueta)
                    .ToList();
            }
            else
            {
                float promedio = kpiDetalles.Average(x => x.kpiValor);
                return new List<KpiSegmentadoDTO>
        {
            new KpiSegmentadoDTO { Etiqueta = "Promedio General", Valor = promedio }
        };
            }
        }


        /// Obtiene el MTBF filtrado por área
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
            int? idAreaMTTA = null, int? idMaquinaMTTA = null, int? anioMTTA = null, int? mesMTTA = null, int? semanaMTTA = null, int? diaSemanaMTTA = null,
            int? idAreaMTTR = null, int? idMaquinaMTTR = null , int? idEmpleadoMTTR = null, int? anioMTTR = null, int? mesMTTR = null, int? semanaMTTR = null, int? diaSemanaMTTR = null,

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
