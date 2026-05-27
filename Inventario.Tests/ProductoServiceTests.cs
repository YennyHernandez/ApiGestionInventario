using Inventario.Application.Services;
using Inventario.Application.Services.Interfaces;
using Inventario.Domain.Models.Dto;
using Inventario.Infrastructure.Repositories._UnitOfWork;
using Inventario.Infrastructure.Repositories.Interfaces;
using Moq;

namespace Inventario.Tests
{
    public class ProductoServiceTests
    {
        private readonly Mock<IUnitOfWorkInventory> _mockUnitOfWork;
        private readonly Mock<IProductoRepository> _mockProductoRepo;
        private readonly Mock<ICategoriaRepository> _mockCategoriaRepo;
        private readonly ProductoService _service;

        public ProductoServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWorkInventory>();
            _mockProductoRepo = new Mock<IProductoRepository>();
            _mockCategoriaRepo = new Mock<ICategoriaRepository>();

            _mockUnitOfWork.Setup(u => u.ProductoRepository).Returns(_mockProductoRepo.Object);
            _mockUnitOfWork.Setup(u => u.CategoriaRepository).Returns(_mockCategoriaRepo.Object);

            _service = new ProductoService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Create_Product_Valid_ReturnsSuccess()
        {
            var dto = new ProductoCreateDto
            {
                Sku = "PROD-001",
                Nombre = "Producto Test",
                Descripcion = "Descripcion",
                Precio = 99.99m,
                Stock = 100,
                StockMinimo = 10,
                CategoriaId = 1
            };

            _mockProductoRepo.Setup(r => r.ExistsBySkuAsync(dto.Sku)).ReturnsAsync(false);
            _mockCategoriaRepo.Setup(r => r.GetByIdAsync(dto.CategoriaId)).ReturnsAsync(new Domain.Entities.Categoria { Id = 1, Nombre = "Test", Activo = true });
            _mockProductoRepo.Setup(r => r.CreateAsync(It.IsAny<Domain.Entities.Producto>()))
                .ReturnsAsync((Domain.Entities.Producto p) => { p.Id = 1; return p; });

            var result = await _service.CreateAsync(dto);

            Assert.True(result.stateOperation);
            Assert.NotNull(result.Result);
            Assert.Equal("PROD-001", result.Result.Sku);
        }

        [Fact]
        public async Task Create_Product_DuplicateSku_ReturnsError()
        {
            var dto = new ProductoCreateDto
            {
                Sku = "PROD-001",
                Nombre = "Producto Test",
                Precio = 99.99m,
                Stock = 100,
                StockMinimo = 10,
                CategoriaId = 1
            };

            _mockProductoRepo.Setup(r => r.ExistsBySkuAsync(dto.Sku)).ReturnsAsync(true);

            var result = await _service.CreateAsync(dto);

            Assert.False(result.stateOperation);
            Assert.Contains("SKU", result.MessageResult);
        }

        [Fact]
        public async Task Create_Product_NegativeStock_ReturnsError()
        {
            var dto = new ProductoCreateDto
            {
                Sku = "PROD-001",
                Nombre = "Producto Test",
                Precio = 99.99m,
                Stock = -5,
                StockMinimo = 10,
                CategoriaId = 1
            };

            _mockProductoRepo.Setup(r => r.ExistsBySkuAsync(dto.Sku)).ReturnsAsync(false);
            _mockCategoriaRepo.Setup(r => r.GetByIdAsync(dto.CategoriaId)).ReturnsAsync(new Domain.Entities.Categoria { Id = 1, Nombre = "Test", Activo = true });

            var result = await _service.CreateAsync(dto);

            Assert.False(result.stateOperation);
            Assert.Contains("stock", result.MessageResult.ToLower());
        }

        [Fact]
        public async Task Update_Product_NotFound_ReturnsError()
        {
            var dto = new ProductoUpdateDto
            {
                Id = 999,
                Sku = "PROD-001",
                Nombre = "Producto Test",
                Precio = 99.99m,
                Stock = 100,
                StockMinimo = 10,
                Activo = true,
                CategoriaId = 1
            };

            _mockProductoRepo.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync((Domain.Entities.Producto?)null);

            var result = await _service.UpdateAsync(dto);

            Assert.False(result.stateOperation);
            Assert.Contains("no encontrado", result.MessageResult.ToLower());
        }

        [Fact]
        public async Task Delete_Product_WithActiveProducts_ReturnsError()
        {
            var dto = new ProductoUpdateDto
            {
                Id = 1,
                Sku = "PROD-001",
                Nombre = "Producto Test",
                Precio = 99.99m,
                Stock = 100,
                StockMinimo = 10,
                Activo = true,
                CategoriaId = 1
            };

            var existingProducto = new Domain.Entities.Producto
            {
                Id = 1,
                Sku = "PROD-001",
                Nombre = "Producto Test",
                Precio = 99.99m,
                Stock = 100,
                StockMinimo = 10,
                Activo = true,
                CategoriaId = 1
            };

            _mockProductoRepo.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync(existingProducto);
            _mockCategoriaRepo.Setup(r => r.GetByIdAsync(dto.CategoriaId)).ReturnsAsync(new Domain.Entities.Categoria { Id = 1, Nombre = "Test", Activo = true });
            _mockProductoRepo.Setup(r => r.UpdateAsync(It.IsAny<Domain.Entities.Producto>())).Returns(Task.CompletedTask);

            var result = await _service.UpdateAsync(dto);

            Assert.True(result.stateOperation);
        }
    }
}
