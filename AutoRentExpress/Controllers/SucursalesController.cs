
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoRentExpress.Models;
using AutoRentExpress.Data;

public class SucursalesController : Controller
{
    private readonly AppDbContext _context;

    public SucursalesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: SUCURSALS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Sucursales.ToListAsync());
    }

    // GET: SUCURSALS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var sucursal = await _context.Sucursales
            .FirstOrDefaultAsync(m => m.Id == id);
        if (sucursal == null)
        {
            return NotFound();
        }

        return View(sucursal);
    }

    // GET: SUCURSALS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: SUCURSALS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Nombre,Direccion,Telefono,Activa,Empleados,Vehiculos")] Sucursal sucursal)
    {
        if (ModelState.IsValid)
        {
            _context.Add(sucursal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(sucursal);
    }

    // GET: SUCURSALS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var sucursal = await _context.Sucursales.FindAsync(id);
        if (sucursal == null)
        {
            return NotFound();
        }
        return View(sucursal);
    }

    // POST: SUCURSALS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Nombre,Direccion,Telefono,Activa,Empleados,Vehiculos")] Sucursal sucursal)
    {
        if (id != sucursal.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(sucursal);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SucursalExists(sucursal.Id))
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
        return View(sucursal);
    }

    // GET: SUCURSALS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var sucursal = await _context.Sucursales
            .FirstOrDefaultAsync(m => m.Id == id);
        if (sucursal == null)
        {
            return NotFound();
        }

        return View(sucursal);
    }

    // POST: SUCURSALS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var sucursal = await _context.Sucursales.FindAsync(id);
        if (sucursal != null)
        {
            _context.Sucursales.Remove(sucursal);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool SucursalExists(int? id)
    {
        return _context.Sucursales.Any(e => e.Id == id);
    }
}
