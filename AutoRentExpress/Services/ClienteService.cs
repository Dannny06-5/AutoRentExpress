using AutoRentExpress.DTOs;
using AutoRentExpress.Models;
using AutoRentExpress.Repositories;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _repository;

        public ClienteService(IClienteRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ClienteDto>> ObtenerTodosAsync()
        {
            var clientes = await _repository.ObtenerTodosAsync();

            return clientes.Select(c => ConvertirDto(c)).ToList();
        }

        public async Task<ClienteDto?> ObtenerPorIdAsync(int id)
        {
            var cliente = await _repository.ObtenerPorIdAsync(id);

            return cliente == null ? null : ConvertirDto(cliente);
        }

        public async Task CrearAsync(ClienteDto dto)
        {
            ValidarDto(dto);

            var identificacion = dto.Identificacion.Trim();

            if (await _repository.ExisteIdentificacionAsync(identificacion))
            {
                throw new InvalidOperationException(
                    "Ya existe un cliente con esa identificación.");
            }

            var cliente = new Cliente
            {
                Nombres = dto.Nombres.Trim(),
                Apellidos = dto.Apellidos.Trim(),
                Identificacion = identificacion,
                Telefono = dto.Telefono.Trim(),
                Correo = NormalizarOpcional(dto.Correo),
                Direccion = NormalizarOpcional(dto.Direccion),
                FechaRegistro = DateTime.UtcNow,
                Activo = true
            };

            await _repository.AgregarAsync(cliente);
            await _repository.GuardarCambiosAsync();
        }

        public async Task ActualizarAsync(ClienteDto dto)
        {
            ValidarDto(dto);

            var cliente = await _repository.ObtenerPorIdAsync(dto.Id);

            if (cliente == null)
            {
                throw new KeyNotFoundException(
                    "El cliente solicitado no existe.");
            }

            var identificacion = dto.Identificacion.Trim();

            if (await _repository.ExisteIdentificacionAsync(
                identificacion, dto.Id))
            {
                throw new InvalidOperationException(
                    "Ya existe otro cliente con esa identificación.");
            }

            cliente.Nombres = dto.Nombres.Trim();
            cliente.Apellidos = dto.Apellidos.Trim();
            cliente.Identificacion = identificacion;
            cliente.Telefono = dto.Telefono.Trim();
            cliente.Correo = NormalizarOpcional(dto.Correo);
            cliente.Direccion = NormalizarOpcional(dto.Direccion);

            await _repository.ActualizarAsync(cliente);
            await _repository.GuardarCambiosAsync();
        }

        public async Task DesactivarAsync(int id)
        {
            var cliente = await _repository.ObtenerPorIdAsync(id);

            if (cliente == null)
            {
                throw new KeyNotFoundException(
                    "El cliente solicitado no existe.");
            }

            cliente.Activo = false;

            await _repository.ActualizarAsync(cliente);
            await _repository.GuardarCambiosAsync();
        }

        public async Task ActivarAsync(int id)
        {
            var cliente = await _repository.ObtenerPorIdAsync(id);

            if (cliente == null)
            {
                throw new KeyNotFoundException(
                    "El cliente solicitado no existe.");
            }

            cliente.Activo = true;

            await _repository.ActualizarAsync(cliente);
            await _repository.GuardarCambiosAsync();
        }

        private static ClienteDto ConvertirDto(Cliente cliente)
        {
            return new ClienteDto
            {
                Id = cliente.Id,
                Nombres = cliente.Nombres,
                Apellidos = cliente.Apellidos,
                Identificacion = cliente.Identificacion,
                Telefono = cliente.Telefono,
                Correo = cliente.Correo,
                Direccion = cliente.Direccion,
                Activo = cliente.Activo
            };
        }

        private static string? NormalizarOpcional(string? valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? null
                : valor.Trim();
        }

        private static void ValidarDto(ClienteDto dto)
        {
            Validator.ValidateObject(
                dto,
                new ValidationContext(dto),
                true);
        }
    }
}