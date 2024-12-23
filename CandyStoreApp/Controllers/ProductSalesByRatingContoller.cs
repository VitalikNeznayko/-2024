using CandyStoreApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CandyStoreApp.Controllers
{
    public class ProductSalesByRatingController : Controller
    {
        private readonly CandyStoreContext _context;

        public ProductSalesByRatingController(CandyStoreContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? rating)
        {
            if (rating == null)
            {
                return View(new List<ProductSalesByRating>());
            }

            @ViewData["rating"] = rating.ToString();

            var ratingParam = new SqlParameter("@rating", rating);

            var salesData = await _context.ProductSalesByRating
                .FromSqlRaw("EXEC GetTotalSoldByRatingForProducts @rating", ratingParam)
                .ToListAsync();

            return View(salesData);
        }

    }
}
