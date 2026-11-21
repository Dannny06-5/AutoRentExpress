using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Models
{
    public class Vehiculo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un modelo")]
        public int ModeloId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una categoría")]
        public int CategoriaVehiculoId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un estado")]
        public int EstadoVehiculoId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una sucursal")]
        public int SucursalId { get; set; }

        [Required(ErrorMessage = "La placa es obligatoria")]
        [StringLength(20)]
        public string Placa { get; set; } = string.Empty;

        [Required(ErrorMessage = "El color es obligatorio")]
        [StringLength(50)]
        public string Color { get; set; } = string.Empty;

        [Range(1990, 2100, ErrorMessage = "Ingrese un año válido")]
        public int Anio { get; set; }

        [Range(0, int.MaxValue)]
        public int Kilometraje { get; set; }

        [Range(1, 100)]
        public int CapacidadPasajeros { get; set; } = 5;

        [Range(0.01, 1000000, ErrorMessage = "La tarifa debe ser mayor que cero")]
        public decimal TarifaDiaria { get; set; }

        [StringLength(500)]
        public string? ImagenUrl { get; set; }

        public bool Activo { get; set; } = true;

        public Modelo Modelo { get; set; } = null!;

        public CategoriaVehiculo CategoriaVehiculo { get; set; } = null!;

        public EstadoVehiculo EstadoVehiculo { get; set; } = null!;

        public Sucursal Sucursal { get; set; } = null!;

        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

        public ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();
    }
}