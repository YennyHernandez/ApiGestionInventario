using Inventario.Infrastructure.Repositories.Interfaces;

namespace Inventario.Infrastructure.Repositories._UnitOfWork
{
    public interface IUnitOfWorkInventory : IDisposable
    {
        IProductoRepository ProductoRepository { get; }
        ICategoriaRepository CategoriaRepository { get; }
        Task BeginAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
