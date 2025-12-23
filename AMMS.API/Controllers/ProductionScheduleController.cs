using AMMS.Application.Interfaces;
using AMMS.Shared.DTOs.Productions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMMS.API.Controllers
{
    [ApiController]
    [Route("api/productions")]
    public class ProductionScheduleController : ControllerBase
    {
        private readonly IProductionSchedulingService _svc;
        public ProductionScheduleController(IProductionSchedulingService svc) => _svc = svc;

        [HttpPost("schedule")]
        public async Task<IActionResult> Schedule([FromBody] ScheduleRequest req)
        {
            var prodId = await _svc.ScheduleOrderAsync(req.order_id, req.product_type_id, req.manager_id);
            return Ok(new { prod_id = prodId });
        }
    }
}
