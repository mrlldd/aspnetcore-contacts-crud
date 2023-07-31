using ContactsStore.Entities.Configuration;
using ContactsStore.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ContactsStore.Persistence;

public sealed class ContactsStoreDbContext : IdentityDbContext<CSUser, CSRole, int, CSUserClaim, CSUserRole, CSUserLogin, CSRoleClaim, CSUserToken>
{
	private readonly IDatabaseConfigurator _databaseConfigurator;

	public ContactsStoreDbContext(IDatabaseConfigurator databaseConfigurator,
							   DbContextOptions<ContactsStoreDbContext> options) : base(options)
	{
		_databaseConfigurator = databaseConfigurator;
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		_databaseConfigurator.OnModelCreating(builder);
	}

	protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
	{
		base.ConfigureConventions(configurationBuilder);
		_databaseConfigurator.ConfigureConventions(configurationBuilder, Database);
	}

	public override int SaveChanges(bool acceptAllChangesOnSuccess) => throw new NotSupportedException();

	public override int SaveChanges() => throw new NotSupportedException();
}