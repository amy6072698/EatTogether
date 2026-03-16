using EatTogether.Models.EfModels;
using System;

namespace EatTogether.Models.Extensions
{
	public class EventInitializerExtensions
	{
		public static void UpdateEventStatuses(IServiceProvider serviceProvider)
		{
			using (var scope = serviceProvider.CreateScope())
			{
				var context = scope.ServiceProvider.GetRequiredService<EatTogetherDBContext>();
				var now = DateTime.Now;

				// 1. 找出應該開始但狀態還是「未開始」的
				var toOngoing = context.Events
					.Where(e => e.Status == 0 && e.StartDate <= now && e.EndDate >= now)
					.ToList();
				foreach (var ev in toOngoing) ev.Status = 1;

				// 2. 找出應該結束但狀態還是「進行中」或「未開始」的
				var toFinished = context.Events
					.Where(e => (e.Status == 0 || e.Status == 1) && e.EndDate < now)
					.ToList();
				foreach (var ev in toFinished) ev.Status = 2;								

				if (toOngoing.Any() || toFinished.Any())
				{
					context.SaveChanges();
				}
			}
		}

	}
}
