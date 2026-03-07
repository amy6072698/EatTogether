-- 05_Categories.sql
-- Seed data only (table schema is managed by CreateDatabase.sql)

IF NOT EXISTS (SELECT 1 FROM dbo.Categories)
BEGIN
    INSERT INTO dbo.Categories
    (
        CategoryName,
        IsActive,
        CreatedAt,
        ParentCategoryId,
        DisplayOrder,
        ImageUrl,
        UpdatedAt
    )
    VALUES
    (N'主餐', 1, GETDATE(), NULL, 1, NULL, NULL),
    (N'飲料', 1, GETDATE(), NULL, 2, NULL, NULL),
    (N'甜點', 1, GETDATE(), NULL, 3, NULL, NULL);
END
GO