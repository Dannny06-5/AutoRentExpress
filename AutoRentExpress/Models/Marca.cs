using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Models
{
    public class Marca
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la marca es obligatorio")]
        [StringLength(80)]
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Modelo> Modelos { get; set; } = new List<Modelo>();
    }
}