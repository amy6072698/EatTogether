USE EatTogetherDB;
GO

-- =============================================
-- 01_Roles.sql
-- =============================================
-- 開啟手動插入 ID 功能 (為了確保大家的 ID 一致)
SET IDENTITY_INSERT [dbo].[Roles] ON;
GO

-- 清除舊資料 (開發階段用，避免重複 Insert 報錯)
-- 注意：若 Roles 已經被 UserRoles 引用，這行 DELETE 會失敗，請視情況使用
DELETE FROM Roles WHERE Id BETWEEN 1 AND 6;
GO

INSERT INTO [dbo].[Roles] (Id, RoleName, Description, CreatedAt)
VALUES
(1, N'店長',     N'最高權限，可使用所有後台功能',               GETDATE()),
(2, N'副店長',   N'協助店長管理，可使用大部分後台功能',          GETDATE()),
(3, N'收銀員',   N'負責結帳，可核驗優惠券及查看會員', GETDATE()),
(4, N'外場服務生', N'負責外場服務，可處理訂位及更新訂單狀態',   GETDATE()),
(5, N'內場廚師', N'負責備餐，可查看並更新訂單狀態',         GETDATE()),
(6, N'工讀生',   N'基層人員，僅可更新訂單狀態',                 GETDATE());

-- 關閉手動插入 ID 功能
SET IDENTITY_INSERT [dbo].[Roles] OFF;
GO