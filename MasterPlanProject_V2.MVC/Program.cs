using MasterPlanProject_V2.MVC.Services;
using MasterPlanProject_V2.MVC.Services.IServices;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

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
				opt.Cookie.Name = "AuthCookie";
				opt.Cookie.HttpOnly = true;
				opt.ExpireTimeSpan = TimeSpan.FromMinutes(5);
				opt.Cookie.MaxAge = opt.ExpireTimeSpan; 
				opt.SlidingExpiration = false;
				opt.LoginPath = "/auth/login";
				opt.AccessDeniedPath = "/auth/accessdenied";
				opt.Events = new CookieAuthenticationEvents()
				{
					OnValidatePrincipal = val =>
					{
						Debug.WriteLine(DateTime.Now + "MVC-VALIDATE " + val.Principal + "  " + val.Request);
						return Task.CompletedTask;
					},
					OnSigningIn = X =>
					{
						Debug.WriteLine("MVC-SIGNIN " + X.Request);
						return Task.CompletedTask;
					},
					OnSignedIn = X =>
					{
						Debug.WriteLine(DateTime.Now + "MVC-SIGNED " + X.Request + "URI: " +X.Request);
						return Task.CompletedTask;
					},
					OnRedirectToLogin = context =>
					{
						Debug.WriteLine(DateTime.Now + "MVC-TOLOGIN " + context.Request + " URI: " + context.RedirectUri);
						if (IsAjaxRequest(context.Request))
						{
							context.Response.Headers.Location = context.RedirectUri;
							context.Response.StatusCode = 401;
						}
						else
						{
							context.Response.Redirect(context.RedirectUri);
						}
						return Task.CompletedTask;
					},
					OnCheckSlidingExpiration = X =>
					{
						Debug.WriteLine(DateTime.Now + "MVC-SLIDINGEXPIRATION " + X.Request);
						return Task.CompletedTask;
					},
					OnSigningOut = X =>
					{
						Debug.WriteLine(DateTime.Now + "MVC-SIGNINGOUT " + X.Request);
						return Task.CompletedTask;
					},
					OnRedirectToReturnUrl = context =>
					{
						Debug.WriteLine(DateTime.Now + "MVC-REDIRECTTOURL " + context.Request + " URI: " + context.RedirectUri);
						if (IsAjaxRequest(context.Request))
						{
							context.Response.Headers.Location = context.RedirectUri;
						}
						else
						{
							context.Response.Redirect(context.RedirectUri);
						}
						return Task.CompletedTask;
					},
					OnRedirectToAccessDenied = context =>
					{
						Debug.WriteLine(DateTime.Now + "MVC-REDIRECTTOACCESSDENID " + context.Request + "URI: "+ context.RedirectUri);
						if (IsAjaxRequest(context.Request))
						{
							context.Response.Headers.Location = context.RedirectUri;
							context.Response.StatusCode = 403;
						}
						else
						{
							context.Response.Redirect(context.RedirectUri);
						}
						return Task.CompletedTask;
					},
					OnRedirectToLogout = context =>
					{
						Debug.WriteLine(DateTime.Now + "MVC-REDIRECTTOLOGOUT " + context.Request + " URI: "+context.RedirectUri);
						if (IsAjaxRequest(context.Request))
						{
							context.Response.Headers.Location = context.RedirectUri;
						}
						else
						{
							context.Response.Redirect(context.RedirectUri);
						}
						return Task.CompletedTask;
					}
				};
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
		private static bool IsAjaxRequest(HttpRequest request)
		{
			return string.Equals(request.Query[HeaderNames.XRequestedWith], "XMLHttpRequest", StringComparison.Ordinal) ||
				string.Equals(request.Headers.XRequestedWith, "XMLHttpRequest", StringComparison.Ordinal);
		} 
	}
}
