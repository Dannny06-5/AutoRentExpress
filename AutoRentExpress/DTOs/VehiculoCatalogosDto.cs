namespace AutoRentExpress.DTOs
{
    public class VehiculoCatalogosDto
    {
        public List<OpcionVehiculoDto> Modelos { get; set; } = new();
        public List<OpcionVehiculoDto> Categorias { get; set; } = new();
        public List<OpcionVehiculoDto> Estados { get; set; } = new();
        public List<OpcionVehiculoDto> Sucursales { get; set; } = new();
    }

    public class OpcionVehiculoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}