using System.Runtime.Serialization;

namespace ContactsStore.Tests.Exceptions;

[Serializable]
public class TestConfigurationException : ContactsStoreTestException
{
	public TestConfigurationException()
	{
	}

	public TestConfigurationException(string? message) : base(message)
	{
	}

	protected TestConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}
}