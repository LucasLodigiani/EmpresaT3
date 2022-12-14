using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmpresaT3.Areas.Identity.Data;
using EmpresaT3.Models;
using Microsoft.AspNetCore.Authorization;

namespace EmpresaT3.Controllers
{
    [Authorize]
    public class CategoriasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CategoriasController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Categorias
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _context.Category.OrderByDescending(s => s.Id).ToListAsync());
            }
            catch (Exception)
            {
                return NotFound("Error");
            }
            
        }

        // GET: Categorias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null || _context.Category == null)
                {
                    return NotFound();
                }

                var category = await _context.Category
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (category == null)
                {
                    return NotFound();
                }

                return View(category);
            }
            catch (Exception)
            {
                return NotFound("Error");
            }
            
        }

        // GET: Categorias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categorias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Categorias")] Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(category);
                    await _context.SaveChangesAsync();
                    GuardarLog(category.Id, "Crear", "Categorias");
                    return RedirectToAction(nameof(Index));
                }
                return View(category);
            }
            catch (Exception)
            {
                return NotFound("Error");
            }
            
        }

        // GET: Categorias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null || _context.Category == null)
                {
                    return NotFound();
                }

                var category = await _context.Category.FindAsync(id);
                if (category == null)
                {
                    return NotFound();
                }
                return View(category);
            }
            catch (Exception)
            {
                return NotFound("Error");
            }
            
        }

        // POST: Categorias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string cat,[Bind("Id,Categorias")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    //ACTUALIZAR CATEGORIA DE PRODUCTOS
                    var product = await _context.Productos.Where(x => x.Categoria == cat).ToListAsync();
                    
                    foreach (var x in product)
                    {
                        x.Categoria = category.Categorias;
                    }

                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    GuardarLog(category.Id, "Editar", "Categorias");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            return View(category);
        }

        // GET: Categorias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null || _context.Category == null)
                {
                    return NotFound();
                }

                var category = await _context.Category
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (category == null)
                {
                    return NotFound();
                }

                return View(category);
            }
            catch(Exception)
            {
                return NotFound("Error");
            }

        }

        // POST: Categorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (_context.Category == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Category'  is null.");
                }
                var category = await _context.Category.FindAsync(id);


                //ELIMINAR PRODUCTOS E IMAGENES
                var product = await _context.Productos.Where(x => x.Categoria == category.Categorias).ToListAsync();
                string deleteFileFromFolder = Path.Combine(_environment.WebRootPath, "Uploads");
                foreach (var p in product)
                {
                    var CurrentImage = Path.Combine(Directory.GetCurrentDirectory(), deleteFileFromFolder, p.ProfilePicture);
                    if (System.IO.File.Exists(CurrentImage))
                    {
                        System.IO.File.Delete(CurrentImage);
                    }
                    _context.Remove<Producto>(p);
                }

                //ELIMINAR CATEGORIA
                if (category != null)
                {
                    _context.Category.Remove(category);
                }

                await _context.SaveChangesAsync();
                GuardarLog(category.Id, "Borrar", "Categorias");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return NotFound("Error");
            }
            
        }

        private bool CategoryExists(int id)
        {
          return _context.Category.Any(e => e.Id == id);
        }






        void GuardarLog(int? id, string accion, string operacion)
        {
            TimeZoneInfo tzone = TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time");
            DateTime dt = DateTime.UtcNow;
            var datetime2 = TimeZoneInfo.ConvertTimeFromUtc(dt, tzone).ToString();

            Logs logs = new Logs()
            {
                Usuario = User.Identity.Name,
                Accion = accion,
                Producto = id,
                Fecha = datetime2,
                Operacion = operacion,
                IpAddress = RemoteIP.GetClientIP(),
            };
            _context.Add(logs);
            _context.SaveChanges();

        }
    }
}
