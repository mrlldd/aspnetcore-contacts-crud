namespace ContactsStore;

public static class AppPartsCollectionExtensions
{
	public static IAppPartsCollection AddContactsPart(this IAppPartsCollection parts)
	{
		parts.Add(new ContactsPart());
		return parts;
	}
}