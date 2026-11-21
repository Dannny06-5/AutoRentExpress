using System;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "La identificación es obligatoria")]
        [StringLength(30)]
        public string Identificacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(30)]
        public string Telefono { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Ingrese un correo válido")]
        [StringLength(150)]
        public string? Correo { get; set; }

        [StringLength(250)]
        public string? Direccion { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public bool Activo { get; set; } = true;

        public ICollection<LicenciaConducir> Licencias { get; set; } = new List<LicenciaConducir>();

        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}