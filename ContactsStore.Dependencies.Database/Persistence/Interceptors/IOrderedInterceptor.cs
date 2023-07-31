using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ContactsStore.Persistence.Interceptors;

public interface IOrderedInterceptor : IInterceptor
{
	uint Order { get; }
}