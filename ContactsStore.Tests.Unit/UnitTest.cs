using ContactsStore.Tests.DependencyInjection;
using Xunit.Abstractions;

namespace ContactsStore.Tests;

[Trait("Category", "Unit")]
public abstract class UnitTest : Test, IClassFixture<UnitDependencyContainerFixture>
{
	protected UnitTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
	{
	}
}