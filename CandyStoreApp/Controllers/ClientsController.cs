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
    public class ClientsController : Controller
    {
        private readonly CandyStoreContext _context;

        public ClientsController(CandyStoreContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index(string searchFirstName, string searchLastName, string searchEmail, string searchPhoneNumber, string searchCity, string searchAddress, string searchIndex, string sortColumn = "first_name", string sortOrder = "asc")
        {
            ViewData["SearchFirstName"] = searchFirstName;
            ViewData["SearchLastName"] = searchLastName;
            ViewData["SearchEmail"] = searchEmail;
            ViewData["SearchPhoneNumber"] = searchPhoneNumber;
            ViewData["SearchCity"] = searchCity;
            ViewData["SearchAddress"] = searchAddress;
            ViewData["SearchIndex"] = searchIndex;
            ViewData["sortColumn"] = sortColumn;
            ViewData["sortOrder"] = sortOrder;

            string sortOrderClause = sortOrder == "asc" ? "ASC" : "DESC";

            var sqlQuery = $@"
                SELECT *
                FROM [Clients]
                WHERE 
                (@SearchFirstName IS NULL OR [First_Name] LIKE @SearchFirstName) AND
                (@SearchLastName IS NULL OR [Last_Name] LIKE @SearchLastName) AND
                (@SearchEmail IS NULL OR [Email] LIKE @SearchEmail) AND
                (@SearchAddress IS NULL OR [address_client] LIKE @SearchAddress) AND
                (@SearchIndex IS NULL OR [postal_code] LIKE @SearchIndex) AND
                (@SearchPhoneNumber IS NULL OR [Phone_Number] LIKE @SearchPhoneNumber) AND
                 (@SearchCity IS NULL OR [City] LIKE @SearchCity)
                ORDER BY {sortColumn} {sortOrderClause}";


            var parameters = new[]
            {
                new SqlParameter("@SearchFirstName", string.IsNullOrEmpty(searchFirstName) ? (object)DBNull.Value : $"%{searchFirstName}%"),
                new SqlParameter("@SearchLastName", string.IsNullOrEmpty(searchLastName) ? (object)DBNull.Value : $"%{searchLastName}%"),
                new SqlParameter("@SearchEmail", string.IsNullOrEmpty(searchEmail) ? (object)DBNull.Value : $"%{searchEmail}%"),
                new SqlParameter("@SearchPhoneNumber", string.IsNullOrEmpty(searchPhoneNumber) ? (object)DBNull.Value : $"%{searchPhoneNumber}%"),
                new SqlParameter("@SearchCity", string.IsNullOrEmpty(searchCity) ? (object)DBNull.Value : $"%{searchCity}%"),
                new SqlParameter("@SearchAddress", string.IsNullOrEmpty(searchAddress) ? (object)DBNull.Value : $"%{searchAddress}%"),
                new SqlParameter("@SearchIndex", string.IsNullOrEmpty(searchIndex) ? (object)DBNull.Value : $"%{searchIndex}%")
            };

          
            var clients = await _context.Clients
                .FromSqlRaw(sqlQuery, parameters)
                .ToListAsync();

          
            return View(clients);
        }



        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdClient,FirstName,LastName,Email,PhoneNumber,AddressClient,City,PostalCode")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdClient,FirstName,LastName,Email,PhoneNumber,AddressClient,City,PostalCode")] Client client)
        {
            if (id != client.IdClient)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.IdClient))
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
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.IdClient == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.IdClient == id);
        }
    }
}
