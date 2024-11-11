using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class Roles
    {
        [Key]
        public int idRol { get; set; }

        [Required]
        public string nombreRol { get; set; }
    }
}
