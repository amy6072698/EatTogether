namespace EatTogether.Models.Services
{
	public class Event_Article_ServiceResult<T>
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public T Data { get; set; }

		// 成功時的靜態方法
		public static Event_Article_ServiceResult<T> Ok(T data, string message = null)
			=> new Event_Article_ServiceResult<T> { Success = true, Data = data, Message = message };


		// 失敗時的靜態方法
		public static Event_Article_ServiceResult<T> Fail(string message)
			=> new Event_Article_ServiceResult<T> { Success = false, Message = message };
	}
}