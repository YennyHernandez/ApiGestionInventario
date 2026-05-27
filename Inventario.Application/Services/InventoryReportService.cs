using Inventario.Application.Services.Interfaces;
using Inventario.Domain.Models.Dto;
using Inventario.Infrastructure.Repositories._UnitOfWork;

namespace Inventario.Application.Services
{
    public class InventoryReportService : IInventoryReportService
    {
        private readonly IUnitOfWorkInventory _unitOfWork;

        public InventoryReportService(IUnitOfWorkInventory unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultOperation<InventorySummaryDto>> GetSummaryAsync()
        {
            var result = new ResultOperation<InventorySummaryDto>();

            try
            {
                var totalValue = await _unitOfWork.ProductoRepository.GetTotalInventoryValueAsync();
                var byCategory = await _unitOfWork.ProductoRepository.GetProductsByCategoryAsync();
                var criticalStock = await _unitOfWork.ProductoRepository.GetCriticalStockProductsAsync();
                var totalStock = await _unitOfWork.ProductoRepository.GetTotalStockAsync();
                var totalCapacity = await _unitOfWork.ProductoRepository.GetTotalCapacityAsync();

                var occupancyPercentage = totalCapacity > 0
                    ? Math.Round((decimal)totalStock / totalCapacity * 100, 2)
                    : 0;

                var summary = new InventorySummaryDto
                {
                    ValorTotalInventario = totalValue,
                    ProductosPorCategoria = byCategory,
                    ProductosStockCritico = criticalStock,
                    PorcentajeOcupacion = occupancyPercentage
                };

                result.stateOperation = true;
                result.Result = summary;

                return result;
            }
            catch (Exception ex)
            {
                result.stateOperation = false;
                result.MessageResult = "Error al obtener el resumen de inventario.";
                result.MessageExceptionTechnical = ex.Message;
                return result;
            }
        }
    }
}
