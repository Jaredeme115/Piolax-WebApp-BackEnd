namespace Piolax_WebApp.DTOs
{
    public class MantenimientoPreventivoDetallesDTO
    {
        public int idMP { get; set; }
        public int idArea { get; set; }
        public string nombreArea { get; set; }
        public int idMaquina { get; set; }
        public string nombreMaquina { get; set; }
        public int semanaPreventivo { get; set; }
        public int idFrecuenciaPreventivo { get; set; }
        public string nombreFrecuenciaPreventivo { get; set; }
        public int idEstatusPreventivo { get; set; }
        public string nombreEstatusPreventivo { get; set; }
        public int idEmpleado { get; set; }
        public string nombreCompletoTecnicoMP { get; set; }
        public bool activo { get; set; } = true;
        public DateTime? ultimaEjecucion { get; set; }
        public DateTime? proximaEjecucion { get; set; }
        public DateTime? fechaEjecucion { get; set; }

    }
}
