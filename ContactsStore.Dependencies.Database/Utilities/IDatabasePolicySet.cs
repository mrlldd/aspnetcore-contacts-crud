using Polly;

namespace ContactsStore.Utilities;

public interface IDatabasePolicySet
{
	IAsyncPolicy DatabaseReadPolicy { get; }
	IAsyncPolicy DatabaseWritePolicy { get; }
}