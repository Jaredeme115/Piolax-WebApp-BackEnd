using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class Asignaciones
    {
        [Key]
        public int idAsignacion { get; set; }

        [Required]
        public int idSolicitud { get; set; }

        public Solicitudes Solicitud { get; set; }

        [Required]
        public int idStatusAsignacion { get; set; }

        public StatusAsignacion StatusAsignacion { get; set; }

        // NUEVO: Acumula la espera total, en minutos, para calcular MTTA
        // (desde la generación de la solicitud hasta que un técnico toma la asignación,
        // y cada vez que está en pausa sin técnico activo)
        public double tiempoEsperaAcumuladoMinutos { get; set; } = 0;

        public DateTime? ultimaVezSinTecnico { get; set; }

        public virtual ICollection<asignacion_refacciones> Asignacion_Refacciones { get; set; } = new List<asignacion_refacciones>(); // Lista de asignacion_refacciones asociados a Asignaciones

        public virtual ICollection<Asignacion_Tecnico> Asignacion_Tecnico { get; set; } = new List<Asignacion_Tecnico>(); // Lista de Asignacion_Tecnico asociados a Asignaciones

    }
}
