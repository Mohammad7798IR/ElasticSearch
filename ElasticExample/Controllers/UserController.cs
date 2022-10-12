using ElasticExample.Services;
using Microsoft.AspNetCore.Mvc;


namespace ElasticExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ElasticService _elasticService;

        public UserController(ElasticService elasticService)
        {
            _elasticService = elasticService;
        }

        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            _elasticService.AddUsers();
            return Ok();
        }


        [HttpGet]
        [Route("Suggest")]
        public async Task<IActionResult> Suggest(string query)
        {
            return Ok(_elasticService.SuggestAsync(query));
        }
    }
}
