using ContactsStore.Entities.Identity;
using ContactsStore.Entities.Information;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace ContactsStore.Startup;

[UsedImplicitly]
internal sealed class SeedUserAction : IAsyncStartupAction
{
	private readonly UserManager<CSUser> _userManager;
	public uint Order => 3;

	public SeedUserAction(UserManager<CSUser> userManager) => _userManager = userManager;

	public async Task PerformActionAsync(CancellationToken cancellationToken = default)
	{
		const string email = "user@mail.com";
		var foundUser = await _userManager.FindByEmailAsync(email);
		if (foundUser is null)
		{
			await _userManager.CreateAsync(new CSUser
			{
				Email = email,
				UserName = email,
				Person = new Person
				{
					Name = "Guest",
					Surname = "User",
					PhoneNumbers = new List<PhoneNumber>
					{
						new()
						{
							Number = "+380541234455"
						}
					},
					EmailAddresses = new List<EmailAddress>
					{
						new()
						{
							Address = email
						}
					}
				}
			},"gentlePassword");
		}
	}
}