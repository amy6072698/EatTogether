-- =============================================
-- 27_OrderDetails.sql
-- 說明：將有效預約明細轉入正式成交明細表
-- =============================================

USE EatTogetherDB;
GO

-- 根據正式 Orders 表的紀錄轉入明細
INSERT INTO [dbo].[OrderDetails] 
(
    [OrderId], 
    [ProductId], 
    [ProductName], 
    [UnitPrice], 
    [Qty], 
    [SubTotal]
)
SELECT 
    O.[Id],            -- 指向正式 Orders 表的自增 Id
    PD.[ProductId], 
    PD.[ProductName], 
    PD.[UnitPrice], 
    PD.[Qty], 
    PD.[SubTotal]
FROM [dbo].[PreOrderDetails] AS PD
INNER JOIN [dbo].[Orders] AS O ON PD.[PreOrderId] = O.[PreOrderId]
WHERE PD.[DoneOrCancel] = 1; -- 只轉入有效(完成或待作)的餐點，排除已取消(2)的餐點
GO

USE EatTogetherDB;
GO

-- 1. 設定今天日期
DECLARE @Today DATETIME = '2026-03-20'; 

-- 建立暫存表來存取這批新生成的 Id，以便後續插入明細
DECLARE @InsertedOrders TABLE (OrderId INT, RowNum INT IDENTITY(1,1));

-- 2. 插入 10 筆 PreOrders (OrderNumber 從 0015 開始)
-- InOrOut: 0 為內用, 1 為外帶
-- DoneOrCancel: 0 為準備中
INSERT INTO [dbo].[PreOrders] 
    ([OrderNumber], [MemberId], [InOrOut], [TableId], [UserId], [OrderAt], 
     [OriginalAmount], [DiscountAmount], [TotalAmount], [DoneOrCancel], 
     [PeopleNum], [PayMethod])
OUTPUT INSERTED.Id INTO @InsertedOrders(OrderId)
VALUES
    -- 內用組 (5筆，對應桌位 A1-E1)
    (N'20260320-0015', NULL, 1, 2,    NULL, @Today + ' 10:00:00', 500, 0, 500, 0, 2, N'Cash'),
    (N'20260320-0016', NULL, 1, 5,    NULL, @Today + ' 10:15:00', 320, 0, 320, 0, 2, N'Cash'),
    (N'20260320-0017', NULL, 1, 7,    NULL, @Today + ' 10:30:00', 899, 0, 899, 0, 4, N'Cash'),
    (N'20260320-0018', NULL, 1, 12,    NULL, @Today + ' 10:45:00', 450, 0, 450, 0, 2, N'Cash'),
    (N'20260320-0019', NULL, 1, 15,    NULL, @Today + ' 11:00:00', 200, 0, 200, 0, 1, N'Cash'),
    -- 外帶組 (5筆)
    (N'20260320-0020', NULL, 0, NULL, NULL, @Today + ' 11:10:00', 180, 0, 180, 0, NULL, N'Cash'),
    (N'20260320-0021', NULL, 0, NULL, NULL, @Today + ' 11:20:00', 699, 0, 699, 0, NULL, N'Cash'),
    (N'20260320-0022', NULL, 0, NULL, NULL, @Today + ' 11:30:00', 350, 0, 350, 0, NULL, N'Cash'),
    (N'20260320-0023', NULL, 0, NULL, NULL, @Today + ' 11:40:00', 420, 0, 420, 0, NULL, N'Cash'),
    (N'20260320-0024', NULL, 0, NULL, NULL, @Today + ' 11:50:00', 150, 0, 150, 0, NULL, N'Cash');

-- 3. 插入 PreOrderDetails (明細檔)
INSERT INTO [dbo].[PreOrderDetails] 
    ([PreOrderId], [ProductId], [ProductName], [UnitPrice], [Qty], [SubTotal], [DoneOrCancel])
SELECT OrderId, 1,  N'義式番茄義大利麵', 180, 1, 180, 0 FROM @InsertedOrders WHERE RowNum = 1
UNION ALL
SELECT OrderId, 3,  N'分享拼盤', 320, 1, 320, 0 FROM @InsertedOrders WHERE RowNum = 2
UNION ALL
SELECT OrderId, 60, N'全家分享餐', 899, 1, 899, 0 FROM @InsertedOrders WHERE RowNum = 3
UNION ALL
SELECT OrderId, 5,  N'青醬蛤蜊麵', 250, 1, 250, 0 FROM @InsertedOrders WHERE RowNum = 4
UNION ALL
SELECT OrderId, 10, N'經典凱薩沙拉', 200, 1, 200, 0 FROM @InsertedOrders WHERE RowNum = 5
UNION ALL
SELECT OrderId, 1,  N'義式番茄義大利麵', 180, 1, 180, 0 FROM @InsertedOrders WHERE RowNum = 6
UNION ALL
SELECT OrderId, 60, N'全家分享餐', 699, 1, 699, 0 FROM @InsertedOrders WHERE RowNum = 7
UNION ALL
SELECT OrderId, 2,  N'奶油培根燉飯', 350, 1, 350, 0 FROM @InsertedOrders WHERE RowNum = 8
UNION ALL
SELECT OrderId, 4,  N'瑪格麗特披薩', 420, 1, 420, 0 FROM @InsertedOrders WHERE RowNum = 9
UNION ALL
SELECT OrderId, 20, N'今日湯品', 150, 1, 150, 0 FROM @InsertedOrders WHERE RowNum = 10;