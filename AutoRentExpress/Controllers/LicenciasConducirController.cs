
using AutoRentExpress.DTOs;
using AutoRentExpress.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoRentExpress.Controllers
{
    public class LicenciasController : Controller
    {
        private readonly ILicenciaConducirService _service;

        public LicenciasController(ILicenciaConducirService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Clientes = await _service.ObtenerClientesAsync();

            return View(await _service.ObtenerTodosAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Clientes = await _service.ObtenerClientesAsync();

            return View(new LicenciaConducirDto
            {
                PaisEmision = "Nicaragua",
                FechaEmision = DateTime.Today,
                FechaVencimiento = DateTime.Today.AddYears(5)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LicenciaConducirDto dto)
        {
            ModelState.Remove(nameof(LicenciaConducirDto.NombreCliente));

            if (!ModelState.IsValid)
            {
                ViewBag.Clientes = await _service.ObtenerClientesAsync();
                return View(dto);
            }

            try
            {
                await _service.CrearAsync(dto);

                TempData["Exito"] = "Licencia registrada correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            ViewBag.Clientes = await _service.ObtenerClientesAsync();

            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var licencia = await _service.ObtenerPorIdAsync(id);

            if (licencia == null)
            {
                return NotFound();
            }

            ViewBag.Clientes = await _service.ObtenerClientesAsync();

            return View(licencia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LicenciaConducirDto dto)
        {
            ModelState.Remove(nameof(LicenciaConducirDto.NombreCliente));

            if (!ModelState.IsValid)
            {
                ViewBag.Clientes = await _service.ObtenerClientesAsync();
                return View(dto);
            }

            try
            {
                await _service.ActualizarAsync(dto);

                TempData["Exito"] = "Licencia actualizada correctamente.";

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

            ViewBag.Clientes = await _service.ObtenerClientesAsync();

            return View(dto);
        }
    }
}
