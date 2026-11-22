
namespace AutoRentExpress.DTOs
{
    public class ReservaTarifaOpcionDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int CategoriaVehiculoId { get; set; }
        public decimal PrecioDiario { get; set; }
        public DateTime VigenteDesde { get; set; }
        public DateTime? VigenteHasta { get; set; }
    }
}
