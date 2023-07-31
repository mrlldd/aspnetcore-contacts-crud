using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ContactsStore;

public interface IAppDependency
{
	void ConfigureServices(IServiceCollection services, IAppPartsCollection parts);
	void ConfigureApplication(IApplicationBuilder builder);
}