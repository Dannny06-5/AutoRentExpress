using AutoRentExpress.DTOs;
using AutoRentExpress.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Controllers
{
    public class MarcasController : Controller
    {
        private readonly IMarcaService _service;

        public MarcasController(IMarcaService service)
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
            return View(new MarcaDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MarcaDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _service.CrearAsync(dto);
                TempData["Exito"] = "Marca registrada correctamente.";
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
            var marca = await _service.ObtenerPorIdAsync(id);

            return marca == null ? NotFound() : View(marca);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MarcaDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _service.ActualizarAsync(dto);
                TempData["Exito"] = "Marca actualizada correctamente.";
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
                TempData["Exito"] = "Marca eliminada correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "La marca no existe.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}