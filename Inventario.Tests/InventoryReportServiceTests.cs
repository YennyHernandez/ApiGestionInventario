using Inventario.Application.Services;
using Inventario.Domain.Models.Dto;
using Inventario.Infrastructure.Repositories._UnitOfWork;
using Inventario.Infrastructure.Repositories.Interfaces;
using Moq;

namespace Inventario.Tests
{
    public class InventoryReportServiceTests
    {
        private readonly Mock<IUnitOfWorkInventory> _mockUnitOfWork;
        private readonly Mock<IProductoRepository> _mockProductoRepo;
        private readonly Mock<ICategoriaRepository> _mockCategoriaRepo;
        private readonly InventoryReportService _service;

        public InventoryReportServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWorkInventory>();
            _mockProductoRepo = new Mock<IProductoRepository>();
            _mockCategoriaRepo = new Mock<ICategoriaRepository>();

            _mockUnitOfWork.Setup(u => u.ProductoRepository).Returns(_mockProductoRepo.Object);
            _mockUnitOfWork.Setup(u => u.CategoriaRepository).Returns(_mockCategoriaRepo.Object);

            _service = new InventoryReportService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetSummary_ReturnsCorrectTotals()
        {
            _mockProductoRepo.Setup(r => r.GetTotalInventoryValueAsync()).ReturnsAsync(150000.00m);
            _mockProductoRepo.Setup(r => r.GetProductsByCategoryAsync()).ReturnsAsync(new List<CategoriaResumenDto>
            {
                new CategoriaResumenDto { CategoriaNombre = "Electronica", CantidadProductos = 5, ValorTotal = 75000.00m },
                new CategoriaResumenDto { CategoriaNombre = "Ropa", CantidadProductos = 3, ValorTotal = 75000.00m }
            });
            _mockProductoRepo.Setup(r => r.GetCriticalStockProductsAsync()).ReturnsAsync(new List<ProductoStockCriticoDto>
            {
                new ProductoStockCriticoDto { Sku = "PROD-003", Nombre = "Producto Bajo", Stock = 2, StockMinimo = 10, CategoriaNombre = "Electronica" }
            });
            _mockProductoRepo.Setup(r => r.GetTotalStockAsync()).ReturnsAsync(1500);
            _mockProductoRepo.Setup(r => r.GetTotalCapacityAsync()).ReturnsAsync(2000);

            var result = await _service.GetSummaryAsync();

            Assert.True(result.stateOperation);
            Assert.NotNull(result.Result);
            Assert.Equal(150000.00m, result.Result.ValorTotalInventario);
            Assert.Equal(2, result.Result.ProductosPorCategoria.Count);
            Assert.Single(result.Result.ProductosStockCritico);
            Assert.Equal(75.00m, result.Result.PorcentajeOcupacion);
        }

        [Fact]
        public async Task GetSummary_EmptyInventory_ReturnsZeroTotals()
        {
            _mockProductoRepo.Setup(r => r.GetTotalInventoryValueAsync()).ReturnsAsync(0m);
            _mockProductoRepo.Setup(r => r.GetProductsByCategoryAsync()).ReturnsAsync(new List<CategoriaResumenDto>());
            _mockProductoRepo.Setup(r => r.GetCriticalStockProductsAsync()).ReturnsAsync(new List<ProductoStockCriticoDto>());
            _mockProductoRepo.Setup(r => r.GetTotalStockAsync()).ReturnsAsync(0);
            _mockProductoRepo.Setup(r => r.GetTotalCapacityAsync()).ReturnsAsync(0);

            var result = await _service.GetSummaryAsync();

            Assert.True(result.stateOperation);
            Assert.Equal(0m, result.Result.ValorTotalInventario);
            Assert.Empty(result.Result.ProductosPorCategoria);
            Assert.Empty(result.Result.ProductosStockCritico);
            Assert.Equal(0m, result.Result.PorcentajeOcupacion);
        }
    }
}
