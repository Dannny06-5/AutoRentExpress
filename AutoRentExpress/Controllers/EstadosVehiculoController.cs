using AutoRentExpress.DTOs;
using AutoRentExpress.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Controllers
{
    public class EstadosVehiculoController : Controller
    {
        private readonly IEstadoVehiculoService _service;

        public EstadosVehiculoController(IEstadoVehiculoService service)
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
            return View(new EstadoVehiculoDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EstadoVehiculoDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _service.CrearAsync(dto);
                TempData["Exito"] = "Estado registrado correctamente.";
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
            var estado = await _service.ObtenerPorIdAsync(id);

            return estado == null ? NotFound() : View(estado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EstadoVehiculoDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _service.ActualizarAsync(dto);
                TempData["Exito"] = "Estado actualizado correctamente.";
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
                TempData["Exito"] = "Estado eliminado correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "El estado no existe.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}