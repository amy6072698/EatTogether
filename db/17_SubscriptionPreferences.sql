USE EatTogetherDB;
GO

-- =============================================
-- 17_SubscriptionPreferences.sql
-- =============================================

SET IDENTITY_INSERT [dbo].[SubscriptionPreferences] ON;
GO

INSERT INTO [dbo].[SubscriptionPreferences] ([Id], [MemberId], [IsEmailSubscribed], [IsPushSubscribed], [UpdatedAt])
VALUES
(1, 1, 1, 1, GETDATE()), (2, 2, 1, 1, GETDATE()), (3, 3, 1, 1, GETDATE()), 
(4, 4, 1, 1, GETDATE()), (5, 5, 1, 1, GETDATE()), (6, 6, 1, 0, GETDATE()), 
(7, 7, 0, 1, GETDATE()), (8, 8, 1, 1, GETDATE()), (9, 9, 1, 0, GETDATE()), 
(10, 10, 0, 0, GETDATE()), (11, 11, 1, 1, GETDATE()), (12, 12, 0, 1, GETDATE()), 
(13, 15, 1, 1, GETDATE()), (14, 18, 1, 0, GETDATE()), (15, 20, 0, 1, GETDATE()), 
(16, 21, 1, 1, GETDATE()), (17, 25, 1, 0, GETDATE()), (18, 27, 0, 1, GETDATE()), 
(19, 33, 1, 1, GETDATE()), (20, 38, 1, 1, GETDATE()), (21, 42, 0, 1, GETDATE()), 
(22, 45, 1, 0, GETDATE()), (23, 50, 1, 1, GETDATE()), (24, 55, 1, 0, GETDATE()), 
(25, 60, 0, 1, GETDATE()), (26, 66, 1, 1, GETDATE()), (27, 29, 0, 0, GETDATE()), 
(28, 30, 0, 0, GETDATE()), (29, 67, 0, 0, GETDATE()), (30, 68, 0, 0, GETDATE()), 
(31, 69, 0, 0, GETDATE()), (32, 72, 0, 0, GETDATE());

SET IDENTITY_INSERT [dbo].[SubscriptionPreferences] OFF;
GO