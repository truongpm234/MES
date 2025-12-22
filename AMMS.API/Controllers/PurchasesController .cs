using AMMS.Application.Interfaces;
using AMMS.Shared.DTOs.Purchases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseService _service;

        public PurchasesController(IPurchaseService service)
        {
            _service = service;
        }

        // POST: api/purchases/request
        [HttpPost("request")]
        public async Task<IActionResult> CreatePurchaseRequest(
            [FromBody] CreatePurchaseRequestDto dto,
            CancellationToken ct)
        {
            // nếu bạn có JWT thì lấy user id từ claim
            int? createdBy = null;

            var result = await _service.CreatePurchaseRequestAsync(dto, createdBy, ct);
            return StatusCode(StatusCodes.Status201Created, result);
        }
    }
}

