﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ContactsStore.Entities;
using ContactsStore.Entities.Configuration;

namespace ContactsStore.Tests.Entities;

public class ConfigurableEntity : IEntity
{
	public class Configurator : EntityConfiguration<ConfigurableEntity>
	{
		public override void Configure(EntityTypeBuilder<ConfigurableEntity> builder)
		{
			base.Configure(builder);
			builder.HasNoKey();
		}
	}
}