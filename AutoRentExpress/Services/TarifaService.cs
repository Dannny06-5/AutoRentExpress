
using AutoRentExpress.DTOs;
using AutoRentExpress.Models;
using AutoRentExpress.Repositories;

namespace AutoRentExpress.Services
{
    public class TarifaService : ITarifaService
    {
        private readonly ITarifaRepository _repository;

        public TarifaService(ITarifaRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TarifaDto>> ObtenerTodosAsync()
        {
            var tarifas = await _repository.ObtenerTodosAsync();

            return tarifas.Select(Mapear).ToList();
        }

        public async Task<TarifaDto?> ObtenerPorIdAsync(int id)
        {
            var tarifa = await _repository.ObtenerPorIdAsync(id);

            return tarifa == null ? null : Mapear(tarifa);
        }

        public async Task<List<TarifaCategoriaDto>> ObtenerCategoriasAsync()
        {
            var categorias = await _repository.ObtenerCategoriasAsync();

            return categorias.Select(c => new TarifaCategoriaDto
            {
                Id = c.Id,
                Nombre = c.Nombre
            }).ToList();
        }

        public async Task CrearAsync(TarifaDto dto)
        {
            await ValidarAsync(dto);

            var tarifa = new Tarifa
            {
                CategoriaVehiculoId = dto.CategoriaVehiculoId,
                Nombre = dto.Nombre.Trim(),
                PrecioDiario = dto.PrecioDiario,
                VigenteDesde = dto.VigenteDesde.Date,
                VigenteHasta = dto.VigenteHasta?.Date,
                Activa = true
            };

            await _repository.AgregarAsync(tarifa);
        }

        public async Task ActualizarAsync(TarifaDto dto)
        {
            var tarifa = await _repository.ObtenerPorIdAsync(dto.Id);

            if (tarifa == null)
            {
                throw new KeyNotFoundException(
                    "La tarifa seleccionada no existe.");
            }

            await ValidarAsync(dto, dto.Id);

            tarifa.CategoriaVehiculoId = dto.CategoriaVehiculoId;
            tarifa.Nombre = dto.Nombre.Trim();
            tarifa.PrecioDiario = dto.PrecioDiario;
            tarifa.VigenteDesde = dto.VigenteDesde.Date;
            tarifa.VigenteHasta = dto.VigenteHasta?.Date;

            await _repository.GuardarCambiosAsync();
        }

        public async Task CambiarEstadoAsync(int id)
        {
            var tarifa = await _repository.ObtenerPorIdAsync(id);

            if (tarifa == null)
            {
                throw new KeyNotFoundException(
                    "La tarifa seleccionada no existe.");
            }

            if (!tarifa.Activa)
            {
                var dto = Mapear(tarifa);
                await ValidarAsync(dto, tarifa.Id);
            }

            tarifa.Activa = !tarifa.Activa;

            await _repository.GuardarCambiosAsync();
        }

        private async Task ValidarAsync(
            TarifaDto dto,
            int excluirId = 0)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
            {
                throw new InvalidOperationException(
                    "El nombre de la tarifa es obligatorio.");
            }

            if (dto.Nombre.Trim().Length > 100)
            {
                throw new InvalidOperationException(
                    "El nombre no puede superar los 100 caracteres.");
            }

            if (dto.PrecioDiario <= 0 ||
                dto.PrecioDiario > 1000000m)
            {
                throw new InvalidOperationException(
                    "El precio diario debe ser mayor que cero y no superar 1000000.");
            }

            if (dto.VigenteDesde == default)
            {
                throw new InvalidOperationException(
                    "Selecciona una fecha de inicio válida.");
            }

            if (dto.VigenteHasta.HasValue &&
                dto.VigenteHasta.Value.Date < dto.VigenteDesde.Date)
            {
                throw new InvalidOperationException(
                    "La fecha de finalización no puede ser anterior al inicio de vigencia.");
            }

            var categoria = await _repository.ObtenerCategoriaAsync(
                dto.CategoriaVehiculoId);

            if (categoria == null)
            {
                throw new InvalidOperationException(
                    "La categoría seleccionada no existe.");
            }

            if (await _repository.ExisteNombreAsync(
                dto.Nombre,
                dto.CategoriaVehiculoId,
                excluirId))
            {
                throw new InvalidOperationException(
                    "Ya existe una tarifa con ese nombre para la categoría seleccionada.");
            }
        }

        private static TarifaDto Mapear(Tarifa tarifa)
        {
            return new TarifaDto
            {
                Id = tarifa.Id,
                CategoriaVehiculoId = tarifa.CategoriaVehiculoId,
                Nombre = tarifa.Nombre,
                PrecioDiario = tarifa.PrecioDiario,
                VigenteDesde = tarifa.VigenteDesde,
                VigenteHasta = tarifa.VigenteHasta,
                Activa = tarifa.Activa,
                NombreCategoria = tarifa.CategoriaVehiculo?.Nombre
                    ?? string.Empty
            };
        }
    }
}
