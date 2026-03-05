-- 18_1_Products.sql
-- Seed Data for Products
-- 依賴：15_Dishes.sql、06_SetMeals.sql 必須先執行

IF NOT EXISTS (SELECT 1 FROM dbo.Products)
BEGIN
    DECLARE @TomatoId    int = (SELECT TOP 1 Id FROM dbo.Dishes   WHERE DishName    = N'義式番茄義大利麵');
    DECLARE @CreamId     int = (SELECT TOP 1 Id FROM dbo.Dishes   WHERE DishName    = N'奶油培根燉飯');
    DECLARE @PlatterId   int = (SELECT TOP 1 Id FROM dbo.Dishes   WHERE DishName    = N'分享拼盤');
    DECLARE @FamilyId    int = (SELECT TOP 1 Id FROM dbo.SetMeals WHERE SetMealName = N'全家分享餐');
    DECLARE @ValentineId int = (SELECT TOP 1 Id FROM dbo.SetMeals WHERE SetMealName = N'情人節限定套餐');
    DECLARE @CNYId       int = (SELECT TOP 1 Id FROM dbo.SetMeals WHERE SetMealName = N'過年限定套餐');

    IF @TomatoId IS NULL OR @CreamId IS NULL OR @PlatterId IS NULL
        THROW 50011, 'Seed failed: Dishes not found. Please run 15_Dishes.sql first.', 1;

    IF @FamilyId IS NULL OR @ValentineId IS NULL OR @CNYId IS NULL
        THROW 50012, 'Seed failed: SetMeals not found. Please run 06_SetMeals.sql first.', 1;

    INSERT INTO dbo.Products (ProductType, DishId, SetMealId)
    VALUES
    -- 單點
    (N'Dish',    @TomatoId,  NULL        ),
    (N'Dish',    @CreamId,   NULL        ),
    (N'Dish',    @PlatterId, NULL        ),
    -- 套餐
    (N'SetMeal', NULL,       @FamilyId   ),
    (N'SetMeal', NULL,       @ValentineId),
    (N'SetMeal', NULL,       @CNYId      );
END
GO
