using AutoRentExpress.DTOs;
using AutoRentExpress.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoRentExpress.Controllers
{
    public class PagosController : Controller
    {
        private readonly IPagoService _service;

        public PagosController(IPagoService service)
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
            ViewBag.Contratos = await _service.ObtenerContratosAsync();

            return View(new PagoDto
            {
                Metodo = "Efectivo"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PagoDto dto)
        {
            ModelState.Remove(nameof(PagoDto.Estado));
            ModelState.Remove(nameof(PagoDto.FechaPago));
            ModelState.Remove(nameof(PagoDto.NumeroContrato));
            ModelState.Remove(nameof(PagoDto.NombreCliente));
            ModelState.Remove(nameof(PagoDto.TotalContrato));
            ModelState.Remove(nameof(PagoDto.TotalPagado));
            ModelState.Remove(nameof(PagoDto.SaldoPendiente));

            if (!ModelState.IsValid)
            {
                ViewBag.Contratos = await _service.ObtenerContratosAsync();
                return View(dto);
            }

            try
            {
                await _service.RegistrarAsync(dto);

                TempData["Exito"] = "Pago registrado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            ViewBag.Contratos = await _service.ObtenerContratosAsync();

            return View(dto);
        }
    }
}