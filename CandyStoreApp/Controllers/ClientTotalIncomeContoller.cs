using CandyStoreApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CandyStoreApp.Controllers
{
    public class ClientTotalIncomeController : Controller
    {
        private readonly CandyStoreContext _context;

        public ClientTotalIncomeController(CandyStoreContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var clientIncomeData = await _context.ClientTotalIncome
                .FromSqlRaw("EXEC GetClientTotalIncome")
                .ToListAsync();

            return View(clientIncomeData);
        }
    }
}
