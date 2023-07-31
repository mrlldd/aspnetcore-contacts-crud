namespace ContactsStore.Exceptions;

public abstract class DependencyException : ContactsStoreException
{
	protected DependencyException(string message) : base(message)
	{
	}
}