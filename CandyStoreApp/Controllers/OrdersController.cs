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
    public class OrdersController : Controller
    {
        private readonly CandyStoreContext _context;

        public OrdersController(CandyStoreContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index(string searchOrderNumber, string searchClientName, string searchOrderDate, string searchTotalCost, string searchOrderStatus, string sortColumn = "order_date", string sortOrder = "asc")
        {
            string sortOrderClause = sortOrder == "asc" ? "ASC" : "DESC";

            ViewData["SearchOrderNumber"] = searchOrderNumber;
            ViewData["SearchClientName"] = searchClientName;
            ViewData["SearchOrderDate"] = searchOrderDate;
            ViewData["SearchTotalCost"] = searchTotalCost;
            ViewData["SearchOrderStatus"] = searchOrderStatus;
            ViewData["sortColumn"] = sortColumn;
            ViewData["sortOrder"] = sortOrder;

            var sqlQuery = $@"
                SELECT o.*
                FROM [Orders] o
                LEFT JOIN [Clients] c ON c.[id_client] = o.[id_client]
                WHERE 
                (@SearchOrderNumber IS NULL OR CAST(o.[id_order] AS VARCHAR) LIKE @SearchOrderNumber) AND
                (@SearchClientName IS NULL OR c.[first_name] LIKE @SearchClientName OR c.[last_name] LIKE @SearchClientName) AND
                (@SearchOrderDate IS NULL OR CAST(o.[order_date] AS VARCHAR) LIKE @SearchOrderDate) AND
                (@SearchTotalCost IS NULL OR CAST(o.[total_cost] AS VARCHAR) LIKE @SearchTotalCost) AND
                (@SearchOrderStatus IS NULL OR o.[order_status] LIKE @SearchOrderStatus)
                ORDER BY {sortColumn} {sortOrderClause}
                OFFSET 0 ROWS";
            var parameters = new[]
            {
                new SqlParameter("@SearchOrderNumber", string.IsNullOrEmpty(searchOrderNumber) ? (object)DBNull.Value : $"%{searchOrderNumber}%"),
                new SqlParameter("@SearchClientName", string.IsNullOrEmpty(searchClientName) ? (object)DBNull.Value : $"%{searchClientName}%"),
                new SqlParameter("@SearchOrderDate", string.IsNullOrEmpty(searchOrderDate) ? (object)DBNull.Value : $"%{searchOrderDate}%"),
                new SqlParameter("@SearchTotalCost", string.IsNullOrEmpty(searchTotalCost) ? (object)DBNull.Value : $"%{searchTotalCost}%"),
                new SqlParameter("@SearchOrderStatus", string.IsNullOrEmpty(searchOrderStatus) ? (object)DBNull.Value : $"%{searchOrderStatus}%")
            };

            var orders = await _context.Orders
                .FromSqlRaw(sqlQuery, parameters)
                .Include(o => o.IdClientNavigation)
                .ToListAsync();

            return View(orders);
        }



        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["IdClient"] = new SelectList(_context.Clients, "IdClient", "IdClient");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdOrder,IdClient,OrderDate,TotalCost,OrderStatus")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdClient"] = new SelectList(_context.Clients, "IdClient", "IdClient", order.IdClient);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["IdClient"] = new SelectList(_context.Clients, "IdClient", "IdClient", order.IdClient);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdOrder,IdClient,OrderDate,TotalCost,OrderStatus")] Order order)
        {
            if (id != order.IdOrder)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.IdOrder))
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
            ViewData["IdClient"] = new SelectList(_context.Clients, "IdClient", "IdClient", order.IdClient);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.IdClientNavigation)
                .FirstOrDefaultAsync(m => m.IdOrder == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.IdOrder == id);
        }
    }
}
