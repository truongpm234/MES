using AMMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AMMS.API.Controllers
{
    [ApiController]
    [Route("api/productions")]
    public class ProductionsController : ControllerBase
    {
        private readonly IProductionService _service;

        public ProductionsController(IProductionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Danh sách đơn đang sản xuất
        /// </summary>
        [HttpGet("in-production")]
        public async Task<IActionResult> GetInProduction()
        {
            var result = await _service.GetOrdersInProductionAsync();
            return Ok(result);
        }

        /// <summary>
        /// Ngày giao gần nhất (để tính xưởng rảnh)
        /// </summary>
        [HttpGet("nearest-delivery")]
        public async Task<IActionResult> GetNearestDelivery()
        {
            var result = await _service.GetNearestDeliveryAsync();
            return Ok(result);
        }
    }
}
