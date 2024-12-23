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
    public class SuppliersController : Controller
    {
        private readonly CandyStoreContext _context;

        public SuppliersController(CandyStoreContext context)
        {
            _context = context;
        }

        // GET: Suppliers
        public async Task<IActionResult> Index(string searchSupplierName, string searchContactPerson, string searchPhoneNumber, string searchSupplierAddress, string sortColumn = "supplier_name", string sortOrder = "asc")
        {
            string sortOrderClause = sortOrder == "asc" ? "ASC" : "DESC";

            // Зберігаємо значення пошукових параметрів у ViewData для відображення у формі
            ViewData["SearchSupplierName"] = searchSupplierName;
            ViewData["SearchContactPerson"] = searchContactPerson;
            ViewData["SearchPhoneNumber"] = searchPhoneNumber;
            ViewData["SearchSupplierAddress"] = searchSupplierAddress;
            ViewData["sortColumn"] = sortColumn;
            ViewData["sortOrder"] = sortOrder;

            // Формуємо SQL-запит з умовами пошуку
            var sqlQuery =  $@"
                SELECT s.* 
                FROM [Suppliers] s
                WHERE
                (@SearchSupplierName IS NULL OR s.[Supplier_Name] LIKE @SearchSupplierName) AND
                (@SearchContactPerson IS NULL OR s.[Contact_Person] LIKE @SearchContactPerson) AND
                (@SearchPhoneNumber IS NULL OR s.[Phone_Number] LIKE @SearchPhoneNumber) AND
                (@SearchSupplierAddress IS NULL OR s.[Supplier_Address] LIKE @SearchSupplierAddress)
                ORDER BY {sortColumn} {sortOrderClause}
                OFFSET 0 ROWS;";

            // Визначаємо параметри для SQL-запиту
            var parameters = new[]
            {
                new SqlParameter("@SearchSupplierName", string.IsNullOrEmpty(searchSupplierName) ? (object)DBNull.Value : $"%{searchSupplierName}%"),
                new SqlParameter("@SearchContactPerson", string.IsNullOrEmpty(searchContactPerson) ? (object)DBNull.Value : $"%{searchContactPerson}%"),
                new SqlParameter("@SearchPhoneNumber", string.IsNullOrEmpty(searchPhoneNumber) ? (object)DBNull.Value : $"%{searchPhoneNumber}%"),
                new SqlParameter("@SearchSupplierAddress", string.IsNullOrEmpty(searchSupplierAddress) ? (object)DBNull.Value : $"%{searchSupplierAddress}%")
            }; 

            // Виконуємо запит до бази даних з параметрами
            var suppliers = await _context.Suppliers
                .FromSqlRaw(sqlQuery, parameters)
                .ToListAsync();

            // Повертаємо список постачальників в вигляді результату для перегляду
            return View(suppliers);
        }



        // GET: Suppliers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdSupplier,SupplierName,ContactPerson,PhoneNumber,SupplierAddress")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // GET: Suppliers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdSupplier,SupplierName,ContactPerson,PhoneNumber,SupplierAddress")] Supplier supplier)
        {
            if (id != supplier.IdSupplier)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supplier);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplierExists(supplier.IdSupplier))
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
            return View(supplier);
        }

        // GET: Suppliers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.IdSupplier == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SupplierExists(int id)
        {
            return _context.Suppliers.Any(e => e.IdSupplier == id);
        }
    }
}
