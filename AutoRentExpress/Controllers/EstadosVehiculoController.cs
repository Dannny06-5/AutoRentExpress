
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoRentExpress.Models;
using AutoRentExpress.Data;

public class EstadosVehiculoController : Controller
{
    private readonly AppDbContext _context;

    public EstadosVehiculoController(AppDbContext context)
    {
        _context = context;
    }

    // GET: ESTADOVEHICULOS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.EstadosVehiculo.ToListAsync());
    }

    // GET: ESTADOVEHICULOS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var estadovehiculo = await _context.EstadosVehiculo
            .FirstOrDefaultAsync(m => m.Id == id);
        if (estadovehiculo == null)
        {
            return NotFound();
        }

        return View(estadovehiculo);
    }

    // GET: ESTADOVEHICULOS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: ESTADOVEHICULOS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Nombre,Vehiculos")] EstadoVehiculo estadovehiculo)
    {
        if (ModelState.IsValid)
        {
            _context.Add(estadovehiculo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(estadovehiculo);
    }

    // GET: ESTADOVEHICULOS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var estadovehiculo = await _context.EstadosVehiculo.FindAsync(id);
        if (estadovehiculo == null)
        {
            return NotFound();
        }
        return View(estadovehiculo);
    }

    // POST: ESTADOVEHICULOS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Nombre,Vehiculos")] EstadoVehiculo estadovehiculo)
    {
        if (id != estadovehiculo.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(estadovehiculo);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EstadoVehiculoExists(estadovehiculo.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(estadovehiculo);
    }

    // GET: ESTADOVEHICULOS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var estadovehiculo = await _context.EstadosVehiculo
            .FirstOrDefaultAsync(m => m.Id == id);
        if (estadovehiculo == null)
        {
            return NotFound();
        }

        return View(estadovehiculo);
    }

    // POST: ESTADOVEHICULOS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var estadovehiculo = await _context.EstadosVehiculo.FindAsync(id);
        if (estadovehiculo != null)
        {
            _context.EstadosVehiculo.Remove(estadovehiculo);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool EstadoVehiculoExists(int? id)
    {
        return _context.EstadosVehiculo.Any(e => e.Id == id);
    }
}
