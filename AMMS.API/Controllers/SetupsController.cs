using AMMS.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMMS.API.Controllers
{
    [ApiController]
    [Route("api/setup")]
    public class SetupController : ControllerBase
    {
        private readonly IProductTypeProcessSeedService _seedSvc;
        public SetupController(IProductTypeProcessSeedService seedSvc) => _seedSvc = seedSvc;

        [HttpPost("seed-product-type-process")]
        public async Task<IActionResult> Seed()
        {
            await _seedSvc.SeedAsync();
            return Ok(new { message = "Seeded product_type_process" });
        }
    }
}
