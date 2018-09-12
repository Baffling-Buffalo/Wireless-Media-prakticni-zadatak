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
            // include categories and companies so I can their names
            var products = _context.Products.Include(p => p.Category).Include(p => p.Manufacturer).Include(p => p.Supplier);

            // make ProductViews foreach product
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

        public IActionResult Create()
        {
            // add categories and companies to viewbags so datalists in view can be filled with autosuggestions
            FillDataListViewBags();
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
                Category category = await GetOrCreateCategory(productView.Category);
                product.CategoryId = category.CategoryId;

                // Check if inputed manufacturer exists, if not, create new one
                Company manufacturer = await GetOrCreateCompany(productView.Manufacturer);
                product.ManufacturerId = manufacturer.CompanyId;

                // Check if inputed supplier exists, if not, create new one
                Company supplier = await GetOrCreateCompany(productView.Supplier);
                product.SupplierId = supplier.CompanyId;


                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // add categories and companies to viewbags so datalists in view can be filled with autosuggestions
            FillDataListViewBags();
            return View(productView);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // include categories and companies so I can get their names
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Supplier)
                .SingleOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            // make a ProductView out of selected product
            ProductView productView = new ProductView{
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Category = product.Category.Name,
                Manufacturer = product.Manufacturer.Name,
                Supplier = product.Supplier.Name,
                Price = product.Price
            };

            // add categories and companies to viewbags so datalists in view can be filled with autosuggestions
            FillDataListViewBags();

            return View(productView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,Description,Category,Manufacturer,Supplier,Price")] ProductView productView)
        {
            // Find product from ProductView
            Product product = _context.Products.Find(productView.ProductId);

            if (product == null)
                return RedirectToAction(nameof(Index));

            if (id != productView?.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Edit changes to product object
                product.Name = productView.Name;
                product.Description = productView.Description;
                product.Price = productView.Price;
                Category category = await GetOrCreateCategory(productView.Category); // Find or create edited category
                product.CategoryId = category.CategoryId; // Apply it's id
                Company manufacturer = await GetOrCreateCompany(productView.Manufacturer); // Find or create edited manufacturer
                product.ManufacturerId = manufacturer.CompanyId; // Apply it's id
                Company supplier = await GetOrCreateCompany(productView.Supplier); // Find or create edited manufacturer
                product.SupplierId = supplier.CompanyId; // Apply it's id

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

            // add categories and companies to viewbags so datalists in view can be filled with autosuggestions
            FillDataListViewBags();
            return View(productView);
        }

        public void FillDataListViewBags()
        {
            // add categories and companies to viewbags so datalists in view can be filled with autosuggestions
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Companies = _context.Companies.ToList();
        }

        // find category with name or create new one if it doesnt exist
        public async Task<Category> GetOrCreateCategory(string categoryName)
        {
            // Search for category with that name
            Category category = _context.Categories.SingleOrDefault(cat => cat.Name.ToLower() == categoryName.ToLower());
            if (category == null)
            {
                // Create new category if null
                Category newCategory = new Category { Name = categoryName };
                try
                {
                    await _context.Categories.AddAsync(newCategory);
                    await _context.SaveChangesAsync();

                    return newCategory;
                }
                catch (Exception)
                {
                    return category; // which is null
                }
            }
            else
            {
                // If not null return the one you found
                return category;
            }
        }

        // find company with name or create new one if it doesnt exist
        public async Task<Company> GetOrCreateCompany(string companyName)
        {
            // Search for company with that name
            Company company = _context.Companies.SingleOrDefault(comp => comp.Name.ToLower() == companyName.ToLower());
            if (company == null)
            {
                // Create new company if null
                Company newCompany = new Company { Name = companyName };
                try
                {
                    await _context.Companies.AddAsync(newCompany);
                    await _context.SaveChangesAsync();

                    return newCompany;
                }
                catch (Exception)
                {
                    return company; // which is null
                }
            }
            else
            {
                // If not null return the one you found
                return company;
            }
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
