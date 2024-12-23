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
    public class ShipmentsController : Controller
    {
        private readonly CandyStoreContext _context;

        public ShipmentsController(CandyStoreContext context)
        {
            _context = context;
        }

        // GET: Shipments
        public async Task<IActionResult> Index(string searchOrderId, string searchShipmentDate, string searchShipmentMethod, string searchTrackingNumber, string sortColumn = "shipment_date", string sortOrder = "asc")
        {
            string sortOrderClause = sortOrder == "asc" ? "ASC" : "DESC";

            // Збереження значень для відображення у формі
            ViewData["SearchOrderId"] = searchOrderId;
            ViewData["SearchShipmentDate"] = searchShipmentDate;
            ViewData["SearchShipmentMethod"] = searchShipmentMethod;
            ViewData["SearchTrackingNumber"] = searchTrackingNumber;

            // SQL-запит з параметрами для пошуку
            var sqlQuery = $@"
                SELECT *
                FROM [Shipments]
                WHERE 
                    (@SearchOrderId IS NULL OR [Id_Order] LIKE @SearchOrderId) AND
                    (@SearchShipmentDate IS NULL OR CAST([Shipment_Date] AS DATE) LIKE @SearchShipmentDate) AND
                    (@SearchShipmentMethod IS NULL OR [Shipment_Method] LIKE @SearchShipmentMethod) AND
                    (@SearchTrackingNumber IS NULL OR [Tracking_Number] LIKE @SearchTrackingNumber)
                    ORDER BY {sortColumn} {sortOrderClause}
                    OFFSET 0 ROWS";

            // Параметри SQL-запиту
            var parameters = new[]
            {
                new SqlParameter("@SearchOrderId", string.IsNullOrEmpty(searchOrderId) ? (object)DBNull.Value : $"%{searchOrderId}%"),
                new SqlParameter("@SearchShipmentDate", string.IsNullOrEmpty(searchShipmentDate) ? (object)DBNull.Value : $"%{searchShipmentDate}%"),
                new SqlParameter("@SearchShipmentMethod", string.IsNullOrEmpty(searchShipmentMethod) ? (object)DBNull.Value : $"%{searchShipmentMethod}%"),
                new SqlParameter("@SearchTrackingNumber", string.IsNullOrEmpty(searchTrackingNumber) ? (object)DBNull.Value : $"%{searchTrackingNumber}%")
            };

            // Виконання SQL-запиту з фільтрами
            var shipments = await _context.Shipments
                .FromSqlRaw(sqlQuery, parameters)
                .Include(s => s.IdOrderNavigation) 
                .ToListAsync();

            ViewData["sortColumn"] = sortColumn;
            ViewData["sortOrder"] = sortOrder;

            return View(shipments);
        }



        // GET: Shipments/Create
        public IActionResult Create()
        {
            ViewData["IdOrder"] = new SelectList(_context.Orders, "IdOrder", "IdOrder");
            return View();
        }

        // POST: Shipments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdShipment,IdOrder,ShipmentDate,ShipmentMethod,TrackingNumber")] Shipment shipment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shipment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdOrder"] = new SelectList(_context.Orders, "IdOrder", "IdOrder", shipment.IdOrder);
            return View(shipment);
        }

        // GET: Shipments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipment = await _context.Shipments.FindAsync(id);
            if (shipment == null)
            {
                return NotFound();
            }
            ViewData["IdOrder"] = new SelectList(_context.Orders, "IdOrder", "IdOrder", shipment.IdOrder);
            return View(shipment);
        }

        // POST: Shipments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdShipment,IdOrder,ShipmentDate,ShipmentMethod,TrackingNumber")] Shipment shipment)
        {
            if (id != shipment.IdShipment)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shipment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShipmentExists(shipment.IdShipment))
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
            ViewData["IdOrder"] = new SelectList(_context.Orders, "IdOrder", "IdOrder", shipment.IdOrder);
            return View(shipment);
        }

        // GET: Shipments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipment = await _context.Shipments
                .Include(s => s.IdOrderNavigation)
                .FirstOrDefaultAsync(m => m.IdShipment == id);
            if (shipment == null)
            {
                return NotFound();
            }

            return View(shipment);
        }

        // POST: Shipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shipment = await _context.Shipments.FindAsync(id);
            if (shipment != null)
            {
                _context.Shipments.Remove(shipment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShipmentExists(int id)
        {
            return _context.Shipments.Any(e => e.IdShipment == id);
        }
    }
}
