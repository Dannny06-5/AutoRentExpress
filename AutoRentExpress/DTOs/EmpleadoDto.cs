using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.DTOs
{
    public class EmpleadoDto
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona una sucursal")]
        public int SucursalId { get; set; }

        [Required(ErrorMessage = "Los nombres son obligatorios")]
        [StringLength(100)]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [StringLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "El cargo es obligatorio")]
        [StringLength(100)]
        public string Cargo { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Ingresa un teléfono válido")]
        public string? Telefono { get; set; }

        public bool Activo { get; set; } = true;

        public string NombreSucursal { get; set; } = string.Empty;
    }

    public class EmpleadoSucursalDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
    }
}