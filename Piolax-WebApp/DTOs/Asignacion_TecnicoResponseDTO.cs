namespace Piolax_WebApp.DTOs
{
    public class Asignacion_TecnicoResponseDTO
    {
        public int idAsignacionTecnico { get; set; }
        public int idAsignacion { get; set; }
        public int idEmpleado { get; set; }
        public int idStatusAprobacionTecnico { get; set; }
        public DateTime horaInicio { get; set; }
        public DateTime horaTermino { get; set; }
        public string solucion { get; set; }
        public string comentarioPausa { get; set; }
        public bool esTecnicoActivo { get; set; }
        public double tiempoAcumuladoMinutos { get; set; }
    }
}
