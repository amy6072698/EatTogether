namespace EatTogether.Models.Services
{
	public class EventServiceResult<T>
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public T Data { get; set; }

		// 成功時的靜態方法
		public static EventServiceResult<T> Ok(T data, string message = null)
			=> new EventServiceResult<T> { Success = true, Data = data, Message = message };


		// 失敗時的靜態方法
		public static EventServiceResult<T> Fail(string message)
			=> new EventServiceResult<T> { Success = false, Message = message };
	}
}