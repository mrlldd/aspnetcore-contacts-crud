using ContactsStore.Entities.Identity;
using ContactsStore.Identity;
using ContactsStore.Persistence;
using ContactsStore.Startup;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ContactsStore;

internal class UserPart : IAppPart
{
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddAuthentication();
		services.AddAuthorization();
		services.AddIdentity<CSUser, CSRole>(options =>
			{
				var signIn = options.SignIn;
				signIn.RequireConfirmedAccount = false;
				signIn.RequireConfirmedEmail = false;
				signIn.RequireConfirmedPhoneNumber = false;

				var password = options.Password;
				password.RequireDigit = false;
				password.RequiredLength = 8;
				password.RequireLowercase = false;
				password.RequireUppercase = false;
				password.RequireNonAlphanumeric = false;
			})
			.AddEntityFrameworkStores<ContactsStoreDbContext>()
			.AddDefaultTokenProviders();

		services.ConfigureApplicationCookie(options =>
		{
			options.Cookie.HttpOnly = true;
			options.ExpireTimeSpan = TimeSpan.FromHours(1);
			options.LoginPath = "/Identity/Account/Login";
			options.AccessDeniedPath = "/Identity/Account/AccessDenied";
			options.SlidingExpiration = true;
			options.SessionStore = new MemoryCacheTicketStore();
		});

		services.AddStartupAction<SeedRoleAction>()
			.AddStartupAction<SeedUserAction>();

		services.TryAddScoped<IUserContextAccessor, UserContextAccessor>();
	}

	private sealed class MemoryCacheTicketStore : ITicketStore
	{
		private const string KeyPrefix = "AuthSessionStore-";
		private readonly IMemoryCache _cache;

		public MemoryCacheTicketStore() => _cache = new MemoryCache(new MemoryCacheOptions());

		public async Task<string> StoreAsync(AuthenticationTicket ticket)
		{
			var key = KeyPrefix + Guid.NewGuid();
			await RenewAsync(key, ticket);
			return key;
		}

		public Task RenewAsync(string key, AuthenticationTicket ticket)
		{
			var options = new MemoryCacheEntryOptions();
			var expiresUtc = ticket.Properties.ExpiresUtc;
			if (expiresUtc.HasValue)
			{
				options.SetAbsoluteExpiration(expiresUtc.Value);
			}

			options.SetSlidingExpiration(TimeSpan.FromHours(1));
			_cache.Set(key, ticket, options);
			return Task.CompletedTask;
		}

		public Task<AuthenticationTicket?> RetrieveAsync(string key)
		{
			_cache.TryGetValue(key, out AuthenticationTicket? ticket);
			return Task.FromResult(ticket);
		}

		public Task RemoveAsync(string key)
		{
			_cache.Remove(key);
			return Task.CompletedTask;
		}
	}
}