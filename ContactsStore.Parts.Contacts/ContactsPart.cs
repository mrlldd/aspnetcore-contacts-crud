using ContactsStore.Startup;
using Microsoft.Extensions.DependencyInjection;

namespace ContactsStore;

internal class ContactsPart : IAppPart
{
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddStartupAction<SeedContactsStartupAction>();
	}
}