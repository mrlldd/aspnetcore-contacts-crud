using System.Runtime.Serialization;

namespace ContactsStore.Tests.Exceptions;

public class ContactsStoreTestException : Exception
{
	public ContactsStoreTestException()
	{
	}

	protected ContactsStoreTestException(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}

	public ContactsStoreTestException(string? message) : base(message)
	{
	}

	public ContactsStoreTestException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}