using ContactsStore.Identity;
using ContactsStore.Models;
using ContactsStore.Operations;
using ContactsStore.Parts.User.Tests.Unit;
using ContactsStore.Tests.DependencyInjection;
using ContactsStore.Tests.Logging;
using ContactsStore.Tests.Moq;
using ContactsStore.Tests.Resources;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace ContactsStore.Tests.Operations;

public class GetContactsTests : UnitTest
{
	private readonly IResourceScope _resources;
	private readonly IServiceProvider _serviceProvider;

	public GetContactsTests(ITestOutputHelper testOutputHelper, UnitDependencyContainerFixture container, ResourceRepositoryFixture resources) : base(testOutputHelper)
	{
		var parts = new AppPartsCollection()
			.AddContactsPart()
			.AddUserPart();
		_resources = resources.CreateTestScope(this);
		_serviceProvider = container
			.WithXunitLogging(TestOutputHelper)
			.WithTestScopeInMemoryDatabase(parts)
			.ReplaceWithEmptyMock<IUserContextAccessor>()
			.ConfigureServices(s => s.AddAppParts(parts))
			.BuildServiceProvider();
	}

	[Fact]
	public async Task MapsCorrectly()
	{
		using var methodResources = _resources.CreateMethodScope();
		var contactToCreate = methodResources.GetJsonInputResource<EditContactDto>();
		var mediator = _serviceProvider.GetRequiredService<IMediator>();
		await mediator.Send(new CreateContact(contactToCreate));
		var page = await mediator.Send(new GetContacts(0, 1));
		methodResources.CompareWithJsonExpectation(TestOutputHelper, page);
	}

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();
		await _serviceProvider.SetDefaultUserContextAsync();
	}
}