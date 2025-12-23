using AMMS.Infrastructure.DBContext;
using AMMS.Infrastructure.Interfaces;
using AMMS.Shared.DTOs.StockMoves;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Infrastructure.Repositories
{
    public class StockMoveRepository : IStockMoveRepository
    {
        private readonly AppDbContext _db;

        public StockMoveRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<StockMoveSummaryDto?> GetSummaryByMaterialAsync(
            int materialId,
            CancellationToken ct = default)
        {
            // 1) Lấy thông tin NVL
            var material = await _db.materials
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.material_id == materialId, ct);

            if (material == null)
                return null;

            var currentStock = material.stock_qty ?? 0m;

            // 2) Lấy toàn bộ stock_moves của NVL này
            var moves = await _db.stock_moves
                .AsNoTracking()
                .Where(x => x.material_id == materialId)
                .ToListAsync(ct);

            decimal totalIn = moves
                .Where(x => x.type == "IN")
                .Sum(x => x.qty ?? 0m);

            decimal totalOut = moves
                .Where(x => x.type == "OUT")
                .Sum(x => x.qty ?? 0m);

            decimal totalReturn = moves
                .Where(x => x.type == "RETURN")
                .Sum(x => x.qty ?? 0m);

            var calculated = totalIn + totalReturn - totalOut;

            return new StockMoveSummaryDto(
                material.material_id,
                material.code,
                material.name,
                material.unit,
                currentStock,
                totalIn,
                totalOut,
                totalReturn,
                calculated
            );
        }
    }
}
