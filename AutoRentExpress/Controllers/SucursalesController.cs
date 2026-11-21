using AutoRentExpress.DTOs;
using AutoRentExpress.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Controllers
{
    public class SucursalesController : Controller
    {
        private readonly ISucursalService _service;

        public SucursalesController(ISucursalService service)
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
            return View(new SucursalDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SucursalDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _service.CrearAsync(dto);
                TempData["Exito"] = "Sucursal registrada correctamente.";
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
            var sucursal = await _service.ObtenerPorIdAsync(id);

            return sucursal == null ? NotFound() : View(sucursal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SucursalDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _service.ActualizarAsync(dto);
                TempData["Exito"] = "Sucursal actualizada correctamente.";
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
        public async Task<IActionResult> CambiarEstado(int id)
        {
            try
            {
                await _service.CambiarEstadoAsync(id);
                TempData["Exito"] = "Estado de la sucursal actualizado.";
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "La sucursal no existe.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}