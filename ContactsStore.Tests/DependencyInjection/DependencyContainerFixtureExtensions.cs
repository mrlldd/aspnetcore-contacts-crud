namespace ContactsStore.Tests.DependencyInjection;

public static class DependencyContainerFixtureExtensions
{
	public static DependencyContainerFixture Clear(this DependencyContainerFixture fixture)
		=> fixture.ConfigureServices(x => x.Clear());
}