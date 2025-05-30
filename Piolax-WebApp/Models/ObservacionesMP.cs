﻿using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class ObservacionesMP
    {
        [Key]
        public int idObservacionMP { get; set; }
        public int idMP { get; set; }

        public MantenimientoPreventivo MantenimientoPreventivos { get; set; }

        public string observacion { get; set; }
        public DateTime? fechaObservacion { get; set; } = DateTime.UtcNow;
    }
}
