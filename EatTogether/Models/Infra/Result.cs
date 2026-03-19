namespace EatTogether.Models.Infra
{
    public class Result
    {
		public bool IsSuccess { get; private set; }
		public string ErrorMessage { get; private set; } = "";

		public static Result Success()
			=> new Result { IsSuccess = true };

		public static Result Fail(string msg)
			=> new Result { IsSuccess = false, ErrorMessage = msg };
	}

	// 泛型版本，多一個 Value 存回傳資料
	public class Result<T>
	{
		public bool IsSuccess { get; private set; }
		public string ErrorMessage { get; private set; } = "";

		private T? _value;
		public T Value => IsSuccess
			? _value!
			: throw new InvalidOperationException("失敗的 Result 不能存取 Value");

		public static Result<T> Success(T value)
			=> new Result<T> { IsSuccess = true, _value = value };

		public static Result<T> Fail(string msg)
			=> new Result<T> { IsSuccess = false, ErrorMessage = msg };
	}
}
