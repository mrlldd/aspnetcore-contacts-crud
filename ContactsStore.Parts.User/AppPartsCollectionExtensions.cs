namespace ContactsStore;

public static class AppPartsCollectionExtensions
{
	public static IAppPartsCollection AddUserPart(this IAppPartsCollection parts)
	{
		parts.Add(new UserPart());
		return parts;
	}
}