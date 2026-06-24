using Microsoft.AspNetCore.Mvc;

namespace Jobtastic.Controllers
{
    public class JobPostingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Form()
        {
           // return Form 
        }
    }
}
