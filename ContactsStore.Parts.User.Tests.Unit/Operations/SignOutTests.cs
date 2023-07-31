using ContactsStore.Entities.Identity;
using ContactsStore.Operations;
using ContactsStore.Parts.User.Tests.Unit.Moq;
using ContactsStore.Tests;
using ContactsStore.Tests.DependencyInjection;
using ContactsStore.Tests.Logging;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit.Abstractions;

namespace ContactsStore.Parts.User.Tests.Unit.Operations;

public class SignOutTests : UnitTest
{
	private readonly DependencyContainerFixture _container;

	public SignOutTests(ITestOutputHelper testOutputHelper, UnitDependencyContainerFixture container) : base(testOutputHelper)
	{
		var parts = new AppPartsCollection()
			.AddUserPart();
		_container = container
			.WithXunitLogging(TestOutputHelper)
			.ConfigureServices(s => s.AddAppParts(parts));
	}

	[Fact]
	public async Task SignOuts()
	{
		var sp = _container
			.WithMockedSignInManager<CSUser>(mock => mock.Setup(x => x.SignOutAsync())
				.Returns(Task.CompletedTask)
				.Verifiable())
			.BuildServiceProvider();

		var mediator = sp.GetRequiredService<IMediator>();
		await mediator.Send(new SignOut());

		var mock = sp.GetRequiredService<Mock<SignInManager<CSUser>>>();
		mock.Verify(x => x.SignOutAsync(), Times.Once());
	}
}