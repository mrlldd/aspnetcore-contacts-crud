using System.ComponentModel.DataAnnotations.Schema;
using ContactsStore.Entities.Configuration;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace ContactsStore.Entities.Identity;

[Table("UserClaims", Schema = "identity")]
public class CSUserClaim : IdentityUserClaim<int>, IEntity
{
	public class Configurator : EntityConfiguration<CSUserClaim>
	{
	}

	[UsedImplicitly]
	internal class Validator : AbstractValidator<CSUserClaim>
	{
	}
}