using Jobtastic.Authorization;
using Jobtastic.Data;
using Jobtastic.Models;
using Jobtastic.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jobtastic
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			// appsettings.Development.json carries a LocalDB fallback so the project runs
			// after a plain clone; a user-secret connection string takes precedence.
			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
			if (string.IsNullOrWhiteSpace(connectionString))
				throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString));
			builder.Services.AddDatabaseDeveloperPageExceptionFilter();
			builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>();

            // Role changes and lockouts live in the auth cookie, which is otherwise
            // only re-checked at sign-in. Revalidating keeps an admin's action from
            // taking effect hours later, when the affected session happens to expire.
            builder.Services.Configure<SecurityStampValidatorOptions>(options =>
                options.ValidationInterval = TimeSpan.FromMinutes(1));

			builder.Services.AddScoped<SetupService>();
            builder.Services.AddScoped<ICurrentUser, CurrentUser>();
            builder.Services.AddScoped<PostingService>();
            builder.Services.AddScoped<AdminService>();
            builder.Services.AddControllersWithViews(options =>
			{
				options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ApiJobpostingService>();
            builder.Services.AddScoped<DemoDataSeeder>();

            var app = builder.Build();

			using (var scope = app.Services.CreateScope())
			{
				// Applying migrations here means a fresh clone needs no CLI step before
				// the first run.
				var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				await context.Database.MigrateAsync();

				var setupService = scope.ServiceProvider.GetRequiredService<SetupService>();
				await setupService.SeedRolesAsync();

				// Demo accounts carry a published password, so they must never reach a
				// deployed environment.
				if (app.Environment.IsDevelopment())
					await scope.ServiceProvider.GetRequiredService<DemoDataSeeder>().SeedAsync();
			}

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseMigrationsEndPoint();
			}
			else
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
			app.MapRazorPages();

			app.Run();
		}
	}
}
