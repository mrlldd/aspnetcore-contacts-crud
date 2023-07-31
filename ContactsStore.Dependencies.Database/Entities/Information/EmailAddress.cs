using System.ComponentModel.DataAnnotations.Schema;
using ContactsStore.Entities.Configuration;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactsStore.Entities.Information;

[Table("EmailAddresses", Schema = "information")]
public class EmailAddress : IAuditableEntity
{
	public int EmailAddressId { get; set; }
	public Person? Owner { get; set; }
	public int OwnerId { get; set; }
	public string Address { get; set; } = null!;
	public DateTime CreatedAt { get; set; }
	public DateTime ModifiedAt { get; set; }

	public class Configurator : EntityConfiguration<EmailAddress>
	{
		public override void Configure(EntityTypeBuilder<EmailAddress> builder)
		{
			base.Configure(builder);
			builder.HasKey(x => x.EmailAddressId);
			builder.Property(x => x.Address).HasMaxLength(50);
		}
	}

	[UsedImplicitly]
	internal class Validator : AbstractValidator<EmailAddress>
	{ 
		public Validator(IValidator<IAuditableEntity> aeValidator)
		{
			Include(aeValidator);
			RuleFor(x => x.Address)
				.NotEmpty()
				.MaximumLength(50)
				.EmailAddress();
		}
	}
}