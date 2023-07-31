using ContactsStore.Persistence;
using JetBrains.Annotations;
using ContactsStore.Utilities;

namespace ContactsStore.Startup;

[UsedImplicitly]
public class DatabaseMigrationAction : IAsyncStartupAction
{
	private readonly ContactsStoreDbContext _context;

	public uint Order => 1;

	public DatabaseMigrationAction(ContactsStoreDbContext context) => _context = context;

	public async Task PerformActionAsync(CancellationToken cancellationToken = default)
	{
		using (new NoTimeoutDbContextScope(_context))
		{
			await _context.Database.MigrateAsync(cancellationToken);
		}
	}
}