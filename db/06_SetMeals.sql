-- 06_SetMeals.sql
-- Seed Data for SetMeals (追加模式，逐筆檢查避免重複)

IF NOT EXISTS (SELECT 1 FROM dbo.SetMeals WHERE SetMealName = N'全家分享餐')
    INSERT INTO dbo.SetMeals (SetMealName, DiscountType, DiscountValue, IsActive, CreatedAt, SetPrice, Description, ImageUrl, StartDate, EndDate, UpdatedAt)
    VALUES (N'全家分享餐', N'percent', 15.00, 1, GETDATE(), 699.00, N'適合4-6人共享的套餐', NULL, '2026-03-01', '2026-12-31', NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.SetMeals WHERE SetMealName = N'情人節限定套餐')
    INSERT INTO dbo.SetMeals (SetMealName, DiscountType, DiscountValue, IsActive, CreatedAt, SetPrice, Description, ImageUrl, StartDate, EndDate, UpdatedAt)
    VALUES (N'情人節限定套餐', N'percent', 20.00, 1, GETDATE(), 520.00, N'情人節浪漫限定套餐', NULL, '2026-02-01', '2026-02-14', NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.SetMeals WHERE SetMealName = N'過年限定套餐')
    INSERT INTO dbo.SetMeals (SetMealName, DiscountType, DiscountValue, IsActive, CreatedAt, SetPrice, Description, ImageUrl, StartDate, EndDate, UpdatedAt)
    VALUES (N'過年限定套餐', N'fixed', 100.00, 1, GETDATE(), 888.00, N'新年團圓限定套餐', NULL, '2026-01-20', '2026-02-10', NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.SetMeals WHERE SetMealName = N'商務午餐套餐')
    INSERT INTO dbo.SetMeals (SetMealName, DiscountType, DiscountValue, IsActive, CreatedAt, SetPrice, Description, ImageUrl, UpdatedAt)
    VALUES (N'商務午餐套餐', N'percent', 10.00, 1, GETDATE(), 240.00, N'主餐搭配飲料，快速美味的商務午餐', NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.SetMeals WHERE SetMealName = N'歡樂雙人套餐')
    INSERT INTO dbo.SetMeals (SetMealName, DiscountType, DiscountValue, IsActive, CreatedAt, SetPrice, Description, ImageUrl, UpdatedAt)
    VALUES (N'歡樂雙人套餐', N'percent', 15.00, 1, GETDATE(), 580.00, N'兩人共享主餐、飲料與甜點的超值組合', NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.SetMeals WHERE SetMealName = N'下午茶甜蜜套餐')
    INSERT INTO dbo.SetMeals (SetMealName, DiscountType, DiscountValue, IsActive, CreatedAt, SetPrice, Description, ImageUrl, UpdatedAt)
    VALUES (N'下午茶甜蜜套餐', N'percent', 10.00, 1, GETDATE(), 199.00, N'精選甜點搭配飲料，午後甜蜜時光', NULL, NULL);
GO
