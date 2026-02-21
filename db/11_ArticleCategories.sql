USE [EatTogetherDB]
GO

-- =============================================
-- 11_ArticleCategories.sql
-- =============================================

SET IDENTITY_INSERT [dbo].[ArticleCategories] ON 
GO

INSERT INTO [dbo].[ArticleCategories] ([Id], [Name], [SortOrder], [IsEnabled])
VALUES 
(1, N'活動介紹', 1, 1),
(2, N'餐廳公告', 2, 1),
(3, N'季節限定', 3, 1),
(4, N'品牌故事', 4, 1),
(5, N'新品上市', 5, 1),
(6, N'媒體報導', 6, 1);

GO
SET IDENTITY_INSERT [dbo].[ArticleCategories] OFF
GO