using Jobtastic.Filters;
using Jobtastic.Services;
using Microsoft.AspNetCore.Mvc;

namespace Jobtastic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyAuthorization]
    public class ApiJobpostingController : ControllerBase
    {
        private readonly ApiJobpostingService _service;
        public ApiJobpostingController(ApiJobpostingService service)
        {
            _service = service;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllPostings() 
        {
            var postings = await _service.GetAllPostings();
            if (postings == null)
                return NotFound();
            return Ok(postings);
        }

        [HttpGet("GetById")]
        public IActionResult GetPostingById(int id) 
        {
            var posting = _service.GetPostingById(id);
            if (posting == null) 
                return NotFound();
            return Ok(posting);
        }
    }
}
