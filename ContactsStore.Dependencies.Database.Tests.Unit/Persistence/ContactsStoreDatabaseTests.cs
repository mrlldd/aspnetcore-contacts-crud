using Microsoft.Extensions.DependencyInjection;
using Moq;
using Polly;
using ContactsStore.Entities;
using ContactsStore.Persistence;
using ContactsStore.Tests.DependencyInjection;
using ContactsStore.Tests.Entities;
using ContactsStore.Tests.Logging;
using ContactsStore.Tests.Moq;
using ContactsStore.Utilities;
using Xunit.Abstractions;

namespace ContactsStore.Tests.Persistence;

public class ContactsStoreDatabaseTests : UnitTest
{
	private readonly DependencyContainerFixture _container;

	public ContactsStoreDatabaseTests(ITestOutputHelper testOutputHelper, UnitDependencyContainerFixture container) : base(
		testOutputHelper)
	{
		_container = container
			.WithXunitLogging(testOutputHelper);
	}

	[Fact]
	public async Task CallsReadPolicyOnRead()
	{
		var sp = _container
			.ReplaceWithMock<IAsyncPolicy>(mock =>
				mock.Setup(x
						=> x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<IEntity>>>(),
							It.IsAny<CancellationToken>()))
					.Verifiable())
			.ReplaceWithMock<IDatabasePolicySet>((sp, mock) =>
				mock.Setup(x => x.DatabaseReadPolicy)
					.Returns(sp.GetRequiredService<IAsyncPolicy>)
					.Verifiable())
			.WithTestScopeInMemoryDatabase(new AppPartsCollection())
			.BuildServiceProvider();
		var policySetMock = sp.GetRequiredService<Mock<IDatabasePolicySet>>();
		var database = new ContactsStoreDatabase(sp.GetRequiredService<ContactsStoreDbContext>(), policySetMock.Object);
		await database.ReadAsync((db, ct) => Task.FromResult<IEntity>(new ConfigurableEntity()), default);

		var asyncPolicyMock = sp.GetRequiredService<Mock<IAsyncPolicy>>();

		policySetMock.Verify(x => x.DatabaseReadPolicy, Times.Once());
		asyncPolicyMock.Verify(x => x.ExecuteAsync(
				It.IsAny<Func<CancellationToken, Task<IEntity>>>(),
				It.IsAny<CancellationToken>()),
			Times.Once());
	}

	[Fact]
	public async Task CallsWritePolicyOnWrite()
	{
		var sp = _container
			.ReplaceWithMock<IAsyncPolicy>(mock =>
				mock.Setup(x
						=> x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task>>(),
							It.IsAny<CancellationToken>()))
					.Verifiable())
			.ReplaceWithMock<IDatabasePolicySet>((sp, mock) =>
				mock.Setup(x => x.DatabaseWritePolicy)
					.Returns(sp.GetRequiredService<IAsyncPolicy>)
					.Verifiable())
			.WithTestScopeInMemoryDatabase(new AppPartsCollection())
			.BuildServiceProvider();
		var policySetMock = sp.GetRequiredService<Mock<IDatabasePolicySet>>();
		var database = new ContactsStoreDatabase(sp.GetRequiredService<ContactsStoreDbContext>(), policySetMock.Object);
		await database.WriteAsync((_, _) => Task.CompletedTask, default);

		var asyncPolicyMock = sp.GetRequiredService<Mock<IAsyncPolicy>>();

		policySetMock.Verify(x => x.DatabaseWritePolicy, Times.Once());
		asyncPolicyMock.Verify(x => x.ExecuteAsync(
				It.IsAny<Func<CancellationToken, Task>>(),
				It.IsAny<CancellationToken>()),
			Times.Once());
	}

	[Fact]
	public async Task CallsWritePolicyOnPersist()
	{
		var sp = _container
			.ReplaceWithMock<IAsyncPolicy>(mock =>
				mock.Setup(x
						=> x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task>>(),
							It.IsAny<CancellationToken>()))
					.Verifiable())
			.ReplaceWithMock<IDatabasePolicySet>((sp, mock) =>
				mock.Setup(x => x.DatabaseWritePolicy)
					.Returns(sp.GetRequiredService<IAsyncPolicy>)
					.Verifiable())
			.WithTestScopeInMemoryDatabase(new AppPartsCollection())
			.BuildServiceProvider();
		var policySetMock = sp.GetRequiredService<Mock<IDatabasePolicySet>>();
		var database = new ContactsStoreDatabase(sp.GetRequiredService<ContactsStoreDbContext>(), policySetMock.Object);
		await database.PersistAsync();

		var asyncPolicyMock = sp.GetRequiredService<Mock<IAsyncPolicy>>();

		policySetMock.Verify(x => x.DatabaseWritePolicy, Times.Once());
		asyncPolicyMock.Verify(x => x.ExecuteAsync(
				It.IsAny<Func<CancellationToken, Task>>(),
				It.IsAny<CancellationToken>()),
			Times.Once());
	}
}