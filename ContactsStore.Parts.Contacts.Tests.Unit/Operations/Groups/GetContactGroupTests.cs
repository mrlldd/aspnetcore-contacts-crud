using ContactsStore.Identity;
using ContactsStore.Models;
using ContactsStore.Models.Groups;
using ContactsStore.Operations;
using ContactsStore.Operations.Groups;
using ContactsStore.Parts.User.Tests.Unit;
using ContactsStore.Tests.DependencyInjection;
using ContactsStore.Tests.Logging;
using ContactsStore.Tests.Moq;
using ContactsStore.Tests.Resources;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace ContactsStore.Tests.Operations.Groups;

public class GetContactGroupTests : UnitTest
{
	private readonly IResourceScope _resources;
	private readonly IServiceProvider _serviceProvider;

	public GetContactGroupTests(ITestOutputHelper testOutputHelper, UnitDependencyContainerFixture container, ResourceRepositoryFixture resources) : base(testOutputHelper)
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
	public async Task ReturnsEmptyContactGroup()
	{
		using var methodResources = _resources.CreateMethodScope();
		var mediator = _serviceProvider.GetRequiredService<IMediator>();

		var group = await mediator.Send(new GetContactGroup(1));
		
		methodResources.CompareWithJsonExpectation(TestOutputHelper, group);
	}

	[Fact]
	public async Task ReturnsNonEmptyContactGroup()
	{
		using var methodResources = _resources.CreateMethodScope();
		var mediator = _serviceProvider.GetRequiredService<IMediator>();
		var contactToCreate = methodResources.GetJsonInputResource<EditContactDto>("contact");
		await mediator.Send(new CreateContact(contactToCreate));
		await mediator.Send(new AddContactToGroup(1, 3));
		var group = await mediator.Send(new GetContactGroup(1));
		methodResources.CompareWithJsonExpectation(TestOutputHelper, group);
	}

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();
		await _serviceProvider.SetDefaultUserContextAsync();
		var mediator = _serviceProvider.GetRequiredService<IMediator>();
		var groupToCreate = _resources.GetJsonInputResource<EditContactGroupDto>("group");
		await mediator.Send(new CreateContactGroup(groupToCreate));
	}
}