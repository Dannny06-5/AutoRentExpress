using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace AutoRentExpress.Models
{
    public class Empleado
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una sucursal")]
        public int SucursalId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "El cargo es obligatorio")]
        [StringLength(80)]
        public string Cargo { get; set; } = string.Empty;

        [StringLength(30)]
        public string? Telefono { get; set; }

        public bool Activo { get; set; } = true;

        public Sucursal Sucursal { get; set; } = null!;

        public ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();

        public ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();

        public ICollection<Inspeccion> Inspecciones { get; set; } = new List<Inspeccion>();
    }
}