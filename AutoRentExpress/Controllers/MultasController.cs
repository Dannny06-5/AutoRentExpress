using AutoRentExpress.DTOs;
using AutoRentExpress.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoRentExpress.Controllers
{
    public class MultasController : Controller
    {
        private readonly IMultaService _service;

        public MultasController(IMultaService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _service.ObtenerTodosAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Contratos = await _service.ObtenerContratosAsync();
            return View(new MultaDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MultaDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Contratos = await _service.ObtenerContratosAsync();
                return View(dto);
            }

            try
            {
                await _service.CrearAsync(dto);
                TempData["Exito"] = "Multa registrada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            ViewBag.Contratos = await _service.ObtenerContratosAsync();
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var multa = await _service.ObtenerPorIdAsync(id);

            if (multa == null)
                return NotFound();

            if (multa.Pagada)
            {
                TempData["Error"] = "No puedes editar una multa pagada.";
                return RedirectToAction(nameof(Index));
            }

            return View(multa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MultaDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _service.ActualizarAsync(dto);
                TempData["Exito"] = "Multa actualizada correctamente.";
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

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarPagada(int id)
        {
            try
            {
                await _service.MarcarPagadaAsync(id);
                TempData["Exito"] = "Multa marcada como pagada correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "La multa no existe.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}