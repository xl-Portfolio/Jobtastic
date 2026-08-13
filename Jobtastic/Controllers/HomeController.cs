using Jobtastic.Data;
using Jobtastic.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Jobtastic.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ApplicationDbContext _context;

		public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
		{
			_logger = logger;
			_context = context;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public async Task<IActionResult> Index()
		{
			var allJobs = await _context.Postings
				.Where(j => j.IsOnline)
				.Include(j => j.Company)
				.ToListAsync();
            return View(allJobs);
		}

		public IActionResult Privacy()
		{
			return View();
		}
	
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
