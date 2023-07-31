using System.ComponentModel.DataAnnotations.Schema;
using ContactsStore.Entities.Configuration;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace ContactsStore.Entities.Identity;

[Table("UserTokens", Schema = "identity")]
public class CSUserToken : IdentityUserToken<int>, IEntity
{
	public class Configurator : EntityConfiguration<CSUserToken>
	{
		
	}

	[UsedImplicitly]
	internal class Validator : AbstractValidator<CSUserToken>
	{
	}
}