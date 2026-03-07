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