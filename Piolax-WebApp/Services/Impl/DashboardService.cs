namespace Piolax_WebApp.Services.Impl;

using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.DTOs.Piolax_WebApp.DTOs;
using Piolax_WebApp.Repositories;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<FiltrosDashboardDTO> ObtenerFiltrosDashboardAsync()
    {
        var areas = await _context.Areas
            .Select(a => a.nombreArea)
            .Distinct()
            .ToListAsync();

        var maquinasPorArea = await _context.Maquinas
            .Include(m => m.Area)
            .GroupBy(m => m.Area.nombreArea)
            .Select(g => new
            {
                Area = g.Key,
                Maquinas = g.Select(m => m.nombreMaquina).ToList()
            }).ToListAsync();

        var tecnicos = await _context.EmpleadoAreaRol
            .Where(ar => ar.idRol == 3) // Rol 3 = Técnico
            .Include(ar => ar.Empleado)
            .Select(ar => ar.Empleado.nombre + " " + ar.Empleado.apellidoPaterno)
            .Distinct()
            .ToListAsync();

        return new FiltrosDashboardDTO
        {
            Areas = areas,
            MaquinasPorArea = maquinasPorArea.ToDictionary(m => m.Area, m => m.Maquinas),
            Tecnicos = tecnicos
        };
    }

    public async Task<List<KpiDashboardDTO>> ObtenerKpisDashboardAsync(int? anio)
    {
        var query = _context.KpisMantenimiento
            .Include(k => k.Maquina)
            .Include(k => k.Area)
            .AsQueryable();

        if (anio.HasValue)
        {
            query = query.Where(k => k.fechaCalculo.Year == anio.Value);
        }

        return await query
            .Select(k => new KpiDashboardDTO
            {
                Area = k.Area.nombreArea,
                Maquina = k.Maquina.nombreMaquina,
                MTTA = k.MTTA,
                MTTR = k.MTTR,
                MTBF = k.MTBF,
                Fecha = k.fechaCalculo
            }).ToListAsync();
    }

    public async Task<List<KpiDashboardDTO>> ObtenerIndicadoresDashboardAsync(FiltroDashboardDTO filtro)
    {
        var query = _context.KpisMantenimiento
            .Include(k => k.Area)
            .Include(k => k.Maquina)
            .AsQueryable();

        if (filtro.IdArea.HasValue)
            query = query.Where(k => k.idArea == filtro.IdArea);
        if (filtro.IdMaquina.HasValue)
            query = query.Where(k => k.idMaquina == filtro.IdMaquina);
        if (filtro.IdEmpleado.HasValue)
            query = query.Where(k => k.idEmpleado == filtro.IdEmpleado);
        if (filtro.FechaInicio.HasValue && filtro.FechaFin.HasValue)
            query = query.Where(k => k.fechaCalculo >= filtro.FechaInicio && k.fechaCalculo <= filtro.FechaFin);

        var resultados = await query
            .GroupBy(k => k.fechaCalculo.Date)
            .Select(g => new KpiDashboardDTO
            {
                Fecha = g.Key,
                Area = g.Select(k => k.Area.nombreArea).FirstOrDefault(),
                Maquina = g.Select(k => k.Maquina.nombreMaquina).FirstOrDefault(),
                MTTA = g.Average(k => k.MTTA),
                MTTR = g.Average(k => k.MTTR),
                MTBF = g.Average(k => k.MTBF)
            })
            .OrderBy(k => k.Fecha)
            .ToListAsync();

        return resultados;
    }

}
