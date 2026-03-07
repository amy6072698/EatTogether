USE EatTogetherDB;
GO
-- =============================================
-- 07_Tables.sql
-- =============================================

SET IDENTITY_INSERT [dbo].[Tables] ON;
GO

-- 建立 14 張桌子：包含A雙人桌4張、B四人桌5張、C六人桌3張與VIP十人包廂2個
INSERT INTO [dbo].[Tables] ([Id], [TableName], [SeatCount]) VALUES 
(1, N'A1', 2),
(2, N'A2', 2),
(3, N'A3', 2),
(4, N'A4', 2),
(5, N'B1', 4),
(6, N'B2', 4),
(7, N'B3', 4),
(8, N'B4', 4),
(9, N'B5', 4),
(10, N'C1', 6),
(11, N'C2', 6),
(12, N'C3', 6),
(13, N'VIP1', 10),
(14, N'VIP2', 10);

SET IDENTITY_INSERT [dbo].[Tables] OFF;
GO