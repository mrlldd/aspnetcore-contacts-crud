using Microsoft.AspNetCore.Http;

namespace ContactsStore.Identity;

public interface IUserContextAccessor
{
	bool HasContext { get; }
	
	bool HasUser { get; }
	int UserId { get; }

	void EnrichWithHttpContext(HttpContext context);
}