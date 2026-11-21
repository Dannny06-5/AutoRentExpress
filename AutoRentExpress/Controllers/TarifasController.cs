
using AutoRentExpress.DTOs;
using AutoRentExpress.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoRentExpress.Controllers
{
    public class TarifasController : Controller
    {
        private readonly ITarifaService _service;

        public TarifasController(ITarifaService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Categorias = await _service.ObtenerCategoriasAsync();

            return View(await _service.ObtenerTodosAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categorias = await _service.ObtenerCategoriasAsync();

            return View(new TarifaDto
            {
                VigenteDesde = DateTime.Today
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TarifaDto dto)
        {
            ModelState.Remove(nameof(TarifaDto.NombreCategoria));

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = await _service.ObtenerCategoriasAsync();
                return View(dto);
            }

            try
            {
                await _service.CrearAsync(dto);

                TempData["Exito"] = "Tarifa registrada correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            ViewBag.Categorias = await _service.ObtenerCategoriasAsync();

            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var tarifa = await _service.ObtenerPorIdAsync(id);

            if (tarifa == null)
            {
                return NotFound();
            }

            ViewBag.Categorias = await _service.ObtenerCategoriasAsync();

            return View(tarifa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TarifaDto dto)
        {
            ModelState.Remove(nameof(TarifaDto.NombreCategoria));

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = await _service.ObtenerCategoriasAsync();
                return View(dto);
            }

            try
            {
                await _service.ActualizarAsync(dto);

                TempData["Exito"] = "Tarifa actualizada correctamente.";

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

            ViewBag.Categorias = await _service.ObtenerCategoriasAsync();

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            try
            {
                await _service.CambiarEstadoAsync(id);

                TempData["Exito"] =
                    "Estado de la tarifa actualizado correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "La tarifa seleccionada no existe.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
