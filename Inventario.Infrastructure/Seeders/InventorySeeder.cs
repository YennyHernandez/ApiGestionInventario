using Inventario.Domain.Entities;
using Inventario.Infrastructure.Context;

namespace Inventario.Infrastructure.Seeders
{
    public static class InventorySeeder
    {
        public static async Task SeedAsync(ContextInventory context)
        {
            if (context.Categorias.Any())
                return;

            var categorias = new List<Categoria>
            {
                new Categoria { Nombre = "Electronica", Descripcion = "Dispositivos y accesorios electronicos", Activo = true, FechaCreacion = DateTime.Now },
                new Categoria { Nombre = "Ropa", Descripcion = "Prendas de vestir para hombre y mujer", Activo = true, FechaCreacion = DateTime.Now },
                new Categoria { Nombre = "Alimentos", Descripcion = "Productos alimenticios no perecederos", Activo = true, FechaCreacion = DateTime.Now },
                new Categoria { Nombre = "Hogar", Descripcion = "Articulos para el hogar y decoracion", Activo = true, FechaCreacion = DateTime.Now },
                new Categoria { Nombre = "Deportes", Descripcion = "Equipamiento y ropa deportiva", Activo = true, FechaCreacion = DateTime.Now }
            };

            context.Categorias.AddRange(categorias);
            await context.SaveChangesAsync();

            var productos = new List<Producto>
            {
                new Producto { Sku = "ELEC-001", Nombre = "Audifonos Bluetooth", Descripcion = "Audifonos inalambricos con cancelacion de ruido", Precio = 89.99m, Stock = 150, StockMinimo = 20, CategoriaId = categorias[0].Id, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Sku = "ELEC-002", Nombre = "Cargador USB-C", Descripcion = "Cargador rapido 65W", Precio = 29.99m, Stock = 300, StockMinimo = 50, CategoriaId = categorias[0].Id, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Sku = "ELEC-003", Nombre = "Mouse Inalambrico", Descripcion = "Mouse ergonomico de precision", Precio = 45.50m, Stock = 8, StockMinimo = 15, CategoriaId = categorias[0].Id, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Sku = "ROPA-001", Nombre = "Camiseta Algodon", Descripcion = "Camiseta 100% algodon talla M", Precio = 19.99m, Stock = 500, StockMinimo = 100, CategoriaId = categorias[1].Id, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Sku = "ROPA-002", Nombre = "Jeans Classic", Descripcion = "Jeans corte recto talla 32", Precio = 49.99m, Stock = 200, StockMinimo = 30, CategoriaId = categorias[1].Id, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Sku = "ALIM-001", Nombre = "Cafe Premium 500g", Descripcion = "Cafe molido de origen colombiano", Precio = 15.99m, Stock = 1000, StockMinimo = 200, CategoriaId = categorias[2].Id, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Sku = "ALIM-002", Nombre = "Aceite de Oliva 1L", Descripcion = "Aceite de oliva extra virgen", Precio = 12.50m, Stock = 5, StockMinimo = 50, CategoriaId = categorias[2].Id, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Sku = "HOGAR-001", Nombre = "Lampara LED", Descripcion = "Lampara de mesa con luz regulable", Precio = 35.00m, Stock = 75, StockMinimo = 10, CategoriaId = categorias[3].Id, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Sku = "DEP-001", Nombre = "Balon de Futbol", Descripcion = "Balon oficial talla 5", Precio = 25.99m, Stock = 120, StockMinimo = 20, CategoriaId = categorias[4].Id, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Sku = "DEP-002", Nombre = "Botella Deportiva", Descripcion = "Botella termica 750ml", Precio = 18.99m, Stock = 3, StockMinimo = 25, CategoriaId = categorias[4].Id, Activo = true, FechaCreacion = DateTime.Now }
            };

            context.Productos.AddRange(productos);
            await context.SaveChangesAsync();
        }
    }
}
