-- 18_SetMealItems.sql
-- Seed Data for SetMealItems
-- 依賴：15_Dishes.sql、06_SetMeals.sql 必須先執行

IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems)
BEGIN
    DECLARE @TomatoId    int = (SELECT TOP 1 Id FROM dbo.Dishes   WHERE DishName    = N'義式番茄義大利麵');
    DECLARE @CreamId     int = (SELECT TOP 1 Id FROM dbo.Dishes   WHERE DishName    = N'奶油培根燉飯');
    DECLARE @PlatterId   int = (SELECT TOP 1 Id FROM dbo.Dishes   WHERE DishName    = N'分享拼盤');
    DECLARE @FamilyId    int = (SELECT TOP 1 Id FROM dbo.SetMeals WHERE SetMealName = N'全家分享餐');
    DECLARE @ValentineId int = (SELECT TOP 1 Id FROM dbo.SetMeals WHERE SetMealName = N'情人節限定套餐');
    DECLARE @CNYId       int = (SELECT TOP 1 Id FROM dbo.SetMeals WHERE SetMealName = N'過年限定套餐');

    IF @TomatoId IS NULL OR @CreamId IS NULL OR @PlatterId IS NULL
        THROW 50013, 'Seed failed: Dishes not found. Please run 15_Dishes.sql first.', 1;

    IF @FamilyId IS NULL OR @ValentineId IS NULL OR @CNYId IS NULL
        THROW 50014, 'Seed failed: SetMeals not found. Please run 06_SetMeals.sql first.', 1;

    INSERT INTO dbo.SetMealItems
    (
        SetMealId,
        DishId,
        Quantity,
        IsOptional,
        OptionGroupNo,
        PickLimit,
        DisplayOrder
    )
    VALUES
    -- 全家分享餐：主餐二選一（OptionGroupNo=1, PickLimit=1）+ 分享拼盤固定包含
    (@FamilyId, @TomatoId,  2, 1, 1,    1,    1),
    (@FamilyId, @CreamId,   2, 1, 1,    1,    2),
    (@FamilyId, @PlatterId, 1, 0, NULL, NULL, 3),

    -- 情人節限定套餐：主餐二選一（OptionGroupNo=1, PickLimit=1）
    (@ValentineId, @TomatoId, 1, 1, 1,    1,    1),
    (@ValentineId, @CreamId,  1, 1, 1,    1,    2),

    -- 過年限定套餐：全部固定包含
    (@CNYId, @TomatoId,  2, 0, NULL, NULL, 1),
    (@CNYId, @CreamId,   2, 0, NULL, NULL, 2),
    (@CNYId, @PlatterId, 1, 0, NULL, NULL, 3);
END
GO
