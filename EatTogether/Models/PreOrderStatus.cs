namespace EatTogether.Models
{
    // 可以放在 Models 或 Constants 資料夾
    public static class PreOrderStatus
    {
        public const int Pending = 0; // 等待中
        public const int Done = 1; // 完成
        public const int Cancel = 2; // 取消
    }
}
