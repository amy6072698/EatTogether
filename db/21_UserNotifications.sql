USE [EatTogetherDB]
GO

SET IDENTITY_INSERT [dbo].[UserNotifications] ON;
GO

INSERT INTO [dbo].[UserNotifications] ([Id], [MemberId], [ArticleId], [Title], [IsRead], [CreatedAt])
SELECT 
    ROW_NUMBER() OVER (ORDER BY t.CreatedAt, m.Id) AS Id, 
    m.Id AS MemberId, 
    t.ArticleId, 
    t.Title, 
    t.IsRead, 
    t.CreatedAt
FROM (
    SELECT 1 AS Id UNION SELECT 2 UNION SELECT 3 UNION SELECT 4 UNION SELECT 5 UNION 
    SELECT 7 UNION SELECT 8 UNION SELECT 11 UNION SELECT 12 UNION SELECT 15 UNION 
    SELECT 20 UNION SELECT 21 UNION SELECT 27 UNION SELECT 33 UNION SELECT 38 UNION 
    SELECT 42 UNION SELECT 50 UNION SELECT 60 UNION SELECT 66
) AS m
CROSS JOIN (
    SELECT 9 AS ArticleId, N'親愛的會員，【新品】全新「全家分享餐」澎湃上市！聚餐首選推薦' AS Title, 1 AS IsRead, '2026-02-21 10:00:00' AS CreatedAt
    UNION ALL
    SELECT 11, N'親愛的會員，【報導】《食尚指南》評選義起吃為全台必吃餐廳', 1, '2026-03-15 09:30:00'
    UNION ALL
    SELECT 1, N'親愛的會員，【活動】五月寵愛母親節，全桌滿額享 85 折優惠公告', 0, '2026-05-01 09:00:00'
    UNION ALL
    SELECT 5, N'親愛的會員，【限定】春末松露慶典，松露薯條滿額招待中！', 0, '2026-05-02 10:00:00'
    UNION ALL
    SELECT 3, N'親愛的會員，【公告】義起吃 5/20 員工旅遊暫停營業一日', 0, '2026-05-11 09:00:00'
) AS t;

SET IDENTITY_INSERT [dbo].[UserNotifications] OFF;
GO