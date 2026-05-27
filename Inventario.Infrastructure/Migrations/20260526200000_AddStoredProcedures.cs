using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventario.Infrastructure.Migrations
{
    public partial class AddStoredProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // PRODUCTOS - Stored Procedures

            migrationBuilder.Sql(@"
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Obtiene una lista paginada de productos activos con el nombre de su categoría.
                -- Parameters:  @Page - Número de página
                --              @PageSize - Cantidad de registros por página
                -- Returns:     Lista paginada de productos con datos de categoría
                -- =============================================

                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_GetPaged]
                    @Page INT,
                    @PageSize INT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DECLARE @Skip INT = (@Page - 1) * @PageSize;

                    SELECT 
                        p.Id,
                        p.Sku,
                        p.Nombre,
                        p.Descripcion,
                        p.Precio,
                        p.Stock,
                        p.StockMinimo,
                        p.Activo,
                        p.CategoriaId,
                        c.Nombre AS CategoriaNombre,
                        p.FechaCreacion,
                        p.FechaActualizacion
                    FROM Productos p
                    LEFT JOIN Categorias c ON p.CategoriaId = c.Id
                    WHERE p.Activo = 1
                    ORDER BY p.Nombre
                    OFFSET @Skip ROWS
                    FETCH NEXT @PageSize ROWS ONLY;

                END
            ");

            migrationBuilder.Sql(@"
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Obtiene el conteo total de productos registrados.
                -- Returns:     Cantidad total de productos
                -- =============================================

                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_GetTotalCount]
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT COUNT(*) AS [Value] FROM Productos;

                END
            ");

            migrationBuilder.Sql(@"
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Obtiene un producto específico por su identificador único.
                -- Parameters:  @Id - Identificador único del producto
                -- Returns:     Datos del producto encontrado o vacío si no existe
                -- =============================================

                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_GetById]
                    @Id INT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT 
                        Id,
                        Sku,
                        Nombre,
                        Descripcion,
                        Precio,
                        Stock,
                        StockMinimo,
                        Activo,
                        CategoriaId,
                        FechaCreacion,
                        FechaActualizacion
                    FROM Productos
                    WHERE Id = @Id;

                END
            ");

            migrationBuilder.Sql(@"
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Obtiene un producto específico por su código SKU.
                -- Parameters:  @Sku - Código SKU del producto
                -- Returns:     Datos del producto encontrado o vacío si no existe
                -- =============================================

                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_GetBySku]
                    @Sku NVARCHAR(50)
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT 
                        Id,
                        Sku,
                        Nombre,
                        Descripcion,
                        Precio,
                        Stock,
                        StockMinimo,
                        Activo,
                        CategoriaId,
                        FechaCreacion,
                        FechaActualizacion
                    FROM Productos
                    WHERE Sku = @Sku;

                END
            ");

            migrationBuilder.Sql(@"
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Obtiene todos los productos activos con el nombre de su categoría.
                -- Returns:     Lista completa de productos activos ordenados por nombre
                -- =============================================

                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_GetAll]
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT 
                        p.Id,
                        p.Sku,
                        p.Nombre,
                        p.Descripcion,
                        p.Precio,
                        p.Stock,
                        p.StockMinimo,
                        p.Activo,
                        p.CategoriaId,
                        c.Nombre AS CategoriaNombre,
                        p.FechaCreacion,
                        p.FechaActualizacion
                    FROM Productos p
                    LEFT JOIN Categorias c ON p.CategoriaId = c.Id
                    WHERE p.Activo = 1
                    ORDER BY p.Nombre;

                END
            ");

            migrationBuilder.Sql(@"
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Crea un nuevo producto en el inventario y retorna los datos del producto creado.
                -- Parameters:  @Sku - Código SKU del producto
                --              @Nombre - Nombre del producto
                --              @Descripcion - Descripción del producto (opcional)
                --              @Precio - Precio unitario del producto
                --              @Stock - Cantidad en stock
                --              @StockMinimo - Stock mínimo antes de alerta
                --              @Activo - Estado activo del producto
                --              @CategoriaId - Identificador de la categoría
                -- Returns:     Datos del producto recién creado con su ID generado
                -- =============================================

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

                    SELECT 
                        Id,
                        Sku,
                        Nombre,
                        Descripcion,
                        Precio,
                        Stock,
                        StockMinimo,
                        Activo,
                        CategoriaId,
                        FechaCreacion,
                        FechaActualizacion
                    FROM Productos
                    WHERE Id = @NewId;

                END
            ");

            migrationBuilder.Sql(@"
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Actualiza los datos de un producto existente y actualiza la fecha de modificación.
                -- Parameters:  @Id - Identificador único del producto
                --              @Sku - Código SKU del producto
                --              @Nombre - Nombre del producto
                --              @Descripcion - Descripción del producto (opcional)
                --              @Precio - Precio unitario del producto
                --              @Stock - Cantidad en stock
                --              @StockMinimo - Stock mínimo antes de alerta
                --              @Activo - Estado activo del producto
                --              @CategoriaId - Identificador de la categoría
                -- Returns:     Datos actualizados del producto
                -- =============================================

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

                    UPDATE Productos
                    SET 
                        Sku = @Sku,
                        Nombre = @Nombre,
                        Descripcion = @Descripcion,
                        Precio = @Precio,
                        Stock = @Stock,
                        StockMinimo = @StockMinimo,
                        Activo = @Activo,
                        CategoriaId = @CategoriaId,
                        FechaActualizacion = GETDATE()
                    WHERE Id = @Id;

                    SELECT 
                        Id,
                        Sku,
                        Nombre,
                        Descripcion,
                        Precio,
                        Stock,
                        StockMinimo,
                        Activo,
                        CategoriaId,
                        FechaCreacion,
                        FechaActualizacion
                    FROM Productos
                    WHERE Id = @Id;

                END
            ");

            migrationBuilder.Sql(@"
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Realiza la eliminación lógica de un producto estableciendo su estado como inactivo.
                -- Parameters:  @Id - Identificador único del producto a eliminar
                -- Returns:     Ninguno
                -- =============================================

                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_Delete]
                    @Id INT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    UPDATE Productos
                    SET Activo = 0, FechaActualizacion = GETDATE()
                    WHERE Id = @Id;

                END
            ");

            migrationBuilder.Sql(@"
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Verifica si existe un producto con el SKU especificado.
                -- Parameters:  @Sku - Código SKU a verificar
                -- Returns:     1 si existe, 0 si no existe
                -- =============================================

                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_ExistsBySku]
                    @Sku NVARCHAR(50)
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT CASE WHEN EXISTS (SELECT 1 FROM Productos WHERE Sku = @Sku) THEN 1 ELSE 0 END AS [Value];

                END
            ");

            migrationBuilder.Sql(@"
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Verifica si existe un producto con el SKU especificado, excluyendo un producto por su ID.
                -- Parameters:  @Sku - Código SKU a verificar
                --              @ExcludeId - ID del producto a excluir de la verificación
                -- Returns:     1 si existe otro producto con el mismo SKU, 0 si no existe
                -- =============================================

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
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Calcula el valor total del inventario sumando el precio por el stock de todos los productos activos.
                -- Returns:     Valor monetario total del inventario
                -- =============================================

                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_GetTotalInventoryValue]
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT ISNULL(SUM(Precio * Stock), 0) AS [Value]
                    FROM Productos
                    WHERE Activo = 1;

                END
            ");

            migrationBuilder.Sql(@"
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Obtiene los productos cuyo stock actual es menor o igual a su stock mínimo configurado.
                -- Returns:     Lista de productos con stock crítico incluyendo nombre de categoría
                -- =============================================

                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_GetCriticalStock]
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT 
                        p.Sku,
                        p.Nombre,
                        p.Stock,
                        p.StockMinimo,
                        c.Nombre AS CategoriaNombre
                    FROM Productos p
                    LEFT JOIN Categorias c ON p.CategoriaId = c.Id
                    WHERE p.Activo = 1 AND p.Stock <= p.StockMinimo
                    ORDER BY p.Stock;

                END
            ");

            migrationBuilder.Sql(@"
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Obtiene un resumen de productos agrupados por categoría activa, incluyendo cantidad de productos y valor total.
                -- Returns:     Resumen por categoría con cantidad de productos activos y valor total del inventario
                -- =============================================

                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_GetByCategory]
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT 
                        c.Nombre AS CategoriaNombre,
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
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Calcula la suma total de unidades en stock de todos los productos activos.
                -- Returns:     Cantidad total de unidades en inventario
                -- =============================================

                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_GetTotalStock]
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT ISNULL(SUM(Stock), 0) AS [Value]
                    FROM Productos
                    WHERE Activo = 1;

                END
            ");

            migrationBuilder.Sql(@"
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Calcula la capacidad total del inventario usando el stock mínimo multiplicado por 10, o el stock actual más 100 si no tiene stock mínimo configurado.
                -- Returns:     Capacidad total estimada del inventario
                -- =============================================

                CREATE OR ALTER PROCEDURE [dbo].[SP_Productos_GetTotalCapacity]
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT ISNULL(SUM(CASE WHEN StockMinimo > 0 THEN StockMinimo * 10 ELSE Stock + 100 END), 0) AS [Value]
                    FROM Productos
                    WHERE Activo = 1;

                END
            ");

            // CATEGORIAS - Stored Procedures

            migrationBuilder.Sql(@"
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Obtiene una lista paginada de categorías con el conteo de productos activos asociados.
                -- Parameters:  @Page - Número de página
                --              @PageSize - Cantidad de registros por página
                -- Returns:     Lista paginada de categorías con conteo de productos
                -- =============================================

                CREATE OR ALTER PROCEDURE [dbo].[SP_Categorias_GetPaged]
                    @Page INT,
                    @PageSize INT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DECLARE @Skip INT = (@Page - 1) * @PageSize;

                    SELECT 
                        c.Id,
                        c.Nombre,
                        c.Descripcion,
                        c.Activo,
                        c.FechaCreacion,
                        COUNT(CASE WHEN p.Activo = 1 THEN 1 END) AS ProductoCount
                    FROM Categorias c
                    LEFT JOIN Productos p ON c.Id = p.CategoriaId
                    GROUP BY c.Id, c.Nombre, c.Descripcion, c.Activo, c.FechaCreacion
                    ORDER BY c.Nombre
                    OFFSET @Skip ROWS
                    FETCH NEXT @PageSize ROWS ONLY;

                END
            ");

            migrationBuilder.Sql(@"
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Obtiene el conteo total de categorías registradas.
                -- Returns:     Cantidad total de categorías
                -- =============================================

                CREATE OR ALTER PROCEDURE [dbo].[SP_Categorias_GetTotalCount]
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT COUNT(*) AS [Value] FROM Categorias;

                END
            ");

            migrationBuilder.Sql(@"
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Obtiene una categoría específica por su identificador único.
                -- Parameters:  @Id - Identificador único de la categoría
                -- Returns:     Datos de la categoría encontrada o vacío si no existe
                -- =============================================

                CREATE OR ALTER PROCEDURE [dbo].[SP_Categorias_GetById]
                    @Id INT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT 
                        Id,
                        Nombre,
                        Descripcion,
                        Activo,
                        FechaCreacion
                    FROM Categorias
                    WHERE Id = @Id;

                END
            ");

            migrationBuilder.Sql(@"
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Obtiene todas las categorías activas con el conteo de productos activos asociados.
                -- Returns:     Lista de categorías activas ordenadas por nombre
                -- =============================================

                CREATE OR ALTER PROCEDURE [dbo].[SP_Categorias_GetAllActive]
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT 
                        c.Id,
                        c.Nombre,
                        c.Descripcion,
                        c.Activo,
                        c.FechaCreacion,
                        COUNT(CASE WHEN p.Activo = 1 THEN 1 END) AS ProductoCount
                    FROM Categorias c
                    LEFT JOIN Productos p ON c.Id = p.CategoriaId
                    WHERE c.Activo = 1
                    GROUP BY c.Id, c.Nombre, c.Descripcion, c.Activo, c.FechaCreacion
                    ORDER BY c.Nombre;

                END
            ");

            migrationBuilder.Sql(@"
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Crea una nueva categoría y retorna los datos de la categoría creada.
                -- Parameters:  @Nombre - Nombre de la categoría
                --              @Descripcion - Descripción de la categoría (opcional)
                --              @Activo - Estado activo de la categoría
                -- Returns:     Datos de la categoría recién creada con su ID generado
                -- =============================================

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

                    SELECT 
                        Id,
                        Nombre,
                        Descripcion,
                        Activo,
                        FechaCreacion
                    FROM Categorias
                    WHERE Id = @NewId;

                END
            ");

            migrationBuilder.Sql(@"
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Actualiza los datos de una categoría existente.
                -- Parameters:  @Id - Identificador único de la categoría
                --              @Nombre - Nombre de la categoría
                --              @Descripcion - Descripción de la categoría (opcional)
                --              @Activo - Estado activo de la categoría
                -- Returns:     Datos actualizados de la categoría
                -- =============================================

                CREATE OR ALTER PROCEDURE [dbo].[SP_Categorias_Update]
                    @Id INT,
                    @Nombre NVARCHAR(100),
                    @Descripcion NVARCHAR(500),
                    @Activo BIT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    UPDATE Categorias
                    SET 
                        Nombre = @Nombre,
                        Descripcion = @Descripcion,
                        Activo = @Activo
                    WHERE Id = @Id;

                    SELECT 
                        Id,
                        Nombre,
                        Descripcion,
                        Activo,
                        FechaCreacion
                    FROM Categorias
                    WHERE Id = @Id;

                END
            ");

            migrationBuilder.Sql(@"
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Realiza la eliminación lógica de una categoría estableciendo su estado como inactivo.
                -- Parameters:  @Id - Identificador único de la categoría a eliminar
                -- Returns:     Ninguno
                -- =============================================

                CREATE OR ALTER PROCEDURE [dbo].[SP_Categorias_Delete]
                    @Id INT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    UPDATE Categorias
                    SET Activo = 0
                    WHERE Id = @Id;

                END
            ");

            migrationBuilder.Sql(@"
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Verifica si existe una categoría activa con el nombre especificado.
                -- Parameters:  @Nombre - Nombre de la categoría a verificar
                -- Returns:     1 si existe, 0 si no existe
                -- =============================================

                CREATE OR ALTER PROCEDURE [dbo].[SP_Categorias_ExistsByName]
                    @Nombre NVARCHAR(100)
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT CASE WHEN EXISTS (SELECT 1 FROM Categorias WHERE Nombre = @Nombre AND Activo = 1) THEN 1 ELSE 0 END AS [Value];

                END
            ");

            migrationBuilder.Sql(@"
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Verifica si existe una categoría activa con el nombre especificado, excluyendo una categoría por su ID.
                -- Parameters:  @Nombre - Nombre de la categoría a verificar
                --              @ExcludeId - ID de la categoría a excluir de la verificación
                -- Returns:     1 si existe otra categoría con el mismo nombre, 0 si no existe
                -- =============================================

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
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Obtiene todas las categorías activas con el conteo de productos activos asociados.
                -- Returns:     Lista de categorías activas con conteo de productos
                -- =============================================

                CREATE OR ALTER PROCEDURE [dbo].[SP_Categorias_GetWithProductCount]
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT 
                        c.Id,
                        c.Nombre,
                        c.Descripcion,
                        c.Activo,
                        c.FechaCreacion,
                        COUNT(CASE WHEN p.Activo = 1 THEN 1 END) AS ProductoCount
                    FROM Categorias c
                    LEFT JOIN Productos p ON c.Id = p.CategoriaId
                    WHERE c.Activo = 1
                    GROUP BY c.Id, c.Nombre, c.Descripcion, c.Activo, c.FechaCreacion
                    ORDER BY c.Nombre;

                END
            ");

            migrationBuilder.Sql(@"
                SET ANSI_NULLS ON;
                SET QUOTED_IDENTIFIER ON;

                -- =============================================
                -- Author:      Yeni Hernández
                -- Create date: 2026-05-26
                -- Description: Verifica si una categoría tiene productos activos asociados.
                -- Parameters:  @CategoriaId - ID de la categoría a verificar
                -- Returns:     1 si tiene productos activos, 0 si no tiene
                -- =============================================

                CREATE OR ALTER PROCEDURE [dbo].[SP_Categorias_HasActiveProducts]
                    @CategoriaId INT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT CASE WHEN EXISTS (SELECT 1 FROM Productos WHERE CategoriaId = @CategoriaId AND Activo = 1) THEN 1 ELSE 0 END AS [Value];

                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Productos_GetPaged]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Productos_GetTotalCount]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Productos_GetById]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Productos_GetBySku]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Productos_GetAll]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Productos_Create]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Productos_Update]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Productos_Delete]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Productos_ExistsBySku]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Productos_ExistsBySkuExcluding]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Productos_GetTotalInventoryValue]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Productos_GetCriticalStock]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Productos_GetByCategory]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Productos_GetTotalStock]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Productos_GetTotalCapacity]");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Categorias_GetPaged]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Categorias_GetTotalCount]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Categorias_GetById]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Categorias_GetAllActive]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Categorias_Create]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Categorias_Update]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Categorias_Delete]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Categorias_ExistsByName]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Categorias_ExistsByNameExcluding]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Categorias_GetWithProductCount]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SP_Categorias_HasActiveProducts]");
        }
    }
}
