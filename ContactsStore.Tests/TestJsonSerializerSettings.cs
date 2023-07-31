using Newtonsoft.Json;

namespace ContactsStore.Tests;

public class TestJsonSerializerSettings : JsonSerializerSettings
{
	public TestJsonSerializerSettings()
	{
		TypeNameHandling = TypeNameHandling.Objects;
		ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
		NullValueHandling = NullValueHandling.Include;
		DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
		ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
	}
}