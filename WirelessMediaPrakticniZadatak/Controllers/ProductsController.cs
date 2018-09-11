using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WirelessMediaPrakticniZadatak;
using WirelessMediaPrakticniZadatak.Models;

namespace WirelessMediaPrakticniZadatak.Controllers
{
    public class ProductsController : Controller
    {
        private readonly WirelessmediaprakticnizadatakdbContext _context;

        public ProductsController(WirelessmediaprakticnizadatakdbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products.Include(p => p.Category).Include(p => p.Manufacturer).Include(p => p.Supplier);

            List<ProductView> productViews = new List<ProductView>();

            foreach(var product in products)
            {
                ProductView prView = new ProductView{
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    Category = product.Category.Name,
                    Manufacturer = product.Manufacturer.Name,
                    Supplier = product.Supplier.Name,
                    Price = product.Price
                };

                productViews.Add(prView);
            }

            return View(productViews);
        }

        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = await _context.Products
        //        .Include(p => p.Category)
        //        .Include(p => p.Manufacturer)
        //        .Include(p => p.Supplier)
        //        .SingleOrDefaultAsync(m => m.ProductId == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(product);
        //}

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Name,Description,Category,Manufacturer,Supplier,Price")] ProductView productView)
        {
            if (ModelState.IsValid)
            {
                // Create a product object out of productView
                Product product = new Product
                {
                    Name = productView.Name,
                    Description = productView.Description,
                    Price = productView.Price
                };

                // Check if inputed category exists, if not, create new one
                Category category = _context.Categories.SingleOrDefault(cat => cat.Name.ToLower() == productView.Category.ToLower());
                if (category == null)
                {
                    // Create new category if null
                    Category newCategory = new Category { Name = productView.Category, Description = "" };
                    try
                    {
                        _context.Categories.Add(newCategory);
                        _context.SaveChanges();

                        product.CategoryId = newCategory.CategoryId;
                    }
                    catch (Exception)
                    {
                        return View("Error");
                    }
                }
                else
                {
                    // If not null apply the one you found
                    product.CategoryId = category.CategoryId;
                }

                // Check if inputed manufacturer exists, if not, create new one
                Company manufacturer = _context.Companies.SingleOrDefault(comp => comp.Name.ToLower() == productView.Manufacturer.ToLower());
                if (manufacturer == null)
                {
                    // Create new company if null
                    Company newCompany = new Company { Name = productView.Manufacturer};
                    try
                    {
                        _context.Companies.Add(newCompany);
                        _context.SaveChanges();

                        product.ManufacturerId = newCompany.CompanyId;
                    }
                    catch (Exception)
                    {
                        return View("Error");
                    }
                }
                else
                {
                    // If not null apply the one you found
                    product.ManufacturerId = manufacturer.CompanyId;
                }

                // Check if inputed supplier exists, if not, create new one
                Company supplier = _context.Companies.SingleOrDefault(comp => comp.Name.ToLower() == productView.Supplier.ToLower());
                if (supplier == null)
                {
                    // Create new company if null
                    Company newCompany = new Company { Name = productView.Supplier };
                    try
                    {
                        _context.Companies.Add(newCompany);
                        _context.SaveChanges();

                        product.SupplierId = newCompany.CompanyId;
                    }
                    catch (Exception)
                    {
                        return View("Error");
                    }
                }
                else
                {
                    // If not null apply the one you found
                    product.SupplierId = supplier.CompanyId;
                }


                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(productView);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.SingleOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            ViewData["ManufacturerId"] = new SelectList(_context.Companies, "CompanyId", "Name", product.ManufacturerId);
            ViewData["SupplierId"] = new SelectList(_context.Companies, "CompanyId", "Name", product.SupplierId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,Description,CategoryId,ManufacturerId,SupplierId,Price")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            ViewData["ManufacturerId"] = new SelectList(_context.Companies, "CompanyId", "Name", product.ManufacturerId);
            ViewData["SupplierId"] = new SelectList(_context.Companies, "CompanyId", "Name", product.SupplierId);
            return View(product);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Supplier)
                .SingleOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.SingleOrDefaultAsync(m => m.ProductId == id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
