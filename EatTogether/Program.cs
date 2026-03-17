using EatTogether.Models.EfModels;
using EatTogether.Models.Extensions;
using EatTogether.Models.Infra;
using EatTogether.Models.Repositories;
using EatTogether.Models.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EatTogether
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

			// 註冊到DBContext
			builder.Services.AddDbContext<EatTogetherDBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

			// 新增 JWT Authentication
			var jwtSettings = builder.Configuration.GetSection("Jwt");
			var secretKey = jwtSettings["SecretKey"]!;

			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				// 從 httpOnly Cookie 讀取 Token
				options.Events = new JwtBearerEvents
				{
					OnMessageReceived = ctx =>
					{
						ctx.Token = ctx.Request.Cookies["jwt"];
						return Task.CompletedTask;
					}
				};

				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = jwtSettings["Issuer"],
					ValidAudience = jwtSettings["Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(
												  Encoding.UTF8.GetBytes(secretKey)),
					ClockSkew = TimeSpan.Zero
				};
			});

			// 註冊Repository
			builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
			builder.Services.AddScoped<IDishRepository, DishRepository>();
			builder.Services.AddScoped<ISetMealRepository, SetMealRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();

            // 註冊Service
            builder.Services.AddScoped<CategoryService>();
			builder.Services.AddScoped<DishService>();
			builder.Services.AddScoped<SetMealService>();
            builder.Services.AddScoped<ProductService>();


            builder.Services.AddScoped<ITableRepository, TableRepository>();
            builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
            builder.Services.AddScoped<ICouponRepository, CouponRepository>();
            builder.Services.AddScoped<IMemberCouponRepository, MemberCouponRepository>();
            builder.Services.AddScoped<TableService>();
            builder.Services.AddScoped<ReservationService>();
            builder.Services.AddScoped<CouponService>();
            builder.Services.AddScoped<ReservationEmailService>();
            builder.Services.AddScoped<BirthdayCouponService>();
            builder.Services.AddHostedService<BirthdayCouponBackgroundService>();
            builder.Services.AddHostedService<CouponNotifyBackgroundService>();

			builder.Services.AddScoped<IUserRepository, UserRepository>();
			builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
			builder.Services.AddScoped<IRoleRepository, RoleRepository>();
			builder.Services.AddScoped<IFunctionRepository, FunctionRepository>();
			builder.Services.AddScoped<IRoleFunctionRepository, RoleFunctionRepository>();
			builder.Services.AddScoped<IMemberRepository, MemberRepository>();
			builder.Services.AddScoped<IAuthService, AuthService>();
			builder.Services.AddScoped<IUserService, UserService>();
			builder.Services.AddScoped<IRoleService, RoleService>();
			builder.Services.AddScoped<IMemberService, MemberService>();
			builder.Services.AddScoped<IPasswordResetEmailService, PasswordResetEmailService>();

            // 欣柔註冊
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IPreOrderRepository, PreOrderRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IReportRepository, ReportRepository>();
            builder.Services.AddScoped<IReportService, ReportService>();



			builder.Services.AddScoped<IEventRepository, EventRepository>();
			builder.Services.AddScoped<EventService>();
			builder.Services.AddScoped<IArticleCategoryRepository, ArticleCategoryRepository>();
			builder.Services.AddScoped<ArticleCategoryService>();
			builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
			builder.Services.AddScoped<ArticleService>();



			// 註冊 Infra（需要 DI 的才註冊）
			builder.Services.AddHttpContextAccessor();
			builder.Services.AddScoped<JwtHelper>();
			builder.Services.AddSingleton<UserNumberGenerator>();

			var app = builder.Build();

            //每次執行，讓系統自動跑活動的狀態
			using (var scope = app.Services.CreateScope())
			{
				EventInitializerExtensions.UpdateEventStatuses(app.Services);
			}



			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
            }

			// 全域錯誤頁路由
			app.UseStatusCodePagesWithReExecute("/Error/{0}");

			app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

			// 新增 Authentication 在 Authorization 之前
			app.UseAuthentication();

			app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
