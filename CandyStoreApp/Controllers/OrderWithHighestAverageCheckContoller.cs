using CandyStoreApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CandyStoreApp.Controllers
{
    public class OrderWithHighestAverageCheckController : Controller
    {
        private readonly CandyStoreContext _context;

        public OrderWithHighestAverageCheckController(CandyStoreContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var highestAverageChecks = await _context.OrderWithHighestAverageCheck
                .FromSqlRaw("EXEC GetOrdersWithHighestAverageCheck")
                .ToListAsync();

            return View(highestAverageChecks);
        }
    }
}
