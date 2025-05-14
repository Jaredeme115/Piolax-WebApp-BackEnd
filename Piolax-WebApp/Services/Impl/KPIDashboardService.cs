/*using Piolax_WebApp.DTOs;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Repositories.Impl;

namespace Piolax_WebApp.Services.Impl
{
    public class KPIDashboardService(IKPIRepository repository, IAsignacionService asignacionService) : IKPIDashboardService
    {
        private readonly IKPIRepository _repository = repository;
        private readonly IAsignacionService _asignacionService = asignacionService;

        /// <summary>
        /// Obtiene el MTTA filtrado por área, máquina y período de tiempo
        /// </summary>
        public async Task<KPIResponseDTO> ObtenerMTTA(int? idArea = null, int? idMaquina = null,
            int? año = null, int? mes = null, int? semana = null, int? diaSemana = null)
        {
            var kpiDetalles = await _repository.ConsultarMTTA(idArea, idMaquina, año, mes, semana, diaSemana);
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
        public async Task<KPIResponseDTO> ObtenerMTTR(int? idArea = null, int? idMaquina = null, int? idEmpleado = null, int? año = null, int? mes = null, int? semana = null, int? diaSemana = null)
        {
            var kpiDetalles = await _repository.ConsultarMTTR(idArea, idMaquina, idEmpleado, año, mes, semana, diaSemana);
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
        public async Task<KPIResponseDTO> ObtenerMTBF(int? idArea = null, int? idMaquina = null, int? año = null, int? mes = null, int? semana = null, int? diaSemana = null)
        {
            var kpiDetalles = await _repository.ConsultarMTBF(idArea, idMaquina, año, mes, semana, diaSemana);
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
            int? idArea = null, int? idMaquina = null, int? idEmpleado = null,
            int? año = null, int? mes = null, int? semana = null, int? diaSemana = null)
        {
            var kpis = new List<KPIResponseDTO>();

            // Obtener MTTA con todos los filtros
            var mtta = await ObtenerMTTA(idArea, idMaquina, año, mes, semana, diaSemana);
            kpis.Add(mtta);

            // Obtener MTTR con todos los filtros
            var mttr = await ObtenerMTTR(idArea, idMaquina, idEmpleado, año, mes, semana, diaSemana);
            kpis.Add(mttr);

            // Obtener MTBF con todos los filtros
            var mtbf = await ObtenerMTBF(idArea, idMaquina, año, mes, semana, diaSemana);
            kpis.Add(mtbf);

            // Obtener TotalDowntime con todos los filtros
            var totalDowntime = await ObtenerTotalDowntime(idArea, idMaquina, año, mes, semana, diaSemana);
            kpis.Add(totalDowntime);

            return kpis;
        }

    }
}*/

using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Repositories.Impl;
using System.Diagnostics.Metrics;
using System.Globalization;

namespace Piolax_WebApp.Services.Impl
{
    public class KPIDashboardService(IKPIRepository repository, IAsignacionService asignacionService, IAreasService areasService) : IKPIDashboardService
    {
        private readonly IKPIRepository _repository = repository;
        private readonly IAsignacionService _asignacionService = asignacionService;
        private readonly IAreasService _areasService = areasService;
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
        ////////// FILTROS MTT  //////////////////
        public async Task<List<KpiSegmentadoDTO>> ObtenerMTTASegmentado(
            int? idArea = null,
            int? idMaquina = null,
            int? anio = null,
            int? mes = null)
        {
            var kpiDetalles = await _repository.ConsultarMTTA(idArea, idMaquina, anio, mes);

            if (!kpiDetalles.Any())
                return new List<KpiSegmentadoDTO>();

            if (anio.HasValue && mes.HasValue)
            {
                // Año + Mes: retornar valor único del mes completo
                float promedio = kpiDetalles.Average(k => k.kpiValor);
                return new List<KpiSegmentadoDTO>
        {
            new KpiSegmentadoDTO
            {
                etiqueta = $"Mes {mes}",
                valor = promedio
            }
        };
            }
            else if (anio.HasValue)
            {
                // Solo año: agrupar por mes
                return kpiDetalles
                    .GroupBy(k => k.KpisMantenimiento.fechaCalculo.Month)
                    .Select(g => new KpiSegmentadoDTO
                    {
                        etiqueta = $"Mes {g.Key}",
                        valor = g.Average(x => x.kpiValor)
                    })
                    .OrderBy(x => x.etiqueta)
                    .ToList();
            }

            return new List<KpiSegmentadoDTO>();
        }

        /// Obtiene el MTTR filtrado por área, máquina y/o técnico
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
        ////////segmento para que funcionen los  filtros por promedio////
        public async Task<List<KpiSegmentadoDTO>> ObtenerMTTRSegmentado(
         int? idArea = null,
         int? idMaquina = null,
         int? idEmpleado = null,
         int? anio = null,
         int? mes = null)
        {
            var kpiDetalles = await _repository.ConsultarMTTR(idArea, idMaquina, idEmpleado, anio, mes);

            if (!kpiDetalles.Any())
                return new List<KpiSegmentadoDTO>();

            if (anio.HasValue && mes.HasValue)
            {
                // ✅ Año + Mes: retornar valor único del mes completo
                float promedio = kpiDetalles.Average(k => k.kpiValor);
                return new List<KpiSegmentadoDTO>
      {
          new KpiSegmentadoDTO
          {
              etiqueta = $"Mes {mes}",
              valor = promedio
          }
      };
            }
            else if (anio.HasValue)
            {
                // ✅ Solo año: agrupar por mes
                return kpiDetalles
                    .GroupBy(k => k.KpisMantenimiento.fechaCalculo.Month)
                    .Select(g => new KpiSegmentadoDTO
                    {
                        etiqueta = $"Mes {g.Key}",
                        valor = g.Average(x => x.kpiValor)
                    })
                    .OrderBy(x => x.etiqueta)
                    .ToList();
            }

            // ❌ Sin año no se devuelve nada
            return new List<KpiSegmentadoDTO>();
        }


        /// Obtiene el MTBF filtrado por área (Minutos)
        /*public async Task<KPIResponseDTO> ObtenerMTBF(int? idArea = null)
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
        }*/

        /// Obtiene el MTBF filtrado por área (Horas)
        public async Task<KPIResponseDTO> ObtenerMTBF(int? idArea = null)
        {
            var kpiDetalles = await _repository.ConsultarMTBF(idArea);
            if (!kpiDetalles.Any())
                return new KPIResponseDTO { Nombre = "MTBF", Valor = 0, UnidadMedida = "horas" };

            // Calcular el promedio de los valores de MTBF en minutos
            float valorPromedioMinutos = kpiDetalles.Average(k => k.kpiValor);

            // Convertir minutos a horas
            float valorPromedioHoras = valorPromedioMinutos / 60f;

            return new KPIResponseDTO
            {
                Nombre = "MTBF",
                Valor = valorPromedioHoras,
                UnidadMedida = "horas"
            };
        }

        /// Calcula el tiempo total de inactividad (TotalDowntime) filtrado por área, máquina y período de tiempo
        public async Task<KPIResponseDTO> ObtenerTotalDowntime(int? idArea = null, int? idMaquina = null, int? anio = null, int? mes = null, int? semana = null, int? diaSemana = null)
        {
            var mantenimientos = await _repository.ConsultarTotalDowntime(
                idArea, idMaquina, anio, mes, semana, diaSemana);

            if (!mantenimientos.Any())
                return new KPIResponseDTO
                {
                    Nombre = "TotalDowntime",
                    Valor = 0,
                    UnidadMedida = "minutos"
                };

            float totalDowntime = mantenimientos
                .SelectMany(m => m.KpisDetalle)
                .Where(d => d.kpiNombre == "MTTR")
                .Sum(d => d.kpiValor);

            return new KPIResponseDTO
            {
                Nombre = "TotalDowntime",
                Valor = totalDowntime,
                UnidadMedida = "minutos"
            };
        }

        /// Devuelve una serie segmentada de Total Downtime para graficar.
        public async Task<List<KpiSegmentadoDTO>> ObtenerTotalDowntimeSegmentado(
            int? idArea = null,
            int? idMaquina = null,
            int? anio = null,
            int? mes = null,
            int? semana = null,
            int? diaSemana = null)
        {
            var mantenimientos = await _repository.ConsultarTotalDowntime(
                idArea, idMaquina, anio, mes, semana, diaSemana);

            var detallesMttr = mantenimientos
                .SelectMany(m => m.KpisDetalle)
                .Where(d => d.kpiNombre == "MTTR")
                .ToList();

            if (!detallesMttr.Any())
                return new List<KpiSegmentadoDTO>();

            // Día X de la semana Y
            if (semana.HasValue && diaSemana.HasValue)
            {
                return detallesMttr
                    .Where(d =>
                        ISOWeek.GetWeekOfYear(d.KpisMantenimiento.fechaCalculo) == semana.Value &&
                        (int)d.KpisMantenimiento.fechaCalculo.DayOfWeek == diaSemana.Value
                    )
                    .GroupBy(d => d.KpisMantenimiento.fechaCalculo.Date)
                    .Select(g => new KpiSegmentadoDTO
                    {
                        etiqueta = g.Key.ToString("dd/MM/yyyy"),
                        valor = g.Sum(x => x.kpiValor)
                    })
                    .ToList();
            }
            // Por día de la semana dentro de esa semana
            else if (semana.HasValue)
            {
                return detallesMttr
                    .Where(d => ISOWeek.GetWeekOfYear(d.KpisMantenimiento.fechaCalculo) == semana.Value)
                    .GroupBy(d => d.KpisMantenimiento.fechaCalculo.DayOfWeek)
                    .Select(g => new KpiSegmentadoDTO
                    {
                        etiqueta = g.Key.ToString(),
                        valor = g.Sum(x => x.kpiValor)
                    })
                    .OrderBy(g => (int)Enum.Parse(typeof(DayOfWeek), g.etiqueta)) // 🔹 ordena por DayOfWeek real
                    .ToList();
            }
            // Por semana ISO dentro del mes
            else if (mes.HasValue)
            {
                return detallesMttr
                    .GroupBy(d => ISOWeek.GetWeekOfYear(d.KpisMantenimiento.fechaCalculo))
                    .Select(g => new KpiSegmentadoDTO
                    {
                        etiqueta = $"Semana {g.Key}",
                        valor = g.Sum(x => x.kpiValor)
                    })
                    .OrderBy(x => x.etiqueta)
                    .ToList();
            }
            // Por mes dentro del año
            else if (anio.HasValue)
            {
                var raw = detallesMttr
                    .GroupBy(d => d.KpisMantenimiento.fechaCalculo.Month)
                    .Select(g => new KpiSegmentadoDTO
                    {
                        etiqueta = $"Mes {g.Key}",
                        valor = g.Sum(x => x.kpiValor)
                    })
                    .ToList();

                // Asegura siempre tener 12 meses
                return Enumerable.Range(1, 12)
                    .Select(m =>
                        raw.FirstOrDefault(r => r.etiqueta == $"Mes {m}")
                        ?? new KpiSegmentadoDTO { etiqueta = $"Mes {m}", valor = 0 }
                    )
                    .ToList();
            }
            // Sin segmentación temporal: un único punto
            else
            {
                float total = detallesMttr.Sum(d => d.kpiValor);
                return new List<KpiSegmentadoDTO> {
            new KpiSegmentadoDTO { etiqueta = "Total Downtime", valor = total }
        };
            }
        }


        /// <summary>
        /// Obtiene un resumen de todos los KPIs aplicando los filtros correspondientes a cada uno
        /// </summary>
        public async Task<IEnumerable<KPIResponseDTO>> ObtenerResumenKPIs(
            int? idAreaMTTA = null, int? idMaquinaMTTA = null, int? anioMTTA = null, int? mesMTTA = null, int? semanaMTTA = null, int? diaSemanaMTTA = null,
            int? idAreaMTTR = null, int? idMaquinaMTTR = null, int? idEmpleadoMTTR = null, int? anioMTTR = null, int? mesMTTR = null, int? semanaMTTR = null, int? diaSemanaMTTR = null,

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

        /// <summary>
        /// Obtiene el MTBF segmentado por mes para un año específico
        /// </summary>
        public async Task<List<KpiSegmentadoDTO>> ObtenerMTBFSegmentado(
            int? idArea = null,
            int? anio = null,
            int? objetivo = null)
        {
            // Si no hay año especificado, usar el actual
            int yearToUse = anio ?? DateTime.Now.Year;

            // Obtener todos los registros de MTBF para el área y año especificados
            var kpiDetalles = await _repository.ConsultarMTBF(idArea);

            if (!kpiDetalles.Any())
                return new List<KpiSegmentadoDTO>();

            // Filtrar por año si está especificado
            if (anio.HasValue)
            {
                kpiDetalles = kpiDetalles.Where(kd => kd.KpisMantenimiento.fechaCalculo.Year == anio.Value);
            }

            // Agrupar por mes y calcular el promedio de cada mes
            /*var mtbfPorMes = kpiDetalles
                .GroupBy(kd => kd.KpisMantenimiento.fechaCalculo.Month)
                .Select(g => new KpiSegmentadoDTO
                {
                    etiqueta = ObtenerNombreMes(g.Key),
                    valor = (int)Math.Round(g.Average(kd => kd.kpiValor))
                })
                .OrderBy(k => Array.IndexOf(
                    new[] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                    "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" },
                    k.etiqueta))
                .ToList();*/

            // Agrupar por mes y calcular el promedio de cada mes (convertido a horas)
            var mtbfPorMes = kpiDetalles
                .GroupBy(kd => kd.KpisMantenimiento.fechaCalculo.Month)
                .Select(g => new KpiSegmentadoDTO
                {
                    etiqueta = ObtenerNombreMes(g.Key),
                    valor = (float)Math.Round(g.Average(kd => kd.kpiValor) / 60, 2) // Convertir minutos a horas
                })
                .OrderBy(k => Array.IndexOf(
                    new[] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
            "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" },
                    k.etiqueta))
                .ToList();

            // Si hay un objetivo, añadirlo como una línea de referencia
            if (objetivo.HasValue && objetivo.Value > 0)
            {
                // Completar los meses faltantes (si es necesario para mostrar todos los meses en el gráfico)
                var mesesExistentes = mtbfPorMes.Select(k => ObtenerNumeroMes(k.etiqueta)).ToList();

                for (int mes = 1; mes <= 12; mes++)
                {
                    if (!mesesExistentes.Contains(mes))
                    {
                        mtbfPorMes.Add(new KpiSegmentadoDTO
                        {
                            etiqueta = ObtenerNombreMes(mes),
                            valor = 0 // Sin datos para este mes
                        });
                    }
                }

                // Reordenar después de añadir los meses faltantes
                mtbfPorMes = mtbfPorMes.OrderBy(k => Array.IndexOf(
                    new[] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                    "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" },
                    k.etiqueta)).ToList();
            }

            return mtbfPorMes;
        }

        public async Task<List<KpiAreaMesSeriesDTO>> ObtenerMTBFPorAreaMes(int? anio = null)
        {
            int yearToUse = anio ?? DateTime.Now.Year;
            var datos = await _repository.ConsultarMTBFPorAreaMes(yearToUse);

            // Agrupamos por área y construimos la lista de 12 meses
            var series = datos
                .GroupBy(d => d.nombreArea)
                .Select(g => new KpiAreaMesSeriesDTO
                   {
            nombreArea = g.Key,
            meses = Enumerable.Range(1, 12).Select(m =>
                                {
            var registro = g.FirstOrDefault(x => x.mes == m);
                            return new KpiSegmentadoDTO
                            {
                                etiqueta = ObtenerNombreMes(m),
                                valor    = registro?.valorHoras ?? 0
                            };
                        })
                        .ToList()
                })
                .ToList();

            return series;
         }

        private string ObtenerNombreMes(int numeroMes)
        {
            return numeroMes switch
            {
                1 => "Enero",
                2 => "Febrero",
                3 => "Marzo",
                4 => "Abril",
                5 => "Mayo",
                6 => "Junio",
                7 => "Julio",
                8 => "Agosto",
                9 => "Septiembre",
                10 => "Octubre",
                11 => "Noviembre",
                12 => "Diciembre",
                _ => $"Mes {numeroMes}"
            };
        }

        private int ObtenerNumeroMes(string nombreMes)
        {
            return nombreMes switch
            {
                "Enero" => 1,
                "Febrero" => 2,
                "Marzo" => 3,
                "Abril" => 4,
                "Mayo" => 5,
                "Junio" => 6,
                "Julio" => 7,
                "Agosto" => 8,
                "Septiembre" => 9,
                "Octubre" => 10,
                "Noviembre" => 11,
                "Diciembre" => 12,
                _ => 0
            };
        }

        
        /// Métodos para KPI Objetivos

        public async Task GuardarObjetivo(int idArea, int anio, int mes, float valorHoras)
        {
            var objetivo = new KpiObjetivos { idArea = idArea, anio = anio, mes = mes, valorHoras = valorHoras, fechaCreacion = DateTime.Now };
            await _repository.GuardarObjetivoAsync(objetivo);
        }

        public async Task<List<KpiObjetivosDTO>> ObtenerObjetivosPorAnio(int anio)
        {
            var lista = await _repository.ObtenerObjetivosPorAnioAsync(anio);
            return lista
              .Select(o => new KpiObjetivosDTO
              {
                  idArea = o.idArea,
                  anio = o.anio,
                  mes = o.mes,
                  valorHoras = o.valorHoras
              })
              .ToList();
        }

        public Task<List<KpiObjetivos>> ObtenerObjetivosPorAnioAsync(int anio)
        => _repository.ObtenerObjetivosPorAnioAsync(anio);

        private List<KpiSegmentadoDTO> SegmentarPorHoraDelDia(IEnumerable<KpisMantenimiento> kpisMantenimiento)
        {
            // Agrupar por hora del día y sumar el downtime
            return kpisMantenimiento
                .SelectMany(km => km.KpisDetalle.Where(kd => kd.kpiNombre == "MTTR"))
                .GroupBy(kd => kd.KpisMantenimiento.fechaCalculo.Hour)
                .Select(g => new KpiSegmentadoDTO
                {
                    etiqueta = $"{g.Key}:00 hrs",
                    valor = (int)Math.Round(g.Sum(kd => kd.kpiValor))
                })
                .OrderBy(k => int.Parse(k.etiqueta.Split(':')[0]))
                .ToList();
        }

        public async Task<List<KpiObjetivosSeriesDTO>> ObtenerObjetivosSeriesAsync(int anio)
        {
            // 1) Trae todos los objetivos de ese año
            var todos = await _repository.ObtenerObjetivosPorAnioAsync(anio);

            // 2) Trae la lista de áreas (para el nombre)
            var areas = await _areasService.ConsultarTodos(); // o tu AreasService

            var result = new List<KpiObjetivosSeriesDTO>();

            foreach (var area in areas)
            {
                // Filtra sólo los objetivos de esta área
                var objsArea = todos
                  .Where(o => o.idArea == area.idArea)
                  .OrderBy(o => o.mes)
                  .ToList();

                var serie = new KpiObjetivosSeriesDTO
                {
                    idArea = area.idArea,
                    nombreArea = area.nombreArea
                };

                float lastValor = 0f;
                // 3) Para cada mes 1…12
                for (int mes = 1; mes <= 12; mes++)
                {
                    // busca registro concreto
                    var encontrado = objsArea.FirstOrDefault(o => o.mes == mes);
                    if (encontrado != null)
                    {
                        lastValor = encontrado.valorHoras;
                    }
                    // agrega al listado
                    serie.meses.Add(new KpiSegmentadoDTO
                    {
                        etiqueta = ObtenerNombreMes(mes),
                        valor = lastValor
                    });
                }

                result.Add(serie);
            }

            return result;
        }

        private List<KpiSegmentadoDTO> SegmentarPorDiaDeLaSemana(IEnumerable<KpisMantenimiento> kpisMantenimiento)
        {
            // Nombres de los días de la semana
            string[] nombresDias = { "Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado" };

            // Agrupar por día de la semana y calcular el downtime
            return kpisMantenimiento
                .SelectMany(km => km.KpisDetalle.Where(kd => kd.kpiNombre == "MTTR"))
                .GroupBy(kd => (int)kd.KpisMantenimiento.fechaCalculo.DayOfWeek)
                .Select(g => new KpiSegmentadoDTO
                {
                    etiqueta = nombresDias[g.Key],
                    valor = (int)Math.Round(g.Sum(kd => kd.kpiValor))
                })
                .OrderBy(k => Array.IndexOf(nombresDias, k.etiqueta))
                .ToList();
        }

        private List<KpiSegmentadoDTO> SegmentarPorSemanasDelMes(IEnumerable<KpisMantenimiento> kpisMantenimiento)
        {
            // Agrupar por semana del mes y calcular el downtime
            return kpisMantenimiento
                .SelectMany(km => km.KpisDetalle.Where(kd => kd.kpiNombre == "MTTR"))
                .GroupBy(kd => ((kd.KpisMantenimiento.fechaCalculo.Day - 1) / 7) + 1) // División por semanas (1-7: semana 1, etc.)
                .Select(g => new KpiSegmentadoDTO
                {
                    etiqueta = $"Semana {g.Key}",
                    valor = (int)Math.Round(g.Sum(kd => kd.kpiValor))
                })
                .OrderBy(k => int.Parse(k.etiqueta.Split(' ')[1]))
                .ToList();
        }

        private List<KpiSegmentadoDTO> SegmentarPorMesDelAnio(IEnumerable<KpisMantenimiento> kpisMantenimiento)
        {
            return kpisMantenimiento
                .SelectMany(km => km.KpisDetalle.Where(kd => kd.kpiNombre == "MTTR"))
                .GroupBy(kd => kd.KpisMantenimiento.fechaCalculo.Month)
                .Select(g => new KpiSegmentadoDTO
                {
                    etiqueta = ObtenerNombreMes(g.Key),
                    valor = (int)Math.Round(g.Sum(kd => kd.kpiValor))
                })
                .OrderBy(k => Array.IndexOf(
                    new[] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                    "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" },
                    k.etiqueta))
                .ToList();
        }

        private List<KpiSegmentadoDTO> SegmentarPorAnio(IEnumerable<KpisMantenimiento> kpisMantenimiento)
        {
            return kpisMantenimiento
                .SelectMany(km => km.KpisDetalle.Where(kd => kd.kpiNombre == "MTTR"))
                .GroupBy(kd => kd.KpisMantenimiento.fechaCalculo.Year)
                .Select(g => new KpiSegmentadoDTO
                {
                    etiqueta = g.Key.ToString(),
                    valor = (int)Math.Round(g.Sum(kd => kd.kpiValor))
                })
                .OrderBy(k => k.etiqueta)
                .ToList();
        }



    }
}
