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
using EmpresaT3.Core;

namespace EmpresaT3.Controllers
{
    [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
    public class LogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Logs
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _context.Logs.OrderByDescending(s => s.Id).ToListAsync());
            }
            catch (Exception)
            {
                return NotFound("Error");
            }
            
        }

        // GET: Logs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null || _context.Logs == null)
                {
                    return NotFound();
                }

                var logs = await _context.Logs
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (logs == null)
                {
                    return NotFound();
                }

                return View(logs);
            }
            catch (Exception)
            {
                return NotFound("Error");
            }
            
        }

        // POST: Logs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (_context.Logs == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Category'  is null.");
                }
                var logs = await _context.Logs.FindAsync(id);
                if (logs != null)
                {
                    _context.Logs.Remove(logs);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return NotFound("Error");
            }
            
        }

        private bool LogsExists(int id)
        {
          return _context.Logs.Any(e => e.Id == id);
        }
    }
}
