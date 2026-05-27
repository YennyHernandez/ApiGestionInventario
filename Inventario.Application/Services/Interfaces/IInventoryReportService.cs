using Inventario.Domain.Models.Dto;

namespace Inventario.Application.Services.Interfaces
{
    public interface IInventoryReportService
    {
        Task<ResultOperation<InventorySummaryDto>> GetSummaryAsync();
    }
}
