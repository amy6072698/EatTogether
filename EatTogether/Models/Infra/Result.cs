namespace EatTogether.Models.Infra
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string ErrorMesssage { get; set; }

        public static Result Success()
            => new Result { IsSuccess = true };

        public static Result Fail(string msg)
                => new Result { IsSuccess = false, ErrorMesssage = msg };
    }
}
