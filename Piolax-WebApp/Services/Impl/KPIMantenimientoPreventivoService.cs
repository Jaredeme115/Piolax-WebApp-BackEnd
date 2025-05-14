using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Repositories.Impl;

namespace Piolax_WebApp.Services.Impl
{
    public class KPIMantenimientoPreventivoService (IKPIRepository repository, IMantenimientoPreventivoRepository mantenimientoPreventivoRepository) : IKPIMantenimientoPreventivoService
    {
        private readonly IKPIRepository _kpiRepository = repository;
        private readonly IMantenimientoPreventivoRepository _mantenimientoPreventivoRepository = mantenimientoPreventivoRepository;

        public async Task CalcularYGuardarKPIs(DateTime inicio, DateTime fin)
        {
            var mps = await _mantenimientoPreventivoRepository.ConsultarMantenimientosPorPeriodo(inicio, fin);

            int totalProgramados = mps.Count();
            int totalEjecutados = mps.Count(mp => mp.fechaEjecucion != null);
            float cumplimiento = totalProgramados > 0 ? (totalEjecutados / (float)totalProgramados * 100) : 100;

            int ejecutadosEnTiempo = mps
                .Where(mp => mp.fechaEjecucion != null)
                .Count(mp => GetWeekOfYear(mp.fechaEjecucion.Value) == mp.semanaOriginalMP);
            float efectividad = totalProgramados > 0 ? (ejecutadosEnTiempo / (float)totalProgramados * 100) : 0;

            var kpi = new KpisMP { fechaCalculo = DateTime.Now };
            await _kpiRepository.GuardarKPIPreventivo(kpi);

            var detalles = new List<KpisMPDetalle>
            {
                new() { kpiMPNombre = "Cumplimiento", kpiMPValor = cumplimiento },
                new() { kpiMPNombre = "Efectividad", kpiMPValor = efectividad }
            };
            await _kpiRepository.GuardarKPIDetallesMP(kpi.idKPIMP, detalles);
        }

        public async Task<KPIResponseDTO> ObtenerCumplimiento(int? año = null, int? mes = null)
        {
            var kpiDetalles = await _kpiRepository.ConsultarKPIsDetallePreventivo("Cumplimiento", año, mes);

            if (!kpiDetalles.Any())
                return new KPIResponseDTO { Nombre = "Cumplimiento", Valor = 0, UnidadMedida = "%" };

            // Calcular el promedio de los valores de Cumplimiento
            float valorPromedio = kpiDetalles.Average(k => k.kpiMPValor);

            return new KPIResponseDTO
            {
                Nombre = "Cumplimiento",
                Valor = valorPromedio,
                UnidadMedida = "%"
            };
        }

        public async Task<KPIResponseDTO> ObtenerEfectividad(int? año = null, int? mes = null)
        {
            var kpiDetalles = await _kpiRepository.ConsultarKPIsDetallePreventivo("Efectividad", año, mes);

            if (!kpiDetalles.Any())
                return new KPIResponseDTO { Nombre = "Efectividad", Valor = 0, UnidadMedida = "%" };

            // Calcular el promedio de los valores de Efectividad
            float valorPromedio = kpiDetalles.Average(k => k.kpiMPValor);

            return new KPIResponseDTO
            {
                Nombre = "Efectividad",
                Valor = valorPromedio,
                UnidadMedida = "%"
            };
        }

        public async Task<IEnumerable<KPIResponseDTO>> ObtenerResumenKPIsPreventivo(int? año = null, int? mes = null)
        {
            var resultados = new List<KPIResponseDTO>();

            // Obtener cumplimiento
            var cumplimiento = await ObtenerCumplimiento(año, mes);
            resultados.Add(cumplimiento);

            // Obtener efectividad
            var efectividad = await ObtenerEfectividad(año, mes);
            resultados.Add(efectividad);

            return resultados;
        }

        public async Task<ContadoresMPDTO> ObtenerContadoresMP(int? año = null, int? mes = null)
        {
            var mps = await _mantenimientoPreventivoRepository.ConsultarTodosMPs();

            if (año.HasValue)
                mps = mps.Where(mp => mp.anioPreventivo == año.Value).ToList();

            if (mes.HasValue && año.HasValue)
            {
                mps = mps.Where(mp =>
                {
                    DateTime fecha;

                    if (mp.proximaEjecucion.HasValue)
                        fecha = mp.proximaEjecucion.Value;
                    else
                    {
                        try
                        {
                            fecha = GetStartOfWeek(mp.anioPreventivo, mp.semanaPreventivo);
                        }
                        catch
                        {
                            return false;
                        }
                    }

                    return fecha.Year == año.Value && fecha.Month == mes.Value;
                }).ToList();
            }

            return new ContadoresMPDTO
            {
                totalMP = mps.Count(),
                pendientes = mps.Count(mp => mp.idEstatusPreventivo == 1),
                noRealizados = mps.Count(mp => mp.idEstatusPreventivo == 2),
                realizados = mps.Count(mp => mp.idEstatusPreventivo == 3),
                reprogramados = mps.Count(mp => mp.idEstatusPreventivo == 4),
                enProceso = mps.Count(mp => mp.idEstatusPreventivo == 5)
            };
        }

        public async Task GuardarContadoresMPHistorico(int año, int mes)
        {
            // 1) Calcula los contadores usando tu método existente
            var cont = await ObtenerContadoresMP(año, mes);

            // 2) Crea la entidad padre
            var fechaCierre = new DateTime(año, mes, DateTime.DaysInMonth(año, mes));
            var kpiMp = new KpisMP { fechaCalculo = fechaCierre };
            await _kpiRepository.GuardarKPIPreventivo(kpiMp);

            // 3) Prepara los detalles
            var detalles = new List<KpisMPDetalle>
            {
                new() { idKPIMP = kpiMp.idKPIMP, kpiMPNombre = "Pendientes",    kpiMPValor = cont.pendientes },
                new() { idKPIMP = kpiMp.idKPIMP, kpiMPNombre = "Realizados",    kpiMPValor = cont.realizados },
                new() { idKPIMP = kpiMp.idKPIMP, kpiMPNombre = "NoRealizados", kpiMPValor = cont.noRealizados },
                new() { idKPIMP = kpiMp.idKPIMP, kpiMPNombre = "Reprogramados", kpiMPValor = cont.reprogramados },
                new() { idKPIMP = kpiMp.idKPIMP, kpiMPNombre = "EnProceso",     kpiMPValor = cont.enProceso }
            };
                await _kpiRepository.GuardarKPIDetallesMP(kpiMp.idKPIMP, detalles);
        }

        public async Task<IEnumerable<KpiHistoricoDTO>> ObtenerHistoricoMP(int? año = null, int? mes = null)
        {
            var kpis = await _kpiRepository.ConsultarKPIsPreventivo(año, mes);
            return kpis.Select(k => new KpiHistoricoDTO
            {
                fecha = k.fechaCalculo,
                detalles = k.KpisMPDetalle.Select(d => new KpiNombreValorDTO
                {
                    nombre = d.kpiMPNombre,
                    valor = d.kpiMPValor
                }).ToList()
            });
        }

        public async Task<ContadoresProgramadosMPDTO> ObtenerContadoresProgramadosMP(int año)
        {
            // 1) Obtener todos los MPs activos del año
            var todos = (await _mantenimientoPreventivoRepository.ConsultarTodosMPs())
                          .Where(mp => mp.anioPreventivo == año)
                          .ToList();

            int semanaActual = GetWeekOfYear(DateTime.Today);
            int total = 0, anteriores = 0, futuros = 0;

            foreach (var mp in todos)
            {
                // Fecha de inicio de la primera ejecución
                DateTime inicio = GetStartOfWeek(año, mp.semanaOriginalMP);
                int intervaloSemanas = ObtenerIntervaloEnSemanas(mp.idFrecuenciaPreventivo);

                // Generar repeticiones mientras queden dentro del mismo año
                while (inicio.Year == año)
                {
                    total++;
                    int semana = GetWeekOfYear(inicio);

                    if (semana <= semanaActual)
                        anteriores++;
                    else
                        futuros++;

                    // Avanzar a la siguiente ejecución
                    inicio = inicio.AddDays(intervaloSemanas * 7);
                }
            }

            return new ContadoresProgramadosMPDTO
            {
                totalProgramados = total,
                anteriores = anteriores,
                futuros = futuros
            };
        }

        private int ObtenerIntervaloEnSemanas(int idFrecuenciaPreventivo)
        {
            return idFrecuenciaPreventivo switch
            {
                1 => 4,   // Mensual → cada 4 semanas
                2 => 8,   // Bimestral → cada 8 semanas
                3 => 12,  // Trimestral → cada 12 semanas
                4 => 52,  // Anual → cada 52 semanas
                _ => 4
            };
        }




        private DateTime GetStartOfWeek(int year, int weekNumber)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Monday - jan1.DayOfWeek; // Ajustar al lunes de la semana 1
            DateTime firstMonday = jan1.AddDays(daysOffset);

            // Calcular el inicio de la semana solicitada
            DateTime startOfWeek = firstMonday.AddDays((weekNumber - 1) * 7);
            return startOfWeek;
        }

        private int GetWeekOfYear(DateTime date)
        {
            var calendar = System.Globalization.CultureInfo.CurrentCulture.Calendar;
            return calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }
    }
}
