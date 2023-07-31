using ContactsStore.Entities;
using ContactsStore.Identity;
using ContactsStore.Models.Groups;
using ContactsStore.Operations.Groups;
using ContactsStore.Parts.User.Tests.Unit;
using ContactsStore.Persistence;
using ContactsStore.Tests.DependencyInjection;
using ContactsStore.Tests.FluentAssertions;
using ContactsStore.Tests.Logging;
using ContactsStore.Tests.Moq;
using ContactsStore.Tests.Resources;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace ContactsStore.Tests.Operations.Groups;

public class CreateContactGroupTests : UnitTest
{
	private readonly IResourceScope _resources;
	private readonly IServiceProvider _serviceProvider;

	public CreateContactGroupTests(ITestOutputHelper testOutputHelper, UnitDependencyContainerFixture container,
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
	public async Task CreatesGroup()
	{
		var mediator = _serviceProvider.GetRequiredService<IMediator>();
		using var methodResources = _resources.CreateMethodScope();

		var groupToCreate = methodResources.GetJsonInputResource<EditContactGroupDto>();
		await mediator.Send(new CreateContactGroup(groupToCreate));

		var ctx = _serviceProvider.GetRequiredService<ContactsStoreDbContext>();

		var group = await ctx.Set<ContactGroup>()
			.SingleAsync();
		methodResources.CompareWithJsonExpectation(TestOutputHelper, group, configure: c => c.ExcludingAuditableEntityProperties());
	}

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();
		await _serviceProvider.SetDefaultUserContextAsync();
	}
}