namespace EatTogether.Models.DTOs
{
    public class SetMealItemGroupDto
    {
        public int GroupNo { get; set; }       // OptionGroupNo
        public int PickLimit { get; set; }     // 這組要選幾個
        public bool IsOptional { get; set; }
        public List<SetMealItemOptionDto> Options { get; set; } = new();
    }
}
