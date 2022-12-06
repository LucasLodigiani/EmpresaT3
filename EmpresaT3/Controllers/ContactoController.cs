using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmpresaT3.Data;
using EmpresaT3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace EmpresaT3.Controllers
{
    public class ContactoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Contacto
        [Authorize]
        public async Task<IActionResult> MensajesLista()
        {
            return View(await _context.Contacto.OrderByDescending(s => s.Id).ToListAsync());

        }

        // GET: Contacto/Create
        public IActionResult Index()
        {
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("Id,Nombre,Email,Mensaje")] Contacto contacto)
        {
            ModelState.Remove("Fecha");
            ModelState.Remove("IpAddress");
            if (ModelState.IsValid)
            {
                TimeZoneInfo tzone = TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time");
                DateTime dt = DateTime.UtcNow;
                var datetime2 = TimeZoneInfo.ConvertTimeFromUtc(dt, tzone).ToString();

                Contacto nuevoMensaje = new()
                {
                    Nombre = contacto.Nombre,
                    Email = contacto.Email,
                    Mensaje = contacto.Mensaje,
                    Fecha = datetime2,
                    IpAddress = RemoteIP.GetClientIP(),
                };
                _context.Add(nuevoMensaje);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contacto);
        }




        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Contacto == null)
            {
                return NotFound();
            }

            var contacto = await _context.Contacto
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contacto == null)
            {
                return NotFound();
            }

            return View(contacto);
        }

        // POST: Contacto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Contacto == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Contacto'  is null.");
            }
            var contacto = await _context.Contacto.FindAsync(id);
            if (contacto != null)
            {
                _context.Contacto.Remove(contacto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactoExists(int id)
        {
            return _context.Contacto.Any(e => e.Id == id);
        }
    }
}
