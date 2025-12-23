using AMMS.Application.Interfaces;
using AMMS.Infrastructure.DBContext;
using AMMS.Shared.DTOs.StockMoves;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Text.Json;

namespace AMMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockMovesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IStockMoveService _stockMoveService;

        public StockMovesController(AppDbContext db, IStockMoveService stockMoveService)
        {
            _db = db;
            _stockMoveService = stockMoveService;
        }

        /// <summary>
        /// ✅ API 1: Tạo QR code cho phiếu purchase
        /// QR chứa: PurchaseId, PurchaseCode, MoveType (IN/OUT/RETURN)
        /// </summary>
        [HttpGet("purchase/{purchaseId:int}/qr")]
        public async Task<IActionResult> GenerateQrForPurchase(
            int purchaseId,
            [FromQuery] string moveType = "IN",
            CancellationToken ct = default)
        {
            moveType = (moveType ?? string.Empty).Trim().ToUpper();

            if (moveType != "IN" && moveType != "OUT" && moveType != "RETURN")
            {
                return BadRequest(new
                {
                    message = "moveType phải là IN / OUT / RETURN",
                    moveType
                });
            }

            var purchase = await _db.purchases
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.purchase_id == purchaseId, ct);

            if (purchase == null)
            {
                return NotFound(new
                {
                    message = "Purchase not found",
                    purchaseId
                });
            }

            // Nếu chưa có code thì fallback PO + id
            var purchaseCode = string.IsNullOrWhiteSpace(purchase.code)
                ? $"PO{purchaseId:D4}"
                : purchase.code;

            var payload = new
            {
                PurchaseId = purchase.purchase_id,
                PurchaseCode = purchaseCode,
                MoveType = moveType
            };

            var payloadJson = JsonSerializer.Serialize(payload);

            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(payloadJson, QRCodeGenerator.ECCLevel.Q);
            var qrPng = new PngByteQRCode(qrData);
            var qrBytes = qrPng.GetGraphic(20);

            var fileName = $"purchase-{purchaseId}-{moveType}.png";

            return File(qrBytes, "image/png", fileName);
        }

        /// <summary>
        /// ✅ API 2: Kiểm tra tình trạng xuất/nhập/trả của 1 NVL
        /// </summary>
        [HttpGet("material/{materialId:int}/summary")]
        public async Task<ActionResult<StockMoveSummaryDto>> GetMaterialStockSummary(
            int materialId,
            CancellationToken ct = default)
        {
            var summary = await _stockMoveService.GetSummaryByMaterialAsync(materialId, ct);
            if (summary == null)
                return NotFound(new { message = "Material not found", materialId });

            return Ok(summary);
        }
    }
}

