using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Models
{
    public class Sucursal
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la sucursal es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(250)]
        public string Direccion { get; set; } = string.Empty;

        [StringLength(30)]
        public string? Telefono { get; set; }

        public bool Activa { get; set; } = true;

        public ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();

        public ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
    }
}