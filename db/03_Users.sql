USE EatTogetherDB;
GO

-- =============================================
-- 03_Users.sql
-- 說明：後台員工帳號資料（共 12 筆）
--
-- 預設密碼一律為 Aa000000（開發測試用）
-- 密碼以 SHA2_256 雜湊儲存於 varbinary(128)
--
-- 員工編號規則：EMP + 年份 + 三位流水號
--   例：EMP2023001 = 2023 年到職第 1 位員工
--
-- 角色對照（UserRoles 請見 13_UserRoles.sql）：
--   1 = 店長  2 = 副店長  3 = 收銀員
--   4 = 外場服務生  5 = 內場廚師  6 = 工讀生
-- =============================================

SET IDENTITY_INSERT [dbo].[Users] ON;
GO

DELETE FROM [dbo].[Users] WHERE Id BETWEEN 1 AND 12;

INSERT INTO [dbo].[Users]
    (Id, Account, Password, EmployeeNumber, Name, Email, Phone, HireDate, TerminationDate, CreatedAt, IsActive, IsDeleted, DeletedAt)
VALUES
-- 店長
(1,
 'manager_chen',
 HASHBYTES('SHA2_256', 'Aa000000'),
 'EMP2021001',
 N'陳志明',
 'manager_chen@eatogether.com',
 '0912345678',
 '2021-03-01',
 NULL,
 '2021-03-01',
 1, 0, NULL),

-- 副店長
(2,
 'vicemgr_lin',
 HASHBYTES('SHA2_256', 'Aa000000'),
 'EMP2021002',
 N'林雅婷',
 'vicemgr_lin@eatogether.com',
 '0923456789',
 '2021-06-15',
 NULL,
 '2021-06-15',
 1, 0, NULL),

-- 收銀員 1
(3,
 'cashier_wang',
 HASHBYTES('SHA2_256', 'Aa000000'),
 'EMP2022001',
 N'王俊傑',
 'cashier_wang@eatogether.com',
 '0934567890',
 '2022-02-10',
 NULL,
 '2022-02-10',
 1, 0, NULL),

-- 收銀員 2
(4,
 'cashier_huang',
 HASHBYTES('SHA2_256', 'Aa000000'),
 'EMP2022002',
 N'黃淑芬',
 'cashier_huang@eatogether.com',
 '0945678901',
 '2022-05-20',
 NULL,
 '2022-05-20',
 1, 0, NULL),

-- 外場服務生 1
(5,
 'waiter_liu',
 HASHBYTES('SHA2_256', 'Aa000000'),
 'EMP2022003',
 N'劉建宏',
 'waiter_liu@eatogether.com',
 '0956789012',
 '2022-08-01',
 NULL,
 '2022-08-01',
 1, 0, NULL),

-- 外場服務生 2
(6,
 'waiter_wu',
 HASHBYTES('SHA2_256', 'Aa000000'),
 'EMP2023001',
 N'吳佳穎',
 'waiter_wu@eatogether.com',
 '0967890123',
 '2023-01-10',
 NULL,
 '2023-01-10',
 1, 0, NULL),

-- 內場廚師 1（主廚）
(7,
 'chef_zhang',
 HASHBYTES('SHA2_256', 'Aa000000'),
 'EMP2021003',
 N'張義大',
 'chef_zhang@eatogether.com',
 '0978901234',
 '2021-03-01',
 NULL,
 '2021-03-01',
 1, 0, NULL),

-- 內場廚師 2（二廚）
(8,
 'chef_li',
 HASHBYTES('SHA2_256', 'Aa000000'),
 'EMP2022004',
 N'李文彬',
 'chef_li@eatogether.com',
 '0989012345',
 '2022-03-15',
 NULL,
 '2022-03-15',
 1, 0, NULL),

-- 工讀生 1（在職）
(9,
 'part_cai',
 HASHBYTES('SHA2_256', 'Aa000000'),
 'EMP2024001',
 N'蔡宜庭',
 'part_cai@eatogether.com',
 '0910111213',
 '2024-07-01',
 NULL,
 '2024-07-01',
 1, 0, NULL),

-- 工讀生 2（在職）
(10,
 'part_xu',
 HASHBYTES('SHA2_256', 'Aa000000'),
 'EMP2024002',
 N'許家豪',
 'part_xu@eatogether.com',
 '0921222324',
 '2024-09-01',
 NULL,
 '2024-09-01',
 1, 0, NULL),

-- 工讀生 3（目前請假，IsActive = 0）
(11,
 'part_zheng',
 HASHBYTES('SHA2_256', 'Aa000000'),
 'EMP2024003',
 N'鄭雨晴',
 'part_zheng@eatogether.com',
 '0932333435',
 '2024-10-01',
 NULL,
 '2024-10-01',
 0, 0, NULL),

-- 前收銀員（已離職，軟刪除）
(12,
 'ex_cashier_su',
 HASHBYTES('SHA2_256', 'Aa000000'),
 'EMP2021004',
 N'蘇志豪',
 'ex_su@eatogether.com',
 '0943444546',
 '2021-09-01',
 '2023-12-31',
 '2021-09-01',
 0, 1, '2023-12-31');

SET IDENTITY_INSERT [dbo].[Users] OFF;
GO
