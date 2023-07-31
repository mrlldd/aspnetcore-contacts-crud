namespace ContactsStore.Exceptions;

public class ContactsStoreValidationException : ContactsStoreException
{
	private const string PrimaryMessage = "The server couldn't make sense of your request";

	public ContactsStoreValidationException(IEnumerable<string> errors)
		: base($"{PrimaryMessage}: {errors.Aggregate((prev, next) => $"{prev}, {next}")}")
	{
	}

	public ContactsStoreValidationException(string validationError)
		: base($"{PrimaryMessage}: {validationError}")
	{
	}

	public override int StatusCode { get; } = 400;
}