using System.Runtime.Intrinsics.X86;

namespace Piolax_WebApp.Models
{
    public class HistoricoMPFirma
    {
        public int idHistoricoMP { get; set; }
        public HistoricoMP HistoricoMP { get; set; }

        public int idFirma { get; set; }
        public Firmas Firmas { get; set; }

    }
}
