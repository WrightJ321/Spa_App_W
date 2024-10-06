using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using SpaApp_2.Client.Pages;
using SpaApp_2.Components;
using SpaApp_2.Components.Account;
using SpaApp_2.Data;
using SpaApp_2.IService;
using SpaApp_2.Service;
using SpaApp_2.Services;
namespace SpaApp_2
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddRazorComponents()
				.AddInteractiveServerComponents()
				.AddInteractiveWebAssemblyComponents();

			builder.Services.AddCascadingAuthenticationState();
			builder.Services.AddScoped<IdentityUserAccessor>();
			builder.Services.AddScoped<IdentityRedirectManager>();
			builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();
			builder.Services.AddDbContext<DataContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
			
			builder.Services.AddScoped<IDayEventService, DayEventService>();

			builder.Services.AddAuthentication(options =>
				{
					options.DefaultScheme = IdentityConstants.ApplicationScheme;
					options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
				})
				.AddIdentityCookies();

			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString));
			builder.Services.AddDatabaseDeveloperPageExceptionFilter();

			builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddSignInManager()
				.AddDefaultTokenProviders();

            //builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

            // Register the real EmailSender service
            builder.Services.AddSingleton<IEmailSender>(new EmailSender(
                builder.Configuration["Smtp:Server"],
                int.Parse(builder.Configuration["Smtp:Port"]),
                builder.Configuration["Smtp:User"],
                builder.Configuration["Smtp:Pass"]
            ));

            var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseWebAssemblyDebugging();
				app.UseMigrationsEndPoint();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();

			app.UseStaticFiles();
			app.UseAntiforgery();

			app.MapRazorComponents<App>()
				.AddInteractiveServerRenderMode()
				.AddInteractiveWebAssemblyRenderMode()
				.AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

			// Add additional endpoints required by the Identity /Account Razor components.
			app.MapAdditionalIdentityEndpoints();

			app.Run();
		}
	}
}
