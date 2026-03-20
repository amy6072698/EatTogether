USE EatTogetherDB;
GO

-- =============================================
-- 04_Members.sql
-- 說明：前台會員資料（共 72 筆）
--
-- 預設密碼一律為 Aa000000（開發測試用）
-- 密碼以 BCrypt 雜湊儲存於 HashedPassword varchar(70)
-- 以下雜湊值為 Aa000000 經 BCrypt (cost=11) 產生的固定測試用雜湊
--
-- 資料涵蓋以下情境：
--   - 一般正常會員（大多數）
--   - 有填生日的會員（可收壽星優惠）
--   - 沒有填生日的會員（BirthDate = NULL）
--   - 被列入黑名單的會員（IsBlacklisted = 1，BlacklistReason 填入原因）
--   - 已自行刪除帳號的會員（IsDeleted = 1，DeletedAt 填入刪除時間）
--
-- 修改紀錄：
--   - 欄位 Password 更名為 HashedPassword（varchar(70)，BCrypt 格式）
--   - 補上 IsConfirmed 欄位（seed 資料預設為 1，視為已驗證）
--   - 補上 AvatarFileName 欄位（seed 資料全為 NULL）
--   - 補上 BlacklistReason 欄位（黑名單會員填入原因，其餘為 NULL）
-- =============================================

SET IDENTITY_INSERT [dbo].[Members] ON;
GO

DELETE FROM [dbo].[Members] WHERE Id BETWEEN 1 AND 72;

-- 欄位順序：
-- Id, Account, Name, Email, HashedPassword, Phone, BirthDate,
-- IsBlacklisted, CreatedAt, IsDeleted, DeletedAt,
-- IsConfirmed, AvatarFileName, BlacklistReason

INSERT INTO [dbo].[Members]
    (Id, Account, Name, Email, HashedPassword, Phone, BirthDate,
     IsBlacklisted, CreatedAt, IsDeleted, DeletedAt,
     IsConfirmed, AvatarFileName, BlacklistReason)
VALUES

-- ── 一般正常會員（有填生日）──────────────────────────────
( 1, 'amy_chen',    N'陳怡伶', 'amy.chen.eng@gmail.com',           '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0912111001', '1988-04-15', 0, '2023-01-08 15:09:37', 0, NULL, 1, NULL, NULL),
( 2, 'brian_lin',     N'鄭婷方', 'una06021209@gmail.com',             '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0923222002', '1995-07-22', 0, '2023-02-14 17:56:37', 0, NULL, 1, NULL, NULL),
( 3, 'cindy_wu99',    N'楊晴淳', 'Yang0005111@gmail.com',         '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0934333003', '1999-12-03', 0, '2023-03-05 12:41:22', 0, NULL, 1, NULL, NULL),
( 4, 'david_huang',   N'吳欣柔', 'rrr20118@gmail.com',         '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0945444004', '1990-02-28', 0, '2023-03-20 09:38:55', 0, NULL, 1, NULL, NULL),
( 5, 'ellen_zhang',   N'李燕芳', 'g2301149040@gmail.com',           '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0956555005', '1987-09-10', 0, '2023-04-01 18:59:11', 0, NULL, 1, NULL, NULL),
( 6, 'frank_liu77',   N'王美麗', 'amy.chenyiling@gmail.com',           '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0967666006', '1977-06-18', 0, '2023-04-18 16:04:34', 0, NULL, 1, NULL, NULL),
( 7, 'grace_xu',      N'許雅柔', 'grace.xu@gmail',              '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0978777007', '2001-03-25', 0, '2023-05-07 05:13:49', 0, NULL, 1, NULL, NULL),
( 8, 'henry_cai',     N'蔡明輝', 'henry.cai@gmail',             '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0989888008', '1983-11-14', 0, '2023-05-22 12:53:11', 0, NULL, 1, NULL, NULL),
( 9, 'iris_zheng',    N'鄭佳蓉', 'iris.zheng@yahoo.tw',         '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0910999009', '1993-08-08', 0, '2023-06-03 11:38:20', 0, NULL, 1, NULL, NULL),
(10, 'jason_yang',    N'楊俊豪', 'jason.yang@gmail',            '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0921000010', '1991-01-30', 0, '2023-06-15 22:47:05', 0, NULL, 1, NULL, NULL),
(11, 'karen_li',      N'李佳欣', 'karen.li@gmail',              '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0932111011', '1996-05-05', 0, '2023-07-01 16:20:49', 0, NULL, 1, NULL, NULL),
(12, 'leo_wang2023',  N'王俊廷', 'leo.wang2023@gmail',          '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0943222012', '1985-10-20', 0, '2023-07-19 14:01:42', 0, NULL, 1, NULL, NULL),
(13, 'mia_guo',       N'郭雅婷', 'mia.guo@gmail',               '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0954333013', '2000-02-14', 0, '2023-08-05 01:03:45', 0, NULL, 1, NULL, NULL),
(14, 'nick_fang',     N'方彥霖', 'nick.fang@hotmail',           '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0965444014', '1989-07-07', 0, '2023-08-28 12:36:06', 0, NULL, 1, NULL, NULL),
(15, 'olivia_tang',   N'唐詩涵', 'olivia.tang@gmail',           '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0976555015', '1997-04-19', 0, '2023-09-10 14:32:26', 0, NULL, 1, NULL, NULL),
(16, 'peter_luo',     N'羅建志', 'peter.luo@gmail',             '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0987666016', '1982-12-25', 0, '2023-09-25 02:14:55', 0, NULL, 1, NULL, NULL),
(17, 'queen_he',      N'何雅琪', 'queen.he@yahoo.tw',           '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0918777017', '1994-03-03', 0, '2023-10-08 21:21:15', 0, NULL, 1, NULL, NULL),
(18, 'ryan_xiao',     N'蕭志遠', 'ryan.xiao@gmail',             '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0929888018', '1998-09-09', 0, '2023-10-20 21:24:45', 0, NULL, 1, NULL, NULL),
(19, 'sally_jiang',   N'江淑君', 'sally.jiang@gmail',           '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0930999019', '1986-06-06', 0, '2023-11-02 18:03:54', 0, NULL, 1, NULL, NULL),
(20, 'tom_bai',       N'白承翰', 'tom.bai@gmail',               '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0941000020', '1992-08-16', 0, '2023-11-18 12:46:07', 0, NULL, 1, NULL, NULL),

-- ── 一般正常會員（未填生日）─────────────────────────────
(21, 'una_peng',      N'彭雅文', 'una.peng@gmail',              '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0952111021', NULL,         0, '2023-12-01 07:14:47', 0, NULL, 1, NULL, NULL),
(22, 'victor_song',   N'宋冠宇', 'victor.song@hotmail',         '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0963222022', NULL,         0, '2023-12-15 16:20:35', 0, NULL, 1, NULL, NULL),
(23, 'wendy_qiu',     N'邱欣妤', 'wendy.qiu@gmail',             '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0974333023', NULL,         0, '2024-01-05 23:22:26', 0, NULL, 1, NULL, NULL),
(24, 'xavier_shi',    N'施柏翰', 'xavier.shi@gmail',            '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0985444024', NULL,         0, '2024-01-20 08:33:09', 0, NULL, 1, NULL, NULL),
(25, 'yuki_mo',       N'莫亭萱', 'yuki.mo@yahoo.tw',            '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0916555025', NULL,         0, '2024-02-08 23:31:57', 0, NULL, 1, NULL, NULL),
(26, 'zack_hou',      N'侯家豪', 'zack.hou@gmail',              '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0927666026', NULL,         0, '2024-02-22 03:14:55', 0, NULL, 1, NULL, NULL),
(27, 'alice_kong',    N'孔雅馨', 'alice.kong@gmail',            '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0938777027', '2003-01-17', 0, '2024-03-10 22:31:57', 0, NULL, 1, NULL, NULL),
(28, 'ben_lu',        N'盧思翰', 'ben.lu@gmail',                '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0949888028', '1980-05-30', 0, '2024-03-28 09:26:35', 0, NULL, 1, NULL, NULL),

-- ── 黑名單會員（IsBlacklisted = 1）──────────────────────────
-- 29 號：曾大量訂位後爽約（NoShow），嚴重影響餐廳座位利用率
-- 30 號：結帳時多次使用無效優惠券，且態度惡劣
(29, 'bad_guy_wu',    N'吳大鬧', 'bad.wu@gmail',                '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0960999029', '1984-03-13', 1, '2023-08-01 02:28:47', 0, NULL, 1, NULL, N'多次大量訂位後爽約（NoShow），嚴重影響座位利用率'),
(30, 'trouble_chen',  N'陳麻煩', 'trouble.chen@hotmail',        '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0971000030', NULL,         1, '2023-10-05 22:26:37', 0, NULL, 1, NULL, N'多次結帳時使用無效優惠券，且態度惡劣'),

-- ── 已自行刪除帳號的會員（IsDeleted = 1）─────────────────
-- 31 號：主動申請刪除帳號（個資保護需求）
-- 32 號：長期未使用後申請刪除
(31, 'deleted_lin',   N'林小華', 'deleted.lin@gmail',           '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0982111031', '1990-11-11', 0, '2023-05-15 07:14:48', 1, '2024-01-10 02:34:05', 1, NULL, NULL),
(32, 'deleted_zhao',  N'趙大明', 'deleted.zhao@yahoo.tw',       '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0913222032', NULL,         0, '2022-11-20 14:25:53', 1, '2024-03-01 14:57:24', 1, NULL, NULL),

-- ── 一般正常會員（有填生日）──────────────────────────────
(33, 'chris_pan',     N'潘俊宇', 'chris.pan@gmail',             '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0912001033', '1993-02-14', 0, '2024-04-03 17:19:12', 0, NULL, 1, NULL, NULL),
(34, 'diana_mei',     N'梅雅琳', 'diana.mei@gmail',             '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0923002034', '1997-06-21', 0, '2024-04-10 07:21:39', 0, NULL, 1, NULL, NULL),
(35, 'eric_zhu',      N'朱建霖', 'eric.zhu@yahoo.tw',           '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0934003035', '1985-09-30', 0, '2024-04-18 09:46:52', 0, NULL, 1, NULL, NULL),
(36, 'fiona_cheng',   N'鄭芳宜', 'fiona.cheng@gmail',           '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0945004036', '2002-01-08', 0, '2024-04-25 21:17:53', 0, NULL, 1, NULL, NULL),
(37, 'gary_shen',     N'沈冠廷', 'gary.shen@hotmail',           '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0956005037', '1988-11-11', 0, '2024-05-02 15:58:33', 0, NULL, 1, NULL, NULL),
(38, 'hannah_fu',     N'傅欣妍', 'hannah.fu@gmail',             '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0967006038', '1995-03-17', 0, '2024-05-09 09:50:04', 0, NULL, 1, NULL, NULL),
(39, 'ivan_ye',       N'葉志豪', 'ivan.ye@gmail',               '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0978007039', '1991-07-04', 0, '2024-05-15 05:42:07', 0, NULL, 1, NULL, NULL),
(40, 'julia_xia',     N'夏佳穎', 'julia.xia@gmail',             '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0989008040', '1999-12-24', 0, '2024-05-22 23:29:07', 0, NULL, 1, NULL, NULL),
(41, 'kevin_mao',     N'毛俊賢', 'kevin.mao@yahoo.tw',          '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0910009041', '1986-04-02', 0, '2024-05-30 21:25:53', 0, NULL, 1, NULL, NULL),
(42, 'laura_gao',     N'高雅雯', 'laura.gao@gmail',             '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0921010042', '1994-08-19', 0, '2024-06-05 11:58:22', 0, NULL, 1, NULL, NULL),
(43, 'mike_tan',      N'譚建志', 'mike.tan@gmail',              '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0932011043', '1980-10-10', 0, '2024-06-12 14:25:50', 0, NULL, 1, NULL, NULL),
(44, 'nina_ding',     N'丁詩涵', 'nina.ding@hotmail',           '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0943012044', '2001-05-28', 0, '2024-06-20 15:37:28', 0, NULL, 1, NULL, NULL),
(45, 'oscar_wei',     N'魏志遠', 'oscar.wei@gmail',             '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0954013045', '1987-02-06', 0, '2024-06-28 13:12:47', 0, NULL, 1, NULL, NULL),
(46, 'penny_gu',      N'顧雅慧', 'penny.gu@gmail',              '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0965014046', '1996-09-15', 0, '2024-07-04 23:22:36', 0, NULL, 1, NULL, NULL),
(47, 'quinn_dai',     N'戴承翰', 'quinn.dai@yahoo.tw',          '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0976015047', '1990-06-23', 0, '2024-07-11 21:27:48', 0, NULL, 1, NULL, NULL),
(48, 'rachel_shao',   N'邵若涵', 'rachel.shao@gmail',           '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0987016048', '1998-03-31', 0, '2024-07-18 15:37:54', 0, NULL, 1, NULL, NULL),
(49, 'steven_wen',    N'溫柏宏', 'steven.wen@gmail',            '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0918017049', '1983-01-20', 0, '2024-07-25 18:22:39', 0, NULL, 1, NULL, NULL),
(50, 'tina_yu',       N'于欣怡', 'tina.yu@gmail',               '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0929018050', '2000-07-07', 0, '2024-08-01 22:34:04', 0, NULL, 1, NULL, NULL),
(51, 'uncle_jin',     N'靳大為', 'uncle.jin@hotmail',           '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0930019051', '1975-11-03', 0, '2024-08-08 08:33:14', 0, NULL, 1, NULL, NULL),
(52, 'vera_cui',      N'崔雅馨', 'vera.cui@gmail',              '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0941020052', '1992-04-14', 0, '2024-08-15 01:10:40', 0, NULL, 1, NULL, NULL),
(53, 'will_liang',    N'梁思翰', 'will.liang@gmail',            '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0952021053', '1989-08-08', 0, '2024-08-22 23:47:46', 0, NULL, 1, NULL, NULL),
(54, 'xena_kong',     N'孔佳蓉', 'xena.kong@yahoo.tw',          '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0963022054', '2003-06-16', 0, '2024-08-29 02:05:45', 0, NULL, 1, NULL, NULL),
(55, 'yale_shi2',     N'施彥廷', 'yale.shi2@gmail',             '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0974023055', '1984-12-01', 0, '2024-09-05 08:27:08', 0, NULL, 1, NULL, NULL),
(56, 'zoe_long',      N'龍雅柔', 'zoe.long@gmail',              '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0985024056', '1997-10-09', 0, '2024-09-12 17:01:58', 0, NULL, 1, NULL, NULL),

-- ── 一般正常會員（未填生日）─────────────────────────────
(57, 'alan_pei',      N'裴冠宇', 'alan.pei@gmail',              '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0916025057', NULL,         0, '2024-09-19 17:50:36', 0, NULL, 1, NULL, NULL),
(58, 'bella_qin',     N'秦美玲', 'bella.qin@hotmail',           '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0927026058', NULL,         0, '2024-09-26 19:44:28', 0, NULL, 1, NULL, NULL),
(59, 'carl_xing',     N'邢志宏', 'carl.xing@gmail',             '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0938027059', NULL,         0, '2024-10-03 06:21:43', 0, NULL, 1, NULL, NULL),
(60, 'daisy_yin',     N'尹淑君', 'daisy.yin@gmail',             '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0949028060', NULL,         0, '2024-10-10 02:10:33', 0, NULL, 1, NULL, NULL),
(61, 'evan_leng',     N'冷明輝', 'evan.leng@yahoo.tw',          '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0950029061', NULL,         0, '2024-10-17 17:10:59', 0, NULL, 1, NULL, NULL),

-- ── 一般正常會員（未驗證）─────────────────────────────
(62, 'faith_bian',    N'邊雅婷', 'faith.bian@gmail',            '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0961030062', NULL,         0, '2024-10-24 12:22:45', 0, NULL, 0, NULL, NULL),
(63, 'glen_chu',      N'褚俊霖', 'glen.chu@gmail',              '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0972031063', NULL,         0, '2024-10-31 13:13:57', 0, NULL, 0, NULL, NULL),
(64, 'helen_ruan',    N'阮詩涵', 'helen.ruan@hotmail',          '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0983032064', NULL,         0, '2024-11-07 14:54:59', 0, NULL, 0, NULL, NULL),
(65, 'ian_meng',      N'孟建志', 'ian.meng@gmail',              '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0914033065', NULL,         0, '2024-11-14 16:24:36', 0, NULL, 0, NULL, NULL),
(66, 'jenny_ao',      N'敖欣妤', 'jenny.ao@gmail',              '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0925034066', NULL,         0, '2024-11-21 11:21:26', 0, NULL, 0, NULL, NULL),

-- ── 黑名單會員（IsBlacklisted = 1）──────────────────────────
-- 67 號：多次在店內大聲喧嘩，影響其他客人用餐體驗，經警告後仍無改善
-- 68 號：曾以假帳號重複領取新會員優惠券，詐取優惠
(67, 'noisy_liao',    N'廖大聲', 'noisy.liao@gmail',            '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0936035067', '1979-05-05', 1, '2024-03-15 17:50:28', 0, NULL, 1, NULL, N'多次在店內大聲喧嘩，影響其他客人用餐體驗，經警告後仍無改善'),
(68, 'fraud_tong',    N'童詐欺', 'fraud.tong@hotmail',          '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0947036068', NULL,         1, '2024-06-01 12:24:57', 0, NULL, 1, NULL, N'以假帳號重複領取新會員優惠券，詐取優惠'),

-- ── 已自行刪除帳號的會員（IsDeleted = 1）─────────────────
-- 69 號：搬家後不再常來，主動申請刪除
-- 70 號：隱私考量申請刪除
-- 71 號：重複註冊後刪除舊帳號
-- 72 號：試用後決定不再使用
(69, 'deleted_niu',   N'牛志偉', 'deleted.niu@gmail',           '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0958037069', '1988-07-17', 0, '2023-04-20 20:02:41', 1, '2024-08-10 15:56:06', 1, NULL, NULL),
(70, 'deleted_bao',   N'鮑雅雯', 'deleted.bao@yahoo.tw',        '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0969038070', NULL,         0, '2023-07-11 06:22:23', 1, '2024-09-05 11:32:34', 1, NULL, NULL),
(71, 'deleted_fei',   N'費承宏', 'deleted.fei@gmail',           '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0970039071', '1995-02-22', 0, '2023-09-30 09:52:44', 1, '2024-10-01 19:46:27', 1, NULL, NULL),
(72, 'deleted_lan',   N'藍若琳', 'deleted.lan@hotmail',         '$2a$11$rBnqTpOHBLFvFM1yKFVcAuXskO9BWlVjDKRmHHlm5Qo9sVFkNeV5C', '0981040072', NULL,         0, '2024-01-15 09:33:57', 1, '2024-11-20 18:14:15', 1, NULL, NULL);

SET IDENTITY_INSERT [dbo].[Members] OFF;
GO
