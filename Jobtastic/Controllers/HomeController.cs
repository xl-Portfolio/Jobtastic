using Jobtastic.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Jobtastic.Services;

namespace Jobtastic.Controllers
{
    public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger; //noch unbenutzt
        private readonly PostingService _postingService;

        public HomeController(ILogger<HomeController> logger, PostingService postingService)
		{
			_logger = logger;
			_postingService = postingService;
        }
		public async Task<IActionResult> Index() => View(await _postingService.GetAllActivePostingsAsync());
        public async Task<IActionResult> JobDetails(int id)
        {
            var posting = await _postingService.GetJobDetailsById(id);
            if (posting == null)
                return NotFound();

            return View(posting);
        }
		public IActionResult Privacy() => View();
	
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
