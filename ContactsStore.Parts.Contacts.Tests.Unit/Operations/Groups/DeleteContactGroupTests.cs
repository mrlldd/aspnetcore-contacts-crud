using ContactsStore.Entities;
using ContactsStore.Identity;
using ContactsStore.Models.Groups;
using ContactsStore.Operations.Groups;
using ContactsStore.Parts.User.Tests.Unit;
using ContactsStore.Persistence;
using ContactsStore.Tests.DependencyInjection;
using ContactsStore.Tests.Logging;
using ContactsStore.Tests.Moq;
using ContactsStore.Tests.Resources;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace ContactsStore.Tests.Operations.Groups;

public class DeleteContactGroupTests : UnitTest
{
	private readonly IServiceProvider _serviceProvider;
	private readonly IResourceScope _resources;

	public DeleteContactGroupTests(ITestOutputHelper testOutputHelper, UnitDependencyContainerFixture container,
	                               ResourceRepositoryFixture resources) : base(testOutputHelper)
	{
		var parts = new AppPartsCollection()
			.AddUserPart()
			.AddContactsPart();
		_resources = resources.CreateTestScope(this);
		_serviceProvider = container
			.WithXunitLogging(TestOutputHelper)
			.WithTestScopeInMemoryDatabase(parts)
			.ReplaceWithEmptyMock<IUserContextAccessor>()
			.ConfigureServices(s => s.AddAppParts(parts))
			.BuildServiceProvider();
	}

	[Fact]
	public async Task DeletesContactGroup()
	{
		using var methodResources = _resources.CreateMethodScope();
		var mediator = _serviceProvider.GetRequiredService<IMediator>();

		await mediator.Send(new DeleteContactGroup(1));

		var ctx = _serviceProvider.GetRequiredService<ContactsStoreDbContext>();
		var groups = await ctx.Set<ContactGroup>()
			.ToListAsync();
		methodResources.CompareWithJsonExpectation(TestOutputHelper, groups);
	}
	
	[Fact]
	public async Task DontReturnDeletedContactGroups()
	{
		using var methodResources = _resources.CreateMethodScope();
		var mediator = _serviceProvider.GetRequiredService<IMediator>();

		await mediator.Send(new DeleteContactGroup(1));

		var page = await mediator.Send(new GetContactGroups(0, 1));
		
		methodResources.CompareWithJsonExpectation(TestOutputHelper, page);
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