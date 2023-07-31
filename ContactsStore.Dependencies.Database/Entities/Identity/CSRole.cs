using System.ComponentModel.DataAnnotations.Schema;
using ContactsStore.Entities.Configuration;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace ContactsStore.Entities.Identity;

[Table("Roles", Schema = "identity")]
public class CSRole : IdentityRole<int>, IEntity
{
	public class Configurator : EntityConfiguration<CSRole>
	{
		
	}

	[UsedImplicitly]
	internal class Validator : AbstractValidator<CSRole>
	{
	}
}