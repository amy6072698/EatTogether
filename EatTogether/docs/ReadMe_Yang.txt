[]共用基礎
	-- Result class
		Result
		IsSuccess, ErrorMessage
		static Success()
		static Fail(msg)

[]資料庫建置
	-- 連結資料庫、設定 EF Models(檔案"EfModels/appsettings.json/Program.cs(DbContext DI)")
	-- 建立 Categories 資料表(含 ParentCategoryId、DisplayOrder)
	-- 建立 Dishes 資料表("FK → Categories IsLimited + 日期範圍")
	-- 建立 SetMeals 資料表(fixed / percent 折扣類型)
	-- 建立 SetMealItems 資料表(OptionGroupNo + PickLimit 互斥邏輯)
	-- 建立 Products 資料表(DishId / SetMealId 互斥 CHECK)

[]菜單分類管理
	-- 建立 CategoryDto & CategoryViewModel 擴充方法
		CategoryDto
		CategoryViewModel
		ToViewModel()
		CategoryDtoExtension

	-- 建立 ICategoryRepository / CategoryRepository
		GetAll()
		GetById()
		Create() / Update()
		SoftDelete()
		
	-- 建立 CategoryService
		ctor(ICategoryRepository)
		GetAll() / GetById()
		Create() / Update() / Disable()

	-- 分類管理後台頁面 CRUD ("軟刪除（IsActive=0，父分類下拉選單)
		Categories/Index.cshtml
		Categories/Create.cshtml
		Categories/Edit.cshtml

[]餐點管理
	--建立 DishDto / DishViewModel 擴充方法
		DishDto
		DishViewModel
		ToViewModel()
		DishDtoExtension

	--建立 IDishRepository / DishRepository
		GetAll()
		GetById()
		Create() / Update()
		SoftDelete()

	--建立 DishService
		ctor(IDishRepository)
		GetAll() / GetById()
		Create() / Update() / Disable()

	--餐點管理後台頁面 CRUD
		Dishes/Index.cshtml
		Dishes/Create.cshtml
		Dishes/Edit.cshtml

[]套餐管理
	--建立 SetMealDto / SetMealItemDto
		SetMealDto
		SetMealItemDto
		SetMealDtoExtension

	--建立 SetMealViewModel / SetMealItemViewModel
		SetMealViewModel
		SetMealItemViewModel
		ToViewModel()

	--建立 ISetMealRepository / SetMealRepository
		GetAll()
		GetById()
		Create() / Update()
		AddItem() / RemoveItem()

	--建立 SetMealService（含互斥邏輯驗證）
		ctor(ISetMealRepository)
		CRUD + 套餐內容管理 
		ValidateOptionGroup()→ 回傳 Result

	--套餐管理後台頁面 (選項群組 UI，折扣類型切換，套餐包含餐點清單)
		SetMeals/Index.cshtml
		SetMeals/Create.cshtml
		SetMeals/Edit.cshtml

[]Products 整合 (讓訂單系統可引用)
	--建立 ProductDto / ProductRepository
		ProductDto
		ProductRepository GetAll() / GetById()

	--建立 ProductService
		ctor(IProductRepository) GetAll() / GetById()→ 訂單系統統一透過 Service

	--與成員B整合測試
		確認訂單系統可正確讀取 Products 資料












