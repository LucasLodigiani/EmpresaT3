using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmpresaT3.Models;
using EmpresaT3.Data;
using EmpresaT3.ViewModels;
using System.Dynamic;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace EmpresaT3.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public ProductosController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index(string searchByName, string searchByCategory)
        {
            if(_context.Productos == null)
            {
                return Problem("Entity error set");
            }
            //METODO LINQ PARA OBTENER LOS PRODUCTOS, SOLO SE OBTIENE UNA VEZ
            var products = from p in _context.Productos select p;

            if (!String.IsNullOrEmpty(searchByName) && !String.IsNullOrEmpty(searchByCategory))
            {
                products = products.Where(x => x.Nombre!.Contains(searchByName) && x.Categoria!.Contains(searchByCategory));
            }

            if (!String.IsNullOrEmpty(searchByName) || !String.IsNullOrEmpty(searchByCategory))
            {
                products = products.Where(x => x.Nombre!.Contains(searchByName) || x.Categoria!.Contains(searchByCategory));
            }


            ViewBag.Categoria = await _context.Category.ToListAsync();

            return View(await products.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var product = await _context.Productos
                    .FirstOrDefaultAsync(m => m.Id == id);

                if(product == null)
                {
                    return NotFound("Eggh lo que estas buscando no se encontro en la base de datos :(");
                }

                var productoViewModel = new ProductoViewModel()
                {
                    Id = product.Id,
                    Nombre = product.Nombre,
                    Descripcion = product.Descripcion,
                    Categoria = product.Categoria,
                    Precio = product.Precio,
                    ExistingImage = product.ProfilePicture
                };

                if (product == null)
                {
                    return NotFound();
                }
                return View(product);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [Authorize]
        public async Task<IActionResult> CreateAsync()
        {

            ViewBag.Categoria = await _context.Category.ToListAsync();
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoViewModel model)
        {
            ModelState.Remove("ExistingImage");
            try
            {
                if (ModelState.IsValid)
                {
                    string uniqueFileName = ProcessUploadedFile(model);
                    Producto product = new()
                    {
                        Nombre = model.Nombre,
                        Descripcion = model.Descripcion,
                        Categoria = model.Categoria,
                        Precio = model.Precio,
                        ProfilePicture = uniqueFileName
                    };

                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    GuardarLog(product.Id, "Crear", "Productos");
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception)
            {

                throw;
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Productos.FindAsync(id);
            var productViewModel = new ProductoViewModel()
            {
                Id = product.Id,
                Nombre = product.Nombre,
                Descripcion = product.Descripcion,
                Categoria = product.Categoria,
                Precio = product.Precio,
                ExistingImage = product.ProfilePicture
            };

            if (product == null)
            {
                return NotFound();
            }
            ViewBag.Categoria = await _context.Category.ToListAsync();

            

            return View(productViewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductoViewModel model)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    var product = await _context.Productos.FindAsync(model.Id);
                    product.Nombre = model.Nombre;
                    product.Descripcion = model.Descripcion;
                    product.Categoria = model.Categoria;
                    product.Precio = model.Precio;

                    if (model.ProductPicture != null)
                    {
                        if (model.ExistingImage != null)
                        {
                            string filePath = Path.Combine(_environment.WebRootPath, "Uploads", model.ExistingImage);
                            System.IO.File.Delete(filePath);
                        }

                        product.ProfilePicture = ProcessUploadedFile(model);
                    }
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    GuardarLog(product.Id, "Editar", "Productos");
                    return RedirectToAction(nameof(Index));
                }
            }
            catch(Exception)
            {
                throw;
            }

            return View();
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Productos
                .FirstOrDefaultAsync(m => m.Id == id);

            var speakerViewModel = new ProductoViewModel()
            {
                Id = product.Id,
                Nombre = product.Nombre,
                Descripcion = product.Descripcion,
                Categoria = product.Categoria,
                Precio = product.Precio,
                ExistingImage = product.ProfilePicture
            };
            if (product == null)
            {
                return NotFound();
            }

            return View(speakerViewModel);
        }

        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Productos.FindAsync(id);
            //string deleteFileFromFolder = "wwwroot\\Uploads\\";
            string deleteFileFromFolder = Path.Combine(_environment.WebRootPath, "Uploads");
            var CurrentImage = Path.Combine(Directory.GetCurrentDirectory(), deleteFileFromFolder, product.ProfilePicture);
            _context.Productos.Remove(product);
            if (System.IO.File.Exists(CurrentImage))
            {
                System.IO.File.Delete(CurrentImage);
            }
            await _context.SaveChangesAsync();
            GuardarLog(product.Id, "Borrar", "Productos");
            return RedirectToAction(nameof(Index));
        }

        private bool SpeakerExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }

        private string ProcessUploadedFile(ProductoViewModel model)
        {
            string uniqueFileName = null;
            string path = Path.Combine(_environment.WebRootPath, "Uploads");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (model.ProductPicture != null)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "Uploads");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProductPicture.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProductPicture.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
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
