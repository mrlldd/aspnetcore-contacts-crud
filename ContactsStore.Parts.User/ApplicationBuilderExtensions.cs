using ContactsStore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ContactsStore;

public static class ApplicationBuilderExtensions
{
	public static IApplicationBuilder UseUserPartMiddlewares(this IApplicationBuilder builder)
		=> builder.Use((context, next) =>
		{
			context.RequestServices.GetRequiredService<IUserContextAccessor>().EnrichWithHttpContext(context);
			return next();
		});
}