Git先git branch -a查看分支，再checkout到對應分支
連結資料庫
[]設定EFModels
[]設定連線字串(appsettings.json)
[]註冊DbContext到DI容器(Program.cs)
---------------------------------------
[]1.PreOrderViewModel（點餐資料）前台點餐時接收資料，不直接影響正式訂單
    建立 PreOrderViewModel包含餐點清單、數量、可選優惠券
    public class PreOrderViewModel
    {
        public List<OrderItemViewModel> Items { get; set; }
        public int? CouponId { get; set; }
    }

[]2️.Repository 撰寫（資料存取）資料存取集中於 Repository，不寫商業邏輯 !!Repository 只負責 CRUD
    PreOrderRepository--Add()、GetById()
    PreOrderDetailRepository--AddRange()
    OrderRepository--Add()
    OrderDetailRepository--AddRange()
    PaymentRepository--Add()

[]3.OrderService（商業邏輯核心）集中處理訂單流程、狀態與轉單邏輯
    public interface IOrderService
    {
        int CreatePreOrder(CreatePreOrderDto dto);
        void SubmitPreOrder(int preOrderId);
        void ConfirmOrder(int preOrderId, int? couponId);
    }
    Service負責--訂單狀態檢查
               --PreOrder → Order 轉換
               --套用優惠券
               --建立 Payment

[]4.OrderController（前台點餐）讓客人完成點餐流程 !!前台不會產生 Order
    顯示菜單
    建立 PreOrder
    送出點餐
    -----流程-----
    前台點餐
    ↓
    建立 PreOrder / PreOrderDetail
    ↓
    狀態設為「已送出」

[]5.AdminOrderController（後台訂單管理）後台人員處理出餐與結帳
    查看待出餐 PreOrder
    查看訂單明細
    確認出餐並結帳
    -----流程-----
    後台確認
    ↓
    呼叫 OrderService.ConfirmOrder()

[]6.訂單轉換流程（PreOrder → Order）!!此流程寫在 Service，不寫在 Controller
    -----流程-----
    驗證 PreOrder 狀態
    建立 Order
    建立 OrderDetail
    呼叫 CouponService 計算折扣
    建立 Payment
    更新 OrderStatus 為已結帳

[]7.後台訂單畫面（顯示與狀態）讓老師「看得到系統在運作」
    待出餐清單
    已完成訂單
    結帳金額（原價 / 折扣 / 實付）

[]8.訂單驗證與防呆處理 避免資料錯亂（報告加分）
    已轉單不可重複轉
    已結帳不可修改
    優惠券僅能於結帳時套用