﻿using System.Reflection;

namespace ContactsStore.Exceptions;

public class CouldNotFindEntityException : DatabaseDependencyException
{
	public CouldNotFindEntityException(MemberInfo entity, long id) : this(entity.Name.ToLowerInvariant(), id)
	{
	}

	public CouldNotFindEntityException(MemberInfo entity) : base($"Couldn't find related {entity.Name.ToLowerInvariant()}")
	{
	}

	public CouldNotFindEntityException(string entity, long id)
		: base($"Couldn't find {entity} with id={id.ToString()}")
	{
	}

	public CouldNotFindEntityException(string entity) : base(
		$"Couldn't find related {entity}")
	{
	}
}