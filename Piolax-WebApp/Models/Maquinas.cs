using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class Maquinas
    {
        [Key]
        public int idMaquina { get; set; }

        [Required]
        [StringLength(30)]
        public string nombreMaquina { get; set; }


        [Required]
        [StringLength(255)]
        public string codigoQR { get; set; }

        [Required]
        public int idArea { get; set; }
        public Areas Area { get; set; }

        public ICollection<Solicitudes> Solicitudes { get; set; } // Lista de solicitudes asociados a esta maquina


    }
}
