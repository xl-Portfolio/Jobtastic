using Jobtastic.Data;
using Jobtastic.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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

		public IActionResult Index()
		{
			var allJobs = _context.JobPostings.ToList();
			return View(allJobs);
		}

		public IActionResult Privacy()
		{
			return View();
		}
		public IActionResult Beispiel()
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
