namespace Piolax_WebApp.DTOs
{
    public class MPRefaccionesResponseDTO
    {
        public int idMPRefaccion { get; set; }

        public int idMP { get; set; }

        public int idRefaccion { get; set; }

        public int cantidad { get; set; }

        public DateTime? fechaUso { get; set; } = DateTime.UtcNow; // Usa fecha actual si no se especifica
    }
}
