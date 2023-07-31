namespace ContactsStore.Persistence;

public interface IContactsStoreDatabase
{
	Task<T> ReadAsync<T>(Func<IContactsStoreDatabase, CancellationToken, Task<T>> func,
						 CancellationToken cancellationToken);

	Task WriteAsync(Func<IContactsStoreDatabase, CancellationToken, Task> func,
					CancellationToken cancellationToken);

	Task PersistAsync(CancellationToken cancellationToken = default);

	ContactsStoreDbContext Context { get; }
}