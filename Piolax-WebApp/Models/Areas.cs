using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class Areas
    {
        [Key]
        public int idArea { get; set; }

        [Required]
        public string nombreArea { get; set; }
    }
}
