using System.ComponentModel.DataAnnotations.Schema;
using ContactsStore.Entities.Configuration;
using ContactsStore.Entities.Information;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactsStore.Entities.Identity;

[Table("Users", Schema = "identity")]
public class CSUser : IdentityUser<int>, IAuditableEntity
{
	public Person? Person { get; set; }
	public int PersonId { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime ModifiedAt { get; set; }

	public class Configurator : EntityConfiguration<CSUser>
	{
		public override void Configure(EntityTypeBuilder<CSUser> builder)
		{
			base.Configure(builder);
			builder.HasOne(x => x.Person)
				.WithOne()
				.HasForeignKey<CSUser>(x => x.PersonId);
		}
	}

	[UsedImplicitly]
	public class Validator : AbstractValidator<CSUser>
	{
		public Validator(IValidator<IAuditableEntity> aeValidator)
		{
			Include(aeValidator);
			RuleFor(x => x.PhoneNumber).Null().Empty();
		}
	}
}