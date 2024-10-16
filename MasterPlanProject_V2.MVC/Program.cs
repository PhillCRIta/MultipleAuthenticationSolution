using MasterPlanProject_V2.MVC.Services;
using MasterPlanProject_V2.MVC.Services.IServices;

namespace MasterPlanProject.Mvc
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddControllersWithViews();

			builder.Services.AddAutoMapper(typeof(MappingConfig));

			builder.Services.AddHttpClient<ILocalitaPugliaService, LocalitaPugliaService>();
			builder.Services.AddHttpClient<IAuthService, AuthService>();
			builder.Services.AddScoped<IBaseService, BaseService>();

			builder.Services.AddScoped<ILocalitaPugliaService, LocalitaPugliaService>();
			builder.Services.AddScoped<IAuthService, AuthService>();
			builder.Services.AddScoped<ITokenProvider, TokenProvider>();

			//aggiungo la gestione della sessione per memorizzare il token e non doverlo recuperare tutte le volte
			builder.Services.AddDistributedMemoryCache();
			builder.Services.AddSession(opt =>
			{
				opt.IdleTimeout = TimeSpan.FromMinutes(10);
				opt.Cookie.HttpOnly = true;
				opt.Cookie.IsEssential = true;
			});

			builder.Services
			.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
			.AddCookie(opt =>
			{
				opt.Cookie.HttpOnly = true;
				opt.ExpireTimeSpan = TimeSpan.FromMinutes(10);
				opt.SlidingExpiration = true;
				opt.LoginPath = "/auth/login";
				opt.AccessDeniedPath = "/auth/accessdenied";
			});

			builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			var app = builder.Build();

			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();

			app.UseAuthorization();

			app.UseSession();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}
