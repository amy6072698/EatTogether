USE EatTogetherDB;
GO

-- =============================================
-- 02_Functions.sql
-- 更新紀錄：
--   - 新增 Id=2「員工查看」(Staff_View)，原 Id 2~12 後移為 3~13
--   - Id=1「員工管理」補上 IsOwnerOnly = 1（⚠️ 僅限店長）
--   - Id=12「報表管理」補上 IsOwnerOnly = 1（⚠️ 僅限店長）
-- =============================================

SET IDENTITY_INSERT [dbo].[Functions] ON;
GO

DELETE FROM [dbo].[Functions] WHERE Id IN (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13);

INSERT INTO [dbo].[Functions] (Id, Category, FunctionName, DisplayName, Description, IsOwnerOnly)
VALUES
( 1, N'人員管理', 'Staff_Manage',        N'員工管理',           N'員工帳號的新增、修改、停用及角色權限指派',               1),  -- ⚠️ 僅限店長
( 2, N'人員管理', 'Staff_View',          N'員工查看',           N'查看員工列表及基本資料，不含任何修改操作',               0),  -- 新增
( 3, N'人員管理', 'Member_Manage',       N'會員管理',           N'查看會員資料、設定黑名單',                               0),
( 4, N'訂單管理', 'Order_Manage',        N'訂單管理（含結帳）',  N'修改、取消訂單及執行結帳，涉及金流操作',                0),
( 5, N'訂單管理', 'Order_StatusUpdate',  N'更新訂單狀態',       N'查看訂單清單並更新訂單狀態，不含結帳',                   0),
( 6, N'菜單管理', 'Menu_Manage',         N'菜單管理',           N'餐點、分類、套餐的新增、修改、上下架',                   0),
( 7, N'訂位管理', 'Reservation_Manage',  N'訂位管理',           N'訂位的新增、修改、報到與取消',                           0),
( 8, N'訂位管理', 'Table_Manage',        N'桌位設定',           N'新增與修改桌位資料（桌名、座位數）',                     0),
( 9, N'行銷管理', 'Coupon_Manage',       N'優惠券管理',         N'建立與修改優惠券設定',                                   0),
(10, N'行銷管理', 'Event_Manage',        N'活動管理',           N'建立與管理行銷活動',                                     0),
(11, N'行銷管理', 'Article_Manage',      N'文章管理',           N'發布與管理最新消息、品牌文章',                           0),
(12, N'報表管理', 'Report_Manage',       N'報表管理',           N'查看營業額、淨收入、熱銷排行等所有報表',                 1),  -- ⚠️ 僅限店長
(13, N'行銷管理', 'Notification_Manage', N'通知管理',           N'管理會員小鈴鐺通知及郵件發送隊列',                       0);

SET IDENTITY_INSERT [dbo].[Functions] OFF;
GO
