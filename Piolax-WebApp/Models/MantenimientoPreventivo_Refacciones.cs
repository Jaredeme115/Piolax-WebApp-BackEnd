using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class MantenimientoPreventivo_Refacciones
    {
        [Key]
        public int idMPRefaccion { get; set; }

        public int idHistoricoMP { get; set; }

        public HistoricoMP HistoricoMP { get; set; }

        public int idRefaccion { get; set; }

        public Inventario Inventario { get; set; }

        public int cantidad { get; set; }

        public DateTime fechaUso { get; set; }
    }
}
