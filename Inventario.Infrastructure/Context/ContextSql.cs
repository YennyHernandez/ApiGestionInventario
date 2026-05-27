using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Inventario.Infrastructure.Context
{
    public class ContextSql : DbContext
    {
        private readonly IConfiguration Config;

        public ContextSql(DbContextOptions<ContextSql> options, IConfiguration config) : base(options)
        {
            Config = config;
        }

        public async Task CommitAsync()
        {
            await SaveChangesAsync().ConfigureAwait(false);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
