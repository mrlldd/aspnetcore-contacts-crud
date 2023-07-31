using Microsoft.Extensions.DependencyInjection;

namespace ContactsStore;

public interface IAppPart
{
	void ConfigureServices(IServiceCollection services);
}