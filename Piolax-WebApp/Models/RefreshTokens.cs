using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class RefreshTokens
    {
        [Key]
        public int idRefreshToken { get; set; }

        [Required]
        public string token { get; set; }

        [Required]
        public int idEmpleado { get; set; }

        [Required]
        public DateTime expiresAt { get; set; }

        public bool revoked { get; set; }
        public DateTime createdAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("idEmpleado")]
        public Empleado Empleado { get; set; }
    }
}
