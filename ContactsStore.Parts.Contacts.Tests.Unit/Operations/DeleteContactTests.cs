using ContactsStore.Entities;
using ContactsStore.Identity;
using ContactsStore.Models;
using ContactsStore.Operations;
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

namespace ContactsStore.Tests.Operations;

public class DeleteContactTests : UnitTest
{
	private readonly IServiceProvider _serviceProvider;
	private readonly IResourceScope _resources;

	public DeleteContactTests(ITestOutputHelper testOutputHelper, UnitDependencyContainerFixture container, ResourceRepositoryFixture resources) : base(testOutputHelper)
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
	public async Task DeletesContact()
	{
		using var methodResources = _resources.CreateMethodScope();
		var mediator = _serviceProvider.GetRequiredService<IMediator>();

		await mediator.Send(new DeleteContact(3));

		var ctx = _serviceProvider.GetRequiredService<ContactsStoreDbContext>();
		var contacts = await ctx.Set<Contact>()
			.ToListAsync();
		methodResources.CompareWithJsonExpectation(TestOutputHelper, contacts);
	}
	
	[Fact]
	public async Task DontReturnDeletedContacts()
	{
		using var methodResources = _resources.CreateMethodScope();
		var mediator = _serviceProvider.GetRequiredService<IMediator>();

		await mediator.Send(new DeleteContact(3));

		var page = await mediator.Send(new GetContacts(0, 1));
		methodResources.CompareWithJsonExpectation(TestOutputHelper, page);
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