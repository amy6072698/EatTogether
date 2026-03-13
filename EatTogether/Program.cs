using EatTogether.Models.EfModels;
using EatTogether.Models.Repositories;
using EatTogether.Models.Services;
using Microsoft.EntityFrameworkCore;

namespace EatTogether
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

			// µù¥U¨ìDBContext
			builder.Services.AddDbContext<EatTogetherDBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


        
			// µù¥URepository
			builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
			builder.Services.AddScoped<IDishRepository, DishRepository>();
			builder.Services.AddScoped<ISetMealRepository, SetMealRepository>();
			builder.Services.AddScoped<IEventRepository, EventRepository>();

			// µù¥UService
			builder.Services.AddScoped<CategoryService>();
			builder.Services.AddScoped<DishService>();
			builder.Services.AddScoped<SetMealService>();


            builder.Services.AddScoped<ITableRepository, TableRepository>();
            builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
            builder.Services.AddScoped<ICouponRepository, CouponRepository>();
            builder.Services.AddScoped<IMemberCouponRepository, MemberCouponRepository>();
            builder.Services.AddScoped<TableService>();
            builder.Services.AddScoped<ReservationService>();
            builder.Services.AddScoped<CouponService>(); ;
			builder.Services.AddScoped<EventService>();




            var app = builder.Build();




            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
