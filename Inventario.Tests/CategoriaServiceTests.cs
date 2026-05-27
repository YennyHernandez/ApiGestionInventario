using Inventario.Application.Services;
using Inventario.Domain.Models.Dto;
using Inventario.Infrastructure.Repositories._UnitOfWork;
using Inventario.Infrastructure.Repositories.Interfaces;
using Moq;

namespace Inventario.Tests
{
    public class CategoriaServiceTests
    {
        private readonly Mock<IUnitOfWorkInventory> _mockUnitOfWork;
        private readonly Mock<IProductoRepository> _mockProductoRepo;
        private readonly Mock<ICategoriaRepository> _mockCategoriaRepo;
        private readonly CategoriaService _service;

        public CategoriaServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWorkInventory>();
            _mockProductoRepo = new Mock<IProductoRepository>();
            _mockCategoriaRepo = new Mock<ICategoriaRepository>();

            _mockUnitOfWork.Setup(u => u.ProductoRepository).Returns(_mockProductoRepo.Object);
            _mockUnitOfWork.Setup(u => u.CategoriaRepository).Returns(_mockCategoriaRepo.Object);

            _service = new CategoriaService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Create_Categoria_Valid_ReturnsSuccess()
        {
            var dto = new CategoriaCreateDto
            {
                Nombre = "Electronica",
                Descripcion = "Productos electronicos"
            };

            _mockCategoriaRepo.Setup(r => r.ExistsByNameAsync(dto.Nombre)).ReturnsAsync(false);
            _mockCategoriaRepo.Setup(r => r.CreateAsync(It.IsAny<Domain.Entities.Categoria>()))
                .ReturnsAsync((Domain.Entities.Categoria c) => { c.Id = 1; return c; });

            var result = await _service.CreateAsync(dto);

            Assert.True(result.stateOperation);
            Assert.NotNull(result.Result);
            Assert.Equal("Electronica", result.Result.Nombre);
        }

        [Fact]
        public async Task Create_Categoria_DuplicateName_ReturnsError()
        {
            var dto = new CategoriaCreateDto
            {
                Nombre = "Electronica",
                Descripcion = "Productos electronicos"
            };

            _mockCategoriaRepo.Setup(r => r.ExistsByNameAsync(dto.Nombre)).ReturnsAsync(true);

            var result = await _service.CreateAsync(dto);

            Assert.False(result.stateOperation);
            Assert.Contains("nombre", result.MessageResult.ToLower());
        }

        [Fact]
        public async Task Delete_Categoria_WithProducts_ReturnsError()
        {
            _mockCategoriaRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Domain.Entities.Categoria { Id = 1, Nombre = "Test", Activo = true });
            _mockCategoriaRepo.Setup(r => r.HasActiveProductsAsync(1)).ReturnsAsync(true);

            var result = await _service.DeleteAsync(1);

            Assert.False(result.stateOperation);
            Assert.Contains("productos activos", result.MessageResult.ToLower());
        }

        [Fact]
        public async Task Delete_Categoria_WithoutProducts_ReturnsSuccess()
        {
            _mockCategoriaRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Domain.Entities.Categoria { Id = 1, Nombre = "Test", Activo = true });
            _mockCategoriaRepo.Setup(r => r.HasActiveProductsAsync(1)).ReturnsAsync(false);
            _mockCategoriaRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            var result = await _service.DeleteAsync(1);

            Assert.True(result.stateOperation);
        }

        [Fact]
        public async Task Delete_Categoria_NotFound_ReturnsError()
        {
            _mockCategoriaRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Domain.Entities.Categoria?)null);

            var result = await _service.DeleteAsync(999);

            Assert.False(result.stateOperation);
            Assert.Contains("no encontrada", result.MessageResult.ToLower());
        }
    }
}
