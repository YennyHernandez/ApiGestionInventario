using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventario.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStoredProceduresFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Re-ejecutar todos los SPs para actualizar su formato en la BD
            // Los SPs usan CREATE OR ALTER, por lo que es seguro re-ejecutarlos

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_GetPaged]
                    @Page INT,
                    @PageSize INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    DECLARE @Skip INT = (@Page - 1) * @PageSize;
                    SELECT 
                        p.Id, p.Sku, p.Nombre, p.Descripcion, p.Precio, p.Stock, p.StockMinimo,
                        p.Activo, p.CategoriaId, c.Nombre AS CategoriaNombre, p.FechaCreacion, p.FechaActualizacion
                    FROM Productos p
                    LEFT JOIN Categorias c ON p.CategoriaId = c.Id
                    WHERE p.Activo = 1
                    ORDER BY p.Nombre
                    OFFSET @Skip ROWS FETCH NEXT @PageSize ROWS ONLY;
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_GetTotalCount]
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT COUNT(*) AS [Value] FROM Productos;
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_GetById]
                    @Id INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT Id, Sku, Nombre, Descripcion, Precio, Stock, StockMinimo, Activo, CategoriaId, FechaCreacion, FechaActualizacion
                    FROM Productos WHERE Id = @Id;
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_GetBySku]
                    @Sku NVARCHAR(50)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT Id, Sku, Nombre, Descripcion, Precio, Stock, StockMinimo, Activo, CategoriaId, FechaCreacion, FechaActualizacion
                    FROM Productos WHERE Sku = @Sku;
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_GetAll]
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT 
                        p.Id, p.Sku, p.Nombre, p.Descripcion, p.Precio, p.Stock, p.StockMinimo,
                        p.Activo, p.CategoriaId, c.Nombre AS CategoriaNombre, p.FechaCreacion, p.FechaActualizacion
                    FROM Productos p
                    LEFT JOIN Categorias c ON p.CategoriaId = c.Id
                    WHERE p.Activo = 1
                    ORDER BY p.Nombre;
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_Create]
                    @Sku NVARCHAR(50),
                    @Nombre NVARCHAR(200),
                    @Descripcion NVARCHAR(1000),
                    @Precio DECIMAL(18,2),
                    @Stock INT,
                    @StockMinimo INT,
                    @Activo BIT,
                    @CategoriaId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    INSERT INTO Productos (Sku, Nombre, Descripcion, Precio, Stock, StockMinimo, Activo, CategoriaId, FechaCreacion)
                    VALUES (@Sku, @Nombre, @Descripcion, @Precio, @Stock, @StockMinimo, @Activo, @CategoriaId, GETDATE());
                    DECLARE @NewId INT = SCOPE_IDENTITY();
                    SELECT Id, Sku, Nombre, Descripcion, Precio, Stock, StockMinimo, Activo, CategoriaId, FechaCreacion, FechaActualizacion
                    FROM Productos WHERE Id = @NewId;
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_Update]
                    @Id INT,
                    @Sku NVARCHAR(50),
                    @Nombre NVARCHAR(200),
                    @Descripcion NVARCHAR(1000),
                    @Precio DECIMAL(18,2),
                    @Stock INT,
                    @StockMinimo INT,
                    @Activo BIT,
                    @CategoriaId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    UPDATE Productos SET Sku = @Sku, Nombre = @Nombre, Descripcion = @Descripcion, Precio = @Precio,
                        Stock = @Stock, StockMinimo = @StockMinimo, Activo = @Activo, CategoriaId = @CategoriaId, FechaActualizacion = GETDATE()
                    WHERE Id = @Id;
                    SELECT Id, Sku, Nombre, Descripcion, Precio, Stock, StockMinimo, Activo, CategoriaId, FechaCreacion, FechaActualizacion
                    FROM Productos WHERE Id = @Id;
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_Delete]
                    @Id INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    UPDATE Productos SET Activo = 0, FechaActualizacion = GETDATE() WHERE Id = @Id;
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_ExistsBySku]
                    @Sku NVARCHAR(50)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT CASE WHEN EXISTS (SELECT 1 FROM Productos WHERE Sku = @Sku) THEN 1 ELSE 0 END AS [Value];
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_ExistsBySkuExcluding]
                    @Sku NVARCHAR(50),
                    @ExcludeId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT CASE WHEN EXISTS (SELECT 1 FROM Productos WHERE Sku = @Sku AND Id != @ExcludeId) THEN 1 ELSE 0 END AS [Value];
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_GetTotalInventoryValue]
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT ISNULL(SUM(Precio * Stock), 0) AS [Value] FROM Productos WHERE Activo = 1;
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_GetCriticalStock]
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT p.Sku, p.Nombre, p.Stock, p.StockMinimo, c.Nombre AS CategoriaNombre
                    FROM Productos p
                    LEFT JOIN Categorias c ON p.CategoriaId = c.Id
                    WHERE p.Activo = 1 AND p.Stock <= p.StockMinimo
                    ORDER BY p.Stock;
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_GetByCategory]
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT c.Nombre AS CategoriaNombre,
                        COUNT(CASE WHEN p.Activo = 1 THEN 1 END) AS CantidadProductos,
                        ISNULL(SUM(CASE WHEN p.Activo = 1 THEN p.Precio * p.Stock ELSE 0 END), 0) AS ValorTotal
                    FROM Categorias c
                    LEFT JOIN Productos p ON c.Id = p.CategoriaId
                    WHERE c.Activo = 1
                    GROUP BY c.Nombre
                    ORDER BY c.Nombre;
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_GetTotalStock]
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT ISNULL(SUM(Stock), 0) AS [Value] FROM Productos WHERE Activo = 1;
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_GetTotalCapacity]
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT ISNULL(SUM(CASE WHEN StockMinimo > 0 THEN StockMinimo * 10 ELSE Stock + 100 END), 0) AS [Value]
                    FROM Productos WHERE Activo = 1;
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Categorias_GetPaged]
                    @Page INT,
                    @PageSize INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    DECLARE @Skip INT = (@Page - 1) * @PageSize;
                    SELECT c.Id, c.Nombre, c.Descripcion, c.Activo, c.FechaCreacion,
                        COUNT(CASE WHEN p.Activo = 1 THEN 1 END) AS ProductoCount
                    FROM Categorias c
                    LEFT JOIN Productos p ON c.Id = p.CategoriaId
                    GROUP BY c.Id, c.Nombre, c.Descripcion, c.Activo, c.FechaCreacion
                    ORDER BY c.Nombre
                    OFFSET @Skip ROWS FETCH NEXT @PageSize ROWS ONLY;
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Categorias_GetTotalCount]
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT COUNT(*) AS [Value] FROM Categorias;
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Categorias_GetById]
                    @Id INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT Id, Nombre, Descripcion, Activo, FechaCreacion FROM Categorias WHERE Id = @Id;
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Categorias_GetAllActive]
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT c.Id, c.Nombre, c.Descripcion, c.Activo, c.FechaCreacion,
                        COUNT(CASE WHEN p.Activo = 1 THEN 1 END) AS ProductoCount
                    FROM Categorias c
                    LEFT JOIN Productos p ON c.Id = p.CategoriaId
                    WHERE c.Activo = 1
                    GROUP BY c.Id, c.Nombre, c.Descripcion, c.Activo, c.FechaCreacion
                    ORDER BY c.Nombre;
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Categorias_Create]
                    @Nombre NVARCHAR(100),
                    @Descripcion NVARCHAR(500),
                    @Activo BIT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    INSERT INTO Categorias (Nombre, Descripcion, Activo, FechaCreacion)
                    VALUES (@Nombre, @Descripcion, @Activo, GETDATE());
                    DECLARE @NewId INT = SCOPE_IDENTITY();
                    SELECT Id, Nombre, Descripcion, Activo, FechaCreacion FROM Categorias WHERE Id = @NewId;
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Categorias_Update]
                    @Id INT,
                    @Nombre NVARCHAR(100),
                    @Descripcion NVARCHAR(500),
                    @Activo BIT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    UPDATE Categorias SET Nombre = @Nombre, Descripcion = @Descripcion, Activo = @Activo WHERE Id = @Id;
                    SELECT Id, Nombre, Descripcion, Activo, FechaCreacion FROM Categorias WHERE Id = @Id;
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Categorias_Delete]
                    @Id INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    UPDATE Categorias SET Activo = 0 WHERE Id = @Id;
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Categorias_ExistsByName]
                    @Nombre NVARCHAR(100)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT CASE WHEN EXISTS (SELECT 1 FROM Categorias WHERE Nombre = @Nombre AND Activo = 1) THEN 1 ELSE 0 END AS [Value];
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Categorias_ExistsByNameExcluding]
                    @Nombre NVARCHAR(100),
                    @ExcludeId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT CASE WHEN EXISTS (SELECT 1 FROM Categorias WHERE Nombre = @Nombre AND Id != @ExcludeId AND Activo = 1) THEN 1 ELSE 0 END AS [Value];
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Categorias_GetWithProductCount]
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT c.Id, c.Nombre, c.Descripcion, c.Activo, c.FechaCreacion,
                        COUNT(CASE WHEN p.Activo = 1 THEN 1 END) AS ProductoCount
                    FROM Categorias c
                    LEFT JOIN Productos p ON c.Id = p.CategoriaId
                    WHERE c.Activo = 1
                    GROUP BY c.Id, c.Nombre, c.Descripcion, c.Activo, c.FechaCreacion
                    ORDER BY c.Nombre;
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[SP_Categorias_HasActiveProducts]
                    @CategoriaId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT CASE WHEN EXISTS (SELECT 1 FROM Productos WHERE CategoriaId = @CategoriaId AND Activo = 1) THEN 1 ELSE 0 END AS [Value];
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No se eliminan los SPs en el Down porque la migración original ya los creó
        }
    }
}
