using AMMS.Application.Interfaces;
using AMMS.Shared.DTOs.Email;
using AMMS.Shared.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AMMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestsController : ControllerBase
    {
        private readonly IRequestService _service;
        private readonly IDealService _dealService;

        public RequestsController(IRequestService service, IDealService dealService)
        {
            _service = service;
            _dealService = dealService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CreateRequestResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateResquest req)
        {
            var result = await _service.CreateAsync(req);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UpdateRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UpdateRequestResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UpdateRequestResponse>> UpdateAsync(int id, [FromBody] UpdateOrderRequest request)
        {
            var update = await _service.UpdateAsync(id, request);
            return StatusCode(StatusCodes.Status200OK, update);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _service.GetByIdAsync(id);
            if (order == null)
                return NotFound(new { message = "Order request not found" });

            return Ok(order);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] int page, [FromQuery] int pageSize)
        {
            var result = await _service.GetPagedAsync(page, pageSize);
            return Ok(result);
        }

        [HttpPost("convert-to-order-by-{id:int}")]
        public async Task<IActionResult> ConvertToOrder(int id)
        {
            var result = await _service.ConvertToOrderAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("send-deal")]
        public async Task<IActionResult> SendDealEmail([FromBody] SendDealEmailRequest req)
        {
            try
            {
                await _dealService.SendDealAndEmailAsync(req.RequestId);
                return Ok(new { message = "Sent deal email", orderRequestId = req.RequestId });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ SendDealEmail failed:");
                Console.WriteLine(ex.Message);

                return StatusCode(StatusCodes.Status502BadGateway, new
                {
                    message = "Send email failed",
                    detail = ex.Message,
                    orderRequestId = req.RequestId
                });
            }
        }

        [HttpGet("deal/accept")]
        public async Task<IActionResult> Accept([FromQuery] int orderRequestId, [FromQuery] string token)
        {
            await _dealService.AcceptDealAsync(orderRequestId);
            await _service.ConvertToOrderAsync(orderRequestId);
            return Ok("Bạn đã đồng ý báo giá. Nhân viên sẽ liên hệ sớm.");
        }

        [HttpPost("deal/reject")]
        public async Task<IActionResult> RejectDeal([FromBody] RejectDealRequest body)
        {
            if (string.IsNullOrWhiteSpace(body.reason))
                return BadRequest("reason is required");

            await _dealService.RejectDealAsync(body.orderRequestId, body.reason);
            return Ok(new { message = "Rejected" });
        }

        [HttpGet("sort-quantity/asc")]
        public async Task<IActionResult> SortByQuantityAsc([FromQuery] int page, [FromQuery] int pageSize, CancellationToken ct)
        {
            var result = await _service.GetSortedByQuantityPagedAsync(true, page, pageSize, ct);
            return Ok(result);
        }

        [HttpGet("sort-quantity/desc")]
        public async Task<IActionResult> SortByQuantityDesc([FromQuery] int page, [FromQuery] int pageSize, CancellationToken ct)
        {
            var result = await _service.GetSortedByQuantityPagedAsync(false, page, pageSize, ct);
            return Ok(result);
        }

        [HttpGet("sort-date/asc")]
        public async Task<IActionResult> SortByDateAsc([FromQuery] int page, [FromQuery] int pageSize, CancellationToken ct)
        {
            var result = await _service.GetSortedByDatePagedAsync(true, page, pageSize, ct);
            return Ok(result);
        }

        [HttpGet("sort-date/desc")]
        public async Task<IActionResult> SortByDateDesc([FromQuery] int page, [FromQuery] int pageSize, CancellationToken ct)
        {
            var result = await _service.GetSortedByDatePagedAsync(false, page, pageSize, ct);
            return Ok(result);
        }

        [HttpGet("sort-delivery/nearest")]
        public async Task<IActionResult> SortByDeliveryNearest([FromQuery] int page, [FromQuery] int pageSize, CancellationToken ct)
        {
            var result = await _service.GetSortedByDeliveryDatePagedAsync(true, page, pageSize, ct);
            return Ok(result);
        }

        [HttpGet("sort-delivery/farthest")]
        public async Task<IActionResult> SortByDeliveryFarthest([FromQuery] int page, [FromQuery] int pageSize, CancellationToken ct)
        {
            var result = await _service.GetSortedByDeliveryDatePagedAsync(false, page, pageSize, ct);
            return Ok(result);
        }

        [HttpGet("stats/email/accepted")]
        public async Task<IActionResult> GetEmailStatsByAccepted(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        CancellationToken ct)
        {
            var result = await _service.GetEmailsByAcceptedCountPagedAsync(page, pageSize, ct);
            return Ok(result);
        }

        [HttpGet("sort-stock-coverage/highest")]
        public async Task<IActionResult> SortByStockCoverageHighest(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        CancellationToken ct)
        {
            var result = await _service.GetSortedByStockCoveragePagedAsync(page, pageSize, ct);
            return Ok(result);
        }
 

        [HttpGet("filter-by-order-date")]
        public async Task<IActionResult> FilterByOrderDate(
            [FromQuery] DateOnly date,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            CancellationToken ct)
        {
            var result = await _service.GetByOrderRequestDatePagedAsync(date, page, pageSize, ct);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery(Name = "keyword")] string keyword,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            CancellationToken ct)
        {
            var result = await _service.SearchPagedAsync(keyword, page, pageSize, ct);
            return Ok(result);
        }
        [HttpGet("design-file/{id:int}")]
        public async Task<IActionResult> GetDesignFile(int id, CancellationToken ct)
        {
            var result = await _service.GetDesignFileAsync(id, ct);

            if (result == null)
                return NotFound(new { message = "Order request not found" });

            return Ok(result);
        }

    }
}
