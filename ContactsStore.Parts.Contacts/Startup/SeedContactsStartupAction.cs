using ContactsStore.Entities;
using ContactsStore.Entities.Identity;
using ContactsStore.Entities.Information;
using ContactsStore.Extensions;
using ContactsStore.Persistence;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ContactsStore.Startup;

[UsedImplicitly]
internal class SeedContactsStartupAction : IAsyncStartupAction
{
	private readonly IContactsStoreDatabase _database;
	public uint Order => 4;

	public SeedContactsStartupAction(IContactsStoreDatabase database) => _database = database;

	public async Task PerformActionAsync(CancellationToken cancellationToken = default)
	{
		var dbHasAnyContacts = await _database.ReadAsync((db, ct) => db.Context
			.Set<Contact>()
			.AnyAsync(ct), cancellationToken);
		if (dbHasAnyContacts)
		{
			return;
		}

		var user = await _database.ReadAsync((db, ct) => db.Context
			.Set<CSUser>()
			.FirstOrExceptionAsync(x => x.Email == "user@mail.com", ct), cancellationToken);
		await SeedFriendContactAsync(user, cancellationToken);
		await SeedGroupedContactAsync(user, cancellationToken);
		await _database.PersistAsync(cancellationToken);
	}

	private ValueTask<EntityEntry<Contact>> SeedFriendContactAsync(CSUser user, CancellationToken cancellationToken)
	{
		var contact = new Contact
		{
			Owner = user,
			Description = "A friend of mine",
			Person = new Person
			{
				Name = "Dear",
				Surname = "Friend",
				EmailAddresses = new List<EmailAddress>
				{
					new()
					{
						Address = "friend@mail.com"
					},
					new()
					{
						Address = "friendshome@mail.com"
					}
				},
				PhoneNumbers = new List<PhoneNumber>
				{
					new()
					{
						Number = "+380223334455"
					},
					new()
					{
						Number = "+380124446677"
					}
				}
			}
		};
		return _database.Context.AddAsync(contact, cancellationToken);
	}
	
	private ValueTask<EntityEntry<Contact>> SeedGroupedContactAsync(CSUser user, CancellationToken cancellationToken)
	{
		var contact = new Contact
		{
			Owner = user,
			Group = new ContactGroup
			{
				Owner = user,
				Name = "Work",
				Description = "Colleagues and friends from my company"
			},
			Description = "Development team mate",
			Person = new Person
			{
				Name = "Foo",
				Surname = "Bar",
				EmailAddresses = new List<EmailAddress>
				{
					new()
					{
						Address = "foobar@mail.com"
					},
					new()
					{
						Address = "foobar@company.com"
					}
				},
				PhoneNumbers = new List<PhoneNumber>
				{
					new()
					{
						Number = "+380345345345"
					},
					new()
					{
						Number = "+380567567567"
					}
				}
			}
		};
		return _database.Context.AddAsync(contact, cancellationToken);
	}
}