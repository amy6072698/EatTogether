-- 06_SetMeals.sql
-- Seed Data for SetMeals (符合 CK_SetMeals_DiscountType: 'percent' or 'fixed')

IF NOT EXISTS (SELECT 1 FROM dbo.SetMeals)
BEGIN
    INSERT INTO dbo.SetMeals
    (
        SetMealName,
        DiscountType,
        DiscountValue,
        IsActive,
        CreatedAt,
        SetPrice,
        Description,
        ImageUrl,
        UpdatedAt
    )
    VALUES
    (N'全家分享餐',     N'percent', 15.00, 1, GETDATE(), 699.00, N'適合4-6人共享的套餐', NULL, NULL),
    (N'情人節限定套餐', N'percent', 20.00, 1, GETDATE(), 520.00, N'情人節浪漫限定套餐', NULL, NULL),
    (N'過年限定套餐',   N'fixed',  100.00, 1, GETDATE(), 888.00, N'新年團圓限定套餐', NULL, NULL);
END
GO