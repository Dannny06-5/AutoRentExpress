
using AutoRentExpress.DTOs;
using AutoRentExpress.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoRentExpress.Controllers
{
    public class ReservasController : Controller
    {
        private readonly IReservaService _service;

        public ReservasController(IReservaService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Catalogos = await _service.ObtenerCatalogosAsync();

            return View(await _service.ObtenerTodosAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Disponibles(
            DateTime fechaInicio,
            DateTime fechaFin,
            int sucursalRetiroId,
            int reservaId = 0)
        {
            var vehiculos = await _service.ObtenerDisponiblesAsync(
                fechaInicio,
                fechaFin,
                sucursalRetiroId,
                reservaId);

            return Json(vehiculos);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Catalogos = await _service.ObtenerCatalogosAsync();

            var inicio = DateTime.Now.AddHours(1);

            inicio = new DateTime(
                inicio.Year,
                inicio.Month,
                inicio.Day,
                inicio.Hour,
                0,
                0);

            return View(new ReservaDto
            {
                FechaInicio = inicio,
                FechaFin = inicio.AddDays(1)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservaDto dto)
        {
            QuitarCamposCalculados();

            if (!ModelState.IsValid)
            {
                ViewBag.Catalogos = await _service.ObtenerCatalogosAsync();
                return View(dto);
            }

            try
            {
                await _service.CrearAsync(dto);

                TempData["Exito"] = "Reserva registrada correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            ViewBag.Catalogos = await _service.ObtenerCatalogosAsync();

            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var reserva = await _service.ObtenerPorIdAsync(id);

            if (reserva == null)
            {
                return NotFound();
            }

            if (reserva.TieneContrato ||
                (reserva.Estado != "Pendiente" &&
                 reserva.Estado != "Confirmada"))
            {
                TempData["Error"] =
                    "Esta reserva no permite modificaciones porque tiene un contrato o su estado está bloqueado.";

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Catalogos = await _service.ObtenerCatalogosAsync();

            return View(reserva);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ReservaDto dto)
        {
            QuitarCamposCalculados();

            if (!ModelState.IsValid)
            {
                ViewBag.Catalogos = await _service.ObtenerCatalogosAsync();
                return View(dto);
            }

            try
            {
                await _service.ActualizarAsync(dto);

                TempData["Exito"] = "Reserva actualizada correctamente.";

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

            ViewBag.Catalogos = await _service.ObtenerCatalogosAsync();

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            try
            {
                await _service.CancelarAsync(id);

                TempData["Exito"] = "Reserva cancelada correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "La reserva no existe.";
            }

            return RedirectToAction(nameof(Index));
        }

        private void QuitarCamposCalculados()
        {
            ModelState.Remove(nameof(ReservaDto.Estado));
            ModelState.Remove(nameof(ReservaDto.FechaRegistro));
            ModelState.Remove(nameof(ReservaDto.TotalEstimado));
            ModelState.Remove(nameof(ReservaDto.NombreCliente));
            ModelState.Remove(nameof(ReservaDto.NombreVehiculo));
            ModelState.Remove(nameof(ReservaDto.PlacaVehiculo));
            ModelState.Remove(nameof(ReservaDto.NombreSucursalRetiro));
            ModelState.Remove(nameof(ReservaDto.NombreSucursalDevolucion));
            ModelState.Remove(nameof(ReservaDto.NombreSeguro));
            ModelState.Remove(nameof(ReservaDto.CostoSeguro));
            ModelState.Remove(nameof(ReservaDto.TieneContrato));
        }
    }
}
