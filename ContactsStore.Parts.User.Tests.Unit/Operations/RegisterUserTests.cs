using ContactsStore.Entities.Identity;
using ContactsStore.Models;
using ContactsStore.Operations;
using ContactsStore.Parts.User.Tests.Unit.Moq;
using ContactsStore.Tests;
using ContactsStore.Tests.DependencyInjection;
using ContactsStore.Tests.Logging;
using ContactsStore.Tests.Resources;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit.Abstractions;

namespace ContactsStore.Parts.User.Tests.Unit.Operations;

public class RegisterUserTests : UnitTest
{
	private readonly DependencyContainerFixture _container;
	private readonly IResourceScope _resources;

	public RegisterUserTests(ITestOutputHelper testOutputHelper, UnitDependencyContainerFixture container, ResourceRepositoryFixture resources) : base(testOutputHelper)
	{
		var parts = new AppPartsCollection()
			.AddUserPart();
		_container = container
			.WithXunitLogging(TestOutputHelper)
			.ConfigureServices(s => s.AddAppParts(parts));
		_resources = resources.CreateTestScope(this);
	}

	[Fact]
	public async Task CreatesUser()
	{
		var sp = _container
			.WithMockedUserManager<CSUser>(mock =>
			{
				mock.Setup(x => x.CreateAsync(It.IsAny<CSUser>(), It.IsAny<string>()))
					.ReturnsAsync(IdentityResult.Success)
					.Verifiable();
			})
			.BuildServiceProvider();

		var mediator = sp.GetRequiredService<IMediator>();
		await mediator.Send(_resources.GetJsonInputResource<RegisterUser>("user"));

		var mock = sp.GetRequiredService<Mock<UserManager<CSUser>>>();
		mock.Verify(x => x.CreateAsync(It.IsAny<CSUser>(), It.IsAny<string>()), Times.Once());
	}
	
	[Fact]
	public async Task ThrowsIfFailedToRegister()
	{
		var sp = _container
			.WithMockedUserManager<CSUser>(mock =>
			{
				mock.Setup(x => x.CreateAsync(It.IsAny<CSUser>(), It.IsAny<string>()))
					.ReturnsAsync(IdentityResult.Failed())
					.Verifiable();
			})
			.BuildServiceProvider();

		var mediator = sp.GetRequiredService<IMediator>();
		var request = _resources.GetJsonInputResource<RegisterUser>("user");
		var action = () => mediator.Send(request);
		
		await action.Should().ThrowExactlyAsync<InvalidOperationException>();
		
		var mock = sp.GetRequiredService<Mock<UserManager<CSUser>>>();
		mock.Verify(x => x.CreateAsync(It.IsAny<CSUser>(), It.IsAny<string>()), Times.Once());
	}
}