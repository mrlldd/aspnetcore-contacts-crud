using Microsoft.AspNetCore.Hosting;

namespace ContactsStore;

public static class AppDependenciesCollectionExtensions
{
	public static IAppDependenciesCollection AddDatabase(this IAppDependenciesCollection dependencies, IWebHostEnvironment hostEnvironment)
	{
		dependencies.Add(new DatabaseDependency(hostEnvironment));
		return dependencies;
	}
}