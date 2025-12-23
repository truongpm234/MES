using AMMS.Shared.DTOs.StockMoves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Infrastructure.Interfaces
{
    public interface IStockMoveRepository
    {
        Task<StockMoveSummaryDto?> GetSummaryByMaterialAsync(
            int materialId,
            CancellationToken ct = default);
    }
}
