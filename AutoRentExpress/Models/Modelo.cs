using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Models
{
    public class Modelo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una marca")]
        public int MarcaId { get; set; }

        [Required(ErrorMessage = "El nombre del modelo es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public Marca Marca { get; set; } = null!;

        public ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
    }
}