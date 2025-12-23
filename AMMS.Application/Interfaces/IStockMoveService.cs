using AMMS.Shared.DTOs.StockMoves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Application.Interfaces
{
    public interface IStockMoveService
    {
        Task<StockMoveSummaryDto?> GetSummaryByMaterialAsync(
            int materialId,
            CancellationToken ct = default);
    }
}
