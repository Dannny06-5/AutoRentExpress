using AutoRentExpress.DTOs;
using AutoRentExpress.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoRentExpress.Controllers
{
    public class ClientesController : Controller
    {
        private readonly IClienteService _service;

        public ClientesController(IClienteService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var clientes = await _service.ObtenerTodosAsync();
            return View(clientes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ClienteDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _service.CrearAsync(dto);
                TempData["Exito"] = "Cliente registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(dto.Identificacion), ex.Message);
            }
            catch (System.ComponentModel.DataAnnotations.ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _service.ObtenerPorIdAsync(id);

            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClienteDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _service.ActualizarAsync(dto);
                TempData["Exito"] = "Cliente actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(dto.Identificacion), ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (System.ComponentModel.DataAnnotations.ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            try
            {
                await _service.DesactivarAsync(id);
                TempData["Exito"] = "Cliente desactivado correctamente.";
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "El cliente no existe.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activar(int id)
        {
            try
            {
                await _service.ActivarAsync(id);
                TempData["Exito"] = "Cliente activado correctamente.";
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "El cliente no existe.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}