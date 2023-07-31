using ContactsStore.Entities.Identity;
using ContactsStore.Identity;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace ContactsStore.Startup;

[UsedImplicitly]
internal sealed class SeedRoleAction : IAsyncStartupAction
{
	private readonly RoleManager<CSRole> _roleManager;
	public uint Order => 2;

	public SeedRoleAction(RoleManager<CSRole> roleManager) => _roleManager = roleManager;

	public async Task PerformActionAsync(CancellationToken cancellationToken = default)
	{
		var foundDefaultRole = await _roleManager.FindByNameAsync(Roles.Default);
		if (foundDefaultRole is null)
		{
			await _roleManager.CreateAsync(new CSRole
			{
				Name = Roles.Default
			});
		}
	}
}