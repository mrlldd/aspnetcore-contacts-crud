using ContactsStore.Entities;
using ContactsStore.Identity;

namespace ContactsStore.Persistence;

public static class ContactsStoreDbContextExtensions
{
	public static IQueryable<ContactGroup> UserContactGroups(this ContactsStoreDbContext ctx,
	                                                         IUserContextAccessor accessor)
		=> ctx
			.Set<ContactGroup>()
			.Where(x => x.OwnerId == accessor.UserId);

	public static IQueryable<Contact> UserContacts(this ContactsStoreDbContext ctx, IUserContextAccessor accessor)
		=> ctx
			.Set<Contact>()
			.Where(x => x.OwnerId == accessor.UserId);
}