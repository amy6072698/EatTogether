-- 18_SetMealItems.sql
-- Seed Data for SetMealItems (追加模式，逐筆檢查避免重複)
-- 依賴：15_Dishes.sql、06_SetMeals.sql 必須先執行

DECLARE @TomatoId      int = (SELECT TOP 1 Id FROM dbo.Dishes   WHERE DishName    = N'義式番茄義大利麵');
DECLARE @CreamId       int = (SELECT TOP 1 Id FROM dbo.Dishes   WHERE DishName    = N'奶油培根燉飯');
DECLARE @PlatterId     int = (SELECT TOP 1 Id FROM dbo.Dishes   WHERE DishName    = N'分享拼盤');
DECLARE @ChickenLegId  int = (SELECT TOP 1 Id FROM dbo.Dishes   WHERE DishName    = N'香烤雞腿排');
DECLARE @SalmonId      int = (SELECT TOP 1 Id FROM dbo.Dishes   WHERE DishName    = N'香煎鮭魚排');
DECLARE @KarageId      int = (SELECT TOP 1 Id FROM dbo.Dishes   WHERE DishName    = N'日式唐揚雞定食');
DECLARE @RedWineBeefId int = (SELECT TOP 1 Id FROM dbo.Dishes   WHERE DishName    = N'紅酒燉牛肉飯');
DECLARE @ColaId        int = (SELECT TOP 1 Id FROM dbo.Dishes   WHERE DishName    = N'可樂');
DECLARE @MilkTeaId     int = (SELECT TOP 1 Id FROM dbo.Dishes   WHERE DishName    = N'招牌鮮奶茶');
DECLARE @AmericanoId   int = (SELECT TOP 1 Id FROM dbo.Dishes   WHERE DishName    = N'美式黑咖啡');
DECLARE @MatchaLatteId int = (SELECT TOP 1 Id FROM dbo.Dishes   WHERE DishName    = N'抹茶拿鐵');
DECLARE @OrangeJuiceId int = (SELECT TOP 1 Id FROM dbo.Dishes   WHERE DishName    = N'柳橙汁');
DECLARE @TiramisuId    int = (SELECT TOP 1 Id FROM dbo.Dishes   WHERE DishName    = N'提拉米蘇');
DECLARE @LavaChocId    int = (SELECT TOP 1 Id FROM dbo.Dishes   WHERE DishName    = N'巧克力熔岩蛋糕');
DECLARE @CremeBruleeId int = (SELECT TOP 1 Id FROM dbo.Dishes   WHERE DishName    = N'法式烤布蕾');
DECLARE @MilleCrepeId  int = (SELECT TOP 1 Id FROM dbo.Dishes   WHERE DishName    = N'草莓千層蛋糕');
DECLARE @FamilyId    int = (SELECT TOP 1 Id FROM dbo.SetMeals WHERE SetMealName = N'全家分享餐');
DECLARE @ValentineId int = (SELECT TOP 1 Id FROM dbo.SetMeals WHERE SetMealName = N'情人節限定套餐');
DECLARE @CNYId       int = (SELECT TOP 1 Id FROM dbo.SetMeals WHERE SetMealName = N'過年限定套餐');
DECLARE @BizLunchId  int = (SELECT TOP 1 Id FROM dbo.SetMeals WHERE SetMealName = N'商務午餐套餐');
DECLARE @CoupleId    int = (SELECT TOP 1 Id FROM dbo.SetMeals WHERE SetMealName = N'歡樂雙人套餐');
DECLARE @AfternoonId int = (SELECT TOP 1 Id FROM dbo.SetMeals WHERE SetMealName = N'下午茶甜蜜套餐');

-- ========== 全家分享餐：主餐二選一 + 分享拼盤固定 ==========
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @FamilyId AND DishId = @TomatoId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@FamilyId, @TomatoId, 2, 1, 1, 1, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @FamilyId AND DishId = @CreamId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@FamilyId, @CreamId, 2, 1, 1, 1, 2);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @FamilyId AND DishId = @PlatterId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@FamilyId, @PlatterId, 1, 0, NULL, NULL, 3);

-- ========== 情人節限定套餐：主餐二選一 ==========
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @ValentineId AND DishId = @TomatoId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@ValentineId, @TomatoId, 1, 1, 1, 1, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @ValentineId AND DishId = @CreamId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@ValentineId, @CreamId, 1, 1, 1, 1, 2);

-- ========== 過年限定套餐：全部固定 ==========
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @CNYId AND DishId = @TomatoId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@CNYId, @TomatoId, 2, 0, NULL, NULL, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @CNYId AND DishId = @CreamId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@CNYId, @CreamId, 2, 0, NULL, NULL, 2);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @CNYId AND DishId = @PlatterId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@CNYId, @PlatterId, 1, 0, NULL, NULL, 3);

-- ========== 商務午餐套餐：主餐四選一 + 飲料三選一 ==========
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @BizLunchId AND DishId = @TomatoId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@BizLunchId, @TomatoId, 1, 1, 1, 1, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @BizLunchId AND DishId = @CreamId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@BizLunchId, @CreamId, 1, 1, 1, 1, 2);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @BizLunchId AND DishId = @KarageId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@BizLunchId, @KarageId, 1, 1, 1, 1, 3);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @BizLunchId AND DishId = @RedWineBeefId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@BizLunchId, @RedWineBeefId, 1, 1, 1, 1, 4);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @BizLunchId AND DishId = @ColaId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@BizLunchId, @ColaId, 1, 1, 2, 1, 5);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @BizLunchId AND DishId = @MilkTeaId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@BizLunchId, @MilkTeaId, 1, 1, 2, 1, 6);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @BizLunchId AND DishId = @AmericanoId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@BizLunchId, @AmericanoId, 1, 1, 2, 1, 7);

-- ========== 歡樂雙人套餐：主餐四選二 + 飲料四選二 + 提拉米蘇固定 ==========
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @CoupleId AND DishId = @ChickenLegId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@CoupleId, @ChickenLegId, 1, 1, 1, 2, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @CoupleId AND DishId = @SalmonId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@CoupleId, @SalmonId, 1, 1, 1, 2, 2);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @CoupleId AND DishId = @TomatoId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@CoupleId, @TomatoId, 1, 1, 1, 2, 3);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @CoupleId AND DishId = @CreamId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@CoupleId, @CreamId, 1, 1, 1, 2, 4);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @CoupleId AND DishId = @OrangeJuiceId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@CoupleId, @OrangeJuiceId, 1, 1, 2, 2, 5);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @CoupleId AND DishId = @MatchaLatteId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@CoupleId, @MatchaLatteId, 1, 1, 2, 2, 6);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @CoupleId AND DishId = @MilkTeaId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@CoupleId, @MilkTeaId, 1, 1, 2, 2, 7);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @CoupleId AND DishId = @ColaId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@CoupleId, @ColaId, 1, 1, 2, 2, 8);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @CoupleId AND DishId = @TiramisuId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@CoupleId, @TiramisuId, 1, 0, NULL, NULL, 9);

-- ========== 下午茶甜蜜套餐：甜點四選一 + 飲料四選一 ==========
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @AfternoonId AND DishId = @TiramisuId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@AfternoonId, @TiramisuId, 1, 1, 1, 1, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @AfternoonId AND DishId = @LavaChocId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@AfternoonId, @LavaChocId, 1, 1, 1, 1, 2);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @AfternoonId AND DishId = @CremeBruleeId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@AfternoonId, @CremeBruleeId, 1, 1, 1, 1, 3);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @AfternoonId AND DishId = @MilleCrepeId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@AfternoonId, @MilleCrepeId, 1, 1, 1, 1, 4);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @AfternoonId AND DishId = @MatchaLatteId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@AfternoonId, @MatchaLatteId, 1, 1, 2, 1, 5);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @AfternoonId AND DishId = @MilkTeaId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@AfternoonId, @MilkTeaId, 1, 1, 2, 1, 6);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @AfternoonId AND DishId = @AmericanoId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@AfternoonId, @AmericanoId, 1, 1, 2, 1, 7);
IF NOT EXISTS (SELECT 1 FROM dbo.SetMealItems WHERE SetMealId = @AfternoonId AND DishId = @OrangeJuiceId)
    INSERT INTO dbo.SetMealItems (SetMealId, DishId, Quantity, IsOptional, OptionGroupNo, PickLimit, DisplayOrder) VALUES (@AfternoonId, @OrangeJuiceId, 1, 1, 2, 1, 8);
GO
