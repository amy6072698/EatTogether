-- =============================================
-- 25_UpdatePreOrdersWithPaymentId.sql
-- 說明：將 Payments 表生成的 Id 回填至 PreOrders 表
-- =============================================

USE EatTogetherDB;
GO

-- =============================================
-- 步驟 4：將 Payments 生成的 Id 回填至 PreOrders.PaymentId
-- =============================================
UPDATE P
SET P.[PaymentId] = PY.[Id]
FROM [dbo].[PreOrders] AS P
INNER JOIN [dbo].[Payments] AS PY ON P.[Id] = PY.[PreOrderId];