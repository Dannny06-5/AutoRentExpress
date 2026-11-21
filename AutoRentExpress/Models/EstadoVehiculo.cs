using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Models
{
    public class EstadoVehiculo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del estado es obligatorio")]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
    }
}