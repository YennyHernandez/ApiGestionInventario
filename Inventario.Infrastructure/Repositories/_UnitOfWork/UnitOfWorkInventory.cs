using Inventario.Infrastructure.Context;
using Inventario.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Inventario.Infrastructure.Repositories._UnitOfWork
{
    public class UnitOfWorkInventory : IUnitOfWorkInventory
    {
        private readonly ContextInventory _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed;

        private IProductoRepository? _productoRepository;
        private ICategoriaRepository? _categoriaRepository;

        public IProductoRepository ProductoRepository =>
            _productoRepository ??= new ProductoRepository(_context);

        public ICategoriaRepository CategoriaRepository =>
            _categoriaRepository ??= new CategoriaRepository(_context);

        public UnitOfWorkInventory(ContextInventory context)
        {
            _context = context;
        }

        public async Task BeginAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
