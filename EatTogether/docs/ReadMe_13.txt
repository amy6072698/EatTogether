# 成員 D｜婷方 — 個人開發 ReadMe
## 桌位管理 × 訂位系統 × 優惠券管理
### 技術棧：ASP.NET Core MVC / .NET 8 / EF Core / MS SQL Server 2025

---

## 進度符號說明
```
[v]           已完成
[working on]  進行中
[]            待開始
```

---

## 連結資料庫（確認共用部分已完成）

```
[v] 設定 EFModels
[v] 設定連線字串
[v] 註冊 DbContext 到 DI
```

---
---

# 一、資料庫建立

---

## 建立資料表

```
[v] 建立 Tables（桌位資料表）
    欄位：
        Id            int           IDENTITY(1,1)  PK
        TableName     nvarchar(20)  NOT NULL        UNIQUE INDEX
        SeatCount     int           NOT NULL        DEFAULT 2, CHECK > 0
        Status        int           NOT NULL        DEFAULT 0, CHECK 0~2
                                                    0=空桌, 1=用餐中, 2=保留

[v] 建立 Reservations（訂位紀錄表）
    欄位：
        Id              int           IDENTITY(1,1)  PK
        BookingNumber   varchar(10)   NOT NULL        UNIQUE INDEX
        Name            nvarchar(50)  NOT NULL
        Phone           varchar(20)   NOT NULL
        Email           varchar(100)  NULL
        ReservationDate datetime2(0)  NOT NULL        INDEX
        AdultsCount     int           NOT NULL        DEFAULT 1, CHECK > 0
        ChildrenCount   int           NOT NULL        DEFAULT 0, CHECK >= 0
        Status          int           NOT NULL        DEFAULT 0, CHECK 0~3
                                                      0=訂位中, 1=已報到
                                                      2=已取消, 3=NoShow
        Remark          nvarchar(200) NULL
        ReservedAt      datetime2(0)  NOT NULL        DEFAULT GETDATE()

[v] 建立 Coupons（優惠券設定表）
    欄位：
        Id              int           IDENTITY(1,1)  PK
        Name            nvarchar(50)  NOT NULL
        Code            varchar(20)   NOT NULL        UNIQUE INDEX
        DiscountType    int           NOT NULL        DEFAULT 0, CHECK 0 or 1
                                                      0=折金額($), 1=打折(%)
        DiscountValue   int           NOT NULL        CHECK > 0
        MinSpend        int           NOT NULL        DEFAULT 0, CHECK >= 0
        StartDate       datetime2(0)  NOT NULL
        EndDate         datetime2(0)  NULL            (Null = 永久有效)
        LimitCount      int           NULL            (Null = 無限量)
        ReceivedCount   int           NOT NULL        DEFAULT 0

[v] 建立 MemberCoupons（會員領券紀錄表）
    欄位：
        Id          int           IDENTITY(1,1)  PK
        MemberId    int           NOT NULL        FK → Members(Id)
        CouponId    int           NOT NULL        FK → Coupons(Id)
        IsUsed      bit           NOT NULL        DEFAULT 0
        UsedDate    datetime2(0)  NULL            (IsUsed=1 時才填入)
```

---

## Seed Data 完成狀況

```
[v] 07_Tables.sql        20 張桌位
    ├─ A 雙人桌  4 張 (A1~A4)
    ├─ B 四人桌  6 張 (B1~B6)
    ├─ C 六人桌  4 張 (C1~C4)
    ├─ VIP 十人包廂 3 張 (VIP1~VIP3)
    └─ 戶外 四人桌 3 張 (OUT1~OUT3)
    Status 分布：
        空桌(0)   : 11 張
        用餐中(1) :  5 張  (A2, B1, B3, C2, VIP1)
        保留(2)   :  4 張  (B4, C3, VIP2, OUT1)

[v] 09_Reservations.sql  50 筆訂位
    ├─ 2025 年歷史紀錄       7 筆  (Status 1/2/3)
    ├─ 2026/1~2 月近期紀錄   8 筆  (Status 1/2/3)
    ├─ 今日 2026/3/11 當日   6 筆  (Status 0/1)  ← 展示重點
    ├─ 2026/3 月本週密集     9 筆  (Status 0)
    ├─ 2026/4~5 月未來預約  10 筆  (Status 0/2)
    └─ 2026/2~3 月補充      10 筆  (Status 1/2)
    Status 分布：
        訂位中(0) : 19 筆
        已報到(1) : 24 筆
        已取消(2) :  5 筆
        NoShow(3) :  2 筆

[v] 08_Coupons.sql       15 筆優惠券
    ├─ 永久有效 (EndDate=NULL) : 3 筆  WELCOME50 / HBD200 / VIP90
    ├─ 已過期 (< 2026/3/11)   : 6 筆  OPEN100 / MOM88 / DAD500 等
    ├─ 目前有效                : 2 筆  SPRING150 / WHITE314
    └─ 未來活動                : 4 筆  WORK85 / DRAGON50 / SUMMER80 / MOON500
    特殊資料：
        防線③測試：OPEN100 已達限量上限 (ReceivedCount=500, LimitCount=500)
        防線②測試：WORK85/DRAGON50/SUMMER80/MOON500 StartDate 在未來 → 「活動尚未開始」

[v] 20_MemberCoupons.sql 70 筆
    ├─ IsUsed=1 (已使用) : 15 筆  (Member Id 1~16)
    └─ IsUsed=0 (未使用) : 55 筆  (Member Id 1~50)
    涵蓋 50 位會員（占 74 位會員的 67%）
    特殊資料：
        防線②測試：Id 38(OPEN100過期), 39(MOM88過期), 40(XMAS300過期) 持有過期券但IsUsed=0
        防線⑤測試：Id 1~15 均 IsUsed=1，可直接用來測試重複使用攔截
```

---
---

二、反向工程


# 三、DTO & ViewModel & 擴充方法

---

## 桌位管理 DTO / ViewModel

```
[] /Models/Dtos/TableDto.cs
    屬性：
        int     Id
        string  TableName
        int     SeatCount
        int     Status
        string  StatusText      ← 由程式轉換：0=空桌, 1=用餐中, 2=保留

[] /Models/Dtos/TableCreateDto.cs
    屬性：
        string  TableName
        int     SeatCount

[] /Models/Dtos/TableUpdateStatusDto.cs
    屬性：
        int  Id
        int  Status

[] /Models/ViewModels/TableCreateViewModel.cs
    屬性：
        string  TableName
        int     SeatCount

[] /Models/Exts/TableExts.cs
    擴充方法：
        TableDto    ToDto(this TableEntity entity)
        TableEntity ToEntity(this TableCreateDto dto)
```

---

## 訂位系統 DTO / ViewModel

```
[] /Models/Dtos/ReservationDto.cs
    屬性：
        int       Id
        string    BookingNumber
        string    Name
        string    Phone
        string?   Email
        DateTime  ReservationDate
        int       AdultsCount
        int       ChildrenCount
        int       Status
        string    StatusText      ← 0=訂位中, 1=已報到, 2=已取消, 3=NoShow
        string?   Remark
        DateTime  ReservedAt

[] /Models/Dtos/ReservationCreateDto.cs
    屬性：
        string    Name
        string    Phone
        string?   Email
        DateTime  ReservationDate
        int       AdultsCount
        int       ChildrenCount
        string?   Remark
        string    BookingNumber   ← 由 Service 填入，不由前端傳入

[] /Models/Dtos/ReservationUpdateStatusDto.cs
    屬性：
        int  Id
        int  Status

[] /Models/ViewModels/ReservationCreateViewModel.cs
    屬性：
        string    Name
        string    Phone
        string?   Email
        DateTime  ReservationDate
        int       AdultsCount
        int       ChildrenCount
        string?   Remark

[] /Models/ViewModels/ReservationSearchViewModel.cs
    屬性：
        DateTime?                   SearchDate
        IEnumerable<ReservationDto> Results

[] /Models/Exts/ReservationExts.cs
    擴充方法：
        ReservationDto    ToDto(this ReservationEntity entity)
        ReservationEntity ToEntity(this ReservationCreateDto dto)
```

---

## 優惠券管理 DTO / ViewModel

```
[] /Models/Dtos/CouponDto.cs
    屬性：
        int       Id
        string    Name
        string    Code
        int       DiscountType
        int       DiscountValue
        int       MinSpend
        DateTime  StartDate
        DateTime? EndDate
        int?      LimitCount
        int       ReceivedCount
        string    DiscountTypeText   ← "折金額" / "打折"
        bool      IsExpired          ← 程式計算：EndDate != null && EndDate < DateTime.Now

[] /Models/Dtos/CouponCreateDto.cs
    屬性：
        string    Name
        string    Code
        int       DiscountType
        int       DiscountValue
        int       MinSpend
        DateTime  StartDate
        DateTime? EndDate
        int?      LimitCount

[] /Models/Dtos/CouponRedeemDto.cs       ← 結帳時核銷優惠券用
    屬性：
        string  Code
        int     MemberId
        int     OrderAmount            ← 用於低消門檻判斷

[] /Models/Dtos/MemberCouponDto.cs       ← 會員「我的優惠券」頁用
    屬性：
        int       CouponId
        string    CouponName
        string    Code
        bool      IsUsed
        DateTime? UsedDate
        DateTime? EndDate
        string    DiscountDescription   ← e.g. "折 $100" 或 "打九折"

[] /Models/ViewModels/CouponCreateViewModel.cs
    屬性：
        string    Name
        string    Code
        int       DiscountType
        int       DiscountValue
        int       MinSpend
        DateTime  StartDate
        DateTime? EndDate
        int?      LimitCount

[] /Models/Exts/CouponExts.cs
    擴充方法：
        CouponDto       ToDto(this CouponEntity entity)
        CouponEntity    ToEntity(this CouponCreateDto dto)
        MemberCouponDto ToMemberCouponDto(this MemberCouponEntity mc)
```

---
---

# 四、Repository 層

---

## 共用 Result 類別（若成員 A 尚未建立則由此處建立）

```
[] /Models/Infra/Result.cs
    屬性：
        bool    IsSuccess
        string? ErrorMessage

    靜態方法：
        static Result    Success()
        static Result    Fail(string errorMessage)

[] /Models/Infra/Result`1.cs       ← 泛型版，帶資料回傳
    繼承 Result
    屬性：
        T Data

    靜態方法：
        static Result<T> Success(T data)
        static Result<T> Fail(string errorMessage)
```

---

## 桌位 Repository

```
[] /Repositories/Interface/ITableRepository.cs
    方法：
        IEnumerable<TableDto>  GetAll()
        TableDto?              GetById(int id)
        void                   Create(TableCreateDto dto)
        void                   UpdateStatus(int id, int newStatus)
        void                   Delete(int id)
        bool                   IsNameExists(string tableName)   ← 建立時防重複

[] /Repositories/TableRepository.cs
    實作 ITableRepository
    注入 EatTogetherDbContext
```

---

## 訂位 Repository

```
[] /Repositories/Interface/IReservationRepository.cs
    方法：
        IEnumerable<ReservationDto>  GetAll()
        IEnumerable<ReservationDto>  GetByDate(DateTime date)
        ReservationDto?              GetById(int id)
        void                         Create(ReservationCreateDto dto)
        void                         UpdateStatus(ReservationUpdateStatusDto dto)
        bool                         IsConflict(DateTime reservationDate, int excludeId = 0)

    IsConflict 查詢邏輯說明：
        查詢條件：
            Status IN (0, 1)                      ← 只計算「訂位中」與「已報到」
            Id != excludeId                       ← 修改時排除自己
            ReservationDate 介於
                reservationDate - 90 分鐘
                reservationDate + 90 分鐘
        若查詢結果 Count > 0 則代表有衝突

[] /Repositories/ReservationRepository.cs
    實作 IReservationRepository
    注入 EatTogetherDbContext
```

---

## 優惠券 Repository

```
[] /Repositories/Interface/ICouponRepository.cs
    方法：
        IEnumerable<CouponDto>  GetAll()
        CouponDto?              GetByCode(string code)
        void                    Create(CouponCreateDto dto)
        bool                    IsCodeExists(string code)        ← 建立時防重複
        void                    IncrementReceivedCount(int id)   ← 核銷成功後 +1

[] /Repositories/Interface/IMemberCouponRepository.cs
    方法：
        MemberCouponDto?              GetByMemberAndCoupon(int memberId, int couponId)
        IEnumerable<MemberCouponDto>  GetByMember(int memberId)
        void                          Add(int memberId, int couponId)
        void                          MarkAsUsed(int memberId, int couponId)

[] /Repositories/CouponRepository.cs
    實作 ICouponRepository
    注入 EatTogetherDbContext

[] /Repositories/MemberCouponRepository.cs
    實作 IMemberCouponRepository
    注入 EatTogetherDbContext
```

---
---

# 五、Service 層（商業邏輯）

---

## 訂位號碼產生工具

```
[] /Models/Infra/BookingNumberGenerator.cs
    靜態方法：
        static string Generate(int latestSeq)

    產生規則：
        格式：R + 年份後2碼 + 月份2碼 + 流水號3碼
        範例：R260311006  →  2026 年 3 月第 6 筆

    使用方式：
        Service 建立訂位前，先查出當月最大序號
        再呼叫 Generate() 產生不重複的 BookingNumber

    注意：Seed Data 已有 BookingNumber 至 R260311006
          新增時序號需從下一個流水號接續
```

---

## 桌位 Service

```
[] /Services/TableService.cs
    注入 ITableRepository

    方法：
        IEnumerable<TableDto>  GetAll()
            呼叫 repo.GetAll()

        Result  Create(TableCreateDto dto)
            Step 1：呼叫 repo.IsNameExists(dto.TableName)
            Step 2：若已存在 → return Result.Fail("桌位名稱已存在")
            Step 3：呼叫 repo.Create(dto)
            Step 4：return Result.Success()

        Result  UpdateStatus(int id, int newStatus)
            Step 1：呼叫 repo.GetById(id)，若不存在 → return Fail
            Step 2：防呆：Status=1(用餐中) 不可直接改為 2(保留)
                    if (current.Status == 1 && newStatus == 2)
                        return Result.Fail("用餐中的桌位無法直接標記為保留")
            Step 3：呼叫 repo.UpdateStatus(id, newStatus)
            Step 4：return Result.Success()

        Result  Delete(int id)
            呼叫 repo.Delete(id)
            return Result.Success()
```

---

## 訂位 Service

```
[] /Services/ReservationService.cs
    注入 IReservationRepository

    方法：
        IEnumerable<ReservationDto>  GetAll()
        IEnumerable<ReservationDto>  GetByDate(DateTime date)

        Result  Create(ReservationCreateDto dto)
            Step 1：呼叫 repo.IsConflict(dto.ReservationDate)
            Step 2：若衝突 → return Result.Fail("此時段已有訂位，請選擇其他時間")
            Step 3：查出當月最大序號 → 呼叫 BookingNumberGenerator.Generate()
            Step 4：dto.BookingNumber = 產生的號碼
            Step 5：呼叫 repo.Create(dto)
            Step 6：return Result.Success()

        Result  UpdateStatus(ReservationUpdateStatusDto dto)
            Step 1：呼叫 repo.GetById(dto.Id)，若不存在 → return Fail
            Step 2：呼叫 repo.UpdateStatus(dto)
            Step 3：return Result.Success()
```

---

## 優惠券 Service

```
[] /Services/CouponService.cs
    注入 ICouponRepository, IMemberCouponRepository

    方法：
        IEnumerable<CouponDto>  GetAll()

        Result  Create(CouponCreateDto dto)
            Step 1：呼叫 couponRepo.IsCodeExists(dto.Code)
            Step 2：若重複 → return Result.Fail("折扣碼已存在")
            Step 3：呼叫 couponRepo.Create(dto)
            Step 4：return Result.Success()

        Result<int>  RedeemCoupon(CouponRedeemDto dto)
        ┌──────────────────────────────────────────────────────────┐
        │  ★ 五道核銷防線（依序執行，任一失敗立即回傳）★            │
        │                                                          │
        │  防線① 折扣碼是否存在                                    │
        │      couponRepo.GetByCode(dto.Code)                      │
        │      若 null → Fail("折扣碼不存在")                       │
        │      測試用：輸入任意亂碼即可觸發                         │
        │                                                          │
        │  防線② 活動是否在有效期限內                               │
        │      DateTime.Now < coupon.StartDate                     │
        │          → Fail("活動尚未開始")                           │
        │          測試用：WORK85 / DRAGON50 / SUMMER80 / MOON500  │
        │      coupon.EndDate != null &&                           │
        │      DateTime.Now > coupon.EndDate                       │
        │          → Fail("優惠券已過期")                           │
        │          測試用：LOVE520 / CNY95 / OPEN100 等            │
        │                                                          │
        │  防線③ 是否超過限量                                      │
        │      coupon.LimitCount != null &&                        │
        │      coupon.ReceivedCount >= coupon.LimitCount           │
        │          → Fail("此優惠券已達領取上限")                   │
        │          測試用：OPEN100 (500/500 已滿)                   │
        │                                                          │
        │  防線④ 是否達到最低消費                                   │
        │      dto.OrderAmount < coupon.MinSpend                   │
        │          → Fail($"未達最低消費 ${coupon.MinSpend}")       │
        │          測試用：SPRING150 MinSpend=800，輸入金額<800     │
        │                                                          │
        │  防線⑤ 此會員是否已使用過                                 │
        │      memberCouponRepo                                    │
        │          .GetByMemberAndCoupon(dto.MemberId, coupon.Id)  │
        │      若 record != null && record.IsUsed == true          │
        │          → Fail("此優惠券已使用過")                       │
        │          測試用：MemberCoupons Id 1~15 均 IsUsed=1        │
        └──────────────────────────────────────────────────────────┘
            全部通過後：
            Step 1：計算折扣金額
                    DiscountType == 0 → discount = DiscountValue（折金額）
                    DiscountType == 1 → discount = OrderAmount * DiscountValue / 100（打折）
            Step 2：若 record == null → memberCouponRepo.Add(dto.MemberId, coupon.Id)
            Step 3：memberCouponRepo.MarkAsUsed(dto.MemberId, coupon.Id)
            Step 4：couponRepo.IncrementReceivedCount(coupon.Id)
            Step 5：return Result<int>.Success(discount)

        IEnumerable<MemberCouponDto>  GetMemberCoupons(int memberId)
            呼叫 memberCouponRepo.GetByMember(memberId)
```

---

## 在 Program.cs 註冊所有服務

```
[] 新增至 Program.cs
    builder.Services.AddScoped<ITableRepository,        TableRepository>()
    builder.Services.AddScoped<IReservationRepository,  ReservationRepository>()
    builder.Services.AddScoped<ICouponRepository,       CouponRepository>()
    builder.Services.AddScoped<IMemberCouponRepository, MemberCouponRepository>()
    builder.Services.AddScoped<TableService>()
    builder.Services.AddScoped<ReservationService>()
    builder.Services.AddScoped<CouponService>()
```

---
---

# 六、Controller & View 層

---

## TablesController

```
[] /Controllers/TablesController.cs
    加入 [Authorize]
    注入 TableService

    Action 清單：
    ┌────────────────────────────────┬────────┬──────────────────────────────────┐
    │ Action                         │ 方法   │ 說明                             │
    ├────────────────────────────────┼────────┼──────────────────────────────────┤
    │ Index()                        │ GET    │ 顯示所有桌位列表（共 20 張）      │
    │ Create()                       │ GET    │ 顯示新增桌位表單                  │
    │ Create(TableCreateViewModel vm)│ POST   │ 新增桌位，成功導向 Index          │
    │ UpdateStatus(int id, int status│ POST   │ 切換桌位狀態，回傳 JSON           │
    │ Delete(int id)                 │ POST   │ 刪除桌位，成功導向 Index          │
    └────────────────────────────────┴────────┴──────────────────────────────────┘

[] /Views/Tables/Index.cshtml
    顯示內容：桌位名稱、座位數、狀態（空桌/用餐中/保留）
    分頁設定：每頁 10 筆（20 張共 2 頁）
    操作按鈕：新增桌位、切換狀態（AJAX）、刪除

[] /Views/Tables/Create.cshtml
    表單欄位：TableName、SeatCount
    送出後導向 Index
```

---

## ReservationsController

```
[] /Controllers/ReservationsController.cs
    加入 [Authorize]
    注入 ReservationService

    Action 清單：
    ┌──────────────────────────────────────┬────────┬──────────────────────────────────┐
    │ Action                               │ 方法   │ 說明                             │
    ├──────────────────────────────────────┼────────┼──────────────────────────────────┤
    │ Index(DateTime? searchDate)          │ GET    │ 訂位列表，可依日期篩選           │
    │ Create()                             │ GET    │ 顯示新增訂位表單                  │
    │ Create(ReservationCreateViewModel vm)│ POST   │ 新增訂位，衝突時顯示錯誤訊息     │
    │ UpdateStatus(dto)                    │ POST   │ 更新訂位狀態，回傳 JSON           │
    └──────────────────────────────────────┴────────┴──────────────────────────────────┘

[] /Views/Reservations/Index.cshtml
    顯示內容：訂位代號、姓名、電話、預約時間、人數、狀態
    分頁設定：每頁 10 筆（50 筆共 5 頁）
    搜尋列：依預約日期篩選
             搜尋 2026/3/11 可看到今日 6 筆資料（展示重點）
    操作按鈕：報到、取消、NoShow（各自呼叫 UpdateStatus AJAX）

[] /Views/Reservations/Create.cshtml
    表單欄位：姓名、電話、Email、預約日期時間、大人/小孩人數、備註
    送出後若衝突：停留在此頁並顯示錯誤訊息
    送出成功：導向 Index
```

---

## CouponsController

```
[] /Controllers/CouponsController.cs
    加入 [Authorize]（後台 Action）
    注入 CouponService

    Action 清單：
    ┌─────────────────────────────────────┬────────┬───────────────────────────────────────┐
    │ Action                              │ 方法   │ 說明                                  │
    ├─────────────────────────────────────┼────────┼───────────────────────────────────────┤
    │ Index()                             │ GET    │ 後台優惠券列表（共 15 筆，1~2 頁）     │
    │ Create()                            │ GET    │ 後台新增優惠券表單                     │
    │ Create(CouponCreateViewModel vm)    │ POST   │ 新增優惠券，Code 重複時顯示錯誤        │
    │ ApplyCoupon(CouponRedeemDto dto)    │ POST   │ 前台結帳核銷 API，回傳 JSON            │
    │ MyCoupons()                         │ GET    │ 會員「我的優惠券」頁（需登入）         │
    └─────────────────────────────────────┴────────┴───────────────────────────────────────┘

    ApplyCoupon 回傳格式（JSON）：
        成功：{ success: true,  discountAmount: 100, message: "" }
        失敗：{ success: false, discountAmount: 0,   message: "錯誤原因" }

[] /Views/Coupons/Index.cshtml
    顯示內容：活動名稱、折扣碼、折扣說明、有效日期、已領/限量、狀態（是否過期）
    分頁設定：每頁 10 筆（15 筆共 2 頁）

[] /Views/Coupons/Create.cshtml
    表單欄位：活動名稱、折扣碼、折扣類型、折扣值、最低消費、開始/結束日期、限量張數

[] /Views/Coupons/MyCoupons.cshtml
    顯示內容：券名、折扣碼、折扣說明、到期日、使用狀態（未使用/已使用）
    使用方式：從 Session 或 Cookie 取得目前登入的 MemberId
    預期呈現：Member Id 1~50 登入後均有至少 1 張券可看到
```

---
---

# 七、跨模組整合

---

## 與成員 A（權限模組）整合

```
[] 確認 [Authorize] 正確套用範圍
    TablesController       → 整個 Controller 加 [Authorize]
    ReservationsController → 整個 Controller 加 [Authorize]
    CouponsController      → Index / Create 加 [Authorize]，ApplyCoupon 另議

[] 確認後台側邊選單加入以下連結（與成員A協調 _Layout）
    桌位管理   → /Tables/Index
    訂位管理   → /Reservations/Index
    優惠券管理 → /Coupons/Index
```

---

## 與成員 B（訂單結帳模組）整合

```
[] 結帳頁核銷優惠券
    成員B 結帳頁呼叫：POST /Coupons/ApplyCoupon
    傳入：{ code, memberId, orderAmount }
    接收：{ success, discountAmount, message }

[] 桌位狀態聯動
    成員B 開單（內用）時：
        呼叫 POST /Tables/UpdateStatus  →  { id: tableId, status: 1 }
    成員B 結帳完成後：
        呼叫 POST /Tables/UpdateStatus  →  { id: tableId, status: 0 }
    注意：Seed Data 中 A1、C1 已設為 Status=0，代表「結帳後恢復空桌」情境

[] 確認 Orders 與 PreOrders 資料表
    CouponId 欄位的 FK 對應到 Coupons(Id)（與成員B確認）
```

---

## 與成員 E（行銷模組）整合

```
[] 若活動（Events）建立時需同步建立優惠券
    由成員E 注入 CouponService 呼叫 Create()
    或由 CouponsController 提供 API（與成員E確認實作方式）
```

---
---

# 八、展示操作腳本

```
展示前 5 分鐘建議執行以下操作，讓畫面更豐富：

桌位管理展示流程：
    Step 1：開啟 /Tables/Index，畫面呈現：
            空桌(綠) 11 張 / 用餐中(橘) 5 張 / 保留(灰) 4 張
    Step 2：點選 A3（空桌）→ 改為用餐中（模擬客人入座）
            系統允許，畫面即時更新
    Step 3：嘗試將 B1（用餐中）→ 改為保留
            系統攔截，顯示「用餐中的桌位無法直接標記為保留」
    Step 4：點選 B4（保留）→ 改為空桌（模擬維修完成）
            系統允許，畫面即時更新

訂位管理展示流程：
    Step 1：開啟 /Reservations/Index，預設顯示全部 50 筆（5 頁）
    Step 2：搜尋日期輸入 2026/3/11，顯示今日 6 筆訂位
            午間 2 筆已報到 / 4 筆訂位中
    Step 3：點選「報到」按鈕，Status 更新為已報到
    Step 4：新增訂位，輸入今日 12:00（與 R260311001 衝突）
            系統攔截，顯示「此時段已有訂位，請選擇其他時間」

優惠券展示流程：
    Step 1：開啟 /Coupons/Index，顯示 15 筆優惠券（2 頁）
    Step 2：結帳輸入 SPRING150 + 金額 1000 → 成功折 150 元
    Step 3：輸入 OPEN100 → 顯示「優惠券已過期」（防線②）
    Step 4：再輸入 SPRING150 同一會員 → 顯示「此優惠券已使用過」（防線⑤）
```

---
---

# 九、測試清單

---

## 桌位管理測試

```
[] 新增桌位：桌位名稱重複時是否正確顯示錯誤
[] 新增桌位：正常新增後列表是否出現新資料
[] 狀態切換：Status=1(用餐中) 嘗試切換為 2(保留) 是否被攔截並顯示錯誤
[] 狀態切換：Status=0(空桌) 切換為 1(用餐中) 是否正常執行
[] 狀態切換：Status=0(空桌) 切換為 2(保留) 是否正常執行
[] 狀態切換：Status=1(用餐中) 切換為 0(空桌) 是否正常執行（模擬結帳完成）
[] 狀態切換：Status=2(保留) 切換為 0(空桌) 是否正常執行（模擬取消保留）
[] 刪除桌位：確認刪除後列表正確更新
[] 分頁：確認 20 筆資料在每頁 10 筆下正確顯示 2 頁
```

---

## 訂位系統測試

```
[] 衝突測試（核心）：同時段(±90分鐘內)新增第二筆訂位 → 是否觸發衝突錯誤
    測試資料：新增 2026/3/11 12:30（與 R260311001 的 12:00 衝突）
[] 非衝突測試：超過 90 分鐘的不同時段 → 是否可以正常新增
[] 狀態更新：訂位中 → 已報到 → 確認 StatusText 正確顯示
[] 狀態更新：訂位中 → 已取消 → 確認不計入衝突判斷範圍
[] BookingNumber：確認自動遞增且不與現有 R260311006 重複
[] 日期搜尋：輸入 2026/3/11 → 僅顯示今日 6 筆
[] 分頁：確認 50 筆資料在每頁 10 筆下正確顯示 5 頁
```

---

## 優惠券核銷測試（五道防線）

```
[] 防線①：輸入不存在的折扣碼 (e.g. XXXXXX)
    預期：顯示「折扣碼不存在」

[] 防線②-A：使用已過期的券 (e.g. OPEN100 / LOVE520 / CNY95)
    預期：顯示「優惠券已過期」

[] 防線②-B：使用尚未開始的券 (e.g. WORK85 / DRAGON50)
    預期：顯示「活動尚未開始」

[] 防線③：使用 OPEN100 (ReceivedCount=500, LimitCount=500)
    預期：顯示「此優惠券已達領取上限」

[] 防線④：使用 SPRING150 (MinSpend=800)，輸入金額 500
    預期：顯示「未達最低消費 $800」

[] 防線⑤：Member Id 1 使用 CouponId 1 (IsUsed=1)
    預期：顯示「此優惠券已使用過」

[] 全程通過（折金額）：Member Id 23 使用 SPRING150，輸入金額 1000
    預期：回傳 discountAmount = 150，IsUsed 更新為 1

[] 全程通過（打折）：Member Id 23 使用 VIP90，輸入金額 1000
    預期：回傳 discountAmount = 100（1000 * 10 / 100）

[] 核銷成功後：確認 MemberCoupons.IsUsed = 1 且 UsedDate 有值
[] 核銷成功後：確認 Coupons.ReceivedCount 正確 +1
[] 分頁：確認 15 筆優惠券在每頁 10 筆下正確顯示 2 頁
[] 我的優惠券：Member Id 1~50 登入後頁面均有券可顯示
```
