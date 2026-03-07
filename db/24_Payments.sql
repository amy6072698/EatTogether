-- =============================================
-- 24_Payments.sql
-- 說明：根據 PreOrders 生成支付紀錄
-- 狀態碼：0 = 未完成(支付失敗/待付), 1 = 已完成(已入帳), 2 = 已取消(已退款)
-- =============================================

USE EatTogetherDB;
GO

-- 2026-03-01 支付紀錄 (對應 PreOrderId 1~10)
INSERT INTO [dbo].[Payments] ([PreOrderId], [OrderId], [Method], [PaidAt], [DoneOrCancel]) VALUES
(1,  NULL, N'Credit Card', '2026-03-01 11:32:05', 1),
(2,  NULL, N'Cash',        '2026-03-01 12:05:10', 1),
(3,  NULL, N'Line Pay',    '2026-03-01 12:18:22', 1),
(4,  NULL, N'Credit Card', '2026-03-01 17:31:00', 0), -- 未付款取消 (支付失敗/未付)
(5,  NULL, N'Cash',        '2026-03-01 18:02:45', 1),
(6,  NULL, N'Line Pay',    '2026-03-01 18:35:12', 1),
(7,  NULL, N'Credit Card', '2026-03-01 19:04:30', 2), -- 已付款但取消 (退款紀錄)
(8,  NULL, N'Cash',        '2026-03-01 19:18:55', 1),
(9,  NULL, N'Line Pay',    '2026-03-01 19:33:10', 1),
(10, NULL, N'Credit Card', '2026-03-01 20:02:15', 1);

-- 2026-03-02 支付紀錄 (對應 PreOrderId 11~20)
INSERT INTO [dbo].[Payments] ([PreOrderId], [OrderId], [Method], [PaidAt], [DoneOrCancel]) VALUES
(11, NULL, N'Cash',        '2026-03-02 11:35:00', 1),
(12, NULL, N'Line Pay',    '2026-03-02 11:48:30', 1),
(13, NULL, N'Credit Card', '2026-03-02 12:04:12', 1),
(14, NULL, N'Cash',        '2026-03-02 12:35:55', 1),
(15, NULL, N'Line Pay',    '2026-03-02 13:02:10', 0), -- 支付未完成
(16, NULL, N'Credit Card', '2026-03-02 18:05:00', 1),
(17, NULL, N'Cash',        '2026-03-02 18:32:18', 1),
(18, NULL, N'Line Pay',    '2026-03-02 18:49:40', 1),
(19, NULL, N'Credit Card', '2026-03-02 19:05:22', 1),
(20, NULL, N'Cash',        '2026-03-02 19:34:05', 2); -- 已付款但取消

-- 2026-03-03 支付紀錄 (對應 PreOrderId 21~30)
INSERT INTO [dbo].[Payments] ([PreOrderId], [OrderId], [Method], [PaidAt], [DoneOrCancel]) VALUES
(21, NULL, N'Line Pay',    '2026-03-03 11:35:00', 1),
(22, NULL, N'Credit Card', '2026-03-03 12:03:00', 1),
(23, NULL, N'Cash',        '2026-03-03 12:18:00', 1),
(24, NULL, N'Line Pay',    '2026-03-03 12:32:00', 0), -- 未完成
(25, NULL, N'Credit Card', '2026-03-03 13:05:00', 1),
(26, NULL, N'Cash',        '2026-03-03 18:05:00', 1),
(27, NULL, N'Line Pay',    '2026-03-03 18:35:00', 1),
(28, NULL, N'Credit Card', '2026-03-03 18:50:00', 1),
(29, NULL, N'Cash',        '2026-03-03 19:05:00', 2), -- 已退款
(30, NULL, N'Line Pay',    '2026-03-03 19:35:00', 1);

-- 2026-03-04 支付紀錄 (對應 PreOrderId 31~40)
INSERT INTO [dbo].[Payments] ([PreOrderId], [OrderId], [Method], [PaidAt], [DoneOrCancel]) VALUES
(31, NULL, N'Credit Card', '2026-03-04 11:35:00', 1),
(32, NULL, N'Cash',        '2026-03-04 12:05:00', 1),
(33, NULL, N'Line Pay',    '2026-03-04 12:18:00', 0), -- 失敗
(34, NULL, N'Credit Card', '2026-03-04 12:35:00', 1),
(35, NULL, N'Cash',        '2026-03-04 13:05:00', 1),
(36, NULL, N'Line Pay',    '2026-03-04 18:05:00', 1),
(37, NULL, N'Credit Card', '2026-03-04 18:35:00', 1),
(38, NULL, N'Cash',        '2026-03-04 18:50:00', 2), -- 已退款
(39, NULL, N'Line Pay',    '2026-03-04 19:05:00', 1),
(40, NULL, N'Credit Card', '2026-03-04 19:35:00', 1);

-- 2026-03-05 支付紀錄 (對應 PreOrderId 41~50)
INSERT INTO [dbo].[Payments] ([PreOrderId], [OrderId], [Method], [PaidAt], [DoneOrCancel]) VALUES
(41, NULL, N'Cash',        '2026-03-05 11:35:00', 1),
(42, NULL, N'Line Pay',    '2026-03-05 12:05:00', 1),
(43, NULL, N'Credit Card', '2026-03-05 12:18:00', 1),
(44, NULL, N'Cash',        '2026-03-05 12:35:00', 0), -- 未完成
(45, NULL, N'Line Pay',    '2026-03-05 13:05:00', 1),
(46, NULL, N'Credit Card', '2026-03-05 18:05:00', 1),
(47, NULL, N'Cash',        '2026-03-05 18:35:00', 1),
(48, NULL, N'Line Pay',    '2026-03-05 18:50:00', 2), -- 已退款
(49, NULL, N'Credit Card', '2026-03-05 19:05:00', 1),
(50, NULL, N'Cash',        '2026-03-05 19:35:00', 1);
GO