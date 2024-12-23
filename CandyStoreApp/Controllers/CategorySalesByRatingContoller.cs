using CandyStoreApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CandyStoreApp.Controllers
{
    public class CategorySalesByRatingController : Controller
    {
        private readonly CandyStoreContext _context;

        public CategorySalesByRatingController(CandyStoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? rating)
        {
            if (rating == null)
            {
                return View(new List<CategorySalesByRating>());
            }
            @ViewData["rating"] = rating.ToString();
            var ratingParam = new SqlParameter("@rating", rating);

            var results = await _context.CategorySalesByRating
                .FromSqlRaw("EXEC GetTotalSoldByRatingForCategory @rating", ratingParam)
                .ToListAsync();

            return View(results);
        }
    }
}
