using ContactsStore.Entities;
using ContactsStore.Persistence;

namespace ContactsStore.Tests.Persistence;

public static class ContactsStoreDatabaseExtensions
{
	public static async ValueTask PersistEntitiesAsync<T>(this IContactsStoreDatabase db, IEnumerable<T> entities, CancellationToken cancellationToken = default)
		where T : class, IEntity
	{
		foreach (var e in entities)
		{
			await db.Context.AddAsync(e, cancellationToken);
		}

		await db.PersistAsync(cancellationToken);
	}

	public static async ValueTask PersistEntityAsync<T>(this IContactsStoreDatabase db, T entity, CancellationToken cancellationToken = default) where T : class, IEntity
	{
		await db.Context.AddAsync(entity, cancellationToken);
		await db.PersistAsync(cancellationToken);
	}
}