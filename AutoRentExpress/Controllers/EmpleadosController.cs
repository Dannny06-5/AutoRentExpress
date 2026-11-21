using AutoRentExpress.DTOs;
using AutoRentExpress.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoRentExpress.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly IEmpleadoService _service;

        public EmpleadosController(IEmpleadoService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Sucursales = await _service.ObtenerSucursalesAsync();

            return View(await _service.ObtenerTodosAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Sucursales = await _service.ObtenerSucursalesAsync();

            return View(new EmpleadoDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmpleadoDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Sucursales = await _service.ObtenerSucursalesAsync();
                return View(dto);
            }

            try
            {
                await _service.CrearAsync(dto);

                TempData["Exito"] = "Empleado registrado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            ViewBag.Sucursales = await _service.ObtenerSucursalesAsync();

            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var empleado = await _service.ObtenerPorIdAsync(id);

            if (empleado == null)
                return NotFound();

            ViewBag.Sucursales = await _service.ObtenerSucursalesAsync();

            return View(empleado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmpleadoDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Sucursales = await _service.ObtenerSucursalesAsync();
                return View(dto);
            }

            try
            {
                await _service.ActualizarAsync(dto);

                TempData["Exito"] = "Empleado actualizado correctamente.";

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

            ViewBag.Sucursales = await _service.ObtenerSucursalesAsync();

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            try
            {
                await _service.CambiarEstadoActivoAsync(id);

                TempData["Exito"] = "Estado del empleado actualizado correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "El empleado no existe.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}