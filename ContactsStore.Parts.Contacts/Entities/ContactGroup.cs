using System.ComponentModel.DataAnnotations.Schema;
using ContactsStore.Entities.Configuration;
using ContactsStore.Entities.Identity;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactsStore.Entities;

[Table("ContactGroups", Schema = "contacts")]
public class ContactGroup : IAuditableEntity
{
	public CSUser? Owner { get; set; }
	public int OwnerId { get; set; }
	public int ContactGroupId { get; set; }

	public string Name { get; set; } = null!;
	public string? Description { get; set; }
	
	public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
	public DateTime CreatedAt { get; set; }
	public DateTime ModifiedAt { get; set; }

	public class Configurator : EntityConfiguration<ContactGroup>
	{
		public override void Configure(EntityTypeBuilder<ContactGroup> builder)
		{
			base.Configure(builder);
			builder.HasKey(x => x.ContactGroupId);
			builder.HasMany(x => x.Contacts)
				.WithOne(x => x.Group)
				.HasForeignKey(x => x.GroupId);
			builder.Property(x => x.Description).HasMaxLength(500);
			builder.Property(x => x.Name).HasMaxLength(50);

			builder.HasOne(x => x.Owner)
				.WithMany()
				.HasForeignKey(x => x.OwnerId);
		}
	}

	[UsedImplicitly]
	internal class Validator : AbstractValidator<ContactGroup>
	{
		public Validator(IValidator<IAuditableEntity> aeValidator)
		{
			Include(aeValidator);
			RuleFor(x => x.Description).MaximumLength(500).When(x => x.Description is not null);
			RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
		}
	}
}