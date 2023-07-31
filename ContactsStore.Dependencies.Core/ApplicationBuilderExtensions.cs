using Microsoft.AspNetCore.Builder;

namespace ContactsStore;

public static class ApplicationBuilderExtensions
{
	public static IApplicationBuilder UseDependencies(this IApplicationBuilder builder, IAppDependenciesCollection dependencies)
	{
		foreach (var d in dependencies)
		{
			d.ConfigureApplication(builder);
		}

		return builder;
	}
}