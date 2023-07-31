using JetBrains.Annotations;
using ContactsStore.Entities;
using ContactsStore.Entities.Configuration;

namespace ContactsStore.Tests.Entities;

[UsedImplicitly]
public class IgnoredEntity : IEntity
{
	[UsedImplicitly]
	public class Configurator : EntityConfiguration<IgnoredEntity>, IIgnoredEntityConfiguration
	{
	}
}