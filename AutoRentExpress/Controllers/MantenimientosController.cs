using AutoRentExpress.DTOs;
using AutoRentExpress.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoRentExpress.Controllers
{
    public class MantenimientosController : Controller
    {
        private readonly IMantenimientoService _service;

        public MantenimientosController(IMantenimientoService service)
        {
            _service = service;
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

            return View(new MantenimientoDto
            {
                FechaInicio = DateTime.Now.AddDays(1),
                Estado = "Pendiente"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MantenimientoDto dto)
        {
            QuitarCamposCalculados();

            if (!ModelState.IsValid)
            {
                ViewBag.Catalogos = await _service.ObtenerCatalogosAsync();
                return View(dto);
            }

            try
            {
                await _service.CrearAsync(dto);

                TempData["Exito"] =
                    "Mantenimiento registrado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            ViewBag.Catalogos = await _service.ObtenerCatalogosAsync();
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var mantenimiento = await _service.ObtenerPorIdAsync(id);

            if (mantenimiento == null)
                return NotFound();

            if (mantenimiento.Estado == "Finalizado")
            {
                TempData["Error"] =
                    "No puedes editar un mantenimiento finalizado.";

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Catalogos = await _service.ObtenerCatalogosAsync();

            return View(mantenimiento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MantenimientoDto dto)
        {
            QuitarCamposCalculados();

            if (!ModelState.IsValid)
            {
                ViewBag.Catalogos = await _service.ObtenerCatalogosAsync();
                return View(dto);
            }

            try
            {
                await _service.ActualizarAsync(dto);

                TempData["Exito"] =
                    "Mantenimiento actualizado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            ViewBag.Catalogos = await _service.ObtenerCatalogosAsync();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Iniciar(int id)
        {
            try
            {
                await _service.IniciarAsync(id);
                TempData["Exito"] =
                    "Mantenimiento iniciado correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "El mantenimiento no existe.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalizar(int id)
        {
            try
            {
                await _service.FinalizarAsync(id);
                TempData["Exito"] =
                    "Mantenimiento finalizado correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "El mantenimiento no existe.";
            }

            return RedirectToAction(nameof(Index));
        }

        private void QuitarCamposCalculados()
        {
            ModelState.Remove(nameof(MantenimientoDto.Estado));
            ModelState.Remove(nameof(MantenimientoDto.FechaFin));
            ModelState.Remove(nameof(MantenimientoDto.NombreVehiculo));
            ModelState.Remove(nameof(MantenimientoDto.PlacaVehiculo));
            ModelState.Remove(nameof(MantenimientoDto.NombreEmpleado));
        }
    }
}