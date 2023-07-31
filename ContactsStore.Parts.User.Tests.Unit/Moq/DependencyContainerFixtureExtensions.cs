using System.ComponentModel;
using ContactsStore.Identity;
using ContactsStore.Tests.DependencyInjection;
using ContactsStore.Tests.Moq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace ContactsStore.Parts.User.Tests.Unit.Moq;

public static class DependencyContainerFixtureExtensions
{
	public static DependencyContainerFixture WithMockedUserManager<T>(this DependencyContainerFixture container,
	                                                                  Action<Mock<UserManager<T>>>? configure = null)
		where T : class
		=> container
			.ReplaceWithEmptyMock<IUserStore<T>>()
			.ReplaceWithEmptyMock<IOptions<IdentityOptions>>()
			.ReplaceWithEmptyMock<IPasswordHasher<T>>()
			.ReplaceWithEmptyMock<IUserValidator<T>>()
			.ConfigureServices(s => s.RemoveAll<IPasswordValidator<T>>())
			.ReplaceWithEmptyMock<IPasswordValidator<T>>()
			.ReplaceWithEmptyMock<ILookupNormalizer>()
			.ReplaceWithEmptyMock<IdentityErrorDescriber>()
			.ConfigureServices(s => s
				.Replace(ServiceDescriptor.Singleton<UserManager<T>>(sp
					=> sp.GetRequiredService<Mock<UserManager<T>>>().Object))
				.Replace(ServiceDescriptor.Singleton<Mock<UserManager<T>>>(sp =>
					{
						var mock = sp.GetRequiredService<MockRepository>()
							.Create<UserManager<T>>(MockBehavior.Loose,
								sp.GetRequiredService<IUserStore<T>>(),
								sp.GetRequiredService<IOptions<IdentityOptions>>(),
								sp.GetRequiredService<IPasswordHasher<T>>(),
								sp.GetServices<IUserValidator<T>>(),
								sp.GetServices<IPasswordValidator<T>>(),
								sp.GetRequiredService<ILookupNormalizer>(),
								sp.GetRequiredService<IdentityErrorDescriber>(),
								sp,
								sp.GetRequiredService<ILogger<UserManager<T>>>());
						configure?.Invoke(mock);
						return mock;
					})
				));

	public static DependencyContainerFixture WithMockedSignInManager<T>(this DependencyContainerFixture container,
	                                                                    Action<Mock<SignInManager<T>>>? configure =
		                                                                    null) where T : class
		=> container
			.WithMockedUserManager<T>()
			.ReplaceWithEmptyMock<IHttpContextAccessor>()
			.ReplaceWithEmptyMock<IUserClaimsPrincipalFactory<T>>()
			.ReplaceWithEmptyMock<IAuthenticationSchemeProvider>()
			.ReplaceWithEmptyMock<IUserConfirmation<T>>()
			.ConfigureServices(s => s.Replace(ServiceDescriptor.Singleton<SignInManager<T>>(sp
					=> sp.GetRequiredService<Mock<SignInManager<T>>>().Object))
				.Replace(ServiceDescriptor.Singleton<Mock<SignInManager<T>>>(sp =>
				{
					var mock = sp.GetRequiredService<MockRepository>()
						.Create<SignInManager<T>>(MockBehavior.Loose,
							sp.GetRequiredService<UserManager<T>>(),
							sp.GetRequiredService<IHttpContextAccessor>(),
							sp.GetRequiredService<IUserClaimsPrincipalFactory<T>>(),
							sp.GetRequiredService<IOptions<IdentityOptions>>(),
							sp.GetRequiredService<ILogger<SignInManager<T>>>(),
							sp.GetRequiredService<IAuthenticationSchemeProvider>(),
							sp.GetRequiredService<IUserConfirmation<T>>());
					configure?.Invoke(mock);
					return mock;
				})));
}