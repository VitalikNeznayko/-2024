using CandyStoreApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CandyStoreApp.Controllers
{
    public class DetailedRevenueByDateRangeController : Controller
    {
        private readonly CandyStoreContext _context;

        public DetailedRevenueByDateRangeController(CandyStoreContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null)
            {
                return View(new List<DetailedRevenueByDateRange>());
            }

            @ViewData["startDate"] = startDate.Value.ToString("yyyy-MM-dd");
            @ViewData["endDate"] = endDate.Value.ToString("yyyy-MM-dd");

            var startParam = new SqlParameter("@StartDate", startDate);
            var endParam = new SqlParameter("@EndDate", endDate);

            var revenueData = await _context.DetailedRevenueByDateRange
                .FromSqlRaw("EXEC GetDetailedRevenueByDateRange @StartDate, @EndDate", startParam, endParam)
                .ToListAsync();

            return View(revenueData);
        }
    }
}
