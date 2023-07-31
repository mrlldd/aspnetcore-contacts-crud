using System.ComponentModel.DataAnnotations.Schema;
using ContactsStore.Entities.Configuration;
using ContactsStore.Entities.Identity;
using ContactsStore.Entities.Information;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactsStore.Entities;

[Table("Contacts", Schema = "contacts")]
public class Contact : IAuditableEntity
{
	public CSUser? Owner { get; set; }
	
	public int OwnerId { get; set; }
	
	public Person? Person { get; set; }
	
	public int PersonId { get; set; }
	
	public ContactGroup? Group { get; set; }
	
	public string? Description { get; set; }
	
	public int? GroupId { get; set; }
	
	public DateTime CreatedAt { get; set; }
	public DateTime ModifiedAt { get; set; }

	public class Configurator : EntityConfiguration<Contact>
	{
		public override void Configure(EntityTypeBuilder<Contact> builder)
		{
			base.Configure(builder);
			builder.HasKey(x => x.PersonId);
			builder.HasOne(x => x.Owner)
				.WithMany()
				.HasForeignKey(x => x.OwnerId);
			
			builder.HasOne(x => x.Person)
				.WithOne()
				.HasForeignKey<Contact>(x => x.PersonId);

			builder.Property(x => x.Description).HasMaxLength(500);
		}
	}

	[UsedImplicitly]
	internal class Validator : AbstractValidator<Contact>
	{
		public Validator(IValidator<IAuditableEntity> aeValidator)
		{
			Include(aeValidator);
			RuleFor(x => x.OwnerId).GreaterThan(0);
			RuleFor(x => x.Description).MaximumLength(500).When(x => x.Description is not null);
		}
	}
}