using AutoRentExpress.DTOs;
using AutoRentExpress.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Controllers
{
    public class ModelosController : Controller
    {
        private readonly IModeloService _service;
        private readonly IMarcaService _marcas;

        public ModelosController(
            IModeloService service,
            IMarcaService marcas)
        {
            _service = service;
            _marcas = marcas;
        }

        public async Task<IActionResult> Index()
        {
            await CargarMarcas();
            return View(await _service.ObtenerTodosAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CargarMarcas();
            return View(new ModeloDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ModeloDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.CrearAsync(dto);
                    TempData["Exito"] = "Modelo registrado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            await CargarMarcas();
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var modelo = await _service.ObtenerPorIdAsync(id);

            if (modelo == null)
                return NotFound();

            await CargarMarcas();
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ModeloDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.ActualizarAsync(dto);
                    TempData["Exito"] = "Modelo actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
            }

            await CargarMarcas();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.EliminarAsync(id);
                TempData["Exito"] = "Modelo eliminado correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "El modelo no existe.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarMarcas()
        {
            var marcas = await _marcas.ObtenerTodosAsync();

            ViewBag.Marcas = new SelectList(
                marcas,
                nameof(MarcaDto.Id),
                nameof(MarcaDto.Nombre));
        }
    }
}