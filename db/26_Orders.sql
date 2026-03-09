-- =============================================
-- 26_Orders.sql
-- 說明：將已完成支付且狀態正常的 PreOrders 轉入正式 Orders 表
-- =============================================

USE EatTogetherDB;
GO

-- 轉入已完成的訂單
INSERT INTO [dbo].[Orders] 
(
    [PreOrderId], 
    [OrderNumber], 
    [MemberId], 
    [InOrOut], 
    [TableId], 
    [UserId], 
    [OrderAt], 
    [CouponId], 
    [OriginalAmount], 
    [DiscountAmount], 
    [TotalAmount], 
    [Note], 
    [PaymentId], 
    [PayMethod]
)
SELECT 
    P.[Id],            -- PreOrderId
    P.[OrderNumber], 
    P.[MemberId], 
    P.[InOrOut], 
    P.[TableId], 
    P.[UserId], 
    P.[OrderAt], 
    P.[CouponId], 
    P.[OriginalAmount], 
    P.[DiscountAmount], 
    P.[TotalAmount], 
    P.[Note], 
    P.[PaymentId], 
    P.[PayMethod]
FROM [dbo].[PreOrders] AS P
INNER JOIN [dbo].[Payments] AS PY ON P.[PaymentId] = PY.[Id]
WHERE P.[DoneOrCancel] = 1  -- 僅選取主單完成者
  AND PY.[DoneOrCancel] = 1; -- 僅選取支付成功者
GO

UPDATE PY
SET PY.[OrderId] = O.[Id]
FROM [dbo].[Payments] AS PY
INNER JOIN [dbo].[Orders] AS O ON PY.[PreOrderId] = O.[PreOrderId]
WHERE O.[PaymentId] = PY.[Id];  -- 確保只更新對應的付款紀錄
GO