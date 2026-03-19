USE EatTogetherDB;
GO

-- =============================================
-- 03_Users.sql
-- 說明：後台員工帳號資料（共 12 筆）
--
-- 預設密碼一律為 Aa000000（開發測試用）
-- 密碼以 BCrypt 雜湊儲存於 HashedPassword varchar(70)
-- 以下雜湊值為 Aa000000 經 BCrypt (cost=11) 產生的固定測試用雜湊
--
-- 員工編號規則：EMP + 年份 + 三位流水號
--   例：EMP2023001 = 2023 年到職第 1 位員工
--
-- 角色對照（UserRoles 請見 13_UserRoles.sql）：
--   1 = 店長  2 = 副店長  3 = 收銀員
--   4 = 外場服務生  5 = 內場廚師  6 = 工讀生
--
-- 修改紀錄：
--   - 欄位 Password 更名為 HashedPassword（varchar(70)，BCrypt 格式）
--   - 移除 TerminationDate、DeletedAt（新版規格已移除此兩欄）
--   - 補上 MustChangePassword 欄位（seed 資料預設為 0，無需強制改密碼）
-- =============================================

SET IDENTITY_INSERT [dbo].[Users] ON;
GO

DELETE FROM [dbo].[Users] WHERE Id BETWEEN 1 AND 12;

INSERT INTO [dbo].[Users]
    (Id, Account, HashedPassword, EmployeeNumber, Name, Email, Phone, HireDate, CreatedAt, IsActive, IsDeleted, MustChangePassword)
VALUES
-- 店長
( 1,
  'manager_amy',
  '$2a$12$sRRXTcuRbA.gUdoHFgZV8.0N8tga/I7mr5UYJWVCWDEKMEv0SEkCa',
  'EMP2021001',
  N'陳怡伶',
  'chen.amy.jap@gmail.com',
  '0912345678',
  '2021-03-01',
  '2021-03-01 09:32:05',
  1, 0, 0),

-- 副店長
( 2,
  'vicemgr_lin',
  '$2a$12$WnctN7pCf5wNAx.4.eRcuuZyExLsVpV7KFklo8xZTZHr.8oqTAV7i',
  'EMP2021002',
  N'林雅婷',
  'vicemgr_lin@eatogether.com',
  '0923456789',
  '2021-06-15',
  '2021-06-15 13:59:32',
  1, 0, 0),

-- 收銀員 1
( 3,
  'cashier_wang',
  '$2a$12$GIapMW9YUJUNuDvbt45I7.vaGq37vNRJX3T1M3790AdyZXRgJbQi.',
  'EMP2022001',
  N'王俊傑',
  'cashier_wang@eatogether.com',
  '0934567890',
  '2022-02-10',
  '2022-02-10 14:25:48',
  1, 0, 0),

-- 收銀員 2
( 4,
  'cashier_huang',
  '$2a$12$2W2kuK3kyK416pqZcfyBXuFOP30W7D7ShqwJnCQDiZzMMLjncXHN6',
  'EMP2022002',
  N'黃淑芬',
  'cashier_huang@eatogether.com',
  '0945678901',
  '2022-05-20',
  '2022-05-20 10:12:53',
  1, 0, 0),

-- 外場服務生 1
( 5,
  'waiter_liu',
  '$2a$12$ROVQzNc9XrIhZvSIsB/uWust4r64gefXHwsBGtNwaOmmSiBUzIpTS',
  'EMP2022003',
  N'劉建宏',
  'waiter_liu@eatogether.com',
  '0956789012',
  '2022-08-01',
  '2022-08-01 15:35:49',
  1, 0, 0),

-- 外場服務生 2
( 6,
  'waiter_wu',
  '$2a$12$yOfQ8w3S9rlrrP9HBlxICu4Uh91MgJLXdjRgU8fjjWHH2xJo76AhC',
  'EMP2023001',
  N'吳佳穎',
  'waiter_wu@eatogether.com',
  '0967890123',
  '2023-01-10',
  '2023-01-10 18:29:56',
  1, 0, 0),

-- 內場廚師 1（主廚）
( 7,
  'chef_zhang',
  '$2a$12$/KYereDn/UYpn4VF6VRDIeIPiea5xsFRicvuDn942uwWR5TwWz5pa',
  'EMP2021003',
  N'張義大',
  'chef_zhang@eatogether.com',
  '0978901234',
  '2021-03-01',
  '2021-03-01 16:22:44',
  1, 0, 0),

-- 內場廚師 2（二廚）
( 8,
  'chef_li',
  '$2a$12$lkrIhUNJ.lO4cOryX3LNJ.OoNpVO7J7PTASeL3SYzdC1cteBxwTNG',
  'EMP2022004',
  N'李文彬',
  'chef_li@eatogether.com',
  '0989012345',
  '2022-03-15',
  '2022-03-15 12:36:29',
  1, 0, 0),

-- 工讀生 1（在職）
( 9,
  'part_cai',
  '$2a$12$XYlE1mDP1KyxLRTizcVxCOUW7BiQ9UzpAxOowauiFmasyrUwfdE7e',
  'EMP2024001',
  N'蔡宜庭',
  'part_cai@eatogether.com',
  '0910111213',
  '2024-07-01',
  '2024-07-01 08:05:32',
  1, 0, 0),

-- 工讀生 2（在職）
(10,
  'part_xu',
  '$2a$12$39kqAEgMBJdI5VhMozB7L./MP3jOCeJ.7/6QsZmiVg3jqNELC0yya',
  'EMP2024002',
  N'許家豪',
  'part_xu@eatogether.com',
  '0921222324',
  '2024-09-01',
  '2024-09-01 17:09:28',
  1, 0, 0),

-- 工讀生 3（目前請假，IsActive = 0）
(11,
  'part_zheng',
  '$2a$12$r9j91opkYVD8cE6v.iurSOHqIBfvzVJWD98oKTZMw60j0cVNFEUMS',
  'EMP2024003',
  N'鄭雨晴',
  'part_zheng@eatogether.com',
  '0932333435',
  '2024-10-01',
  '2024-10-01 13:17:46',
  0, 0, 0),

-- 前收銀員（已離職，軟刪除，IsDeleted = 1）
(12,
  'ex_cashier_su',
  '$2a$12$jlUJjQQv5JZ14oXgjAHonuK3rM3pT5POC/GxwQciH7mBmSgx0tFpO',
  'EMP2021004',
  N'蘇志豪',
  'ex_su@eatogether.com',
  '0943444546',
  '2021-09-01',
  '2021-09-01 11:23:57',
  0, 1, 0);

SET IDENTITY_INSERT [dbo].[Users] OFF;
GO
