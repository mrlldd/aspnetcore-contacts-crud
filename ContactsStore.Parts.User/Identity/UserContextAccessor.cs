using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ContactsStore.Identity;

internal class UserContextAccessor : IUserContextAccessor
{
	public bool HasContext { get; private set; }
	public bool HasUser { get; private set; }
	public int UserId { get; private set; }

	public void EnrichWithHttpContext(HttpContext context) => Enrich(() =>
	{
		var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (!string.IsNullOrEmpty(userIdClaim))
		{
			HasUser = true;
			UserId = int.Parse(userIdClaim);
		}
	});

	private void Enrich(Action enrich)
	{
		if (HasContext)
		{
			throw new InvalidOperationException("User context is already enriched");
		}

		enrich();
		HasContext = true;
	}
}