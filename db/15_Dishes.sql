IF NOT EXISTS (SELECT 1 FROM dbo.Dishes)
BEGIN
    DECLARE @MainId   int = (SELECT TOP 1 Id FROM dbo.Categories WHERE CategoryName = N'主餐');
    DECLARE @DrinkId  int = (SELECT TOP 1 Id FROM dbo.Categories WHERE CategoryName = N'飲料');
    DECLARE @DessertId int = (SELECT TOP 1 Id FROM dbo.Categories WHERE CategoryName = N'甜點');

    
    IF @MainId IS NULL OR @DrinkId IS NULL OR @DessertId IS NULL
        THROW 50001, 'Seed failed: Categories not found. Please run 05_Categories.sql first.', 1;

    INSERT INTO dbo.Dishes
    (
        CategoryId,
        DishName,
        Price,
        IsActive,
        CreatedAt,
        Description,
        ImageUrl,
        IsTakeOut,
        IsLimited,
        StartDate,
        EndDate,
        UpdatedAt
    )
    VALUES
    (@MainId,   N'義式番茄義大利麵', 180.00, 1, GETDATE(), N'經典義式料理', NULL, 1, 0, NULL, NULL, NULL),
    (@MainId,   N'奶油培根燉飯',     200.00, 1, GETDATE(), N'濃郁奶油風味', NULL, 1, 0, NULL, NULL, NULL),
    (@DessertId, N'分享拼盤',         320.00, 1, GETDATE(), N'多人分享餐點', NULL, 1, 0, NULL, NULL, NULL);
END
GO