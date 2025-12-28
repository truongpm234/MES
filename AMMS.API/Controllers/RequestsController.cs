using AMMS.Application.Interfaces;
using AMMS.Infrastructure.Entities;
using AMMS.Infrastructure.Interfaces;
using AMMS.Infrastructure.DBContext;
using AMMS.Shared.DTOs.Email;
using AMMS.Shared.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Text.Json;

namespace AMMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestsController : ControllerBase
    {
        private readonly IRequestService _service;
        private readonly IDealService _dealService;
        private readonly IPaymentsService _paymentService;
        private readonly AppDbContext _db;

        public RequestsController(
            IRequestService service,
            IDealService dealService,
            IPaymentsService paymentService,
            AppDbContext db)
        {
            _service = service;
            _dealService = dealService;
            _paymentService = paymentService;
            _db = db;
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
                Console.WriteLine(ex);

                return StatusCode(StatusCodes.Status502BadGateway, new
                {
                    message = "Send email failed",
                    detail = ex.Message,
                    orderRequestId = req.RequestId
                });
            }
        }

        // Người dùng bấm “đồng ý” -> redirect qua PayOS checkout.
        // Webhook KHÔNG gọi từ đây; PayOS sẽ gọi webhook sau khi trả tiền thành công.
        [HttpGet("accept-pay")]
        public async Task<IActionResult> AcceptPay([FromQuery] int orderRequestId, [FromQuery] string token)
        {
            var checkoutUrl = await _dealService.AcceptAndCreatePayOsLinkAsync(orderRequestId);
            return Redirect(checkoutUrl);
        }

        [HttpGet("reject-form")]
        public IActionResult RejectForm([FromQuery] int orderRequestId, [FromQuery] string token)
        {
            var fe = "https://sep490-fe.vercel.app";
            return Redirect($"{fe}/reject-deal?orderRequestId={orderRequestId}&token={token}");
        }

        [HttpPost("reject")]
        public async Task<IActionResult> RejectDeal([FromBody] RejectDealRequest dto)
        {
            await _dealService.RejectDealAsync(dto.orderRequestId, dto.reason ?? "Customer rejected");
            return Ok(new { ok = true });
        }

        [HttpGet("payos/return")]
        public IActionResult PayOsReturn([FromQuery] int orderRequestId, [FromQuery] long orderCode)
        {
            var fe = "https://sep490-fe.vercel.app";
            return Redirect($"{fe}");
        }

        [HttpGet("payos/cancel")]
        public IActionResult PayOsCancel([FromQuery] int orderRequestId, [FromQuery] long orderCode)
        {
            var fe = "https://sep490-fe.vercel.app";
            return Redirect($"{fe}");
        }

        [AllowAnonymous]
        [HttpPost("/api/payos/webhook")]
        public async Task<IActionResult> PayOsWebhook(
            [FromBody] JsonElement raw,
            [FromServices] IPaymentRepository paymentRepo,
            CancellationToken ct)
        {
            Console.WriteLine("✅ PAYOS WEBHOOK HIT");
            Console.WriteLine("RAW = " + raw.ToString());

            try
            {
                var node = raw.TryGetProperty("data", out var data) ? data : raw;

                long orderCode =
                    node.TryGetProperty("orderCode", out var oc) && oc.ValueKind == JsonValueKind.Number
                        ? oc.GetInt64()
                        : 0;

                var status =
                    node.TryGetProperty("status", out var st) ? (st.GetString() ?? "") : "";

                long amount =
                    node.TryGetProperty("amount", out var am) && am.ValueKind == JsonValueKind.Number
                        ? am.GetInt64()
                        : 0;

                var paymentLinkId =
                    node.TryGetProperty("paymentLinkId", out var pl) ? pl.GetString() : null;

                var transactionId =
                    node.TryGetProperty("transactionId", out var tx) ? tx.GetString() : null;

                Console.WriteLine($"Parsed => orderCode={orderCode}, status={status}, amount={amount}");

                var isPaid =
                    string.Equals(status, "PAID", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(status, "SUCCESS", StringComparison.OrdinalIgnoreCase);

                if (!isPaid) return Ok(new { ok = true, ignored = true });

                if (orderCode <= 0)
                    return Ok(new { ok = true, error = "orderCode missing/invalid" });

                // orderRequestId encode ở 5 số cuối
                var orderRequestId = (int)(orderCode % 100000);

                // Check FK tồn tại trước khi insert payments (tránh 23503)
                var existsOrderRequest = await _db.order_requests
                    .AsNoTracking()
                    .AnyAsync(x => x.order_request_id == orderRequestId, ct);

                if (!existsOrderRequest)
                {
                    Console.WriteLine($"❌ order_request_id={orderRequestId} not found, skip insert payment");
                    // vẫn trả 200 để PayOS không retry vô hạn
                    return Ok(new { ok = true, error = "order_request_not_found", orderRequestId, orderCode });
                }

                // idempotent: nếu đã PAID thì bỏ qua
                var existedPaid = await _paymentService.GetPaidByProviderOrderCodeAsync("PAYOS", (int)orderCode, ct);
                if (existedPaid != null && string.Equals(existedPaid.status, "PAID", StringComparison.OrdinalIgnoreCase))
                    return Ok(new { ok = true, message = "Already processed" });

                // Với timestamp without time zone: dùng Unspecified để Npgsql khỏi chửi
                var now = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

                // Nếu bạn muốn chặt hơn: unique (provider, order_code) và update nếu có
                await paymentRepo.AddAsync(new payment
                {
                    order_request_id = orderRequestId,
                    provider = "PAYOS",
                    order_code = orderCode,
                    amount = (decimal)amount,
                    currency = "VND",
                    status = "PAID",
                    paid_at = now,
                    payos_payment_link_id = paymentLinkId,
                    payos_transaction_id = transactionId,
                    payos_raw = raw.ToString(),
                    created_at = now,
                    updated_at = now
                }, ct);

                await paymentRepo.SaveChangesAsync(ct);

                // mark accepted
                await _dealService.MarkAcceptedAsync(orderRequestId);

                // convert -> bọc try/catch để không văng 500
                bool converted = false;
                string? convertMessage = null;

                try
                {
                    var convert = await _service.ConvertToOrderAsync(orderRequestId);
                    converted = convert.Success;
                    convertMessage = convert.Message;

                    if (!convert.Success)
                        Console.WriteLine($"❌ ConvertToOrder failed: {convert.Message}");
                }
                catch (DbUpdateException dbEx) when (dbEx.InnerException is PostgresException pg && pg.SqlState == "23505")
                {
                    // duplicate key (orders_code_key) => coi như idempotent/đã có order
                    Console.WriteLine("⚠️ Duplicate orders.code, treating as already converted");
                    converted = true;
                    convertMessage = "Already converted (duplicate code handled)";
                }

                // notify email (đừng để mail fail làm 500)
                try
                {
                    await _dealService.NotifyConsultantPaidAsync(orderRequestId, (decimal)amount, now);
                    await _dealService.NotifyCustomerPaidAsync(orderRequestId, (decimal)amount, now);
                }
                catch (Exception mailEx)
                {
                    Console.WriteLine("⚠️ Email notify failed:");
                    Console.WriteLine(mailEx);
                }

                Console.WriteLine("✅ Payment saved + Accepted + Converted");

                return Ok(new
                {
                    ok = true,
                    saved = true,
                    orderRequestId,
                    orderCode,
                    converted,
                    convertMessage
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Webhook unhandled error:");
                Console.WriteLine(ex);

                return Ok(new { ok = true, error = "internal_error", detail = ex.Message });
            }
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
