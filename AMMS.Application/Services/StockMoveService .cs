using AMMS.Application.Interfaces;
using AMMS.Infrastructure.Interfaces;
using AMMS.Shared.DTOs.StockMoves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Application.Services
{
    public class StockMoveService : IStockMoveService
    {
        private readonly IStockMoveRepository _repo;

        public StockMoveService(IStockMoveRepository repo)
        {
            _repo = repo;
        }

        public Task<StockMoveSummaryDto?> GetSummaryByMaterialAsync(
            int materialId,
            CancellationToken ct = default)
            => _repo.GetSummaryByMaterialAsync(materialId, ct);
    }
}
