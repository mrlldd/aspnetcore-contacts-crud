using ContactsStore.Utilities;

namespace ContactsStore.Persistence;

internal sealed class ContactsStoreDatabase : IContactsStoreDatabase
{
	private readonly IDatabasePolicySet _policySet;

	public ContactsStoreDatabase(ContactsStoreDbContext context, IDatabasePolicySet policySet)
	{
		_policySet = policySet;
		Context = context;
	}

	public Task<T> ReadAsync<T>(Func<IContactsStoreDatabase, CancellationToken, Task<T>> func,
								CancellationToken cancellationToken)
		=> _policySet.DatabaseReadPolicy.ExecuteAsync(ct => func(this, ct), cancellationToken);

	public Task WriteAsync(Func<IContactsStoreDatabase, CancellationToken, Task> func, CancellationToken cancellationToken)
		=> _policySet.DatabaseWritePolicy.ExecuteAsync(ct => func(this, ct), cancellationToken);

	public Task PersistAsync(CancellationToken cancellationToken = default)
		=> WriteAsync((db, ct) => db.Context.SaveChangesAsync(ct), cancellationToken);

	public ContactsStoreDbContext Context { get; }
}