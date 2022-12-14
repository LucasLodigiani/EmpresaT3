using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmpresaT3.Models;
using EmpresaT3.Areas.Identity.Data;
using EmpresaT3.ViewModels;
using System.Dynamic;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using X.PagedList;
using EmpresaT3.Core;

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

        public async Task<IActionResult> Index(int? page, string searchByName, string searchByCategory, string currentName, string currentCategory)
        {
            try
            {
                if (_context.Productos == null)
                {
                    return Problem("Entity error set");
                }

                //BUSQUEDA
                if (searchByName != null || searchByCategory != null)
                {
                    page = 1;
                }
                else
                {
                    searchByName = currentName;
                    searchByCategory = currentCategory;
                }

                ViewBag.CurrentName = searchByName;
                ViewBag.CurrentCategory = searchByCategory;




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


                //////PAGINACION
                var pageNumber = page ?? 1;
                var onePageOfProducts = products.OrderByDescending(s => s.Id).ToPagedList(pageNumber, 5);
                ViewBag.OnePageOfProducts = onePageOfProducts;


                ViewBag.Categoria = await _context.Category.ToListAsync();

                return View();
            }
            catch(Exception)
            {
                return NotFound("Error desconocido");
            }
            
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

                if (product == null)
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

                return NotFound("Error");
            }
        }

        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
        public async Task<IActionResult> CreateAsync()
        {
            try
            {
                ViewBag.Categoria = await _context.Category.ToListAsync();
                return View();
            }
            catch (Exception)
            {
                return NotFound("Error desconocido");
            } 
        }

        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
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

                return NotFound("Error desconocido");
            }
            return View(model);
        }

        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
        public async Task<IActionResult> Edit(int? id)
        {
            try
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
            catch (Exception)
            {
                return NotFound("Error desconocido");
            }
            
        }

        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
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
            catch (Exception)
            {
                return NotFound("Error desconocido");
            }

            return View();
        }

        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
        public async Task<IActionResult> Delete(int? id)
        {
            try
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
            catch (Exception)
            {
                return NotFound("Error desconocido");
            }
            
        }

        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
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
            catch (Exception)
            {
                return NotFound("Error desconocido");
            }
            
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