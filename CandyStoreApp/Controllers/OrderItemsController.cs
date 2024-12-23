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
    public class OrderItemsController : Controller
    {
        private readonly CandyStoreContext _context;

        public OrderItemsController(CandyStoreContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchQuantity, string searchTotalAmount, string searchOrderNumber, string searchProduct, string sortColumn = "quantity", string sortOrder = "asc")
        {
            string sortOrderClause = sortOrder == "asc" ? "ASC" : "DESC";

            ViewData["SearchQuantity"] = searchQuantity;
            ViewData["SearchTotalAmount"] = searchTotalAmount;
            ViewData["SearchOrderNumber"] = searchOrderNumber;
            ViewData["SearchProduct"] = searchProduct;
            ViewData["sortColumn"] = sortColumn;
            ViewData["sortOrder"] = sortOrder;

            var sqlQuery = $@"
                SELECT oi.*
                FROM [Order_Items] oi
                JOIN [Orders] o ON o.[id_order] = oi.[id_order]
                JOIN [Products] p ON p.[id_product] = oi.[id_product] 
                WHERE 
                (@SearchQuantity IS NULL OR oi.[Quantity] = @SearchQuantity) AND
                (@SearchTotalAmount IS NULL OR CAST(oi.[total_amount] AS VARCHAR) LIKE @SearchTotalAmount) AND
                (@SearchOrderNumber IS NULL OR CAST(oi.[id_order] AS VARCHAR) LIKE @SearchOrderNumber) AND
                (@SearchProduct IS NULL OR p.[product_name] LIKE @SearchProduct OR p.[id_product] LIKE @SearchProduct)
                ORDER BY {sortColumn} {sortOrderClause}
                OFFSET 0 ROWS";

            var parameters = new[]
            {
                new SqlParameter("@SearchQuantity", string.IsNullOrEmpty(searchQuantity) ? (object)DBNull.Value : searchQuantity),
                new SqlParameter("@SearchTotalAmount", string.IsNullOrEmpty(searchTotalAmount) ? (object)DBNull.Value : $"%{searchTotalAmount}%"),
                new SqlParameter("@SearchOrderNumber", string.IsNullOrEmpty(searchOrderNumber) ? (object)DBNull.Value : $"%{searchOrderNumber}%"),
                new SqlParameter("@SearchProduct", string.IsNullOrEmpty(searchProduct) ? (object)DBNull.Value : $"%{searchProduct}%")
            };

            var orderItems = await _context.OrderItems
                .FromSqlRaw(sqlQuery, parameters)
                .Include(oi => oi.IdProductNavigation)
                .Include(oi => oi.IdOrderNavigation)
                .ToListAsync();

            return View(orderItems);
        }

        // GET: OrderItems/Create
        public IActionResult Create()
        {
            ViewData["IdOrder"] = new SelectList(_context.Orders, "IdOrder", "IdOrder");
            ViewData["IdProduct"] = new SelectList(_context.Products, "IdProduct", "IdProduct");
            return View();
        }

        // POST: OrderItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdOrderItem,IdOrder,IdProduct,Quantity,TotalAmount")] OrderItem orderItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdOrder"] = new SelectList(_context.Orders, "IdOrder", "IdOrder", orderItem.IdOrder);
            ViewData["IdProduct"] = new SelectList(_context.Products, "IdProduct", "IdProduct", orderItem.IdProduct);
            return View(orderItem);
        }

        // GET: OrderItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }
            ViewData["IdOrder"] = new SelectList(_context.Orders, "IdOrder", "IdOrder", orderItem.IdOrder);
            ViewData["IdProduct"] = new SelectList(_context.Products, "IdProduct", "IdProduct", orderItem.IdProduct);
            return View(orderItem);
        }

        // POST: OrderItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdOrderItem,IdOrder,IdProduct,Quantity,TotalAmount")] OrderItem orderItem)
        {
            if (id != orderItem.IdOrderItem)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderItemExists(orderItem.IdOrderItem))
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
            ViewData["IdOrder"] = new SelectList(_context.Orders, "IdOrder", "IdOrder", orderItem.IdOrder);
            ViewData["IdProduct"] = new SelectList(_context.Products, "IdProduct", "IdProduct", orderItem.IdProduct);
            return View(orderItem);
        }

        // GET: OrderItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItems
                .Include(o => o.IdOrderNavigation)
                .Include(o => o.IdProductNavigation)
                .FirstOrDefaultAsync(m => m.IdOrderItem == id);
            if (orderItem == null)
            {
                return NotFound();
            }

            return View(orderItem);
        }

        // POST: OrderItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem != null)
            {
                _context.OrderItems.Remove(orderItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderItemExists(int id)
        {
            return _context.OrderItems.Any(e => e.IdOrderItem == id);
        }
    }
}
