using System.ComponentModel.DataAnnotations.Schema;
using ContactsStore.Entities.Configuration;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace ContactsStore.Entities.Identity;

[Table("UserRoles", Schema = "identity")]
public class CSUserRole : IdentityUserRole<int>, IEntity
{

	public class Configurator : EntityConfiguration<CSUserRole>
	{
	}

	[UsedImplicitly]
	internal class Validator : AbstractValidator<CSUserRole>
	{
	}
}