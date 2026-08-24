using Jobtastic.Data;
using Jobtastic.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Jobtastic.Services;

namespace Jobtastic.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger; //noch unbenutzt
		private readonly ApplicationDbContext _context;
        private readonly PostingService _postingService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, PostingService postingService)
		{
			_logger = logger;
			_context = context;
			_postingService = postingService;
        }
		public async Task<IActionResult> Index()
		{
			var allJobs = await _context.Postings
				.Where(j => j.IsOnline)
				.Include(j => j.Company)
				.ToListAsync();
            return View(allJobs);
		}
        public async Task<IActionResult> JobDetails(int id)
        {
            var posting = await _postingService.GetJobDetailsById(id);
            if (posting == null)
                return NotFound();

            if (!posting.IsOnline && !_postingService.IsAuthorized(posting))
                return NotFound();

            return View(posting);
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
