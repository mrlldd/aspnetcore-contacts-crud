namespace ContactsStore.Exceptions;

public abstract class ContactsStoreException : ApplicationException
{
	protected ContactsStoreException(string message) : base(message)
	{
	}

	public virtual int StatusCode { get; } = 500;
}