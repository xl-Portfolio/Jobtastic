using Jobtastic.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jobtastic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiJobpostingController : ControllerBase
    {
        private readonly ApiJobpostingService _service;
        public ApiJobpostingController(ApiJobpostingService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAllPostings() 
        {
            var postings = _service.GetAllPostings();
            return Ok(postings);
        }

    }
}
