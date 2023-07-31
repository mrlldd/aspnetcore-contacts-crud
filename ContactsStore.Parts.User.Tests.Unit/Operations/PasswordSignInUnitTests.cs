using ContactsStore.Entities.Identity;
using ContactsStore.Operations;
using ContactsStore.Parts.User.Tests.Unit.Moq;
using ContactsStore.Tests;
using ContactsStore.Tests.DependencyInjection;
using ContactsStore.Tests.Logging;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit.Abstractions;

namespace ContactsStore.Parts.User.Tests.Unit.Operations;

public class PasswordSignInUnitTests : UnitTest
{
	private readonly DependencyContainerFixture _container;

	public PasswordSignInUnitTests(ITestOutputHelper testOutputHelper, UnitDependencyContainerFixture container) : base(
		testOutputHelper)
	{
		var parts = new AppPartsCollection()
			.AddUserPart();
		_container = container
			.WithXunitLogging(TestOutputHelper)
			.ConfigureServices(s => s.AddAppParts(parts));
	}

	[Fact]
	public async Task SignInsUser()
	{
		var sp = _container
			.WithMockedSignInManager<CSUser>(mock =>
			{
				mock.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(),
						It.Is<bool>(b => b == true), It.Is<bool>(b => b == true)))
					.ReturnsAsync(SignInResult.Success);
			})
			.BuildServiceProvider();
		var request = new PasswordSignIn("test@mail.com", "123123123");
		var mediator = sp.GetRequiredService<IMediator>();
		await mediator.Send(request);

		var mock = sp.GetRequiredService<Mock<SignInManager<CSUser>>>();
		mock.Verify(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(),
			It.Is<bool>(b => b == true), It.Is<bool>(b => b == true)), Times.Once());
	}

	[Fact]
	public async Task ThrowsIfRequestIsInvalid()
	{
		var sp = _container
			.WithMockedSignInManager<CSUser>(mock => mock
				.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(),
					It.IsAny<string>(),
					It.Is<bool>(b => b == true),
					It.Is<bool>(b => b == true)))
				.ReturnsAsync(SignInResult.Failed))
			.BuildServiceProvider();
		var request = new PasswordSignIn("test@mail.com", "123123123");
		var mediator = sp.GetRequiredService<IMediator>();
		var action = () => mediator.Send(request);

		await action.Should()
			.ThrowExactlyAsync<InvalidOperationException>();

		var mock = sp.GetRequiredService<Mock<SignInManager<CSUser>>>();
		mock.Verify(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(),
			It.Is<bool>(b => b == true), It.Is<bool>(b => b == true)), Times.Once());
	}
}