using AutoRentExpress.DTOs;
using AutoRentExpress.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Controllers
{
    public class VehiculosController : Controller
    {
        private readonly IVehiculoService _service;
        private readonly IWebHostEnvironment _environment;

        public VehiculosController(
            IVehiculoService service,
            IWebHostEnvironment environment)
        {
            _service = service;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Catalogos = await _service.ObtenerCatalogosAsync();
            return View(await _service.ObtenerTodosAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Catalogos = await _service.ObtenerCatalogosAsync();

            return View(new VehiculoDto
            {
                Anio = DateTime.Now.Year,
                CapacidadPasajeros = 5,
                Activo = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(6291456)]
        public async Task<IActionResult> Create(VehiculoDto dto)
        {
            ModelState.Remove(nameof(VehiculoDto.ImagenUrl));

            if (!ModelState.IsValid)
            {
                ViewBag.Catalogos = await _service.ObtenerCatalogosAsync();
                return View(dto);
            }

            string? imagenNueva = null;

            try
            {
                imagenNueva = await GuardarImagenAsync(dto.ImagenArchivo);
                dto.ImagenUrl = imagenNueva;

                await _service.CrearAsync(dto);

                TempData["Exito"] = "Vehículo registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                EliminarImagenLocal(imagenNueva);
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (ValidationException ex)
            {
                EliminarImagenLocal(imagenNueva);
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch
            {
                EliminarImagenLocal(imagenNueva);
                throw;
            }

            ViewBag.Catalogos = await _service.ObtenerCatalogosAsync();
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vehiculo = await _service.ObtenerPorIdAsync(id);

            if (vehiculo == null)
                return NotFound();

            ViewBag.Catalogos = await _service.ObtenerCatalogosAsync();
            return View(vehiculo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(6291456)]
        public async Task<IActionResult> Edit(VehiculoDto dto)
        {
            var vehiculoActual = await _service.ObtenerPorIdAsync(dto.Id);

            if (vehiculoActual == null)
                return NotFound();

            ModelState.Remove(nameof(VehiculoDto.ImagenUrl));
            dto.ImagenUrl = vehiculoActual.ImagenUrl;

            if (!ModelState.IsValid)
            {
                ViewBag.Catalogos = await _service.ObtenerCatalogosAsync();
                return View(dto);
            }

            string? imagenNueva = null;

            try
            {
                imagenNueva = await GuardarImagenAsync(dto.ImagenArchivo);

                if (imagenNueva != null)
                    dto.ImagenUrl = imagenNueva;

                await _service.ActualizarAsync(dto);

                if (imagenNueva != null)
                    EliminarImagenLocal(vehiculoActual.ImagenUrl);

                TempData["Exito"] = "Vehículo actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                EliminarImagenLocal(imagenNueva);
                dto.ImagenUrl = vehiculoActual.ImagenUrl;
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (ValidationException ex)
            {
                EliminarImagenLocal(imagenNueva);
                dto.ImagenUrl = vehiculoActual.ImagenUrl;
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch
            {
                EliminarImagenLocal(imagenNueva);
                throw;
            }

            ViewBag.Catalogos = await _service.ObtenerCatalogosAsync();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            try
            {
                await _service.CambiarEstadoActivoAsync(id);
                TempData["Exito"] = "Estado del vehículo actualizado correctamente.";
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "El vehículo no existe.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<string?> GuardarImagenAsync(IFormFile? archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return null;

            if (archivo.Length > 5 * 1024 * 1024)
                throw new InvalidOperationException(
                    "La imagen no puede superar los 5 MB.");

            var extensionOriginal = Path.GetExtension(archivo.FileName)
                .ToLowerInvariant();

            if (extensionOriginal != ".jpg" &&
                extensionOriginal != ".jpeg" &&
                extensionOriginal != ".png" &&
                extensionOriginal != ".webp")
            {
                throw new InvalidOperationException(
                    "Solo se permiten imágenes JPG, PNG o WEBP.");
            }

            await using var memoria = new MemoryStream();
            await archivo.CopyToAsync(memoria);

            var datos = memoria.ToArray();

            var esJpeg = datos.Length >= 3 &&
                datos[0] == 0xFF &&
                datos[1] == 0xD8 &&
                datos[2] == 0xFF;

            var esPng = datos.Length >= 8 &&
                datos[0] == 0x89 &&
                datos[1] == 0x50 &&
                datos[2] == 0x4E &&
                datos[3] == 0x47 &&
                datos[4] == 0x0D &&
                datos[5] == 0x0A &&
                datos[6] == 0x1A &&
                datos[7] == 0x0A;

            var esWebp = datos.Length >= 12 &&
                System.Text.Encoding.ASCII.GetString(datos, 0, 4) == "RIFF" &&
                System.Text.Encoding.ASCII.GetString(datos, 8, 4) == "WEBP";

            var extension = esJpeg ? ".jpg"
                : esPng ? ".png"
                : esWebp ? ".webp"
                : null;

            if (extension == null)
                throw new InvalidOperationException(
                    "El archivo seleccionado no es una imagen válida.");

            var nombre = $"{Guid.NewGuid():N}{extension}";
            var carpeta = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "vehiculos");

            Directory.CreateDirectory(carpeta);

            var ruta = Path.Combine(carpeta, nombre);

            await System.IO.File.WriteAllBytesAsync(ruta, datos);

            return $"/uploads/vehiculos/{nombre}";
        }

        private void EliminarImagenLocal(string? url)
        {
            if (string.IsNullOrWhiteSpace(url) ||
                !url.StartsWith(
                    "/uploads/vehiculos/",
                    StringComparison.Ordinal))
            {
                return;
            }

            var nombre = Path.GetFileName(url);

            if (string.IsNullOrWhiteSpace(nombre))
                return;

            var ruta = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "vehiculos",
                nombre);

            if (System.IO.File.Exists(ruta))
                System.IO.File.Delete(ruta);
        }
    }
}