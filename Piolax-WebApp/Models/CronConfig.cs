using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class CronConfig
    {

        [Key]
        public int idCronConfig { get; set; }

        public string nombreCronConfig { get; set; }

        public string horaCron { get; set; }

        public string descripcionCron { get; set; }

    }
}
