using ContactsStore.Entities;
using ContactsStore.Identity;
using ContactsStore.Models;
using ContactsStore.Models.Groups;
using ContactsStore.Operations;
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

namespace ContactsStore.Tests.Operations;

public class CreateContactTests : UnitTest
{
	private readonly IServiceProvider _serviceProvider;
	private readonly IResourceScope _resources;

	public CreateContactTests(ITestOutputHelper testOutputHelper, UnitDependencyContainerFixture container, ResourceRepositoryFixture resources) : base(testOutputHelper)
	{
		var parts = new AppPartsCollection()
			.AddUserPart()
			.AddContactsPart();
		_serviceProvider = container
			.WithXunitLogging(TestOutputHelper)
			.ReplaceWithEmptyMock<IUserContextAccessor>()
			.WithTestScopeInMemoryDatabase(parts)
			.ConfigureServices(s => s.AddAppParts(parts))
			.BuildServiceProvider();
		
		_resources = resources.CreateTestScope(this);
	}

	[Fact]
	public async Task CreatesContact()
	{
		using var methodResources = _resources.CreateMethodScope();
		var contactToCreate = methodResources.GetJsonInputResource<EditContactDto>();
		var mediator = _serviceProvider.GetRequiredService<IMediator>();
		await mediator.Send(new CreateContact(contactToCreate));

		var ctx = _serviceProvider.GetRequiredService<ContactsStoreDbContext>();
		var contact = await ctx.Set<Contact>()
			.Include(x => x.Person!)
			.ThenInclude(x => x.EmailAddresses)
			.Include(x => x.Person!)
			.ThenInclude(x => x.PhoneNumbers)
			.SingleAsync();
		
		methodResources.CompareWithJsonExpectation(TestOutputHelper, contact, configure: c => c.ExcludingAuditableEntityProperties());
	}

	[Fact]
	public async Task CreatesGroupedContact()
	{
		using var methodResources = _resources.CreateMethodScope();
		var groupToCreate = methodResources.GetJsonInputResource<EditContactGroupDto>("group");
		var mediator = _serviceProvider.GetRequiredService<IMediator>();
		await mediator.Send(new CreateContactGroup(groupToCreate));
		
		var contactToCreate = methodResources.GetJsonInputResource<EditContactDto>();
		await mediator.Send(new CreateContact(contactToCreate));

		var ctx = _serviceProvider.GetRequiredService<ContactsStoreDbContext>();
		var contact = await ctx.Set<Contact>()
			.Include(x => x.Person!)
			.ThenInclude(x => x.EmailAddresses)
			.Include(x => x.Person!)
			.ThenInclude(x => x.PhoneNumbers)
			.Include(x => x.Group)
			.SingleAsync();
		
		methodResources.CompareWithJsonExpectation(TestOutputHelper, contact, configure: c => c.ExcludingAuditableEntityProperties());
	}

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();
		await _serviceProvider.SetDefaultUserContextAsync();
	}
}