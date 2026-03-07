-- 15_Dishes.sql
-- Seed Data for Dishes (追加模式，逐筆檢查避免重複)
-- 依賴：05_Categories.sql 必須先執行

DECLARE @MainId    int = (SELECT TOP 1 Id FROM dbo.Categories WHERE CategoryName = N'主餐');
DECLARE @DrinkId   int = (SELECT TOP 1 Id FROM dbo.Categories WHERE CategoryName = N'飲料');
DECLARE @DessertId int = (SELECT TOP 1 Id FROM dbo.Categories WHERE CategoryName = N'甜點');

IF @MainId IS NULL OR @DrinkId IS NULL OR @DessertId IS NULL
    THROW 50001, 'Seed failed: Categories not found. Please run 05_Categories.sql first.', 1;

-- ==================== 主餐 (30筆) ====================
IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'義式番茄義大利麵')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'義式番茄義大利麵', 180.00, 1, GETDATE(), N'經典義式番茄醬搭配彈牙麵條', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'奶油培根燉飯')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'奶油培根燉飯', 200.00, 1, GETDATE(), N'濃郁奶油培根風味燉飯', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'青醬海鮮義大利麵')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'青醬海鮮義大利麵', 220.00, 1, GETDATE(), N'新鮮海鮮搭配羅勒青醬', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'松露野菇燉飯')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'松露野菇燉飯', 250.00, 1, GETDATE(), N'頂級松露香氣搭配多種野菇', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'香烤雞腿排')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'香烤雞腿排', 260.00, 1, GETDATE(), N'嫩煎去骨雞腿搭配季節蔬菜', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'BBQ豬肋排')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'BBQ豬肋排', 320.00, 1, GETDATE(), N'慢烤入味豬肋排佐BBQ醬', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'香煎鮭魚排')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'香煎鮭魚排', 280.00, 1, GETDATE(), N'挪威鮭魚佐檸檬奶油醬', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'瑪格麗特披薩')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'瑪格麗特披薩', 240.00, 1, GETDATE(), N'番茄醬底搭配新鮮莫札瑞拉起司', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'牛肉漢堡排')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'牛肉漢堡排', 290.00, 1, GETDATE(), N'手工牛肉漢堡排佐特製醬汁', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'總匯三明治')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'總匯三明治', 150.00, 1, GETDATE(), N'火腿培根蛋起司豐盛總匯', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'墨西哥雞肉捲')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'墨西哥雞肉捲', 190.00, 1, GETDATE(), N'嫩煎雞肉搭配莎莎醬與酸奶', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'泰式打拋豬飯')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'泰式打拋豬飯', 170.00, 1, GETDATE(), N'泰式九層塔炒豬肉搭配白飯', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'日式唐揚雞定食')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'日式唐揚雞定食', 210.00, 1, GETDATE(), N'酥炸日式唐揚雞搭配味噌湯與白飯', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'韓式石鍋拌飯')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'韓式石鍋拌飯', 220.00, 1, GETDATE(), N'豐富蔬菜與牛肉搭配石鍋香飯', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'起司焗烤通心粉')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'起司焗烤通心粉', 200.00, 1, GETDATE(), N'濃郁白醬焗烤起司通心粉', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'蒜香奶油蝦義大利麵')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'蒜香奶油蝦義大利麵', 230.00, 1, GETDATE(), N'大蒜奶油炒鮮蝦搭配天使細麵', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'法式洋蔥湯牛排')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'法式洋蔥湯牛排', 380.00, 1, GETDATE(), N'8oz嫩肩牛排搭配濃郁法式洋蔥湯', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'香草烤半雞')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'香草烤半雞', 300.00, 1, GETDATE(), N'迷迭香與百里香醃製慢烤半雞', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'海鮮總匯披薩')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'海鮮總匯披薩', 270.00, 1, GETDATE(), N'滿載蝦仁花枝透抽的海鮮披薩', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'燻鴨胸沙拉')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'燻鴨胸沙拉', 210.00, 1, GETDATE(), N'煙燻鴨胸搭配芝麻葉與核桃油醋醬', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'辣味肉醬千層麵')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'辣味肉醬千層麵', 230.00, 1, GETDATE(), N'辣味牛肉醬與白醬交疊千層麵', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'檸檬奶油鱈魚排')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'檸檬奶油鱈魚排', 270.00, 1, GETDATE(), N'香煎鱈魚佐檸檬奶油白酒醬', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'紅酒燉牛肉飯')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'紅酒燉牛肉飯', 280.00, 1, GETDATE(), N'法式紅酒慢燉牛肉搭配白飯', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'培根菠菜鹹派')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'培根菠菜鹹派', 180.00, 1, GETDATE(), N'酥脆派皮搭配培根菠菜蛋奶餡', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'番茄羅勒燉雞')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'番茄羅勒燉雞', 240.00, 1, GETDATE(), N'南歐風味番茄羅勒燉嫩雞', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'脆皮烤豬五花飯')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'脆皮烤豬五花飯', 220.00, 1, GETDATE(), N'慢烤脆皮豬五花搭配滷汁白飯', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'焗烤海鮮飯')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'焗烤海鮮飯', 260.00, 1, GETDATE(), N'白酒奶油海鮮搭配起司焗烤燉飯', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'夏威夷雞肉披薩')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'夏威夷雞肉披薩', 250.00, 1, GETDATE(), N'雞肉鳳梨火腿搭配番茄醬底', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'季節限定主廚套餐')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'季節限定主廚套餐', 350.00, 1, GETDATE(), N'主廚精選當季食材特製套餐', NULL, 1, 1, '2026-03-01', '2026-05-31', NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'龍蝦奶油義大利麵')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@MainId, N'龍蝦奶油義大利麵', 480.00, 1, GETDATE(), N'整隻波士頓龍蝦搭配奶油寬麵', NULL, 0, 1, '2026-01-01', '2026-12-31', NULL);

-- ==================== 飲料 (10筆) ====================
IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'可樂')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@DrinkId, N'可樂', 50.00, 1, GETDATE(), N'冰涼暢快經典碳酸飲料', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'柳橙汁')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@DrinkId, N'柳橙汁', 65.00, 1, GETDATE(), N'現榨新鮮柳橙汁', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'招牌鮮奶茶')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@DrinkId, N'招牌鮮奶茶', 70.00, 1, GETDATE(), N'紅茶搭配濃醇鮮奶', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'抹茶拿鐵')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@DrinkId, N'抹茶拿鐵', 80.00, 1, GETDATE(), N'日式抹茶粉搭配義式濃縮牛奶', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'美式黑咖啡')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@DrinkId, N'美式黑咖啡', 60.00, 1, GETDATE(), N'深烘焙豆現磨美式咖啡', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'卡布奇諾')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@DrinkId, N'卡布奇諾', 85.00, 1, GETDATE(), N'濃縮咖啡搭配細緻奶泡', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'草莓奶昔')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@DrinkId, N'草莓奶昔', 90.00, 1, GETDATE(), N'新鮮草莓搭配香草冰淇淋', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'西瓜汁')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@DrinkId, N'西瓜汁', 65.00, 1, GETDATE(), N'夏季限定現打西瓜汁', NULL, 1, 1, '2026-05-01', '2026-09-30', NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'熱可可')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@DrinkId, N'熱可可', 75.00, 1, GETDATE(), N'比利時巧克力搭配溫熱鮮奶', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'蜂蜜檸檬氣泡水')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@DrinkId, N'蜂蜜檸檬氣泡水', 60.00, 1, GETDATE(), N'天然蜂蜜搭配新鮮檸檬與氣泡水', NULL, 1, 0, NULL, NULL, NULL);

-- ==================== 甜點 (5筆) ====================
IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'分享拼盤')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@DessertId, N'分享拼盤', 320.00, 1, GETDATE(), N'多人分享綜合甜點拼盤', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'提拉米蘇')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@DessertId, N'提拉米蘇', 120.00, 1, GETDATE(), N'經典義式咖啡風味甜點', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'巧克力熔岩蛋糕')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@DessertId, N'巧克力熔岩蛋糕', 130.00, 1, GETDATE(), N'外酥內軟流心巧克力蛋糕', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'法式烤布蕾')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@DessertId, N'法式烤布蕾', 115.00, 1, GETDATE(), N'焦糖脆皮搭配香草卡士達', NULL, 1, 0, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Dishes WHERE DishName = N'草莓千層蛋糕')
    INSERT INTO dbo.Dishes (CategoryId, DishName, Price, IsActive, CreatedAt, Description, ImageUrl, IsTakeOut, IsLimited, StartDate, EndDate, UpdatedAt)
    VALUES (@DessertId, N'草莓千層蛋糕', 150.00, 1, GETDATE(), N'手工千層薄餅搭配新鮮草莓與奶油', NULL, 1, 0, NULL, NULL, NULL);

GO
