namespace Piolax_WebApp.DTOs
{
    public class MantenimientoPreventivoCreateDTO
    {
        public int idArea { get; set; }
        public int idMaquina { get; set; }
        public int semanaPreventivo { get; set; }
        public int idFrecuenciaPreventivo { get; set; }
        public int? idEstatusPreventivo { get; set; }
        public int idEmpleado { get; set; }
        public bool activo { get; set; } = true;
        public int semanaOriginalMP { get; set; } // Semana original del mantenimiento preventivo
    }
}
