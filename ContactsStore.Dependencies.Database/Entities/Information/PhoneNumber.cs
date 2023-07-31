using System.ComponentModel.DataAnnotations.Schema;
using ContactsStore.Entities.Configuration;
using ContactsStore.Validation;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactsStore.Entities.Information;

[Table("PhoneNumbers", Schema = "information")]
public class PhoneNumber : IAuditableEntity
{
	public int PhoneNumberId { get; set; }
	public Person? Owner { get; set; }
	public int OwnerId { get; set; }
	public string Number { get; set; } = null!;
	public DateTime CreatedAt { get; set; }
	public DateTime ModifiedAt { get; set; }

	public class Configurator : EntityConfiguration<PhoneNumber>
	{
		public override void Configure(EntityTypeBuilder<PhoneNumber> builder)
		{
			base.Configure(builder);
			builder.HasKey(x => x.PhoneNumberId);
			builder.Property(x => x.Number).HasMaxLength(15);
		}
	}

	[UsedImplicitly]
	internal class Validator : AbstractValidator<PhoneNumber>
	{
		public Validator(IValidator<IAuditableEntity> aeValidator)
		{
			Include(aeValidator);
			RuleFor(x => x.Number)
				.NotEmpty()
				.MaximumLength(15)
				.PhoneNumber();
		}
	}
}