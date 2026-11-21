using AutoRentExpress.DTOs;
using AutoRentExpress.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Controllers
{
    public class CategoriaVehiculoController : Controller
    {
        private readonly ICategoriaVehiculoService _service;

        public CategoriaVehiculoController(ICategoriaVehiculoService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _service.ObtenerTodosAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CategoriaVehiculoDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoriaVehiculoDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _service.CrearAsync(dto);
                TempData["Exito"] = "Categoría registrada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(dto.Nombre), ex.Message);
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var categoria = await _service.ObtenerPorIdAsync(id);

            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoriaVehiculoDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _service.ActualizarAsync(dto);
                TempData["Exito"] = "Categoría actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(dto.Nombre), ex.Message);
            }
            catch (ValidationException ex)
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
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.EliminarAsync(id);
                TempData["Exito"] = "Categoría eliminada correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "La categoría no existe.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}