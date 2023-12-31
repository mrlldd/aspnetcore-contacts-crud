﻿using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ContactsStore.Persistence;
using ContactsStore.Tests.DependencyInjection;
using ContactsStore.Tests.Entities;
using ContactsStore.Tests.Logging;
using ContactsStore.Tests.Moq;
using ContactsStore.Tests.Persistence;
using ContactsStore.Utilities.Paging;
using FluentValidation;
using Xunit.Abstractions;

namespace ContactsStore.Tests.Utilities.Paging;

public class QueryableExtensionsTests : UnitTest
{
	private readonly IServiceProvider _serviceProvider;

	public QueryableExtensionsTests(ITestOutputHelper testOutputHelper, UnitDependencyContainerFixture container) :
		base(testOutputHelper)
	{
		var parts = new AppPartsCollection
		{
			new DatabaseUnitTestsPart()
		};
		_serviceProvider = container
			.WithXunitLogging(TestOutputHelper)
			.WithTestScopeInMemoryDatabase(parts)
			.ReplaceWithEmptyMock<IValidator<ConfigurableEntity>>()
			.ReplaceWithEmptyMock<IValidator<IgnoredEntity>>()
			.ConfigureServices(s => s.AddAppParts(parts))
			.BuildServiceProvider();
	}

	[Fact]
	public async Task CountsCorrectly()
	{
		var database = _serviceProvider.GetRequiredService<IContactsStoreDatabase>();
		var entities = new[]
		{
			new UnitAuditableEntity(),
			new UnitAuditableEntity()
		};
		await database.PersistEntitiesAsync(entities);
		await database.PersistAsync();
		var page = await database.ReadAsync((db, ct) => db.Context
			.Set<UnitAuditableEntity>()
			.ToPagedResultAsync(new UnitPagedRequest(0, 0), ct), default);
		page.Should().BeEquivalentTo(new
		{
			Page = 0,
			Size = 0,
			Total = 2
		});
	}

	[Fact]
	public async Task RetrievesSetPart()
	{
		var database = _serviceProvider.GetRequiredService<IContactsStoreDatabase>();
		var entities = new[]
		{
			new UnitAuditableEntity(),
			new UnitAuditableEntity()
		};
		await database.PersistEntitiesAsync(entities);
		await database.PersistAsync();
		var page = await database.ReadAsync((db, ct) => db.Context
			.Set<UnitAuditableEntity>()
			.ToPagedResultAsync(new UnitPagedRequest(0, 1), ct), default);
		page.Should().BeEquivalentTo(new PagedResult<UnitAuditableEntity>(entities.Take(1).ToArray(), 0, 1, 2));
	}

	[Fact]
	public async Task RetrievesEmptyPage()
	{
		var database = _serviceProvider.GetRequiredService<IContactsStoreDatabase>();
		var entities = new[]
		{
			new UnitAuditableEntity(),
			new UnitAuditableEntity()
		};
		await database.PersistEntitiesAsync(entities);
		await database.PersistAsync();
		var page = await database.ReadAsync((db, ct) => db.Context
			.Set<UnitAuditableEntity>()
			.Where(x => x.Invalid)
			.ToPagedResultAsync(new UnitPagedRequest(0, entities.Length), ct), default);
		page.Should().BeEquivalentTo(new PagedResult<UnitAuditableEntity>(Array.Empty<UnitAuditableEntity>(), 0, entities.Length, 0));
	}

	[Fact]
	public async Task RetrievesConditionBasedPage()
	{
		var database = _serviceProvider.GetRequiredService<IContactsStoreDatabase>();
		var entities = new[]
		{
			new UnitAuditableEntity(),
			new UnitAuditableEntity()
		};
		await database.PersistEntitiesAsync(entities);
		await database.PersistAsync();
		var page = await database.ReadAsync((db, ct) => db.Context
			.Set<UnitAuditableEntity>()
			.Where(x => x.Id == 2)
			.ToPagedResultAsync(new UnitPagedRequest(0, entities.Length), ct), default);
		page.Should().BeEquivalentTo(new PagedResult<UnitAuditableEntity>(entities.Skip(1).ToArray(), 0, entities.Length, 1));
	}

	private record UnitPagedRequest(int Page, int Size) : IPagedRequest;
}