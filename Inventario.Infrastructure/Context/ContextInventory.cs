using Inventario.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Infrastructure.Context
{
    public class ContextInventory : DbContext
    {
        public ContextInventory(DbContextOptions<ContextInventory> options) : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.ToTable("Categorias");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.Descripcion)
                    .HasMaxLength(500);
                entity.Property(e => e.Activo)
                    .HasDefaultValue(true);
                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasIndex(e => e.Nombre)
                    .IsUnique()
                    .HasDatabaseName("IX_Categorias_Nombre");
            });

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("Productos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Sku)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.Descripcion)
                    .HasMaxLength(1000);
                entity.Property(e => e.Precio)
                    .HasPrecision(18, 2);
                entity.Property(e => e.StockMinimo)
                    .HasDefaultValue(0);
                entity.Property(e => e.Activo)
                    .HasDefaultValue(true);
                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasIndex(e => e.Sku)
                    .IsUnique()
                    .HasDatabaseName("IX_Productos_Sku");

                entity.HasIndex(e => e.CategoriaId)
                    .HasDatabaseName("IX_Productos_CategoriaId");

                entity.HasOne(e => e.Categoria)
                    .WithMany(c => c.Productos)
                    .HasForeignKey(e => e.CategoriaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
