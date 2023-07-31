using System.ComponentModel.DataAnnotations.Schema;
using ContactsStore.Entities.Configuration;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactsStore.Entities.Information;

[Table("Persons", Schema = "information")]
public class Person : IAuditableEntity
{
	public int PersonId { get; set; }
	public string Name { get; set; } = null!;
	public string Surname { get; set; } = null!;

	public ICollection<PhoneNumber> PhoneNumbers { get; set; } = new List<PhoneNumber>();

	public ICollection<EmailAddress> EmailAddresses { get; set; } = new List<EmailAddress>();

	public DateTime CreatedAt { get; set; }
	public DateTime ModifiedAt { get; set; }

	public class Configurator : EntityConfiguration<Person>
	{
		public override void Configure(EntityTypeBuilder<Person> builder)
		{
			base.Configure(builder);
			builder.HasKey(x => x.PersonId);
			builder.Property(x => x.Name).HasMaxLength(50);
			builder.Property(x => x.Surname).HasMaxLength(50);
			
			builder.HasMany(x => x.PhoneNumbers)
				.WithOne(x => x.Owner)
				.HasForeignKey(x => x.OwnerId);

			builder.HasMany(x => x.EmailAddresses)
				.WithOne(x => x.Owner)
				.HasForeignKey(x => x.OwnerId);
		}
	}

	[UsedImplicitly]
	internal class Validator : AbstractValidator<Person>
	{
		public Validator(IValidator<IAuditableEntity> aeValidator)
		{
			Include(aeValidator);
			RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
			RuleFor(x => x.Surname).NotEmpty().MaximumLength(50);
		}
	}
}