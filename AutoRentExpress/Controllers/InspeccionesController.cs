using AutoRentExpress.DTOs;
using AutoRentExpress.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoRentExpress.Controllers
{
    public class InspeccionesController : Controller
    {
        private readonly IInspeccionService _service;

        public InspeccionesController(IInspeccionService service)
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

            return View(new InspeccionDto
            {
                Tipo = "Entrega",
                NivelCombustible = 100
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InspeccionDto dto)
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

                TempData["Exito"] = "Inspección registrada correctamente.";

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
            var inspeccion = await _service.ObtenerPorIdAsync(id);

            if (inspeccion == null)
                return NotFound();

            ViewBag.Catalogos = await _service.ObtenerCatalogosAsync();

            return View(inspeccion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(InspeccionDto dto)
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

                TempData["Exito"] = "Inspección actualizada correctamente.";

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

        private void QuitarCamposCalculados()
        {
            ModelState.Remove(nameof(InspeccionDto.Fecha));
            ModelState.Remove(nameof(InspeccionDto.NumeroContrato));
            ModelState.Remove(nameof(InspeccionDto.NombreCliente));
            ModelState.Remove(nameof(InspeccionDto.NombreVehiculo));
            ModelState.Remove(nameof(InspeccionDto.NombreEmpleado));
        }
    }
}