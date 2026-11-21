
using AutoRentExpress.DTOs;
using AutoRentExpress.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoRentExpress.Controllers
{
    public class ContratosController : Controller
    {
        private readonly IContratoService _service;

        public ContratosController(IContratoService service)
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
            ViewBag.Catalogos = await _service.ObtenerCatalogosAsync();

            return View(new ContratoDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContratoDto dto)
        {
            ModelState.Remove(nameof(ContratoDto.NumeroContrato));
            ModelState.Remove(nameof(ContratoDto.FechaFirma));
            ModelState.Remove(nameof(ContratoDto.FechaEntrega));
            ModelState.Remove(nameof(ContratoDto.FechaDevolucionPrevista));
            ModelState.Remove(nameof(ContratoDto.FechaDevolucionReal));
            ModelState.Remove(nameof(ContratoDto.TotalContrato));
            ModelState.Remove(nameof(ContratoDto.Estado));
            ModelState.Remove(nameof(ContratoDto.NombreCliente));
            ModelState.Remove(nameof(ContratoDto.Vehiculo));
            ModelState.Remove(nameof(ContratoDto.PlacaVehiculo));

            if (!ModelState.IsValid)
            {
                ViewBag.Catalogos = await _service.ObtenerCatalogosAsync();
                return View(dto);
            }

            try
            {
                await _service.CrearAsync(dto);

                TempData["Exito"] = "Contrato generado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "No se pudo registrar el contrato. Comprueba que la reserva no tenga otro contrato.");
            }

            ViewBag.Catalogos = await _service.ObtenerCatalogosAsync();

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEntrega(int id)
        {
            try
            {
                await _service.ConfirmarEntregaAsync(id);

                TempData["Exito"] =
                    "Entrega confirmada. El alquiler está en curso.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "El contrato no existe.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarDevolucion(int id)
        {
            try
            {
                await _service.ConfirmarDevolucionAsync(id);

                TempData["Exito"] =
                    "Devolución confirmada. El alquiler ha finalizado.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "El contrato no existe.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
