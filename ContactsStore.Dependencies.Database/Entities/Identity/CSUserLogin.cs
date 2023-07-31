using System.ComponentModel.DataAnnotations.Schema;
using ContactsStore.Entities.Configuration;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace ContactsStore.Entities.Identity;

[Table("UserLogins", Schema = "identity")]
public class CSUserLogin : IdentityUserLogin<int>, IEntity
{
	public class Configurator : EntityConfiguration<CSUserLogin>
	{
		
	}

	[UsedImplicitly]
	internal class Validator : AbstractValidator<CSUserLogin>
	{
	}
}