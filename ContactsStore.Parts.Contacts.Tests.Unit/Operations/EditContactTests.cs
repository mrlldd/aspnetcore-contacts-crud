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

public class EditContactTests : UnitTest
{
	private readonly IResourceScope _resources;
	private readonly IServiceProvider _serviceProvider;

	public EditContactTests(ITestOutputHelper testOutputHelper, UnitDependencyContainerFixture container,
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
	public async Task EditsContact()
	{
		using var methodResources = _resources.CreateMethodScope();
		var mediator = _serviceProvider.GetRequiredService<IMediator>();

		var contactToEdit = methodResources.GetJsonInputResource<EditContactDto>();
		await mediator.Send(new EditContact(contactToEdit));

		var ctx = _serviceProvider.GetRequiredService<ContactsStoreDbContext>();
		var contact = await ctx.Set<Contact>()
			.SingleAsync();
		methodResources.CompareWithJsonExpectation(TestOutputHelper, contact, configure:c => c.ExcludingAuditableEntityProperties());
	}

	[Fact]
	public async Task EditsPerson()
	{
		using var methodResources = _resources.CreateMethodScope();
		var mediator = _serviceProvider.GetRequiredService<IMediator>();

		var contactToEdit = methodResources.GetJsonInputResource<EditContactDto>();
		await mediator.Send(new EditContact(contactToEdit));

		var ctx = _serviceProvider.GetRequiredService<ContactsStoreDbContext>();
		var contact = await ctx.Set<Contact>()
			.Include(x => x.Person!)
			.SingleAsync();
		methodResources.CompareWithJsonExpectation(TestOutputHelper, contact,
			configure: c => c.ExcludingAuditableEntityProperties());
	}

	[Fact]
	public async Task AddsPhoneNumbers()
	{
		using var methodResources = _resources.CreateMethodScope();
		var mediator = _serviceProvider.GetRequiredService<IMediator>();

		var contactToEdit = methodResources.GetJsonInputResource<EditContactDto>();
		await mediator.Send(new EditContact(contactToEdit));

		var ctx = _serviceProvider.GetRequiredService<ContactsStoreDbContext>();
		var contact = await ctx.Set<Contact>()
			.Include(x => x.Person!)
			.ThenInclude(x => x.PhoneNumbers)
			.SingleAsync();
		methodResources.CompareWithJsonExpectation(TestOutputHelper, contact,
			configure: c => c.ExcludingAuditableEntityProperties());
	}

	[Fact]
	public async Task RemovesPhoneNumbers()
	{
		using var methodResources = _resources.CreateMethodScope();
		var mediator = _serviceProvider.GetRequiredService<IMediator>();

		var contactToEdit = methodResources.GetJsonInputResource<EditContactDto>();
		await mediator.Send(new EditContact(contactToEdit));

		var ctx = _serviceProvider.GetRequiredService<ContactsStoreDbContext>();
		var contact = await ctx.Set<Contact>()
			.Include(x => x.Person!)
			.ThenInclude(x => x.PhoneNumbers)
			.SingleAsync();
		methodResources.CompareWithJsonExpectation(TestOutputHelper, contact,
			configure: c => c.ExcludingAuditableEntityProperties());
	}
	
	[Fact]
	public async Task AddsEmailAddresses()
	{
		using var methodResources = _resources.CreateMethodScope();
		var mediator = _serviceProvider.GetRequiredService<IMediator>();

		var contactToEdit = methodResources.GetJsonInputResource<EditContactDto>();
		await mediator.Send(new EditContact(contactToEdit));

		var ctx = _serviceProvider.GetRequiredService<ContactsStoreDbContext>();
		var contact = await ctx.Set<Contact>()
			.Include(x => x.Person!)
			.ThenInclude(x => x.EmailAddresses)
			.SingleAsync();
		methodResources.CompareWithJsonExpectation(TestOutputHelper, contact,
			configure: c => c.ExcludingAuditableEntityProperties());
	}
	
	[Fact]
	public async Task RemovesEmailAddresses()
	{
		using var methodResources = _resources.CreateMethodScope();
		var mediator = _serviceProvider.GetRequiredService<IMediator>();

		var contactToEdit = methodResources.GetJsonInputResource<EditContactDto>();
		await mediator.Send(new EditContact(contactToEdit));

		var ctx = _serviceProvider.GetRequiredService<ContactsStoreDbContext>();
		var contact = await ctx.Set<Contact>()
			.Include(x => x.Person!)
			.ThenInclude(x => x.EmailAddresses)
			.SingleAsync();
		methodResources.CompareWithJsonExpectation(TestOutputHelper, contact,
			configure: c => c.ExcludingAuditableEntityProperties());
	}
	
	[Fact]
	public async Task AddsToGroup()
	{
		var mediator = _serviceProvider.GetRequiredService<IMediator>();

		var groupToCreate = _resources.GetJsonInputResource<EditContactGroupDto>("group");
		await mediator.Send(new CreateContactGroup(groupToCreate));
		
		using var methodResources = _resources.CreateMethodScope();

		var contactToEdit = methodResources.GetJsonInputResource<EditContactDto>();
		await mediator.Send(new EditContact(contactToEdit));

		var ctx = _serviceProvider.GetRequiredService<ContactsStoreDbContext>();
		var contact = await ctx.Set<Contact>()
			.Include(x => x.Group)
			.SingleAsync();
		methodResources.CompareWithJsonExpectation(TestOutputHelper, contact,
			configure: c => c.ExcludingAuditableEntityProperties());
	}

	[Fact]
	public async Task RemovesFromGroup()
	{
		var mediator = _serviceProvider.GetRequiredService<IMediator>();

		var groupToCreate = _resources.GetJsonInputResource<EditContactGroupDto>("group");
		await mediator.Send(new CreateContactGroup(groupToCreate));
		
		using var methodResources = _resources.CreateMethodScope();

		var contactToEdit = methodResources.GetJsonInputResource<EditContactDto>();
		await mediator.Send(new EditContact(contactToEdit));

		contactToEdit.GroupId = null;
		await mediator.Send(new EditContact(contactToEdit));

		var ctx = _serviceProvider.GetRequiredService<ContactsStoreDbContext>();
		var contact = await ctx.Set<Contact>()
			.Include(x => x.Group)
			.SingleAsync();
		methodResources.CompareWithJsonExpectation(TestOutputHelper, contact,
			configure: c => c.ExcludingAuditableEntityProperties());
	}

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();
		await _serviceProvider.SetDefaultUserContextAsync();
		var mediator = _serviceProvider.GetRequiredService<IMediator>();
		var contactToCreate = _resources.GetJsonInputResource<EditContactDto>("contact");
		await mediator.Send(new CreateContact(contactToCreate));
	}
}