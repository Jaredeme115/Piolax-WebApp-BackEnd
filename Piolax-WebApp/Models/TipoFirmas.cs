using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class TipoFirmas
    {
        [Key]
        public int idTipoFirma { get; set; }
        public string nombreTipoFirma { get; set; }

        public virtual ICollection<Firmas> Firmas { get; set; } = new List<Firmas>(); // Lista de Firmas asociadas a TipoFirmas
    }
}
