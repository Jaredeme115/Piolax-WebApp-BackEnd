namespace Piolax_WebApp.DTOs
{
    public class MantenimientoPreventivoDTO
    {
        public int idMP { get; set; }
        public int idArea { get; set; }
        public int idMaquina { get; set; }
        public int semanaPreventivo { get; set; }
        public int idFrecuenciaPreventivo { get; set; }
        public int? idEstatusPreventivo { get; set; }
        public int idEmpleado { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime? ultimaEjecucion { get; set; }
        public DateTime? proximaEjecucion { get; set; }
        public DateTime? fechaEjecucion { get; set; }
    }
}
