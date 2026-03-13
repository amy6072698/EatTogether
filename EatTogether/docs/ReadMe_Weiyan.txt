後台部分

開發活動機制
[working]add 活動新增頁面 url:/Events/Create
	[V]add ViewModel, Dto
		EventCreateViewModel class
			Title, Summary, MinSpend, StartDate, EndDate, RewardItem, DiscountType, DiscountValue, Status

		EventCreateDto class
			Title, Summary, MinSpend, StartDate, EndDate, RewardItem, DiscountType, DiscountValue, Status

		EventsMappingExtension
			活動新增
			vm -> dto
			→ ToDto(this EventCreateViewModel  vm)
			Dto → Entity（Repository 寫入用）
			→Event ToEntity(this EventsCreateDto  dto)	


	[V]add Service/Repository
		IEventRepository interface
			void Create (EventCreateDto  dto)

		EventRepository 實作 IEventRepository 
			ctor(EatTogetherContext context)

		EventService
			ctor(IEventRepository  repo)
			void Create(EventCreateDto   dto)

		在 Program.cs 註冊IEventRepository , EventRepository,  service

	[V]add EventsController
		Create()
		ctor(EventService eventService)
		Create(EventCreateViewModel  vm)[Autorize]

	[working]美化頁面


[working]add 活動首頁 url: /Events/Index
	[V]add ViewModel, Dto 
		EventViewModel class
			Id, Title, Summary, MinSpend, StartDate, EndDate, RewardItem, DiscountType, DiscountValue, Status
	
		EventDto class
			Id, Title, Summary, MinSpend, StartDate, EndDate, RewardItem, DiscountType, DiscountValue, Status
			

	[V]modify EventRepository
		IEventRepository interface
			add IEnumerable<EventDto> GetAll()

	[V]modify 	EventService
			List<EventDto> GetAllForIndex()

	[V]modify EventsMappingExtension
		活動列表 dto -> vm
		→ ToViewModel(this EventDto dto)
		// Entity → Dto（Repository 讀取用）
		EventDto ToDto(this Event entity)


	[V]modify EventsController
		add IActionResult index action[Autorize]
			Index.cshtml

	**活動狀態:進行中、未開始、已結束

	[working]美化頁面
	


[working]add 活動編輯 url: /Events/Edit?eventsId=00
	[V]add ViewModel, Dto , VM轉Dto的擴充方法
		EventEditViewModel class
			Id, Title, Summary, MinSpend, StartDate, EndDate, RewardItem, DiscountType, DiscountValue, Status

		EventEditDto class
			Id, Title, Summary, MinSpend, StartDate, EndDate, RewardItem, DiscountType, DiscountValue, Status

		modify EventsMappingExtension
			活動編輯 
			vm <-> dto
 			→ ToDto(this EventEditViewModel vm)
   			→ ToViewModel(this AEventEditDto dto)
			// Dto → Entity（Repository 寫入用）
			→Event ToEntity(this EventEditDto dto)

			// Entity → Dto（Repository 讀取用）
			EventDto ToDto(this Event entity)	


	[V]modify EventRepository
		IEventRepository  interface
			add void Edit(EventEditDto  dto)
			add EventEditDto GetEditById(int id)

	[working]modify	EventService
			void Edit(EventEditDto dto)

	[]modify EventsController
		add IActionResult Edit action[Autorize]
			Edit.cshtml
		HttpGet Edit(int id)[Autorize]

		HttpPost Edit(EventEditViewModel vm)[Autorize]

	**1.進行中活動開始日期不能改
	    2.未開始活動則都可以改 
	    3.編輯頁面放置「活動停用」超連結，跳出活動詳細視窗，按下停用再跳一個警告視窗

	[]美化頁面


[]add 活動停用   >>情境:可能贈品送完、折扣有誤或是有臨時狀況需要緊急停止該活動
  	條件式：僅「進行中」活動可執行停用操作
  	入口：Edit.cshtml 頁面內的「活動停用」超連結（已標記）
  	流程：
    	1. 點擊「活動停用」→ 跳出活動詳細資訊 Modal
    	2. 按下停用 → 二次確認警告視窗
    	3. 確認後執行停用

  	停用後狀態邏輯：
   	 - 停用 = 強制將 Status 設為「已結束」（或新增 IsDisabled 欄位區分）
    	- 與「自然結束」（EndDate 到期）的差異建議透過 IsDisabled 欄位區分，
     	 方便日後查詢「是否為提前停用的活動」
	

[]add 活動複製
	條件式:已結束活動，edit頁面呈現資料為唯讀狀態，該頁面僅有「複製並建立新活動」超連結可以點擊
	**複製並建立新活動: 跳轉至create頁面，輸入框自動填入資料
	**撈EventId資料，日期清空，新的EventId一樣在新增完成時自動生成



開發文章機制

********************文章分類********************

[]add 文章分類新增  url:/ArticleCategories/Create (不會用網址跳轉，像是接口，讓modal去連接)
	[]add ViewModel, Dto
		ArticleCategoryCreateViewModel class
			Name, SortOrder, IsEnabled

		ArticleCategoryCreateDto class
			Name, SortOrder, IsEnabled

		ArticleCategoryMappingExtension
			文章分類新增
			vm -> dto
			→ ToDto(this ArticleCategoryCreateViewModel vm)
			Dto → Entity（Repository 寫入用）
			→ArticleCategory ToEntity(this ArticleCategoryCreateDto dto)

			文章分類編輯 
			vm <-> dto
 			→ ToDto(this ArticleCategoryEditViewModel vm)
   			→ ToViewModel(this ArticleCategoryEditDto dto)
			// Dto → Entity（Repository 寫入用）
			→Article ToEntity(this ArticleCategoryEditDto dto)

			// Entity → Dto（Repository 讀取用）
			ArticleCategoryDto ToDto(this ArticleCategory entity)		


	[]add Service/Repository
		IArticleCategoryRepository interface
			void Create (ArticleCategoryCreateDto dto)

		ArticleCategoryRepository 實作 IArticleCategoryRepository 
			ctor(EatTogetherContext context)

		ArticleCategoryService
			ctor(IArticleCategoryRepository  repo)
			void Create(ArticleCategoryCreateDto  dto)

		在 Program.cs 註冊IArticleCategoryRepository , ArticleCategoryRepository, service

	[]add ArticleCategoriesController
		Create()
		ctor(ArticleCategoryService articleCategoryService)
		Create(ArticleCategoryCreateViewModel  vm)[Authorize]


[]add 文章分類首頁頁面 url:/ArticleCategories/Index
	[]add ViewModel, Dto
		ArticleCategoryViewModel class
			Name, SortOrder, IsEnabled
			(分類名、排序、是否啟用)

		ArticleCategoryDto class
			Name, SortOrder, IsEnabled

		ArticleCategoryViewModelExtension class /  Dto → VM / Entity → Dto
			文章分類列表 dto -> vm
			→ ToViewModel(this ArticleCategoryDto  dto)
			// Entity → Dto（Repository 讀取用）
			ArticleCategoryDto ToDto(this ArticleCategory entity)	
		

	[]modify ArticleCategoryRepository 
		ArticleCategoryRepository interface
			add IEnumerable<ArticleCategoryDto> GetAll()

	[]modify 	ArticleService
			List<ArticleCategoryDto> GetAllForIndex()

		

	[]modify ArticleCategoriesController
		add IActionResult index action[Authorize]
		**運用modal方式去增刪查改
			Index.cshtml


[]add 文章分類編輯
	url: /ArticleCategories/Edit?articleCategoryId=00 (不會用網址跳轉，像是接口，讓modal去連接)
	[]add ViewModel, Dto , VM轉Dto的擴充方法
		ArticleCategoryEditViewModel class
			Id, Name, SortOrder, IsEnabled

		ArticleCategoryEditDto class
			Id, Name, SortOrder, IsEnabled

		

	[]modify ArticleCategoryRepository
		IArticleCategoryRepository interface
			add void Edit(ArticleCategoryEditDto dto)
			add ArticleCategoryEditDto GetEditById(int id)

	[]modify 	ArticleCategoryService
			void Edit(ArticleCategoryEditDto dto)

	[]modify ArticleCategoriesController
		add IActionResult Edit action[Authorize]
		HttpGet Edit(int id)[Authorize]
		HttpPost Edit(ArticleCategoryEditViewModel  vm)[Authorize]



[]文章分類停用 / 硬刪除   (如果該文章分類底下有文章，不可刪除)
	[]modify IArticleCategoryRepository interface
    		// 刪除（硬刪除，有條件）
   		 add bool HasArticles(int id)
    		add void Delete(int id)

    		// 啟用/停用（切換 IsEnabled）
    		add void ToggleEnabled(int id, bool isEnabled)

	[]modify  ArticleCategoryService  ---停用不需要檢查底下是否有文章，因為只是隱藏分類，文章資料不受影響。
    		// 刪除：有文章不能刪
    		void Delete(int id)
        		if repo.HasArticles(id) → 拋出錯誤

   		 // 停用/啟用：不管底下有沒有文章都可以操作
   		 void ToggleEnabled(int id, bool isEnabled)
      		 repo.ToggleEnabled(id, isEnabled)

	[]modify ArticleCategoriesController(這邊再思考該如何處理)
		[HttpPost][Authorize]
		IActionResult Delete(int id)
    		// 失敗回傳 { success: false, message: "底下仍有文章" }

		[HttpPost][Authorize]
		IActionResult ToggleEnabled(int id, bool isEnabled)
		// 單純切換，不需要額外驗證




********************文章********************


[]add 文章新增頁面
	url: /Articles/Create
	[]add ViewModel, Dto , VM轉Dto的擴充方法
		ArticleCreateViewModel class
			CategoryName, EventTitle, Title, Description, CoverImageUrl, PublishDate, ExpiryDate, IsPinned, Status
			IEnumerable<SelectListItem> 用於選取文章分類及活動;需要注入對應 Service 填充(ICategoryService.GetSelectList()+ IEventService.GetSelectList())
	
		ArticleCreateDto class
			CategoryName,  EventTitle, Title, Description, CoverImageUrl, PublishDate, ExpiryDate, IsPinned, Status

		//ArticleCreateViewModelExtension class /  VM → Dto
			ArticleCreateDto  ToDto(this ArticleCreateViewModel  vm)

	[]add  ArticleMappingExtension class  放文章區域全部的ToVm, ToDto, ToEntity方法
	            新增文章 vm -> dto
		→ ToDto(this ArticleCreateViewModel vm)
		// Dto → Entity（Repository 寫入用）
		→Article ToEntity(this ArticleCreateDto dto)
		
	            文章列表 dto -> vm
		→ ToViewModel(this ArticleDto dto)
		// Entity → Dto（Repository 讀取用）
		ArticleDto ToDto(this Article entity)

	            文章預覽 dto -> vm
		→ ToViewModel(this ArticleDetailsDto dto)
		// Entity → Dto（Repository 讀取用）
		ArticleDetailsDto ToDto(this Article entity)

	            編輯文章 vm <-> dto
 		→ ToDto(this ArticleEditViewModel vm)
   		→ ToViewModel(this ArticleEditDto dto)
		// Dto → Entity（Repository 寫入用）
		→Article ToEntity(this ArticleEditDto dto)

		// Entity → Dto（Repository 讀取用）
		ArticleDto ToDto(this Article entity)
				

	[]add Service/Repository
		IArticleRepository interface
			void Create(ArticleCreateDto dto)

		ArticleRepository 實作 IArticleRepository
			ctor(EatTogetherContext context)

		ArticleService
			ctor(IArticleRepository repo)
			void Create(ArticleCreateDto dto)
			預設給 Status = 0 (草稿)

		在 Program.cs 註冊IArticleRepository, ArticleRepository, service

	[]add ArticlesController
		Create()
			Create.cshtml
		ctor(ArticleService articleService)
		Create(ArticleCreateViewModel vm)[Authorize]


[]add 文章列表頁面
	url: /Articles/Index
	[]add ViewModel, Dto , VM轉Dto的擴充方法
		ArticleViewModel class
			Id, Title, CategoryName,  EventTitle, IsPinned, Status
	
		ArticleDto class
			Id, Title, CategoryName,  EventTitle, IsPinned, Status

			

	[]modify ArticleRepository
		IArticleRepository interface
			add IEnumerable<ArticleDto> GetAll()

	[]modify 	ArticleService
			List<ArticleDto> GetAllForIndex()

	[]modify ArticlesController
		add IActionResult index action[Authorize]
			Index.cshtml

	**文章狀態:已發佈、草稿、已下架

[]add 文章預覽頁面
	url: /Articles/Details?articleId=00
	[]add ViewModel, Dto , VM轉Dto的擴充方法
		ArticleDetailsViewModel class
			Id, Title, Description, CategoryName , PublishDate , IsPinned
	
		ArticleDetailsDto class
			Id, Title, Description, CategoryName ,PublishDate , IsPinned

	[]modify ArticleRepository
		IArticleRepository interface
			add   ArticleDetailsDto GetById(int id)

	[]modify 	ArticleService
			add ArticleDetailsDto GetById(int id)

	[]modify ArticlesController
		add IActionResult Details action[Authorize]
			Details.cshtml
				add EditArticle button(Redirct to Edit頁面)

[]add 文章編輯頁面
	url: /Articles/Edit?articleId=00
	[]add ViewModel, Dto , VM轉Dto的擴充方法
		ArticleEditViewModel class
			Id, CategoryName, EventTitle, Title, Description, CoverImageUrl, PublishDate, ExpiryDate, IsPinned, Status
			IEnumerable<SelectListItem> 用於選取文章分類及活動;需要注入對應 Service 填充(ICategoryService.GetSelectList()+
IEventService.GetSelectList())

		ArticleEditDto class
			Id, CategoryName, EventTitle, Title, Description, CoverImageUrl, PublishDate, ExpiryDate, IsPinned, Status


	[]modify ArticleRepository
		IArticleRepository interface
			add void Edit(ArticleEditDto dto)
			add ArticleEditDto GetEditById(int id)

	[]modify 	ArticleService
			void Edit(ArticleEditDto dto)

	[]modify ArticlesController
		add IActionResult Edit action[Authorize]
			Edit.cshtml
		HttpGet Edit(int id)[Authorize]

		HttpPost Edit(ArticleEditViewModel vm)[Authorize]
		
[]文章軟刪除(下架)
	[]modify IArticleRepository interface
    		add void UpdateStatus(int id, int status)

	[]modify  ArticleService
    		add void Unpublish(int id) 

	[]modify ArticlesController  (這邊再思考該如何處理)
	// Index 頁面上的下架按鈕
	[HttpPost]
	[Authorize]
	IActionResult Unpublish(int id)