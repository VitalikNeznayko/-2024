using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CandyStoreApp.Models;
using Microsoft.Data.SqlClient;

namespace CandyStoreApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly CandyStoreContext _context;

        public ProductsController(CandyStoreContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(string searchProductName, string searchCategory, string searchDescription, string searchSupplier, string searchPrice, string searchQuantity, string sortColumn = "product_name", string sortOrder = "asc")
        {
            string sortOrderClause = sortOrder == "asc" ? "ASC" : "DESC";
            ViewData["SearchProductName"] = searchProductName;
            ViewData["SearchCategory"] = searchCategory;
            ViewData["SearchSupplier"] = searchSupplier;
            ViewData["SearchPrice"] = searchPrice;
            ViewData["SearchQuantity"] = searchQuantity;
            ViewData["SearchDescription"] = searchDescription;
            ViewData["sortColumn"] = sortColumn;
            ViewData["sortOrder"] = sortOrder;

            var sqlQuery = $@"
                SELECT p.* 
                FROM [Products] p
                LEFT JOIN [Categories] c ON c.[Id_Category] = p.[Id_Category]
                LEFT JOIN [Suppliers] s ON s.[Id_Supplier] = p.[Id_Supplier]
                WHERE
                (@SearchProductName IS NULL OR p.[product_name] LIKE @SearchProductName) AND
                (@SearchDescription IS NULL OR p.[product_description] LIKE @SearchDescription) AND
                (@SearchCategory IS NULL OR c.[category_name] LIKE @SearchCategory OR c.[id_category] LIKE @SearchCategory) AND
                (@SearchSupplier IS NULL OR s.[supplier_name] LIKE @SearchSupplier OR s.[id_supplier] LIKE @SearchSupplier) AND
                (@SearchPrice IS NULL OR CAST(p.[Price] AS VARCHAR) LIKE @SearchPrice) AND
                (@SearchQuantity IS NULL OR CAST(p.[Quantity] AS VARCHAR) LIKE @SearchQuantity)
                ORDER BY {sortColumn} {sortOrderClause}
                OFFSET 0 ROWS";

            var parameters = new[]
            {
                new SqlParameter("@SearchProductName", string.IsNullOrEmpty(searchProductName) ? (object)DBNull.Value : $"%{searchProductName}%"),
                new SqlParameter("@SearchCategory", string.IsNullOrEmpty(searchCategory) ? (object)DBNull.Value : $"%{searchCategory}%"),
                new SqlParameter("@SearchSupplier", string.IsNullOrEmpty(searchSupplier) ? (object)DBNull.Value : $"%{searchSupplier}%"),
                new SqlParameter("@SearchPrice", string.IsNullOrEmpty(searchPrice) ? (object)DBNull.Value : $"%{searchPrice}%"),
                new SqlParameter("@SearchQuantity", string.IsNullOrEmpty(searchQuantity) ? (object)DBNull.Value : $"%{searchQuantity}%"),
                new SqlParameter("@SearchDescription", string.IsNullOrEmpty(searchDescription) ? (object)DBNull.Value : $"%{searchDescription}%")
            };

            var products = await _context.Products
                .FromSqlRaw(sqlQuery, parameters)
                .Include(p => p.IdCategoryNavigation)
                .Include(p => p.IdSupplierNavigation)
                .ToListAsync();

            return View(products);
        }


        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["IdCategory"] = new SelectList(_context.Categories, "IdCategory", "IdCategory");
            ViewData["IdSupplier"] = new SelectList(_context.Suppliers, "IdSupplier", "IdSupplier");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdProduct,ProductName,IdCategory,IdSupplier,Price,Quantity,ProductDescription")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCategory"] = new SelectList(_context.Categories, "IdCategory", "IdCategory", product.IdCategory);
            ViewData["IdSupplier"] = new SelectList(_context.Suppliers, "IdSupplier", "IdSupplier", product.IdSupplier);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["IdCategory"] = new SelectList(_context.Categories, "IdCategory", "IdCategory", product.IdCategory);
            ViewData["IdSupplier"] = new SelectList(_context.Suppliers, "IdSupplier", "IdSupplier", product.IdSupplier);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdProduct,ProductName,IdCategory,IdSupplier,Price,Quantity,ProductDescription")] Product product)
        {
            if (id != product.IdProduct)
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
                    if (!ProductExists(product.IdProduct))
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
            ViewData["IdCategory"] = new SelectList(_context.Categories, "IdCategory", "IdCategory", product.IdCategory);
            ViewData["IdSupplier"] = new SelectList(_context.Suppliers, "IdSupplier", "IdSupplier", product.IdSupplier);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.IdCategoryNavigation)
                .Include(p => p.IdSupplierNavigation)
                .FirstOrDefaultAsync(m => m.IdProduct == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.IdProduct == id);
        }
    }
}
