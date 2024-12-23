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
    public class ReviewsController : Controller
    {
        private readonly CandyStoreContext _context;

        public ReviewsController(CandyStoreContext context)
        {
            _context = context;
        }

        // GET: Reviews
        public async Task<IActionResult> Index(string searchRating, string searchReviewText, string searchProduct, string searchClient, string searchReviewDate, string sortColumn = "rating", string sortOrder = "asc")
        {
            string sortOrderClause = sortOrder == "asc" ? "ASC" : "DESC";

            ViewData["SearchRating"] = searchRating;
            ViewData["SearchReviewText"] = searchReviewText;
            ViewData["SearchProduct"] = searchProduct;
            ViewData["SearchClient"] = searchClient;
            ViewData["SearchReviewDate"] = searchReviewDate;
            ViewData["sortColumn"] = sortColumn;
            ViewData["sortOrder"] = sortOrder;

            var sqlQuery = $@"
                SELECT r.* 
                FROM [Reviews] r
                LEFT JOIN [Products] p ON p.[Id_Product] = r.[Id_Product]
                LEFT JOIN [Clients] c ON c.[Id_Client] = r.[Id_Client]
                WHERE
                (@SearchRating IS NULL OR r.[Rating] = @SearchRating) AND
                (@SearchReviewText IS NULL OR r.[Review_Text] LIKE @SearchReviewText) AND
                (@SearchProduct IS NULL OR p.[product_name] LIKE @SearchProduct OR p.[id_product] LIKE @SearchProduct) AND
                (@SearchClient IS NULL OR c.[first_name] LIKE @SearchClient OR c.[last_name] LIKE @SearchClient) AND
                (@SearchReviewDate IS NULL OR CAST(r.[Review_Date] AS VARCHAR) LIKE @SearchReviewDate)
                ORDER BY {sortColumn} {sortOrderClause}
                OFFSET 0 ROWS";

            var parameters = new[]
            {
                new SqlParameter("@SearchRating", string.IsNullOrEmpty(searchRating) ? (object)DBNull.Value : searchRating),
                new SqlParameter("@SearchReviewText", string.IsNullOrEmpty(searchReviewText) ? (object)DBNull.Value : $"%{searchReviewText}%"),
                new SqlParameter("@SearchProduct", string.IsNullOrEmpty(searchProduct) ? (object)DBNull.Value : $"%{searchProduct}%"),
                new SqlParameter("@SearchClient", string.IsNullOrEmpty(searchClient) ? (object)DBNull.Value : $"%{searchClient}%"),
                new SqlParameter("@SearchReviewDate", string.IsNullOrEmpty(searchReviewDate) ? (object)DBNull.Value : $"%{searchReviewDate}%")
            };

            var reviews = await _context.Reviews
                .FromSqlRaw(sqlQuery, parameters)
                .Include(r => r.IdProductNavigation)
                .Include(r => r.IdClientNavigation)
                .ToListAsync();

            return View(reviews);
        }


        // GET: Reviews/Create
        public IActionResult Create()
        {
            ViewData["IdClient"] = new SelectList(_context.Clients, "IdClient", "IdClient");
            ViewData["IdProduct"] = new SelectList(_context.Products, "IdProduct", "IdProduct");
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdReview,IdClient,IdProduct,Rating,ReviewText,ReviewDate")] Review review)
        {
            if (ModelState.IsValid)
            {
                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdClient"] = new SelectList(_context.Clients, "IdClient", "IdClient", review.IdClient);
            ViewData["IdProduct"] = new SelectList(_context.Products, "IdProduct", "IdProduct", review.IdProduct);
            return View(review);
        }

        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            ViewData["IdClient"] = new SelectList(_context.Clients, "IdClient", "IdClient", review.IdClient);
            ViewData["IdProduct"] = new SelectList(_context.Products, "IdProduct", "IdProduct", review.IdProduct);
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdReview,IdClient,IdProduct,Rating,ReviewText,ReviewDate")] Review review)
        {
            if (id != review.IdReview)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.IdReview))
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
            ViewData["IdClient"] = new SelectList(_context.Clients, "IdClient", "IdClient", review.IdClient);
            ViewData["IdProduct"] = new SelectList(_context.Products, "IdProduct", "IdProduct", review.IdProduct);
            return View(review);
        }

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.IdClientNavigation)
                .Include(r => r.IdProductNavigation)
                .FirstOrDefaultAsync(m => m.IdReview == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.IdReview == id);
        }
    }
}
