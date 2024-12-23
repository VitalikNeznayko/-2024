using CandyStoreApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CandyStoreApp.Controllers
{
    public class TopSoldProductByCategoryController : Controller
    {
        private readonly CandyStoreContext _context;

        public TopSoldProductByCategoryController(CandyStoreContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? year)
        {
            if (year == null)
            {
                return View(new List<TopSoldProductByCategory>());
            }

            var yearParam = new SqlParameter("@Year", year);

            var result = await _context.TopSoldProductByCategory
                .FromSqlRaw("EXEC GetTopSoldProductsByCategory @Year", yearParam)
                .ToListAsync();

            return View(result);
        }
    }
}
